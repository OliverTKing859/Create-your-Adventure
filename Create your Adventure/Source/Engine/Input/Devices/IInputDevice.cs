using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    public interface IInputDevice : IDisposable
    {
        string Name { get; }

        bool IsConnected { get; }

        void Initialize();

        void RegisterEvents(InputState state);

        void UnregisterEvents();

        void Poll(InputState state);
    }
}