using FavoritesModule.Common;
using FavoritesModule.Models;
using FavoritesModule.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FavoritesModule.ViewModels
{
    public class FavoriteViewModel
    {
        private readonly IFavoriteCallbacks callbacks;

        public string Name { get; set; }
        public string Comment { get; set; }
        public string Command { get; set; }

        private void HandleDelete()
        {
            callbacks.Delete(this);
        }

        private void Init()
        {
            DeleteCommand = new SimpleCommand((obj) => HandleDelete());
        }

        public FavoriteViewModel(IFavoriteCallbacks callbacks)
        {
            this.callbacks = callbacks;

            Init();
        }

        public FavoriteViewModel(IFavoriteCallbacks callbacks, Favorite sourceCommand)
        {
            this.callbacks = callbacks;

            Name = sourceCommand.Key;
            Comment = sourceCommand.Comment;
            Command = sourceCommand.Command;

            Init();
        }

        public ICommand DeleteCommand { get; private set; }
    }
}
