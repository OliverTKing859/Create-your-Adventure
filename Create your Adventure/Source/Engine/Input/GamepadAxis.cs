namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Defines gamepad analog axis identifiers for both analog sticks.
    /// Each stick has X (horizontal) and Y (vertical) axes with values ranging from -1.0 to 1.0.
    /// </summary>
    public enum GamepadAxis
    {
        // ═══ Left analog stick axes
        /// <summary>Left stick horizontal axis. -1.0 = full left, 0.0 = center, 1.0 = full right.</summary>
        LeftStickX,

        /// <summary>Left stick vertical axis. -1.0 = full down, 0.0 = center, 1.0 = full up.</summary>
        LeftStickY,

        // ═══Right analog stick axes
        /// <summary>Right stick horizontal axis. -1.0 = full left, 0.0 = center, 1.0 = full right.</summary>
        RightStickX,

        /// <summary>Right stick vertical axis. -1.0 = full down, 0.0 = center, 1.0 = full up.</summary>
        RightStickY
    }
}