using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Events
{
    public class PositionChangedEvent : BaseEvent
    {
        public PositionChangedEvent(int x, int y, object origin)
        {
            X = x;
            Y = y;
            Origin = origin;
        }

        public object Origin { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
    }
}
