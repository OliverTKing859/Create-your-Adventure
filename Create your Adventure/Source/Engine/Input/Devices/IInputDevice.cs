namespace Create_your_Adventure.Source.Engine.Input.Devices
{
    /// <summary>
    /// Defines the contract for input devices (keyboard, mouse, gamepad).
    /// Provides a unified interface for device initialization, event handling, and polling.
    /// Devices can use event-driven input (keyboard, mouse) or polling-based input (gamepad analog sticks).
    /// </summary>
    public interface IInputDevice : IDisposable
    {
        /// <summary>
        /// Gets the name of this input device.
        /// Used for identification and debugging purposes.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this device is currently connected and available.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Initializes the input device and prepares it for use.
        /// Called once during input system setup.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Registers event handlers to capture input from this device.
        /// Event-driven devices (keyboard, mouse) use this to subscribe to input events.
        /// </summary>
        /// <param name="state">The input state object where input events will be recorded.</param>
        void RegisterEvents(InputState state);

        /// <summary>
        /// Unregisters all event handlers for this device.
        /// Called during cleanup to prevent memory leaks.
        /// </summary>
        void UnregisterEvents();

        /// <summary>
        /// Polls the device for its current state.
        /// Used for analog inputs (gamepad sticks, triggers) that need continuous sampling.
        /// Event-driven devices (keyboard, mouse) can leave this empty.
        /// </summary>
        /// <param name="state">The input state object to update with current device values.</param>
        void Poll(InputState state);
    }
}