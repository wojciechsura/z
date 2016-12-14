using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Common;
using Z.Models.Configuration;

namespace Z.BusinessLogic.Services.Interfaces
{
    public interface IConfigurationService
    {
        Configuration Configuration { get; }
    }
}
