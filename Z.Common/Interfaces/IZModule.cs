using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Types;

namespace Z.Common.Interfaces
{
    public interface IZModule
    {
        IEnumerable<KeywordActionInfo> GetKeywordActions();
        string InternalName { get; }
    }
}
