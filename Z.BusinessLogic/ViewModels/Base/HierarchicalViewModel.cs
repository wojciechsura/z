using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.ViewModels.Base
{
    public abstract class HierarchicalViewModel<T>: BaseViewModel
        where T : HierarchicalViewModel<T>
    {
        public abstract T Parent { get; }
    }
}
