using System;

namespace Z.BusinessLogic.Services.Interfaces
{
    public interface IGlobalHotkeyService
    {
        event EventHandler HotkeyHit;
    }
}