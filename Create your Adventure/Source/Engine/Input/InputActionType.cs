namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Defines the types of input actions that can be detected and triggered.
    /// Determines how input bindings are evaluated (instant press, continuous hold, release, etc.).
    /// </summary>
    public enum InputActionType
    {
        /// <summary>
        /// Triggered once on the frame the input becomes active (button down, key down).
        /// Used for single actions like jumping, shooting, or menu selection.
        /// </summary>
        Pressed,

        /// <summary>
        /// Triggered continuously while the input remains active.
        /// Used for continuous actions like movement, charging attacks, or holding breath.
        /// </summary>
        Held,

        /// <summary>
        /// Triggered once on the frame the input becomes inactive (button up, key up).
        /// Used for release mechanics like bow shooting or ending a charge attack.
        /// </summary>
        Released,

        /// <summary>
        /// Triggered when the input has been held for a threshold duration (typically 0.5 seconds).
        /// Used for context-sensitive actions like opening radial menus or alternative interactions.
        /// </summary>
        LongPress,

        /// <summary>
        /// Triggered when the input is pressed twice rapidly within a time window.
        /// Used for dash mechanics, double-jump, or quick commands.
        /// </summary>
        DoublePress,

        /// <summary>
        /// Triggered continuously for analog inputs (gamepad sticks, triggers).
        /// Provides a continuous value range (-1.0 to 1.0) rather than binary on/off.
        /// Used for smooth movement, camera control, or variable speed actions.
        /// </summary>
        Axis
    }
}