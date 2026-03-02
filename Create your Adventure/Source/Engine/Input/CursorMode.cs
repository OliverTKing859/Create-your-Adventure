using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.Input
{
    /// <summary>
    /// Defines cursor display and behavior modes.
    /// Used to control cursor visibility and movement constraints for different game scenarios.
    /// </summary>
    public enum CursorMode
    {
        /// <summary>
        /// Cursor is visible and moves freely across the screen.
        /// Standard mode for UI interaction and menus.
        /// </summary>
        Visible,

        /// <summary>
        /// Cursor is hidden but still moves freely.
        /// Useful for cinematic sequences or fullscreen video playback.
        /// </summary>
        Hidden,

        /// <summary>
        /// Cursor is locked to the center of the window and hidden.
        /// Ideal for first-person camera control where mouse movement rotates the view.
        /// Provides infinite mouse movement by continuously recentering the cursor.
        /// </summary>
        Locked,

        /// <summary>
        /// Cursor is visible but cannot leave the window bounds.
        /// Prevents accidental clicks outside the game window.
        /// </summary>
        Confined,

        /// <summary>
        /// Cursor is hidden and cannot leave the window bounds.
        /// Combines hidden and confined modes.
        /// </summary>
        ConfinedHidden
    }
}