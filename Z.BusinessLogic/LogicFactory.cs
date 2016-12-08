using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic
{
    public class LogicFactory : ILogicFactory
    {
        private readonly IHotkeyService hotkeyService;
        private readonly IKeywordService keywordService;
        private readonly IModuleService moduleService;

        public LogicFactory(IHotkeyService hotkeyService,
            IKeywordService keywordService,
            IModuleService moduleService)
        {
            this.hotkeyService = hotkeyService;
            this.keywordService = keywordService;
            this.moduleService = moduleService;
        }

        public MainWindowLogic GenerateMainWindowLogic(IMainWindowViewModelAccess viewModel)
        {
            return new MainWindowLogic(viewModel, hotkeyService, keywordService, moduleService);
        }
    }
}
