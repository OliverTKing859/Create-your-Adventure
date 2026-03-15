# 'Titel / Klasse / System / Subsystem' 'DEUTSCH'

## Zweck

Thread-sicheres Singleton, das die gesamte Eingabeverarbeitung für die Engine koordiniert.
Es initialisiert Eingabegeräte, sammelt rohe Hardwareereignisse in einem gemeinsamen Status und
stellt sowohl direkte Abfragen als auch aktionsbasierte Gameplay-Bindungen für jeden Frame bereit.

---

## Verantwortlichkeiten

•    Initialisierung und Verwaltung von KeyboardDevice, MouseDevice und GamepadDevice über Silk.NETs IInputContext
•    Koordination des Eingabe-Lebenszyklus pro Frame (BeginFrame() / EndFrame()) über alle Geräte und Zustände hinweg
•    Delegierung der Speicherung von Rohzuständen an InputState und von hochrangigen Abfragen an InputAnalyzer
•    Registrierung und Verarbeitung benannter Gameplay-Aktionen über InputRegistry
•    Steuerung der Cursorsichtbarkeit und des Sperrmodus über MouseDevice

---

## Architektur

Hardwaregeräte
→ Device Wrappers
→ InputState
→ InputAnalyzer
→ InputRegistry
→ Game Logic

---

## Pipeline

Engine-Start
WindowManager.Initialize()
→ InputManager.Initialize()

Frame Update
BeginFrame()
→ Device events write to InputState
→ EndFrame(deltaTime)
→ GamepadDevice.Poll()
→ InputRegistry.ProcessActions()
→ InputState.EndFrame()

---

## Abhängigkeiten

•	WindowManager		— stellt den vor der Initialisierung erforderlichen IInputContext bereit
•	InputState			— Rohspeicherung aller Geräteereignisse Frame für Frame
•	InputAnalyzer		— hochrangige Abfrageschicht (halten, drücken, loslassen, Kombinationen, Bewegungsvektoren)
•	InputRegistry		— benannte Aktionszuordnungen mit Ereignisverteilung
•	KeyboardDevice /
	MouseDevice /
    GamepadDevice		– Silk.NET-Geräte-Wrapper
•	Logger				– Diagnoseausgabe bei Initialisierung, Cursoränderungen und Entsorgung

---

## Geplante Implementierungen

•    GetVerticalMovement() wird auf InputManager angezeigt, hat jedoch keine XML-Zusammenfassung – wahrscheinlich eine kürzlich hinzugefügte Funktion, die noch nicht dokumentiert ist
•    Das Flag skipNextDelta in MouseDevice ist gesetzt, wird jedoch nie gelesen, was bedeutet, dass der Delta-Sprung im ersten Frame nach einem Cursor-Moduswechsel noch nicht unterdrückt wird
•    CursorMode.Confined und CursorMode.ConfinedHidden fallen beide auf Silk.NET Normal(int) / Hidden zurück – die tatsächliche Fensterbegrenzung ist noch nicht implementiert.

PrioritätProblemOrt
🔴 Bug		skipNextDelta wird nie ausgewertet				MouseDevice
🔴 Bug		DoublePress definiert aber nie implementiert	Alle IsActive()-Methoden
🔴 Bug		Tippfehler NumpadSubstract						InputConverter
🟡 Qualität	LongPressThreshold doppelt definiert			KeyBinding + InputAnalyzer
🟡 Qualität	analyzer! ohne Guard							InputManager
🟡 Qualität	LINQ-Allokation in EndFrame						InputState
🟡 Qualität	isDisposed nicht in Frame-Methoden geprüft		InputManager

---

## Zukünftige Ideen

•    Unterstützung für Hot-Plugging von Gamepads (derzeit wird nur das erste bei der Initialisierung erkannte Gamepad verwendet).
•    IsKeyReleased(KeyCode), IsMouseButtonPressed/Released und GetScrollDelta() direkt im InputManager verfügbar machen, um die Konsistenz mit den vorhandenen Shortcut-Methoden zu gewährleisten
•    Das Laden von InputRegistry-Aktionen aus einer Konfigurationsdatei ermöglichen, um vom Benutzer neu zuweisbare Bindungen zu unterstützen
•    Unterstützung für mehrere Gamepads hinzufügen, indem GamepadDevice zu einem listenbasierten Modell erweitert wird

🟢 Später	AddMouseBinding() fehlt im Fluent API			InputAction
🟢 Später	Gamepad Hotplug									GamepadDevice
🟢 Später	Singleton-Reset für Tests						InputManager
🟢 Später	LongPress/DoublePress für Mouse + Gamepad		Binding-Klassen

---



---

# 'InputManager System' 'ENGLISH'

## Purpose

Thread-safe singleton that orchestrates all input processing for the engine.
It initializes input devices, collects raw hardware events into a shared state,
and exposes both direct queries and action-based gameplay bindings each frame.

---

## Responsibilities

•	Initialize and manage KeyboardDevice, MouseDevice, and GamepadDevice via Silk.NET's IInputContext
•	Coordinate the per-frame input lifecycle (BeginFrame() / EndFrame()) across all devices and state
•	Delegate raw state storage to InputState and high-level queries to InputAnalyzer
•	Register and process named gameplay actions through InputRegistry
•	Control cursor visibility and lock mode via MouseDevice

---

## Architecture

Hardware Devices
→ Device Wrappers
→ InputState
→ InputAnalyzer
→ InputRegistry
→ Game Logic

---

## Pipeline

Engine Startup
WindowManager.Initialize()
→ InputManager.Initialize()

Frame Update
BeginFrame()
→ Device events write to InputState
→ EndFrame(deltaTime)
→ GamepadDevice.Poll()
→ InputRegistry.ProcessActions()
→ InputState.EndFrame()

---

## Dependencies

•	WindowManager		— provides the IInputContext required before initialization
•	InputState			— raw frame-by-frame storage for all device events
•	InputAnalyzer		— high-level query layer (hold, press, release, combos, movement vectors)
•	InputRegistry		— named action bindings with event dispatch
•	KeyboardDevice /
	MouseDevice /
	GamepadDevice		— Silk.NET device wrappers
•	Logger				— diagnostic output at init, cursor changes, and disposal

---

## Planned implementations

•	GetVerticalMovement() is exposed on InputManager but has no XML summary — likely a recent addition not yet documented
•	skipNextDelta flag in MouseDevice is set but never read, meaning the first-frame delta jump after a cursor mode switch is not yet suppressed
•	CursorMode.Confined and CursorMode.ConfinedHidden both fall back to Silk.NET Normal(int) / Hidden — actual window confinement is not yet implemented

---

## Future Ideas

•	Support hot-plugging of gamepads (currently only the first detected gamepad at initialization is used)
•	Expose IsKeyReleased(KeyCode), IsMouseButtonPressed/Released, and GetScrollDelta() directly on InputManager for consistency with the existing shortcut methods
•	Allow InputRegistry actions to be loaded from a config file to support user-remappable bindings
•	Add support for multiple gamepads by extending GamepadDevice to a list-based model