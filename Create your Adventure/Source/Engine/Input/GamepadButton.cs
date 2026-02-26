using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public enum GamepadButton
    {
        // ═══ Face Buttons
        A, B, Y, X,

        // ═══ Bumpers & Triggers
        LeftBumper, RightBumper,

        // ═══ Triggers (analogous, but treated as a button)
        LeftTrigger, RightTrigger,

        // ═══ Sticks
        LeftStick, RightStick,

        // ═══ D-Pad
        DPadUp, DPadDown, DPadLeft, DPadRight,

        // ═══ Special
        Start, Back, Guide
    }
}