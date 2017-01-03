using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Api.Types
{
    public class ExecuteOptions
    {
        /// <summary>
        /// Prevents closing main window after returning.
        /// </summary>
        /// <remarks>Useful, when executing command failed.</remarks>
        public bool PreventClose { get; set; } = false;

        /// <summary>
        /// Error text to display, if any.
        /// </summary>
        public string ErrorText { get; set; } = null;
    }
}
