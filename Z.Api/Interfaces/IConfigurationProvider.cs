using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.Api.Interfaces
{
    public interface IConfigurationProvider : IDisposable
    {
        /// <summary>
        /// Displays module's configuration
        /// </summary>
        void Show();

        /// <summary>
        /// Called, when user presses "Save" button in configuration window. Should persist module's configuration.
        /// </summary>
        void Save();
    }
}
