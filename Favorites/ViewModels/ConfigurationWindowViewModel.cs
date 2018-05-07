using FavoritesModule.Common;
using FavoritesModule.Models;
using FavoritesModule.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FavoritesModule.ViewModels
{
    public class ConfigurationWindowViewModel : IFavoriteCallbacks
    {
        private Configuration configuration;
        private IConfigurationWindowAccess access;

        private ObservableCollection<FavoriteViewModel> favorites;

        private void HandleAdd()
        {
            favorites.Add(new FavoriteViewModel(this));
        }

        private void HandleOK()
        {
            access.CloseWindow();
        }

        void IFavoriteCallbacks.Delete(FavoriteViewModel favorite)
        {
            favorites.Remove(favorite);
        }

        public ConfigurationWindowViewModel(Configuration configuration)
        {
            this.configuration = configuration;

            favorites = new ObservableCollection<FavoriteViewModel>(configuration.Favorites
                .Select(c => new FavoriteViewModel(this, c)));

            AddFavoriteCommand = new SimpleCommand((obj) => HandleAdd());
            OKCommand = new SimpleCommand((obj) => HandleOK());
        }

        internal IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (favorites
                .Select(c => c.Name.ToUpper())
                .GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Any())
                errors.Add("Favorite names must be unique!");

            if (favorites
                .Any(c => c.Name.Contains(" ")))
                errors.Add("Custom command keys cannot contain space characters!");

            return errors;
        }

        public void Save()
        {
            configuration.Favorites.Clear();

            favorites
                .Select(cc => new Favorite
                {
                    Key = cc.Name,
                    Command = cc.Command,
                    Comment = cc.Comment
                })
                .ToList()
                .ForEach(c => configuration.Favorites.Add(c));
        }

        public ObservableCollection<FavoriteViewModel> Favorites => favorites;

        public ICommand AddFavoriteCommand { get; private set; }

        public ICommand OKCommand { get; private set; }

        public IConfigurationWindowAccess Access
        {
            set
            {
                access = value;
            }
        }
    }
}
