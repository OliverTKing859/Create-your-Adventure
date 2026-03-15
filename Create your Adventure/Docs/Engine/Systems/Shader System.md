# 'Shader' 'DEUTSCH'

## Zweck

API-agnostisches Shader-Management-System, das Shader-Programme über ein Singleton-Pattern lädt, kompiliert, cached und verwaltet. 
Abstrahiert die zugrunde liegende Graphics-API (aktuell OpenGL) durch ein Factory-Pattern und ein gemeinsames Interface (IShaderProgram).

---

## Verantwortlichkeiten

•	Singleton-Zugriff auf eine zentrale Shader-Verwaltung (thread-safe via Lock)
•	Factory-basierte Initialisierung des Graphics-API-Backends (erkannt über GlContext)
•	Caching aller geladenen Shader-Programme in einem Dictionary<string, IShaderProgram>
•	Kompilierung von Shader-Programmen aus Quellcode-Strings oder Dateipfaden
•	State-Tracking des aktiven Shader-Programms zur Vermeidung redundanter UseProgram(string)-Aufrufe
•	Ressourcen-Freigabe aller gecachten Shader-Programme via IDisposable

---

## Architektur

// Um welche Art von Klassenarchitektur handelt es sich?

---

## Pipeline

WindowManager.Initialize() -> 
ShaderManager.Initialize() [Factory registrieren] -> 
LoadProgram() / LoadFromFiles() [Quellcode lesen] -> 
IShaderProgram.Compile() [Vertex + Fragment kompilieren & linken] -> 
Cache speichern -> 
UseProgram() [State-optimiert aktivieren] -> 
SetUniform() [Daten an GPU übergeben] -> 
Dispose() [GPU-Ressourcen freigeben]

---

## Abhängigkeiten

•	IShaderProgram — API-agnostisches Interface für Shader-Programme (IShaderProgram.cs)
•	OpenGLShaderProgram — OpenGL-spezifische Implementierung von IShaderProgram (OpenGLShaderProgram.cs:1-313)
•	windowManager — Stellt den GL-Kontext (GlContext) für die Factory-Erstellung bereit (WindowManager.cs)
•	Logger — Statische Logging-Klasse für Info/Warn/Error-Ausgaben (Logger.cs)
•	Silk.NET.OpenGL — OpenGL-Bindings für Shader-Kompilierung, Linking und Uniform-Operationen
•	Silk.NET.Maths — Vektor- und Matrix-Typen (Vector2D, Vector3D, Vector4D, Matrix4X4)


---

## Geplante Implementierungen

•	Vulkan-Backend — Kommentar im Code: // Future extension point: else if (vulkanContext is not null) { ... } in ShaderManager.Initialize()
•	DirectX-Backend — Im XML-Doc von Initialize() erwähnt: "future: Vulkan, DirectX"

---

## Zukünftige Ideen

•	Hot-Reloading — LoadFromFiles() liest Shader von der Festplatte; ein File-Watcher könnte geänderte Shader zur Laufzeit neu kompilieren
•	Asynchrones Laden — File.ReadAllText in LoadFromFiles() ist synchron; eine async-Variante würde den Haupt-Thread entlasten
•	Shader-Validierung vor Caching — Aktuell wird bei Cache-Hit keine Re-Validierung durchgeführt; ein Integritätscheck wäre bei dynamischem Nachladen sinnvoll
•	Geometry/Compute-Shader-Support — IShaderProgram.Compile() akzeptiert nur Vertex + Fragment; das Interface könnte um weitere Shader-Stufen erweitert werden
•	Uniform-Buffer-Objects (UBOs) — Aktuell werden Uniforms einzeln gesetzt; UBOs würden gemeinsame Daten (z.B. View/Projection-Matrix) effizienter verteilen
•	Singleton-Reset/Reinitialisierung — Kein Mechanismus zum Zurücksetzen der instance; bei Kontext-Wechsel (z.B. Window-Neuaufbau) wäre das relevant

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