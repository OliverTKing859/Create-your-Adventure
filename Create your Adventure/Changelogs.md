ChangeLogs

# Alpha Version

## 0.0.0.0 Alpha | Projekt Start - 10.01.2026
- Projekt start
- Repository erstellt

## 0.0.0.1 Alpha | Hello World - 20.01.2026
- Hello World

## 0.0.1.0 Alpha | Create Window - 20.01.2026
- Erstelle ein Fenster in C# mit Silk.NET auf OpenGL Basis
	- Fenster ist auf HD Auflösung eingestellt
	- Nutzt 4.6 OpenGL
	- Hat vier Methoden
		- OnLoad
		- OnRender
		- OnUpdate
		- OnClose
	- aktuelle Hintergrundfarbe ist fast Schwarz (RGB)

## 0.0.1.1 Alpha | KHR Debug - 24.01.2026
- Erlaube es Unsafe Code zu nutzen
	- Unsafe OnLoad
- Erstellt ein KHR Debug Fenster
	- Debug Nachrichten werden in der Konsole ausgegeben

## 0.0.2.0 Alpha | Create Triangle - 30.01.2026
- Erstellt ein Dreieck
	- Nutzt VBO, VAO und EBO
	- Nutzt Shader (Vertex & Fragment)
	- Nutzt GLSL 4.6
	- Nutzt Farben für das Dreieck (RGB)

## 0.0.3.0 Alpha | Create Cube - 26.01.2026
- Erstellt einen Würfel
	- Nutzt VBO, VAO und EBO
	- Nutzt Shader (Vertex & Fragment)
	- Nutzt GLSL 4.6
	- Nutzt Farben für den Würfel (RGB)

## 0.0.4.0 Alpha | Create Camera - 26.01.2026
- Erstellt eine Kamera
	- Nutzt Matrix Transformationen (Model, View, Projection)
	- Nutzt Yaw, Pitch

## 0.0.4.1 Alpha | Camera Controls - 27.01.2026
- Erstellt Kamera Steuerung
	- Nutzt Maus (Gyro) und Tastatur (WASD, Space, Left Shift) Eingaben
	- Nutzt Delta Time für Bewegung
	- Pitch begrenzt auf -89 bis 89 Grad

## 0.0.4.2 Alpha | Acceleration / Deceleration, Smooth - 29.01.2026
- Erstellt eine Beschleunigung / Verzögerung für die Kamera Bewegung
- Nutzt Smooth Mouse Movement
- Erhöht die Max Speed der Kamera

## 0.0.4.3 Alpha |  - 29.01.2026 Mouse Sensitivity / Smoothing Factor
- Nutzt Mouse Sensitivity
- Nutzt Mouse Smoothing Faktor
- Ändert Left Shift zu Left STRG

## 0.0.4.4 Alpha | Revision of camera functions - 30.01.2026

- Ändert Acceleration / Deceleration zu eine horizontale Achse und eine vertikale Achse
- Nutzt aufbauende Geschwindigkeit und abbauende Geschwindigkeit
- Überarbeitung von Smoothing Factor

## 0.0.4.5 Alpha | Fine-tuning the camera - 30.01.2026

- Überarbeitung der Variablen Namen
- Code Optimierung
- Vorbereitung auf Abstraktion für der Kamera Klasse

## 0.0.4.6 Alpha | Smoothing reworked again - 30.01.2026

## 0.0.5.0 Alpha | Abtraction Camera To Camera Class - 01.02.2026

- Abstraktion der Kamera in eine eigene Kamera Klasse

## 0.0.5.1 Alpha | Added Commentary Improvements - 01.02.2026

- Hinzugefügt Kommentare zur Kamera Klasse

## 0.0.6.0 Alpha | Texture Support - 02.02.2026

- Integriert StbImageSharp für Textur-Dekodierung
- Erstellt Textur-Loader mit dirt.png
    - Nutzt RGBA Format
    - Nutzt Mipmaps für bessere Darstellung
    - Nutzt Nearest Filtering für pixeliges Minecraft-Feeling
