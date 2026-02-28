using Create_your_Adventure.Source.Debug;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static Create_your_Adventure.Source.Engine.Input.InputConverter;

namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    public sealed class MouseDevice : IInputDevice
    {
        private readonly IMouse? mouse;
        private InputState? state;
        private Vector2 lastPosition;

        public string Name => mouse?.Name ?? "Unknown Mouse";
        public bool IsConnected => mouse is not null;

        public IMouse? RawMouse => mouse;

        public MouseDevice(IInputContext context)
        {
            mouse = context.Mice.Count > 0 ? context.Mice[0] : null;
        }

        public void Initialize()
        {
            if (IsConnected)
            {
                lastPosition = mouse!.Position;
                Logger.Info($"[INPUT] Mouse initialized: {Name}");
            }
            else
            {
                Logger.Warn("[INPUT] No mouse detected");
            }
        }

        public void RegisterEvents(InputState inputState)
        {
            if (mouse is null) return;

            state = inputState;
            mouse.MouseDown += OnMouseDown;
            mouse.MouseUp += OnMouseUp;
            mouse.MouseMove += OnMouseMove;
            mouse.Scroll += OnScroll;
        }

        public void UnregisterEvents()
        {
            if (mouse is null) return;

            mouse.MouseDown -= OnMouseDown;
            mouse.MouseUp -= OnMouseUp;
            mouse.MouseMove -= OnMouseMove;
            mouse.Scroll -= OnScroll;
        }

        public void Poll(InputState state) { } // ═══ Mouse does not require polling

        private void OnMouseUp(IMouse m, Silk.NET.Input.MouseButton button)
        {
            var btn = MouseConverter.Convert(button);
            if (btn.HasValue)
                state?.SetMouseButtonUp(btn.Value);
        }
        private void OnMouseDown(IMouse m, Silk.NET.Input.MouseButton button)
        {
            var btn = MouseConverter.Convert(button);
            if (btn.HasValue)
                state?.SetMouseButtonUp(btn.Value);
        }

        private void OnMouseMove(IMouse m, Vector2 position)
        {
            var delta = position - lastPosition;
            lastPosition = position;

            state?.SetMousePosition(position);
            state?.SetMouseDelta(delta);
        }

        private void OnScroll(IMouse m, ScrollWheel scroll)
        {
            state?.SetScrollDelta(scroll.Y);
        }

        public void SetCursorMode(CursorMode mode)
        {
            if (mouse is null) return;

            mouse.Cursor.CursorMode = mode switch
            {
                CursorMode.Visible => Silk.NET.Input.CursorMode.Normal,
                CursorMode.Hidden => Silk.NET.Input.CursorMode.Hidden,
                CursorMode.Locked => Silk.NET.Input.CursorMode.Disabled,
                CursorMode.Confined => Silk.NET.Input.CursorMode.Normal,
                CursorMode.ConfinedHidden => Silk.NET.Input.CursorMode.Hidden,
                _ => Silk.NET.Input.CursorMode.Normal
            };
        }

        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}