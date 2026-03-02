using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Defines gamepad button identifiers following Xbox controller layout.
    /// Includes face buttons, bumpers, triggers (as digital buttons), stick presses, d-pad, and menu buttons.
    /// </summary>
    public enum GamepadButton
    {
        // ═══ Face buttons (Xbox layout: A bottom, B right, X left, Y top)
        A, B, Y, X,

        // ═══ Shoulder buttons (bumpers)
        LeftBumper, RightBumper,

        // ═══ Triggers treated as digital buttons (analog values available separately)
        // ═══ Become "pressed" when trigger position exceeds threshold
        LeftTrigger, RightTrigger,

        // ═══ Analog stick click buttons (pressing down on the sticks)
        LeftStick, RightStick,

        // ═══ D-Pad directional buttons
        DPadUp, DPadDown, DPadLeft, DPadRight,

        // ═══ Menu/system buttons
        Start,  // ═══ Menu/options button
        Back,   // ═══ View/select button
        Guide   // ═══ Home/Xbox/PlayStation button
    }
}