- Shader überarbeitet für Texturen
    - Vertex Shader: vec2 aTexCoord
    - Fragment Shader: uniform sampler2D uTexture
- Vertex-Layout korrigiert: 24 Vertices (4 pro Seite) für korrektes UV-Mapping

## 0.0.7.0 Alpha | GPU Instancing - 02.02.2026

- Implementiert GPU-Instancing für performantes Rendern vieler Blöcke
    - 1 Draw Call für 4096 Blöcke (16×16×16 Grid)
    - Instance VBO mit Model-Matrizen (Matrix4X4[])
    - Vertex Attribute Divisor für per-instance Daten
- Shader-Verbesserungen
    - Vertex Shader: Explizite vec4-Attribute (aInstanceMatrix0-3) für Treiber-Kompatibilität
    - Fragment Shader: Konsistente vec2 vTexCoord
    - Shader-Fehlerprüfung mit CheckShaderCompileErrors() und CheckProgramLinkErrors()
- Neue Methode CreateInstanceData() generiert Instanz-Matrizen
    - Erstellt 16×16×16 Grid mit Positions-Offsets
    - Abstimmung für zentrierte Renderierung
- Entfernt uModel Uniform (jetzt per-instance aus aInstanceMatrix)

## 0.0.8.0 Alpha | Chunk Coordinate System - 03.02.2026

- Implementiert Chunk-basiertes Koordinaten-System
    - ChunkPosition (Chunk-Koordinaten in 3D)
    - LocalPosition (relative Koordinaten 0-15 innerhalb eines Chunks)
    - WorldPosition (absolute Welt-Koordinaten)
- Konvertierungs-Funktionen
    - LocalToWorld(): lokale → Welt-Koordinaten
    - WorldToChunkPosition(): Welt → Chunk-Koordinaten
    - WorldToLocal(): Welt → lokale Koordinaten
- Vorbereitung für dynamisches Chunk-Loading
- Instance-Daten basierend auf Chunk-Position berechnet

## 0.0.8.1 Alpha | Chunk Class Abstraction & Naming Conventions - 04.02.2026

- Abstrahiert Instance-Daten in Chunk-Klasse
    - `InstanceMatrices` und `InstanceCount` sind nun Properties der Chunk-Klasse
    - `RebuildInstanceData()` ist öffentliche Methode für dynamische Updates
- Implementiert C# Naming Conventions
    - Constants: `ChunkSize` (PascalCase)
    - Properties: `ChunkPosition` (PascalCase)
    - Parameter: `chunkPosition` (camelCase)
- Constructor korrekt dokumentiert
    - Chunk wird automatisch initialisiert beim `new Chunk()`
- Program.cs angepasst
    - Liest `chunk.InstanceMatrices` und `chunk.InstanceCount` für Rendering
    - Trennung von Logik (Chunk) und Rendering (Program)
- Typo behoben: `RebuildInsanceData()` → `RebuildInstanceData()`

## 0.0.8.2 Alpha | XML Documentation & AssetLoader - 04.02.2026

- Implementiert XML-Dokumentation (Summaries) für alle Public-Methoden
    - Chunk.cs: Klassen-Summary, Properties und Methoden dokumentiert
    - Camera.cs: Klassen-Summary mit Interface-Hinweis, alle Public-Methoden dokumentiert
    - AssetLoader.cs: Klassen-Summary mit Suchreihenfolge, Asset-Retrieval-Methoden dokumentiert
- AssetLoader-Klasse verbessert
    - Sucht Assets in `assets/base/` und `assets/modded/`
    - Caching für schnelle wiederholte Lookups
    - Methoden: GetAudioPath, GetModelPath, GetShaderPath, GetTexturePath
- Typos behoben
    - `RebuildInsanceData()` → `RebuildInstanceData()`
    - `WorldTolocal()` → `WorldToLocal()`
- Code-Stil vereinheitlicht (Englische Summaries, Inline-Kommentare)

