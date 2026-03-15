# 'Gameloop' 'DEUTSCH'

## Zweck

Zentrale Orchestrierungsklasse der Engine. 
Verbindet alle Subsysteme über Event-Callbacks des windowManager (Silk.NET) 
und steuert den vollständigen Lebenszyklus von Laden, Update, Rendering bis Shutdown.

---

## Verantwortlichkeiten

•	Registrierung aller Window-Events (Loaded, Updated, Rendered, OnResize(Vector2D<int>), OnClose())
•	Sequentielle Initialisierung aller Manager-Singletons in definierter Reihenfolge (OnLoad())
•	Steuerung des Frame-Loops mit getrenntem Fixed-Tick (Physik-Slot) und variablem Update (Kamera, UI)
•	Rendering-Pipeline pro Frame: Shader-Bindung, Kamera-Uniforms, Atlas-Bindung, Mesh-Draw
•	Geordnetes Dispose aller Manager in umgekehrter Reihenfolge (LIFO) beim Schließen

---

## Architektur

---

## Pipeline

[OnLoad: Init Manager 01–07]
   -> [OnUpdate Phase 1: Time & Input BeginFrame]
   -> [OnUpdate Phase 2: ConsumeFixedTicks (Physik-Slot)]
   -> [OnUpdate Phase 3: Variable Update (Camera)]
   -> [OnUpdate Phase 4: Input EndFrame]
   -> [OnRender: BeginFrame → Shader/Camera Uniforms → BindAtlas → Draw → EndFrame]
   -> [OnClose: Dispose 07–01 LIFO]

---

## Abhängigkeiten

•	windowManager		— Fenster-Lifecycle und Event-Delegation
•	RendererManager		— OpenGL-Frame-Management (clear/swap)
•	ShaderManager		— Shader-Laden und Uniform-Verwaltung
•	TextureManager		— Block-Atlas-Erstellung und UV-Lookup
•	MeshManager			— Mesh-Erstellung (Cube-Geometrie)
•	InputManager		— Eingabe-Handling und Cursor-Locking
•	TimeManager			— DeltaTime, Fixed-Tick-Akkumulation
•	CameraManager		— View/Projection-Matrizen, Aspect-Ratio
•	AssetLoader			— Shader-Dateipfad-Auflösung
•	Logger				— Diagnostische Konsolen-Ausgabe
•	IMesh				— Mesh-Draw-Abstraktion

---

## Geplante Implementierungen

•	Physik-System: Phase 2 in OnUpdate(double) enthält leere Platzhalter-Kommentare für PhysicsSystem.Tick(fixedDt), 
	Player-Logic und Chunk-Simulation (Zeilen 117–118)
•	Weitere Meshes / Welt: Aktuell wird nur ein einzelner testCube gerendert 
	— kein Scene-Graph oder Chunk-System aktiv

---

## Zukünftige Ideen

•	Einführung eines Scene-/Entity-Systems, um testCube durch dynamisches World-Management zu ersetzen
•	Auslagerung der Render-Logik in eine dedizierte RenderPipeline-Klasse zur Entkopplung von GameLoop
•	UI-Update-Phase in OnUpdate(double) Phase 3 ergänzen (derzeit nur Kamera)
•	Event-basiertes Manager-Lifecycle-Interface (z.B. IEngineSystem.Init() / Dispose()) für automatisierte Initialisierungs-Reihenfolge

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