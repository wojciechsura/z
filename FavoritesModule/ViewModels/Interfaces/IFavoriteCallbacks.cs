using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoritesModule.ViewModels.Interfaces
{
    public interface IFavoriteCallbacks
    {
        void Delete(FavoriteViewModel favorite);
    }
}