## 0.0.8.3 Alpha | Logger System Implementation - 05.02.2026

- Implementiert Logger-System für strukturierte Debug-Ausgaben
    - Logger-Klasse mit Info, Warn, Error und Debug Methoden
    - Farbcodierte Konsolen-Ausgabe (Weiß, Gelb, Rot, Grau)
    - EnableDebug Toggle zur Kontrolle von Debug-Nachrichten
    - XML-Dokumentation für alle Logger-Methoden
- Kategorisierte Log-Nachrichten in allen Hauptklassen
    - [ENGINE]: Anwendungs-Start/-Shutdown
    - [OPENGL]: GL-Kontext, VAO, VBO, EBO, Depth Testing
    - [SHADER]: Vertex/Fragment Compilation, Linking, Uniform-Locations
    - [TEXTURE]: Asset-Loading, Dimensionen, Fehlerbehandlung
    - [CHUNK]: Chunk-Erstellung, Instance-Daten, World-Area
    - [ASSETLOADER]: Asset-Suche, Cache, Modded-Assets
    - [CAMERA]: Initialisierung, Projection-Matrix, Mouse-Input
    - [INPUT]: Keyboard/Mouse-Geräte-Erkennung
- Verbesserte Fehlerbehandlung
    - OpenGL Debug Callback kategorisiert nach Severity (High/Medium/Low)
    - Warnungen bei fehlenden Input-Geräten oder ungültigen Viewport-Dimensionen
    - Error-Logs für fehlgeschlagene Texture-/Shader-Loads
- Console.WriteLine() durch Logger-Aufrufe ersetzt für konsistente Ausgaben

## 0.0.8.4 Alpha | Infinite World Architecture - 06.02.2026

- Implementiert 64-Bit Chunk-Koordinaten für unendliche Weltgenerierung
    - `ChunkPosition` nutzt jetzt `Vector3D<long>` statt `Vector3D<int>`
    - Ermöglicht ±9,2 Trillionen Chunks (±147 Billionen Blöcke pro Achse)
    - Übertrifft Minecraft's 30 Millionen Block Limit deutlich
- Architektur-Verbesserung für Koordinatensysteme
    - Intern: `long` für Chunk-Positionen, `int` für lokale Positionen (0-15)
    - Rendering: `float` für Weltpositionen (aktuell)
    - Vorbereitung für kamera-relatives Rendering zur Vermeidung von Float-Präzisionsproblemen
- Methoden zu public static geändert
    - `WorldToChunkPosition()` und `WorldToLocal()` sind jetzt öffentlich zugänglich
    - Typo korrigiert: `WorldTolocal()` → `WorldToLocal()`
- Erweiterte Dokumentation für zukünftige Optimierungen
    - TODO-Kommentare für Block-Daten-Speicherung
    - TODO-Kommentare für Visibility Culling (nur sichtbare Blöcke)
    - TODO-Kommentare für Face Culling (versteckte Flächen nicht rendern)
    - TODO-Kommentare für Greedy Meshing (Flächen zusammenfassen)
    - Kommentierte Beispiel-Methode `LocalToWorldRelative()` für World-Offset Rendering
- Architektur-Hinweise in XML-Dokumentation
    - Chunk-Klasse soll zukünftig reiner Daten-Container werden (Rendering-Logik separieren)
    - Aktuelle Instanz-Matrix-Generierung ist temporär für frühe Entwicklung

## 0.0.9.0 Alpha | ImGui Debug Display - 08.02.2026

- Implementiert ImGui-basiertes Debug-Display-System
    - FPS-Tracking mit Min/Max-Statistiken
    - Frame-Time-Messung in Millisekunden
    - Dynamische Farbcodierung basierend auf Performance
        - Grün: ≥60 FPS
        - Gelb: ≥30 FPS
        - Rot: <30 FPS
    - ImGui-Fenster mit decorativen Optionen
        - Automatisches Resizing
        - Transparenter Hintergrund (Alpha 0.35)
        - Positioniert in oberer linker Ecke (10, 10)
    - Statistik-Reset-Button zur Datenbereinigung
