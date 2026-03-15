# 'Window' 'DEUTSCH'

## Zweck

Verwaltet das Anwendungsfenster als Thread-sicherer Singleton über **Silk.NET**.  
Verantwortlich für Fenster-Erstellung, OpenGL-Kontext-Initialisierung, 
Input-Setup und die Weiterleitung von Lifecycle-Events (Load, Update, Render, Close, Resize).

---

## Verantwortlichkeiten

- Bereitstellung einer globalen, thread-sicheren Singleton-Instanz (`Instance`).
- Erstellung und Konfiguration des Fensters anhand von `WindowSettings`.
- Initialisierung des **OpenGL-Kontexts** (`GL`) und des **Input-Kontexts** (`IInputContext`) beim Laden.
- Zentrieren des Fensters auf dem primären Monitor.
- Weiterleitung der Silk.NET-Lifecycle-Events an Subscriber (`Loaded`, `Updated`, `Rendered`, `OnClose`, `OnResize`).
- Blockierender Main-Loop via `Run()`.
- Sauberes Freigeben von Ressourcen über `IDisposable`.

---

## Architektur

---

## Pipeline

WindowSettings ─→ 
Initialize() ─→ 
Window.Create() ─→ 
Run() ─→ HandleLoad [GL + Input Init, Center] ─→ 
Update/Render Loop ─→ 
Close() ─→ 
Dispose()

---

## Abhängigkeiten

- **Silk.NET.Windowing** – `IWindow`, `WindowOptions`, `WindowState`
- **Silk.NET.OpenGL** – `GL` (OpenGL-API-Zugriff)
- **Silk.NET.Input** – `IInputContext` (Keyboard, Mouse, Gamepad)
- **Silk.NET.Maths** – `Vector2D<int>` (Fenstergrößen / Positionen)
- **WindowSettings** – Konfigurations-DTO (Titel, Auflösung, VSync, Fullscreen, GL-Version, Debug-Flag)
- **Logger** (`Create_your_Adventure.Source.Debug`) – Logging aller Lifecycle-Schritte

### Referenziert von:

- `KingsEngine`, `GameLoop`, `RendererManager`, `InputManager`, `MeshManager`, `ShaderManager`, `TextureManager`

---

## Geplante Implementierungen

- Keine expliziten `// TODO`-Kommentare im Code erkennbar.

---

## Zukünftige Ideen

- **Multi-Monitor-Support** – `CenterWindow()` nutzt aktuell nur den primären Monitor; Auswahl eines bestimmten Monitors wäre möglich.
- **Runtime-Resize / Fullscreen-Toggle** – Öffentliche Methoden zum dynamischen Wechsel zwischen Fenster- und Vollbildmodus.
- **WindowSettings-Persistenz** – Laden/Speichern der Settings aus einer Konfigurationsdatei (JSON/YAML).
- **GL-Context-Validation** – Prüfung, ob die angeforderte OpenGL-Version vom Treiber unterstützt wird, bevor das Fenster erstellt wird.
- **Event-Unsubscription** – Explizites Abmelden der Silk.NET-Events in `Dispose()` zur Vermeidung potenzieller Leaks.

---


 
---

# 'Titel / Class / System / Subsystem' - 'ENGLISH'

// Keep the documentation short and technical.
// Avoid long explanations.
// Which systems use this system

## Purpose

// What does the process do? (in a short text)

---

## Responsibilities

// What responsibilities does this process entail in terms of key points?

-
-
-

---

## Architecture

// What kind of class architect is that?

---

## Pipeline

// What exactly does the process look like in phases?

[TITEL] -> [TITEL] -> [TITEL] -> [TITEL]

---

## Dependencies

// What dependencies does this process require? (Listed in order of importance)

- [TITEL / CLASS]
- [TITEL / CLASS]
- [TITEL / CLASS]

---

## Planned implementations

// What else does this process need?
// What still needs to be implemented?
// Optimizations / Bug fixes / Organization / Cleanliness / Independences

---

## Future Ideas

// What else could be implemented? (In sum) 