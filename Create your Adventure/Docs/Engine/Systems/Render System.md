# 'Render' 'DEUTSCH'

## Zweck

Singleton-Manager, der das Rendering-System der KingsEngine kapselt. 
Er abstrahiert die zugrunde liegende Grafik-API über ein IRenderContext-Interface und steuert den Frame-Lebenszyklus (Begin/End), 
die Viewport-Verwaltung und die Ressourcenfreigabe.

---

## Verantwortlichkeiten

•	Singleton-Instanzierung des Rendering-Subsystems (thread-safe via Lock)
•	Erzeugung und Initialisierung des konkreten IRenderContext (aktuell OpenGLRenderContext)
•	Steuerung des Frame-Lebenszyklus (BeginFrame() / EndFrame())
•	Automatische Viewport-Anpassung bei Window-Resize-Events
•	Saubere Ressourcenfreigabe via IDisposable (Unsubscribe + Context-Dispose)

---

## Architektur

---

## Pipeline

### Render
WindowManager.GlContext bereit
    -> RendererManager.Initialize() erzeugt OpenGLRenderContext
        -> OpenGLRenderContext.Initialize() (DebugOutput, ClearColor, DepthTest)
            -> Viewport auf Fenstergröße setzen

### Pro Frame
[BeginFrame] -> [Clear Color+Depth Buffer] -> [Shader/Texture/Mesh Rendering (GameLoop)] -> [EndFrame]

### Resize
[WindowManager.OnResize] -> [RendererManager.OnWindowResize] -> [SetViewport]

### Shutdown
[OnClose] -> [Unsubscribe OnResize] -> [IRenderContext.Dispose] -> [RendererManager disposed]

---

## Abhängigkeiten

•	IRenderContext – Abstraktes Interface für Grafik-API-Operationen (IRenderContext.cs)
•	OpenGLRenderContext – Einzige konkrete Implementierung von IRenderContext, nutzt Silk.NET OpenGL (OpenGLRenderContext.cs:1-171)
•	windowManager – Liefert den GL-Kontext (GlContext), die Fenstergröße (Size) und das OnResize(Vector2D<int>)-Event (WindowManager.cs)
•	Logger – Konsolen-Logging mit farbcodierten Leveln (Logger.cs)
•	GameLoop – Orchestriert den Aufruf von Initialize(), BeginFrame(), EndFrame() und Dispose() (GameLoop.cs)


---

## Geplante Implementierungen

•	Factory Pattern für Rendering-Backends – Kommentar im Code: "future: use factory pattern for different rendering APIs" (aktuell wird OpenGLRenderContext direkt instanziiert)
•	EndFrame() in OpenGLRenderContext – Aktuell ein No-Op; vorgesehen für Double-Buffering oder Post-Frame-Operationen
•	Physik-/Simulations-Ticks – OnUpdate(double) enthält Platzhalter: "Physics, Player Logic, Chunk Simulation here"

---

## Zukünftige Ideen

•	Render-Backend-Abstraktion – IRenderContext ist bereits als Multi-Backend-Interface entworfen (OpenGL, DirectX, Vulkan). Eine RenderContextFactory könnte anhand der Konfiguration das Backend wählen.
•	Render-Pass-System – BeginFrame()/EndFrame() könnte um mehrere Render-Passes erweitert werden (z. B. Shadow Pass, Post-Processing).
•	Viewport-Stack / Multi-Viewport – SetViewport(int, int, int, int) unterstützt aktuell nur einen einzelnen Fullscreen-Viewport; Split-Screen oder Editor-Viewports wären denkbar.
•	Draw-Call-Batching – Aktuell werden Meshes einzeln gezeichnet (testCube?.Draw()); ein Batch-/Command-Buffer-System im RendererManager könnte die Effizienz steigern.
•	Render-Statistiken – Draw-Call-Counter, Frame-Time-Tracking oder GPU-Memory-Monitoring über den RendererManager bereitstellen.

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