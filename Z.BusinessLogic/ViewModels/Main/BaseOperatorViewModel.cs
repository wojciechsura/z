using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.ViewModels.Main
{
    public abstract class BaseOperatorViewModel
    {
        public abstract bool EscapePressed();
        public abstract bool SpacePressed();
        public abstract bool BackspacePressed();
        public abstract bool TabPressed();
        public abstract bool EnterPressed();
        public abstract bool UpPressed();
        public abstract bool DownPressed();
        public abstract void WindowLostFocus();
        public abstract bool Closing();
        public abstract void Initialized();
    }
}
