using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Events;

namespace Z.BusinessLogic.Services.EventBus
{
    public interface IEventListener<T> where T : BaseEvent
    {
        void Receive(T @event);
    }
}
