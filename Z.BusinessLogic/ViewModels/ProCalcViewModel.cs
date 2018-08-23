using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels.Interfaces;

namespace Z.BusinessLogic.ViewModels
{
    public class ProCalcViewModel
    {
        private IProCalcWindowAccess proCalcWindowAccess;

        public ProCalcViewModel()
        {

        }

        public IProCalcWindowAccess ProCalcWindowAccess
        {
            set
            {
                if (proCalcWindowAccess != null)
                    throw new InvalidOperationException("ProCalcWindowAccess can be set only once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                proCalcWindowAccess = value;
            }
        }

        public void Initialized()
        {
            throw new NotImplementedException();
        }

        public void Dismiss()
        {
            throw new NotImplementedException();
        }

        public void Summon()
        {
            throw new NotImplementedException();
        }

        public bool EscapePressed()
        {
            throw new NotImplementedException();
        }

        public bool EnterPressed()
        {
            throw new NotImplementedException();
        }

        public void WindowLostFocus()
        {
            throw new NotImplementedException();
        }
    }
}
