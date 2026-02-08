using System;
using System.Collections.Generic;
using System.Text;

namespace Create_your_Adventure.Source.Engine.DevDebug
{
    /// <summary>
    /// Provides logging functionality with color-coded console output for different log levels.
    /// Supports informational messages, warnings, errors, and debug output for data inspection.
    /// </summary>
    public class Logger
    {
        // -------- Messages Methods --------
        // --- Enable Debug Messages

        /// <summary>
        /// Gets or sets a value indicating whether debug messages are enabled.
        /// Set to <c>false</c> to suppress debug output.
        /// </summary>
        public static bool EnableDebug = true;

        // --- Info Message

        /// <summary>
        /// Logs an informational message in white.
        /// Use for normal operational information such as initialization status or successful file loading.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // --- Warn Message

        /// <summary>
        /// Logs a warning message in yellow.
        /// Use for warning conditions such as connection loss or deprecated API usage.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // --- Error Message

        /// <summary>
        /// Logs an error message in red.
        /// Use for error conditions such as exceptions, missing files, or shader loading failures.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // --- Debug Message

        /// <summary>
        /// Logs a debug message in gray when debug mode is enabled.
        /// Use as a tool for inspecting data such as camera position, FPS, tick values, and chunk positions.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        public static void Debug(string message)
        {
            if (!EnableDebug) return;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