- DebugDisplay.cs als Static-Klasse mit Update() und RenderImGui() Methoden
- Properties für FPS-Daten: CurrentFPS, MinFPS, MaxFPS, FrameTimeMs, ShowDebugWindow

## 0.0.9.1 Alpha | Fix Everything Bullshit - 08.02.2026

- DebugDisplay.cs allgemeine Verbesserungen und Bugfixes
    - FPS-Farbcodierung zuverlässiger implementiert
    - ImGui-Layout und Abstände optimiert
    - Reset Stats Funktionalität stabilisiert
    - Floating-Point-Formatierung konsistent gestaltet

## 0.0.9.2 Alpha | ImGui Debug Display - 08.02.2026

- Verbesserungen am ImGui-Debug-Display
    - Stabile FPS-Berechnung mit Akkumulator und 1s-Updateintervall
    - Min-/Max-FPS-Tracking und Frame-Time-Anzeige in Millisekunden
    - Dynamische Farbcodierung der FPS (Grün ≥60, Gelb ≥30, Rot <30)
    - Reset-Stats-Button repariert und Stabilität erhöht
    - ImGui-Fensterflags, Positionierung und Transparenz (Alpha 0.35) optimiert

## 0.0.10.0 Alpha | Refactor: WindowManager Abstraction - 08.02.2026 ⚠️ **BROKEN**
- **Status:** ⚠️ Programm läuft aktuell **NICHT** - In Entwicklung
- Beginn der Architektur-Refaktorierung
  - Erstellt `WindowManager` als Singleton Pattern
    - Zentralisiert Fenster, GL Context und Input Verwaltung
    - Entfernt globale statische Variablen für Window/GL/Input
    - Event-basierte Architektur (OnLoad, OnUpdate, OnRender, OnClose)
  - Erstellt `WindowSettings` für flexible Fenster-Konfiguration
    - Title, Width, Height, GL Version konfigurierbar
    - Debug Context optional
  - Program.Main() vereinfacht zu 10 Zeilen
- **Bekannte Probleme:**
  - ❌ `OnLoad()` greift immer noch auf statische `gl` und `window` zu (sind null)
  - ❌ `OnLoad()` nutzt nicht `WindowManager.Instance.GL`
  - ❌ Input-Variablen (`keyboard`, `mouse`) sind nicht initialisiert
  - ❌ InputManager nicht implementiert
  - ❌ ShaderManager nicht implementiert
  - ❌ TextureManager nicht implementiert
  - ❌ RenderPipeline nicht implementiert
- **Nächste Schritte:**
  - InputManager implementieren
  - OnLoad() mit WindowManager kompatibel machen
  - Shader & Texture Manager abstrahieren

  ## 0.0.11.0 Alpha | Refactor: RenderManager Abstraction - 09.02.2026

- Implementiert RenderManager als zentrale Render-Verwaltung (Singleton Pattern)
  - Abstraktion der Rendering-Pipeline für verschiedene Graphics APIs
  - Event-basierte Architektur (OnWindowResize Integration)
- Erstellt IRenderContext Interface
  - Graphics API agnostisch
  - Definiert Standard-Rendering-Operationen (BeginFrame, EndFrame, SetViewport, etc.)
- OpenGLRenderContext Implementation
  - Konkrete OpenGL-Implementierung des IRenderContext Interfaces
  - Debug Output Aktivierung
  - Viewport-Management mit automatischem Window-Resize-Handling
  - Clear Color und Depth Test Konfiguration
- RenderManager Integration mit WindowManager
  - Zugriff auf GL Context vom WindowManager
  - Automatisches Viewport-Update bei Window-Resize
  - Proper Disposal bei Shutdown
- **Status:** ⚠️ RenderManager erstellt, Integration in Program.cs noch ausstehend
- **Nächste Schritte:** ShaderManager, TextureLoader, RenderPipeline Abstraktion