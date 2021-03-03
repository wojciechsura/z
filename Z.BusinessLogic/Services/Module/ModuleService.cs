using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Models;
using Z.BusinessLogic.Services.EventBus;
using Z.BusinessLogic.Services.Paths;

namespace Z.BusinessLogic.Services.Module
{
    class ModuleService : IModuleService, IEventListener<ShuttingDownEvent>
    {
        // Private constants --------------------------------------------------

        public const string PLUGIN_PATH = "Plugins";

        // Private types ------------------------------------------------------

        private class SuggestionCollector : ISuggestionCollector, IDisposable
        {
            private readonly List<SuggestionData> suggestions;
            private readonly IZModule currentModule;
            private bool disposed = false;

            public SuggestionCollector(List<SuggestionData> suggestions, IZModule currentModule)
            {
                this.suggestions = suggestions;
                this.currentModule = currentModule;
            }

            public void AddSuggestion(SuggestionInfo suggestion)
            {
                if (disposed)
                    throw new InvalidOperationException("Do not store this class - use only when passed through a parameter!");
                if (suggestion == null)
                    throw new ArgumentNullException(nameof(suggestion));

                suggestions.Add(new SuggestionData(suggestion, currentModule));
            }

            public void Dispose()
            {
                this.disposed = true;
            }
        }

        private class ModuleContext : IModuleContext
        {
            private readonly IPathService pathService;
            private readonly string moduleName;

            public ModuleContext(IPathService pathService, string moduleName)
            {
                this.pathService = pathService;
                this.moduleName = moduleName;
            }

            public FileStream OpenConfigurationFile(string filename, FileMode fileMode, FileAccess fileAccess)
            {
                if (filename.Contains('\\'))
                    throw new ArgumentException("Invalid filename!", nameof(filename));

                string path = Path.Combine(pathService.GetModuleConfigDirectory(moduleName), filename);

                return new FileStream(path, fileMode, fileAccess);
            }

            public bool ConfigurationFileExists(string filename)
            {
                if (filename.Contains('\\'))
                    throw new ArgumentException("Invalid filename!", nameof(filename));

                string path = Path.Combine(pathService.GetModuleConfigDirectory(moduleName), filename);

                return File.Exists(path);
            }
        }

        // Private fields -----------------------------------------------------

        private readonly List<IZModule> modules;
        private readonly IPathService pathService;
        private readonly IEventBus eventBus;

        // Private methods ----------------------------------------------------

        private void InitDefaultModules()
        {
            AddModule(new WebSearchModule.Module());
            AddModule(new Filesystem.Module());
            AddModule(new ControlPanelModule.Module());
            AddModule(new StartMenuModule.Module());
            AddModule(new ProCalcModule.Module());
            AddModule(new PowerModule.Module());
            AddModule(new DesktopModule.Module());
            AddModule(new CustomCommandsModule.Module());
            AddModule(new ProjectsModule.Module());
            AddModule(new ShellFoldersModule.Module());
            AddModule(new ProcessModule.Module());
            AddModule(new HashModule.Module());
            AddModule(new RunModule.Module());
            AddModule(new FavoritesModule.Module());
        }

        private void LoadPluginModules()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string pluginPath = Path.Combine(Path.GetDirectoryName(path), PLUGIN_PATH);

            if (!Directory.Exists(pluginPath))
                return;

            foreach (var module in Directory.EnumerateFiles(pluginPath, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(module);
                foreach (var type in assembly.GetExportedTypes())
                {
                    if (typeof(IZModule).IsAssignableFrom(type))
                    {
                        var ctor = type.GetConstructor(Type.EmptyTypes);

                        // No parameterless ctor?
                        if (ctor == null)
                            continue;

                        IZModule pluginModule = (IZModule)Activator.CreateInstance(type);
                        AddModule(pluginModule);
                    }
                }
            }
        }

        private bool IsValidName(string moduleName)
        {
            return Regex.Match(moduleName.ToUpperInvariant(), "^[A-Z0-9_][A-Z0-9_\\.]*$").Success;
        }

        // Protected methods --------------------------------------------------

        protected virtual void OnModulesChanged()
        {
            eventBus.Send(new ModulesChangedEvent());
        }

        // IEnumerable<IZModule> implementation -------------------------------

        public IEnumerator<IZModule> GetEnumerator()
        {
            return modules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return modules.GetEnumerator();
        }

        // Public methods -----------------------------------------------------

        public IZModule GetModule(string internalName)
        {
            return modules
                .SingleOrDefault(m => m.Name == internalName);
        }

        public ModuleService(IPathService pathService, IEventBus eventBus)
        {
            this.pathService = pathService;
            this.eventBus = eventBus;

            eventBus.Register((IEventListener<ShuttingDownEvent>)this);

            modules = new List<IZModule>();
            InitDefaultModules();
            LoadPluginModules();
        }

        public IZModule GetModule(int index)
        {
            return modules[index];
        }

        public List<SuggestionData> GetSuggestionsFor(string text, KeywordData keyword, bool perfectMatchesOnly = false)
        {
            var suggestions = new List<SuggestionData>();

            var exclusiveModule = modules
                .FirstOrDefault(m => m is IZExclusiveSuggestions && ((IZExclusiveSuggestions) m).IsExclusiveText(text));

            if (keyword != null)
            {
                using (var collector = new SuggestionCollector(suggestions, keyword.Module))
                {
                    keyword.Module.CollectSuggestions(text, keyword.ActionName, perfectMatchesOnly, collector);
                }
            }
            else
            {
                var source = exclusiveModule != null ? 
                    new List<IZModule> { exclusiveModule } : 
                    modules;

                foreach (var module in source)
                {
                    using (var collector = new SuggestionCollector(suggestions, module))
                    {
                        module.CollectSuggestions(text, null, perfectMatchesOnly, collector);
                    }
                }
            }

            return suggestions;
        }

        public void AddModule(IZModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            if (!IsValidName(module.Name))
                throw new ArgumentException($"Invalid module name: {module.Name}", nameof(module));
            if (modules.Any(m => m.Name.ToUpper() == module.Name.ToUpper()))
                throw new ArgumentException($"Module with name {module.Name} is already registerd!");

            modules.Add(module);

            if (module is IZInitializable)
                ((IZInitializable) module).Initialize(new ModuleContext(pathService, module.Name));

            OnModulesChanged();
        }

        void IEventListener<ShuttingDownEvent>.Receive(ShuttingDownEvent @event)
        {
            foreach (var module in modules.OfType<IZInitializable>())
                module.Deinitialize();
        }

        // Public properties --------------------------------------------------

        public int ModuleCount => modules.Count;
    }
}
