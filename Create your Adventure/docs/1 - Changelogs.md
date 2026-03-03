ChangeLogs

# Alpha Version

## 0.0.0.0 Alpha | Projekt Start - 10.01.2026
- Projekt start
- Repository erstellt

## 0.1.0.0 Alpha | Hello World - 20.01.2026
- Hello World

## 0.1.1.0 Alpha | Create Window - 20.01.2026
- Erstelle ein Fenster in C# mit Silk.NET auf OpenGL Basis
	- Fenster ist auf HD Auflösung eingestellt
	- Nutzt 4.6 OpenGL
	- Hat vier Methoden
		- OnLoad
		- OnRender
		- OnUpdate
		- OnClose
	- aktuelle Hintergrundfarbe ist fast Schwarz (RGB)

## 0.1.2.0 Alpha | KHR Debug - 24.01.2026
- Erlaube es Unsafe Code zu nutzen
	- Unsafe OnLoad
- Erstellt ein KHR Debug Fenster
	- Debug Nachrichten werden in der Konsole ausgegeben

## 0.1.3.0 Alpha | Create Triangle - 30.01.2026
- Erstellt ein Dreieck
	- Nutzt VBO, VAO und EBO
	- Nutzt Shader (Vertex & Fragment)
	- Nutzt GLSL 4.6
	- Nutzt Farben für das Dreieck (RGB)

## 0.1.4.0 Alpha | Create Cube - 26.01.2026
- Erstellt einen Würfel
	- Nutzt VBO, VAO und EBO
	- Nutzt Shader (Vertex & Fragment)
	- Nutzt GLSL 4.6
	- Nutzt Farben für den Würfel (RGB)

## 0.1.5.0 Alpha | Create Camera - 26.01.2026
- Erstellt eine Kamera
	- Nutzt Matrix Transformationen (Model, View, Projection)
	- Nutzt Yaw, Pitch

## 0.1.6.0 Alpha | Camera Controls - 27.01.2026
- Erstellt Kamera Steuerung
	- Nutzt Maus (Gyro) und Tastatur (WASD, Space, Left Shift) Eingaben
	- Nutzt Delta Time für Bewegung
	- Pitch begrenzt auf -89 bis 89 Grad

## 0.1.7.0 Alpha | Acceleration / Deceleration, Smooth - 29.01.2026
- Erstellt eine Beschleunigung / Verzögerung für die Kamera Bewegung
- Nutzt Smooth Mouse Movement
- Erhöht die Max Speed der Kamera

## 0.1.8.0 Alpha |  - 29.01.2026 Mouse Sensitivity / Smoothing Factor
- Nutzt Mouse Sensitivity
- Nutzt Mouse Smoothing Faktor
- Ändert Left Shift zu Left STRG

## 0.1.9.0 Alpha | Revision of camera functions - 30.01.2026

- Ändert Acceleration / Deceleration zu eine horizontale Achse und eine vertikale Achse
- Nutzt aufbauende Geschwindigkeit und abbauende Geschwindigkeit
- Überarbeitung von Smoothing Factor

## 0.1.10.0 Alpha | Fine-tuning the camera - 30.01.2026

- Überarbeitung der Variablen Namen
- Code Optimierung
- Vorbereitung auf Abstraktion für der Kamera Klasse

## 0.1.11.0 Alpha | Smoothing reworked again - 30.01.2026

## 0.1.12.0 Alpha | Abtraction Camera To Camera Class - 01.02.2026

- Abstraktion der Kamera in eine eigene Kamera Klasse

## 0.1.13.0 Alpha | Added Commentary Improvements - 01.02.2026

- Hinzugefügt Kommentare zur Kamera Klasse

## 0.1.14.0 Alpha | Texture Support - 02.02.2026

- Integriert StbImageSharp für Textur-Dekodierung
- Erstellt Textur-Loader mit dirt.png
    - Nutzt RGBA Format
    - Nutzt Mipmaps für bessere Darstellung
    - Nutzt Nearest Filtering für pixeliges Minecraft-Feeling
- Shader überarbeitet für Texturen
    - Vertex Shader: vec2 aTexCoord
    - Fragment Shader: uniform sampler2D uTexture
- Vertex-Layout korrigiert: 24 Vertices (4 pro Seite) für korrektes UV-Mapping

## 0.1.15.0 Alpha | GPU Instancing - 02.02.2026

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

## 0.1.16.0 Alpha | Chunk Coordinate System - 03.02.2026

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

## 0.1.16.0 Alpha | Chunk Class Abstraction & Naming Conventions - 04.02.2026

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

## 0.2.0.0 Alpha | XML Documentation & AssetLoader - 04.02.2026

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

## 0.2.1.0 Alpha | Logger System Implementation - 05.02.2026

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

## 0.2.2.0 Alpha | Infinite World Architecture - 06.02.2026

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

## 0.2.3.0 Alpha | ImGui Debug Display - 08.02.2026

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

## 0.2.3.1 Alpha | Fix Everything Bullshit - 08.02.2026

- DebugDisplay.cs allgemeine Verbesserungen und Bugfixes
    - FPS-Farbcodierung zuverlässiger implementiert
    - ImGui-Layout und Abstände optimiert
    - Reset Stats Funktionalität stabilisiert
    - Floating-Point-Formatierung konsistent gestaltet

## 0.2.4.1 Alpha | ImGui Debug Display - 08.02.2026

- Verbesserungen am ImGui-Debug-Display
    - Stabile FPS-Berechnung mit Akkumulator und 1s-Updateintervall
    - Min-/Max-FPS-Tracking und Frame-Time-Anzeige in Millisekunden
    - Dynamische Farbcodierung der FPS (Grün ≥60, Gelb ≥30, Rot <30)
    - Reset-Stats-Button repariert und Stabilität erhöht
    - ImGui-Fensterflags, Positionierung und Transparenz (Alpha 0.35) optimiert

## 0.3.0.0 Alpha | Refactor: WindowManager Abstraction - 08.02.2026 ⚠️ **BROKEN**
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

## 0.3.1.0 Alpha | Refactor: RenderManager Abstraction - 09.02.2026

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

## 0.3.2.0 Alpha | ShaderProgram Class Finalization - 13.02.2026

- Implementiert vollständige ShaderProgram Klasse mit Compile und Link Funktionalität
    - Vertex und Fragment Shader Kompilierung mit Fehlerprüfung
    - Automatisches Program Linking mit Status-Validierung
    - XML-Dokumentation für alle Public-Methoden
- Uniform-Management-System
    - Uniform-Caching für Optimierung
    - GetUniformLocation() mit Dictionary-Cache für wiederholte Lookups
    - SetUniform() Überladungen für int, float, Vector2D, Vector3D, Vector4D, Matrix4X4
- Fehlerbehandlung und Logging
    - [SHADER] kategorisierte Log-Nachrichten
    - Detaillierte Kompilierungs- und Linking-Fehlerberichte
    - CheckShaderCompileStatus() und CheckProgramLinkStatus() Validierung
- Ressourcenmanagement
    - Proper Disposal Pattern mit isDisposed Flag
    - Automatische Bereinigung von Shader- und Program-Objekten
    - Logger-Output bei Ressourcen-Freigabe

- **Status:** ⚠️ ShaderProgram erstellt, Integration in Program.cs noch ausstehend
- **Nächste Schritte:** ShaderManager, TextureLoader, RenderPipeline Abstraktion

## 0.3.3.0 Alpha | ShaderManager Implementation & Documentation - 15.02.2026

- Implementiert ShaderManager als Graphics API abstraction layer (Singleton Pattern)
    - Thread-safe Initialization mit Lock-Pattern
    - Shader-Caching für wiederholte Lookups (LoadProgram, GetProgram)
    - Aktive Program-Verwaltung (UseProgram, UnbindProgram)
- Vollständige XML-Dokumentation
    - Public Methods dokumentiert (LoadProgram, LoadFromFiles, GetProgram, UseProgram)
    - Parameter und Return-Types erklärt
    - Exception-Dokumentation für InvalidOperationException
- Detaillierte Inline-Comments
    - Caching-Logik erklärt
    - Initialization Flow dokumentiert
    - Error-Handling nachvollziehbar
- Integration mit ShaderProgram
    - ShaderManager verwaltet Lifecycle von ShaderProgram Instanzen
    - Automatic Disposal bei Shutdown
    - Program.cs nutzt ShaderManager statt direkter ShaderProgram-Verwaltung
- Logger-Integration
    - [SHADER] kategorisierte Log-Nachrichten
    - Cached Program Count tracking
    - Initialization und Disposal Logging
- **Status:** ✅ ShaderManager vollständig implementiert - Graphics API unabhängig
- **Nächste Schritte:** TextureManager Abstraktion, RenderPipeline Abstraktion, VAO/VBO/EBO Manager

## 0.3.4.0 Alpha | API Abstraction: Graphics API Agnostic Shader System - 15.02.2026

- Implementiert vollständiges Graphics API Abstraction Layer für Shader-Verwaltung
  - IShaderProgram Interface definiert API-unabhängigen Vertrag
    - Alle Methoden (Compile, Use, SetUniform) sind API-agnostisch
    - Implementierungen kapseln Graphics API Details
  - ShaderManager arbeitet nur mit IShaderProgram Interface
    - Factory Pattern für automatische Backend-Erkennung
    - Manager weiß nicht, ob OpenGL, Vulkan oder DirectX dahinter steckt
    - Erweiterbar auf zukünftige Graphics APIs
  - OpenGLShaderProgram als konkrete OpenGL-Implementierung
    - GL-Context wird injiziert (nicht global)
    - Shader-Kompilierung mit Fehlerbehandlung
    - Uniform-Caching für Performance-Optimierung
- **Architektur-Verbesserung:** Vollständige Trennung von Logik (Manager) und API-Details
- **Erweiterbarkeit:** Neue Graphics APIs können hinzugefügt werden ohne Manager zu ändern

## 0.3.5.0 Alpha | Shader System Abstraction & File Loading - 15.02.2026

- **Implementiert vollständiges Shader-Abstraktions-System mit API-Unabhängigkeit**
  - IShaderProgram Interface definiert Graphics API agnostischen Vertrag
    - Compile(), Use(), SetUniform() Methoden für alle APIs
    - Unterstützt int, float, Vector2/3/4, Matrix4x4 Uniforms
  - ShaderManager als zentrale Shader-Verwaltung (Singleton Pattern)
    - Factory Pattern für automatische Backend-Erkennung (OpenGL, zukünftig Vulkan/DirectX)
    - Thread-safe Shader-Caching mit Dictionary<string, IShaderProgram>
    - LoadProgram() und LoadFromFiles() für flexible Shader-Erstellung
    - UseProgram() mit State-Optimierung (bindet nur wenn nicht bereits aktiv)
  - OpenGLShaderProgram als konkrete OpenGL-Implementierung
    - GLSL Shader-Kompilierung mit Fehlerprüfung
    - Uniform-Location-Caching für Performance-Optimierung
    - Automatische Ressourcen-Bereinigung mit Dispose Pattern
  
- **Shader-Dateisystem mit AssetLoader Integration**
  - Shader aus .vert und .frag Dateien laden
  - Ordnerstruktur: assets/base/shaders/opengl/
  - basic.vert (Vertex Shader mit Instancing-Support)
  - basic.frag (Fragment Shader mit Texture-Sampling)
  - AssetLoader.GetShaderPath() für automatische Asset-Lokalisierung
  
- **Vollständige XML-Dokumentation**
  - Alle Public Methods dokumentiert (IShaderProgram, ShaderManager, OpenGLShaderProgram)
  - Parameter, Return-Types und Exceptions erklärt
  - Detaillierte Inline-Comments für Caching, Initialization und Error-Handling
  
- **Architektur-Verbesserungen**
  - Vollständige Trennung von Logik (Manager) und API-Details (OpenGLShaderProgram)
  - Program.cs reduziert auf 15 Zeilen Shader-Code (vorher >100 Zeilen)
  - Erweiterbar auf zukünftige Graphics APIs ohne Manager-Änderungen
  - Logger-Integration: [SHADER] kategorisierte Log-Nachrichten
  
- **Performance-Optimierungen**
  - Uniform-Location-Caching vermeidet wiederholte OpenGL-Queries
  - Shader-Caching verhindert Neu-Kompilierung bei mehrfachem Laden
  - State-Tracking für UseProgram() (bindet nur wenn nötig)

- **Initialisierungs-Reihenfolge etabliert**
  - OnLoad: WindowManager → RendererManager → ShaderManager
  - OnClose: ShaderManager → RendererManager (reverse order)
  - Proper Dependency Chain für GL-Context-Abhängigkeiten

- **Status:** ✅ Shader-System vollständig funktional und getestet
- **Console-Output bestätigt:**
  - ShaderManager initialized (OpenGL backend)
  - Assets loaded from files (basic.vert, basic.frag)
  - Shader compiled successfully (Handle: 3)
  - 1 program cached

- **Nächste Schritte:** TextureManager Abstraktion, Input Manager, VAO/VBO Pipeline Manager

## 0.4.0.0 Alpha | Texture System - Part 1 (API Abstraction) - 16.02.2026 ⚠️ **IN ENTWICKLUNG**

- **Status:** ⚠️ Interfaces und Manager fertig, aber noch nicht bereit zum Starten
- Implementiert vollständiges Texture-Abstraktions-System mit API-Unabhängigkeit
  - ITexture Interface definiert Graphics API agnostischen Vertrag
    - LoadFromFile() und LoadFromData() für flexible Texture-Erstellung
    - Bind() und Unbind() für Rendering-Integration
  - ITextureAtlas Interface definiert Atlas-Management
    - AddTexture() für dynamische Texture-Hinzufügung
    - Build() zum Generieren der finalen Atlas
    - GetRegion() für UV-Koordinaten-Abfragen
  - AtlasRegion Struct für Regions-Verwaltung
    - Pixel-Koordinaten: X, Y, Width, Height
    - Normalisierte UV-Koordinaten: U0, V0, U1, V1
    - Factory Method für automatische UV-Berechnung
  - TextureSettings Record für Texture-Konfiguration
    - MinFilter, MagFilter (Filtering-Verhalten)
    - WrapS, WrapT (Wrapping-Verhalten)
    - GenerateMipmaps, FlipVertically (Lade-Optionen)
    - Presets: PixelArt, Smooth, Atlas
  - TextureManager als zentrale Verwaltung (Singleton Pattern)
    - Factory Pattern für automatische Backend-Erkennung (OpenGL, zukünftig Vulkan/DirectX)
    - Thread-safe Initialization mit Lock-Pattern
    - Texture-Caching für single Texturen
    - Atlas-Caching für Texture Atlases
    - Binding State Tracking zur Optimierung
    - Integration mit AssetLoader für automatische Asset-Lokalisierung
  - OpenGLTextureAtlas als konkrete OpenGL-Implementierung
    - Multi-Texture-Atlas-Building aus Pending-Texturen
    - StbImageSharp Integration für RGBA-Bildladung
    - Grid-basierter Packing-Algorithmus für gleich große Texturen
    - Power-of-Two Atlas-Größe (256-4096 Pixel)
    - Region-Tracking für UV-Mapping
    - OpenGL Texture-Erstellung mit Parameter-Konfiguration (Filtering, Wrapping, Mipmaps)
- XML-Dokumentation für alle Public-Interfaces und Klassen
- Logger-Integration mit [TEXTURE] Kategorie
- **Status:** ⚠️ Interfaces und Manager sind funktional, OpenGLTextureAtlas benötigt noch:
  - ❌ Helper-Methoden Implementation (NextPowerOfTwo, ConvertWrapMode, ConvertMinFilter, ConvertMagFilter)
  - ❌ Integration in Program.cs
  - ❌ Testing der Atlas-Building-Funktionalität
- **Nächste Schritte:** 
  - Helper-Methoden finalisieren
  - OpenGLTexture2D (Single Texture) implementieren
  - TextureManager in Program.cs integrieren
  - Atlas-Rendering und Block-UV-System testen

## 0.4.1.0 Alpha | Texture System Implementation - Part 2 - 16.02.2026

- **Vollständiges Texture-Management-System implementiert**
  - ITexture Interface für API-Abstraktion (OpenGL, Vulkan, DirectX)
  - ITextureAtlas Interface für Atlas-Verwaltung
  - TextureManager Singleton mit Factory-Pattern
  - API-agnostische Enum-Wrapper (TexWrapMode, TexMinFilter, TexMagFilter)

- **OpenGL-Implementierungen**
  - OpenGLTexture2D für einzelne 2D-Texturen
    - LoadFromFile() mit StbImageSharp
    - LoadFromData() für Pixel-Daten
    - Mipmaps, Filtering, Wrapping konfigurierbar
  - OpenGLTextureAtlas mit Grid-Packing
    - Automatische Größenberechnung (Potenz von 2)
    - Einfaches Grid-Layout für gleiche Texture-Größen
    - UV-Koordinaten normalisiert (0.0-1.0)

- **TextureSettings & Presets**
  - TexWrapMode: Repeat, ClampToEdge, MirroredRepeat
  - TexMinFilter: 6 verschiedene Optionen inkl. Mipmaps
  - TexMagFilter: Nearest, Linear
  - Presets: PixelArt (Minecraft-Style), Smooth, Atlas

- **TextureManager Features**
  - Singleton Pattern mit Lock-Protection
  - Dual-Cache für Texturen und Atlases
  - BuildBlockAtlas() für schnelles Laden aus assets/base/textures/blocks/
  - BindTexture() / BindAtlas() mit State-Tracking
  - GetBlockUV() für UV-Zugriff
  - Korrekte Initialization nach ShaderManager
  - Korrekte Disposal vor ShaderManager (LIFO)

- **Asset-Integration**
  - Automatisches Laden von PNG-Dateien aus Ordnern
  - Verwendung von AssetLoader.GetTexturePath()
  - Fehlerbehandlung für fehlende Dateien
  - Logging für alle Schritte

- **Testing & Validation**
  - ✅ Atlas wird korrekt aus 2 Test-Texturen (debug, dirt) gebaut
  - ✅ UV-Koordinaten korrekt berechnet
  - ✅ Atlas-Größe korrekt (256x256 für 2×16x16 Texturen)
  - ✅ Initialization & Disposal in korrekter Reihenfolge

- **Status:** ✅ TextureSystem vollständig funktional
- **Nächste Schritte:** VAO/VBO Manager, RenderPipeline, Chunk-Mesh-Integration

## 0.5.0.0 Alpha | Mesh System - Part 1 (VAO/VBO/EBO Abstraction) - 20.02.2026 🔄 **IN ARBEIT**

- **Status:** 🔄 In Arbeit - Interfaces und OpenGL-Implementierung abgeschlossen, Integration pending
- Implementiert vollständiges Mesh-Abstraktions-System mit API-Unabhängigkeit
  - IMesh Interface definiert Graphics API agnostischen Vertrag
    - Create() Überladungen für Vertices und Indices
    - Bind/Unbind für Rendering-Integration
    - Draw() für Standard-Rendering
    - DrawInstanced() für GPU-Instancing
  - IVertexBuffer Interface für VBO-Management
    - SetData<T>() für initiale Buffer-Erstellung
    - UpdateData<T>() für partielle Updates
    - Properties: VertexCount, SizeInBytes, Layout
  - IIndexBuffer Interface für EBO-Management
    - SetData() für Index-Daten
    - Properties: IndexCount, SizeInBytes
  - VertexAttribute Struct für Vertex-Attribute-Definition
    - Name, Location, ComponentCount, Type, Normalized, Offset
    - Factory Methods: Position(), TexCoord(), Normal()
  - VertexLayout Klasse für flexible Layout-Definition
    - Add() Method für Attribute-Chaining
    - Automatische Stride-Berechnung
    - Presets: PositionTexCoord(), PositionTexCoordNormal()
  - VertexAttributeType Enum: Float, Int, UInt, Byte

- **OpenGL-Implementierungen**
  - OpenGLMesh für VAO-Management
    - Create<T>(vertices, layout) für nicht-indizierte Meshes
    - Create<T>(vertices, indices, layout) für indizierte Meshes
    - SetupVertexAttributes() für automatische Attribute-Konfiguration
    - Bind/Unbind mit VAO State Management
    - Draw() für Standard-Rendering
    - DrawInstanced() für GPU-Instancing (4096 Blöcke)
    - Vollständiges Dispose Pattern
  - OpenGLVertexBuffer für VBO-Verwaltung
    - Generic SetData<T>() für flexible Daten-Typen
    - UpdateData<T>() für Partial Updates
    - Vertexcount und Stride Tracking
  - OpenGLIndexBuffer für EBO-Verwaltung
    - SetData() für Index-Daten
    - IndexCount Tracking
  - MeshManager Singleton
    - Factory Pattern für API-Abstraktion (OpenGL, zukünftig Vulkan/DirectX)
    - Thread-safe Initialization mit Lock-Pattern
    - Mesh-Caching für Performance
    - Disposal Pattern in korrekter Reihenfolge

- **Architektur & Design Patterns**
  - VAO State Pattern: 1 Bind statt VBO + EBO + Attributes
  - Factory Pattern in MeshManager für API-Abstraction
  - Generic<T> für flexible Datentypen
  - Builder Pattern in VertexLayout
  - Proper Resource Management (LIFO Disposal)

- **Logging & Debugging**
  - [MESH] kategorisierte Log-Nachrichten
  - Ausführliches Logging bei Creation, Binding, Draw-Calls
  - Warnungen bei uninitialisiertem Mesh oder doppelter Initialisierung

- **Performance-Features**
  - GPU-Instancing Support (DrawInstanced)
  - Indexed vs Non-Indexed Rendering Optionen
  - Vertex Attribute Divisor bereit für Instance-Daten
  - Efficient State Management durch VAO

- **Status:** 🔄 In Arbeit - Folgende Tasks ausstehend:
  - ❌ MeshManager in Program.cs integrieren
  - ❌ Chunk-Mesh-Integration (Chunk → OpenGLMesh)
  - ❌ Shader-Uniform für Instance-Matrizen
  - ❌ Testing mit echten Chunk-Daten
  - ❌ Render-Pipeline für Mesh-Drawing

- **Nächste Schritte:** 
  - MeshManager Initialization in OnLoad
  - Chunk.CreateMesh() Methode
  - Render-Loop Integration
  - Performance-Benchmarking mit 4096+ Meshes
  - Face Culling & Greedy Meshing (zukünftig)

## 0.5.1.0 Alpha | Project Documentation - LICENSE & README - 20.02.2026

- Hinzugefügt MIT License für Open-Source-Projekt
  - Klare Lizenzierung für Entwickler und Beitragstäter
  - Standard-Open-Source-Bedingungen

- Erstellt umfassendes README.md
  - Projekt-Übersicht: "Create Your Adventure" - Voxel-Engine mit C# und OpenGL
  - Feature-Liste (Texture System, GPU-Instancing, Infinite World, etc.)
  - Setup-Anleitung für Entwickler
  - Architektur-Dokumentation
    - Manager Pattern (Shader, Texture, Mesh, etc.)
    - API-Abstraktion für Graphics Backends
    - Component-basierte Struktur
  - Roadmap für zukünftige Features
  - Contributing Guidelines
  - Links zu Dependencies (Silk.NET, ImGui, StbImageSharp)

- **Status:** ✅ Projekt-Dokumentation fertiggestellt

## 0.5.2.0 Alpha | Mesh System Implementation (VAO/VBO/EBO Abstraction) - 22.02.2026

- **Vollständiges Mesh-Abstraktions-System mit API-Unabhängigkeit implementiert**
  - IMesh Interface definiert Graphics API agnostischen Vertrag
    - Create<T>() Überladungen für Vertices und Indices (Generic-Support)
    - Bind/Unbind für Rendering-Integration
    - Draw() für Standard-Rendering
    - DrawInstanced() für GPU-Instancing (4096+ Instanzen)
  - IVertexBuffer Interface für VBO-Management
    - SetData<T>() für initiale Buffer-Erstellung
    - UpdateData<T>() für partielle Updates (Dynamic Meshes)
    - Properties: VertexCount, SizeInBytes, Layout
  - IIndexBuffer Interface für EBO-Management
    - SetData() für Index-Daten
    - Properties: IndexCount, SizeInBytes
  - VertexAttribute Struct für Vertex-Attribut-Definition
    - Name, Location, ComponentCount, Type, Normalized, Offset
    - Factory Methods: Position(), TexCoord(), Normal()
  - VertexLayout Klasse für flexible Layout-Definition
    - Add() Method für Attribute-Chaining (Fluent API)
    - Automatische Stride-Berechnung
    - Presets: PositionTexCoord(), PositionTexCoordNormal()
  - VertexAttributeType Enum: Float, Int, UInt, Byte

- **OpenGL-Implementierungen**
  - OpenGLMesh für VAO-Management
    - Create<T>(vertices, layout) für nicht-indizierte Meshes
    - Create<T>(vertices, indices, layout) für indizierte Meshes
    - SetupVertexAttributes() für automatische Attribute-Konfiguration
    - Bind/Unbind mit VAO State Management (1 Bind statt VBO + EBO + Attributes!)
    - Draw() für Standard-Rendering
    - DrawInstanced() für GPU-Instancing
    - Vollständiges Dispose Pattern (VAO, VBO, EBO)
  - OpenGLVertexBuffer für VBO-Verwaltung
    - Generic SetData<T>() für flexible Daten-Typen
    - UpdateData<T>() für Partial Updates
    - VertexCount und SizeInBytes Tracking
  - OpenGLIndexBuffer für EBO-Verwaltung
    - SetData() für Index-Daten
    - IndexCount und SizeInBytes Tracking
  - MeshManager Singleton
    - Factory Pattern für API-Abstraktion (OpenGL, zukünftig Vulkan/DirectX)
    - Thread-safe Initialization mit Lock-Pattern
    - Mesh-Caching für Performance
    - CreateQuad() und CreateCube() Convenience-Methoden
    - Automatische Vertex-Layout-Generierung

- **Architektur & Design Patterns**
  - VAO State Pattern: 1 Bind statt VBO + EBO + Attributes (Effizienzgewinn!)
  - Factory Pattern in MeshManager für API-Abstraction
  - Generic<T> für flexible Datentypen (wo T : unmanaged)
  - Builder Pattern in VertexLayout (Fluent API mit Add())
  - Proper Resource Management (LIFO Disposal)

- **Logging & Debugging**
  - [MESH] kategorisierte Log-Nachrichten
  - [VBO] und [EBO] spezifische Logs
  - Ausführliches Logging bei Creation, Binding, Draw-Calls
  - Warnungen bei uninitialisiertem Mesh oder doppelter Initialisierung

- **Performance-Features**
  - GPU-Instancing Support (DrawInstanced für 4096+ Blöcke)
  - Indexed vs Non-Indexed Rendering Optionen
  - Vertex Attribute Divisor bereit für Instance-Daten
  - Efficient State Management durch VAO (reduziert OpenGL State Changes)
  - Mesh-Caching verhindert Duplikate

- **Vollständige XML-Dokumentation**
  - Alle Public Interfaces dokumentiert (IMesh, IVertexBuffer, IIndexBuffer)
  - Parameter, Return-Types und Exceptions erklärt
  - Detaillierte Inline-Comments für VAO-Setup, Buffer-Upload und Attribute-Konfiguration

- **Testing & Validation**
  - ✅ Cube-Mesh erfolgreich erstellt (24 Vertices, 36 Indices)
  - ✅ VAO/VBO/EBO Handles korrekt generiert
  - ✅ Vertex-Attribute korrekt konfiguriert (Position, TexCoord)
  - ✅ Initialization & Disposal in korrekter Reihenfolge

- **Status:** ✅ Mesh-System vollständig funktional, Integration in Rendering-Pipeline ausstehend
- **Nächste Schritte:** 
  - InputManager für Keyboard/Mouse (Camera-Vorbereitung)
  - Camera-System mit View/Projection-Matrizen
  - Render-Loop Integration mit Draw-Calls
  - Chunk-Mesh-Integration (Chunk → OpenGLMesh)

## 0.6.0.0 Alpha | Input System - Part 1 (Enums, State & Bindings) - 24.02.2026 🔄 **IN ENTWICKLUNG**

- **Status:** 🔄 In Entwicklung - Umfassendes Input-System mit Enum-Definitionen, State-Management und Binding-System
- Implementiert vollständiges Input-Abstraktions-System für Keyboard, Mouse und Gamepad

- **Input-Enums (API-agnostisch)**
  - CursorMode: Visible, Hidden, Locked, Confined, ConfinedHidden
    - Platform-unabhängige Cursor-Verwaltung
  - KeyCode: 60+ Tasten
    - Letters: A-Z
    - Numbers: 0-9
    - Function Keys: F1-F12
    - Modifiers: LeftShift, RightShift, LeftControl, RightControl, LeftAlt, RightAlt
    - Navigation: Up, Down, Left, Right, Home, End, PageUp, PageDown, Insert, Delete
    - Special: Space, Enter, Escape, Tab, Backspace, CapsLock, NumLock, ScrollLock, PrintScreen, Pause
    - Numpad: 0-9, Add, Subtract, Multiply, Divide, Enter, Decimal
    - Miscellaneous: Grave, Minus, Equals, LeftBracket, RightBracket, Backslash, Semicolon, Apostrophe, Comma, Period, Slash
  - MouseButton: Left, Right, Middle, Button4, Button5
    - Basis für 5-Button Mäuse Support
  - GamepadButton: 15 Buttons
    - Face Buttons: A, B, X, Y (Xbox-Standard)
    - Bumpers & Triggers: LeftBumper, RightBumper, LeftTrigger, RightTrigger
    - Analog Sticks: LeftStick, RightStick (Pressbar)
    - D-Pad: DPadUp, DPadDown, DPadLeft, DPadRight
    - Special: Start, Back, Guide
  - GamepadAxis: 6 Achsen
    - LeftStickX, LeftStickY
    - RightStickX, RightStickY
    - LeftTrigger, RightTrigger
  - InputActionType: 6 Event-Typen
    - Pressed: Einmalige Aktivierung
    - Held: Kontinuierlich gedrückt
    - Released: Freigabe-Event
    - LongPress: Nach 0.5s Halten
    - DoublePress: Doppel-Klick (0.3s Fenster)
    - Axis: Analoge Eingabe

- **InputState Klasse - Zustand-Management**
  - Keyboard State Tracking
    - currentKeys, previousKeys für Frame-Vergleich
    - IsKeyDown(), IsKeyPressed(), IsKeyReleased() Queries
    - GetKeyHoldTime() für Dauer-Tracking
  - Mouse State Tracking
    - MousePosition, MouseDelta, ScrollDelta Properties
    - IsMouseButtonDown/Pressed/Released() Queries
  - Gamepad State Tracking
    - currentGamepadButtons, previousGamepadButtons
    - GamepadAxes mit Deadzone-Support (default 0.15)
    - GetGamepadAxis(), GetLeftStick(), GetRightStick() mit Deadzone
  - Long Press Detection
    - keyHoldTimes, mouseHoldTimes, gamepadHoldTimes Dictionaries
    - LongPressThreshold: 0.5 Sekunden
    - IsKeyLongPressed() Query
  - Double Press Detection
    - doublePressTracking Dictionary
    - DoublePressWindow: 0.3 Sekunden
    - IsKeyDoublePressed(key, currentTime) Query
  - Frame Lifecycle
    - BeginFrame(): Speichert previous State
    - EndFrame(deltaTime): Aktualisiert Hold Times
  - Kombinationen
    - IsKeyCombinationDown(params KeyCode[] keys): Alle Tasten gedrückt?
    - IsKeyCombinationPressed(mainKey, params modifiers): Modifier-Kombination?

- **InputAction Klasse - Action-Binding-System**
  - Name und InputActionType Properties
  - Multiple Bindings pro Action (List<InputBinding>)
  - Event-basierte Architektur
    - Triggered Event: Action wurde ausgelöst
    - AxisChanged Event: Analoge Eingabe mit float-Value
  - Fluent API für Bindings
    - AddKeyBinding(key, modifiers): Tastatur-Binding hinzufügen
    - AddGamepadBinding(button): Gamepad-Button-Binding
    - AddAxisBinding(axis): Gamepad-Achsen-Binding
  - RaiseTrigger() und RaiseAxis() für Internal-Events

- **InputBinding System - Flexible Bindungen**
  - Abstraktes InputBinding Base
    - IsActive(InputState, InputActionType): Query für Action-Aktivierung
    - GetAxisValue(InputState): Analog-Wert
    - Serialization: Serialize() / Deserialize(string data)
  - KeyBinding Konkretisierung
    - Key + Modifiers Support (z.B. Ctrl+Shift+A)
    - IsActive() evaluiert alle InputActionTypes
    - Serialisierung: "Key:Modifier1+Modifier2+MainKey"
  - MouseButtonBinding Konkretisierung
    - Alle 5 Mouse-Buttons supportiert
    - Pressed, Held, Released Support
    - Serialisierung: "Mouse:Button"
  - GamepadButtonBinding Konkretisierung
    - Xbox-Style Button-Mapping
    - Pressed, Held Support
    - Serialisierung: "Gamepad:Button"
  - GamepadAxisBinding Konkretisierung
    - Deadzone-Support (0.15 default)
    - IsActive() für Achsen-Bewegung
    - GetAxisValue() für kontinuierliche Eingabe
    - Serialisierung: "Axis:AxisName"

- **Architektur & Design Patterns**
  - Factory Pattern für Binding-Deserialisierung
  - Observer Pattern mit Events (Triggered, AxisChanged)
  - Fluent API in InputAction für Binding-Konfiguration
  - State Pattern mit BeginFrame/EndFrame Lifecycle
  - Deadzone-Pattern für Gamepad-Achsen

- **Performance & Optimierung**
  - HashSet für schnelle Key/Button-Lookups (O(1))
  - Dictionary-basiertes Hold-Time-Tracking
  - Frame-basierte State-Verwaltung (current/previous)
  - Minimale GC-Allocations durch HashSet

- **Vollständige XML-Dokumentation**
  - Alle Enums dokumentiert mit Beschreibungen
  - InputState Klasse: Zweck und Verwendung erklärt
  - InputAction und InputBinding: Event-System dokumentiert
  - Methoden-Summaries und Parameter-Dokumentation

- **Status:** 🔄 In Entwicklung - Folgende Tasks ausstehend:
  - ❌ IInputDevice Interface Implementation (KeyboardDevice, MouseDevice, GamepadDevice)
  - ❌ InputManager Singleton-Integration
  - ❌ Platform-spezifische Input-Polling (Silk.NET Integration)
  - ❌ Testing mit echten Input-Events
  - ❌ Serialization von Input-Konfigurationen

- **Nächste Schritte:** 
  - IInputDevice Interface für Devices
  - Keyboard/Mouse/Gamepad Device Implementations
  - InputManager für Koordination aller Devices
  - Integration in Program.cs OnLoad/OnUpdate
  - Key-Rebinding UI System

## 0.6.1.0 Alpha | Input System - Part 2 (InputManager & Silk.NET Integration) - 25.02.2026 🔄 **IN ENTWICKLUNG**

- **Status:** 🔄 In Entwicklung - InputManager implementiert, Integration in Program.cs ausstehend
- Implementiert vollständigen InputManager als zentrale Input-Verwaltung (Singleton Pattern)

- **InputManager - Zentrale Verwaltung**
  - Singleton Pattern mit Lock-Protection
  - Thread-safe Initialization
  - Integration mit WindowManager.InputContext (Silk.NET)
  - Device-Erkennung und Initialisierung
    - Keyboard: inputContext.Keyboards[0]
    - Mouse: inputContext.Mice[0]
    - Gamepad: inputContext.Gamepads[0] (optional)
  - Properties: HasKeyboard, HasMouse, HasGamepad, IsInitialized, CurrentCursorMode
  - BeginFrame/EndFrame Lifecycle-Management
    - BeginFrame(): InputState.BeginFrame() → previous State speichern
    - EndFrame(deltaTime): PollGamepadAxes() → CheckActions() → InputState.EndFrame()
  - Automatische Action-Evaluierung jeden Frame
  - Proper Disposal Pattern mit Event-Unsubscription

- **Cursor-Control-System**
  - SetCursorMode(CursorMode): 5 Modi verfügbar
    - Visible: Normal sichtbar
    - Hidden: Unsichtbar aber beweglich
    - Locked: FPS-Modus (Disabled in Silk.NET)
    - Confined: Auf Fenster beschränkt (TODO: Window-Clipping)
    - ConfinedHidden: Beschränkt + versteckt (TODO: Window-Clipping)
  - Convenience-Methoden
    - LockCursor(): Setzt Locked-Modus
    - UnlockCursor(): Setzt Visible-Modus
    - ToggleCursorLock(): Wechselt zwischen Locked/Visible
  - Silk.NET CursorMode Mapping (Normal, Hidden, Disabled)

- **Direct Input Query API**
  - Keyboard Queries
    - IsKeyDown(KeyCode): Aktuell gedrückt?
    - IsKeyPressed(KeyCode): Gerade heruntergedrückt?
    - IsKeyReleased(KeyCode): Gerade losgelassen?
    - IsKeyLongPressed(KeyCode): >0.5s gehalten?
    - GetKeyHoldTime(KeyCode): Halte-Dauer in Sekunden
    - IsKeyCombinationPressed(mainKey, modifiers): Ctrl+Shift+A etc.
  - Mouse Queries
    - IsMouseButtonDown(MouseButton): Aktuell gedrückt?
    - IsMouseButtonPressed(MouseButton): Gerade heruntergedrückt?
    - GetMousePosition(): Vector2 absolute Position
    - GetMouseDelta(): Vector2 Frame-Delta
    - GetScrollDelta(): float Scroll-Wert
  - Gamepad Queries
    - IsGamepadButtonDown(GamepadButton): Aktuell gedrückt?
    - IsGamepadButtonPressed(GamepadButton): Gerade heruntergedrückt?
    - GetGamepadAxis(GamepadAxis): float -1.0 bis 1.0
    - GetLeftStick(): Vector2 mit Deadzone
    - GetRightStick(): Vector2 mit Deadzone

- **High-Level Input Helpers**
  - GetMovementVector(): Automatisches WASD + Gamepad Fallback
    - Keyboard hat Priorität (WASD)
    - Fallback auf linken Gamepad-Stick
    - Normalisiert diagonal Movement
    - Returns Vector2.Zero wenn kein Input
  - GetLookVector(): Maus-Delta + Gamepad Fallback
    - Mouse-Delta hat Priorität
    - Fallback auf rechten Gamepad-Stick
    - Für Camera-Look-Rotation

- **Action System Implementation**
  - RegisterAction(name, type): Erstellt InputAction mit Type
    - Returns InputAction für Fluent API Chaining
    - Cached in Dictionary<string, InputAction>
    - Logging für jede registrierte Action
  - GetAction(name): Gibt InputAction zurück oder null
  - IsActionTriggered(name): Prüft ob Action aktiv
    - Iteriert durch alle Bindings der Action
    - Returns true wenn mindestens ein Binding aktiv
  - CheckAction(action): Interne Frame-Evaluierung
    - Prüft alle Bindings pro Action
    - Raised Triggered-Event für normale Actions
    - Raised AxisChanged-Event für Axis-Actions
    - Break nach erstem aktiven Binding (nur 1 pro Frame)

- **Default Action Bindings**
  - Movement Actions
    - MoveForward: W / DPadUp
    - MoveBackward: S / DPadDown
    - MoveLeft: A / DPadLeft
    - MoveRight: D / A (Gamepad)
    - MoveUp: Space / RightBumper
    - MoveDown: LeftControl / LeftBumper
    - Sprint: LeftShift / LeftStick (Press)
  - Camera Actions
    - ToggleCursorLock: Escape
  - Kommentiert: Jump, Interact, Inventory, Save, Load, Pause (für spätere Implementierung)
  - Logging: "{count} default actions registered"

- **Gamepad Axes Polling**
  - PollGamepadAxes(): Jedes Frame aufgerufen in EndFrame()
    - Liest Thumbsticks[0-1] aus Gamepad
      - LeftStickX: Thumbsticks[0].X
      - LeftStickY: Thumbsticks[0].Y
      - RightStickX: Thumbsticks[1].X
      - RightStickY: Thumbsticks[1].Y
    - Liest Triggers[0-1] aus Gamepad
      - LeftTrigger: Triggers[0].Position
      - RightTrigger: Triggers[1].Position
    - Setzt Werte in InputState über SetGamepadAxis()
    - Null-Check für Gamepad-Verfügbarkeit

- **Event Handlers - Silk.NET Integration**
  - Keyboard Events
    - OnKeyDown(IKeyboard kb, Key key, int scancode)
      - Konvertiert Silk.NET Key → KeyCode
      - Ruft state.SetKeyDown(keyCode) auf
    - OnKeyUp(IKeyboard kb, Key key, int scancode)
      - Konvertiert Silk.NET Key → KeyCode
      - Ruft state.SetKeyUp(keyCode) auf
  - Mouse Events
    - OnMouseDown(IMouse m, MouseButton button)
      - Konvertiert Silk.NET MouseButton → MouseButton
      - Ruft state.SetMouseButtonDown(button) auf
    - OnMouseUp(IMouse m, MouseButton button)
      - Konvertiert Silk.NET MouseButton → MouseButton
      - Ruft state.SetMouseButtonUp(button) auf
    - OnMouseMove(IMouse m, Vector2 position)
      - Berechnet delta = position - lastMousePosition
      - Aktualisiert lastMousePosition
      - Ruft state.SetMousePosition(position) und SetMouseDelta(delta) auf
    - OnMouseScroll(IMouse m, ScrollWheel scroll)
      - Ruft state.SetScrollDelta(scroll.Y) auf
  - Gamepad Events
    - OnGamepadButtonDown(IGamepad gp, Button button)
      - Konvertiert ButtonName → GamepadButton
      - Ruft state.SetGamepadButtonDown(button) auf
    - OnGamepadButtonUp(IGamepad gp, Button button)
      - Konvertiert ButtonName → GamepadButton
      - Ruft state.SetGamepadButtonUp(button) auf

- **Input Enum Converters - Silk.NET Mapping**
  - ConvertKey(Silk.NET.Input.Key): Konvertiert zu KeyCode
    - 60+ Key-Mappings
    - Letters: A-Z → KeyCode.A-Z
    - Numbers: Number0-9 → KeyCode.Number0-9
    - Modifiers: ShiftLeft → LeftShift, ControlLeft → LeftControl, AltLeft → LeftAlt
    - Navigation: Up/Down/Left/Right → KeyCode.Up/Down/Left/Right
    - Special: Space, Enter, Escape, Tab, Backspace, etc.
    - Numpad: Keypad0-9 → Numpad0-9, KeypadAdd → NumpadAdd, etc.
    - Miscellaneous: GraveAccent → Grave, BackSlash → Backslash, etc.
    - Returns KeyCode? (nullable für unbekannte Tasten)
  - ConvertMouseButton(Silk.NET.Input.MouseButton): Konvertiert zu MouseButton
    - Left → MouseButton.Left
    - Right → MouseButton.Right
    - Middle → MouseButton.Middle
    - Button4 → MouseButton.Button4
    - Button5 → MouseButton.Button5
    - Returns MouseButton? (nullable)
  - ConvertGamepadButton(ButtonName): Konvertiert zu GamepadButton
    - Face Buttons: A, B, X, Y → GamepadButton.A/B/X/Y
    - Bumpers: LeftBumper, RightBumper
    - Sticks: LeftStick, RightStick
    - D-Pad: DPadUp, DPadDown, DPadLeft, DPadRight
    - Special: Start, Back, Home → Guide
    - ⚠️ LeftTrigger/RightTrigger fehlen in ButtonName (Triggers sind Achsen!)
    - Returns GamepadButton? (nullable)

- **Logger-Integration**
  - [INPUT] Kategorie für alle Input-Logs
  - Initialization Logs
    - "Keyboard initialized" oder "No keyboard detected"
    - "Mouse initialized" oder "No mouse detected"
    - "Gamepad initialized: {name}" oder "No gamepad detected (optional)"
    - "InputManager initialized"
  - Action Registration
    - "Action '{name}' registered (Type: {type})"
    - "{count} default actions registered"
    - "Action '{name}' already registered" (Warnung)
  - Cursor Mode
    - "Cursor mode set to: {mode}"
  - Disposal
    - "InputManager disposed"

- **Architektur & Design Patterns**
  - Singleton Pattern für globalen Input-Zugriff
  - Observer Pattern für Event-Handling (Silk.NET Events → InputState)
  - Adapter Pattern für Enum-Konvertierung (Silk.NET → Custom Enums)
  - Facade Pattern: InputManager als einfache API über komplexe Input-Devices
  - Frame-based Input Processing (Begin/End Frame)

- **Vollständige XML-Dokumentation**
  - ⚠️ XML-Dokumentation fehlt noch für alle Public-Methoden
  - TODO: Summaries für Initialize, BeginFrame, EndFrame
  - TODO: Parameter-Dokumentation für alle Queries

- **Status:** 🔄 In Entwicklung - Folgende Tasks ausstehend:
  - ❌ XML-Dokumentation für InputManager hinzufügen
  - ❌ Integration in Program.cs (OnLoad: InputManager.Instance.Initialize())
  - ❌ BeginFrame/EndFrame Calls in Program.cs OnUpdate
  - ❌ Camera-Integration mit GetMovementVector() und GetLookVector()
  - ❌ Testing mit echten Keyboard/Mouse/Gamepad-Inputs
  - ❌ ToggleCursorLock Action-Binding testen
  - ❌ Export/Import Bindings (JSON Serialization) implementieren

- **Nächste Schritte:** 
  - XML-Dokumentation ergänzen
  - Program.cs Integration (OnLoad, OnUpdate)
  - Camera.cs Update für Input-System
  - Cursor-Lock beim Fenster-Focus
  - Key-Rebinding UI (zukünftig)

## 0.6.2.0 Alpha | Input System - Part 3 (Device Abstraction & Converters) - 26.02.2026 🔄 **IN ENTWICKLUNG**

- **Status:** 🔄 In Entwicklung - Device-Abstraktions-Layer implementiert, Part 4 (Finalisierung) ausstehend
- Implementiert vollständige Device-Abstraktion für Keyboard, Mouse und Gamepad

- **IInputDevice Interface - Generische Device-Abstraktion**
  - Definiert einheitliches Interface für alle Input-Geräte
  - Properties: Name, IsConnected
    - Name: Geräte-Identifikation (z.B. "Logitech G502")
    - IsConnected: Verfügbarkeits-Check
  - Initialize(): Geräte-Setup und Logger-Output
  - RegisterEvents(InputState): Event-Handler-Registrierung
  - UnregisterEvents(): Event-Handler-Cleanup
  - Poll(InputState): Frame-basiertes Input-Polling (für Gamepad-Achsen)
  - IDisposable: Proper Resource Management
  - Ermöglicht polymorphe Device-Verwaltung im InputManager

- **KeyboardDevice - Silk.NET Keyboard-Wrapper**
  - Wrapper für Silk.NET IKeyboard
  - Event-basierte Input-Verarbeitung
    - OnKeyDown(IKeyboard, Key, scancode): Key → KeyCode Konvertierung
    - OnKeyUp(IKeyboard, Key, scancode): Key → KeyCode Konvertierung
  - Kein Polling nötig (rein event-driven)
  - Automatische KeyConverter-Integration
  - Logger-Integration
    - "Keyboard initialized: {Name}" oder "No keyboard detected"
  - Proper Disposal mit Event-Unsubscription

- **MouseDevice - Silk.NET Mouse-Wrapper**
  - Wrapper für Silk.NET IMouse
  - Event-basierte Input-Verarbeitung
    - OnMouseDown(IMouse, MouseButton): Button-Erkennung
    - OnMouseUp(IMouse, MouseButton): Button-Release
    - OnMouseMove(IMouse, Vector2 position): Delta-Berechnung
    - OnScroll(IMouse, ScrollWheel): Scroll-Delta
  - lastPosition Tracking für präzise Delta-Berechnung
  - SetCursorMode(CursorMode): Cursor-Steuerung
    - Visible → Normal
    - Hidden → Hidden
    - Locked → Disabled
    - Confined → Normal (TODO: Window-Clipping)
    - ConfinedHidden → Hidden (TODO: Window-Clipping)
  - RawMouse Property: Direkter Zugriff auf IMouse-Instanz
  - Kein Polling nötig (rein event-driven)

- **GamepadDevice - Silk.NET Gamepad-Wrapper**
  - Wrapper für Silk.NET IGamepad
  - Hybrid-Architektur: Events + Polling
    - Events: ButtonDown, ButtonUp für digitale Inputs
    - Polling: Thumbsticks und Triggers für analoge Inputs
  - TriggerThreshold Property (0.5f default)
    - Behandelt Trigger als Button ab Schwellwert
    - Ermöglicht Trigger-Bindings wie normale Buttons
  - Polling in Poll(InputState)
    - Thumbsticks[0]: LeftStickX, LeftStickY
    - Thumbsticks[1]: RightStickX, RightStickY
    - Triggers[0]: LeftTrigger (analog + button)
    - Triggers[1]: RightTrigger (analog + button)
  - SetTriggerValue(GamepadButton, float): Speichert analoge Trigger-Werte
  - Automatische GamepadConverter-Integration

- **InputConverter - Zentrale Enum-Konvertierung**
  - Separate Static-Klasse (vorher inline im InputManager)
  - Nested Static Classes für Übersichtlichkeit
    - KeyConverter: Silk.NET.Key → KeyCode
      - 60+ Tastatur-Mappings
      - A-Z, 0-9, F1-F12
      - Modifiers: ShiftLeft → LeftShift, ControlLeft → LeftControl
      - Navigation: Up/Down/Left/Right, Home/End, PageUp/PageDown
      - Special: Space, Enter, Escape, Tab, Backspace, CapsLock, NumLock
      - Numpad: Keypad0-9 → Numpad0-9, KeypadAdd → NumpadAdd
      - Miscellaneous: GraveAccent → Grave, BackSlash → Backslash
    - MouseConverter: Silk.NET.MouseButton → MouseButton
      - Left, Right, Middle, Button4, Button5
    - GamepadConverter: ButtonName → GamepadButton
      - Face Buttons: A, B, X, Y
      - Bumpers: LeftBumper, RightBumper
      - Sticks: LeftStick, RightStick (Pressable)
      - D-Pad: DPadUp, DPadDown, DPadLeft, DPadRight
      - Special: Start, Back, Home → Guide
  - Nullable Returns (KeyCode?, MouseButton?, GamepadButton?)
    - null für nicht-unterstützte Inputs
  - Architektur-Verbesserung: Trennung von Device-Logik und Konvertierung

- **InputState Erweiterungen**
  - Access Modifier Änderungen
    - Internal statt private für Fields (CurrentKeys, PreviousKeys, etc.)
    - Ermöglicht direkten Zugriff von Device-Klassen
    - Bessere Performance (kein Setter-Overhead)
  - TriggerValues Dictionary hinzugefügt
    - Dictionary<GamepadButton, float> für analoge Trigger-Werte
    - Speichert LeftTrigger/RightTrigger Positionen (0.0-1.0)
  - SetTriggerValue(GamepadButton, float) Methode
    - Setzt analoge Trigger-Werte
    - Wird von GamepadDevice.Poll() aufgerufen

- **InputAnalyzer - Placeholder-Klasse**
  - Leere Klasse für zukünftige Funktionalität
  - Geplante Features (zukünftig):
    - Input-Pattern-Erkennung (Combo-Detection)
    - Gesture-Recognition (Swipe, Pinch, etc.)
    - Input-Replay/Recording für Testing
    - Input-Heatmaps für UI/UX-Analyse
  - Vorbereitet für Part 4 oder spätere Updates

- **Architektur & Design Patterns**
  - Strategy Pattern: IInputDevice für polymorphe Device-Verwaltung
  - Adapter Pattern: Silk.NET → Custom Enums via Converter
  - Observer Pattern: Events von Silk.NET → InputState Updates
  - Polling Pattern: Gamepad Axes pro Frame (wegen analoger Natur)
  - Separation of Concerns: Converter separiert von Devices und Manager

- **Performance & Optimierung**
  - Internal Fields für direkten Zugriff (kein Property-Overhead)
  - Event-basiert wo möglich (Keyboard, Mouse) statt Polling
  - Polling nur für Gamepad-Achsen (kontinuierliche Werte)
  - Nullable Returns vermeiden Exception-Overhead

- **Logger-Integration**
  - [INPUT] Kategorie konsistent
  - Device-spezifische Logs
    - "Keyboard initialized: {Name}"
    - "Mouse initialized: {Name}"
    - "Gamepad initialized: {Name}"
    - "No {device} detected" Warnungen

- **Vollständige XML-Dokumentation**
  - ⚠️ XML-Dokumentation fehlt noch für Device-Klassen
  - TODO: Summaries für IInputDevice Interface
  - TODO: Methoden-Dokumentation für alle Devices
  - TODO: InputConverter Dokumentation

- **Status:** 🔄 In Entwicklung - Folgende Tasks ausstehend (Part 4):
  - ❌ InputManager Refactoring auf Device-Abstraktion
  - ❌ Device-Registrierung und -Verwaltung im Manager
  - ❌ Integration in Program.cs (OnLoad, OnUpdate)
  - ❌ XML-Dokumentation vervollständigen
  - ❌ Testing mit echten Devices
  - ❌ InputAnalyzer Funktionalität (später)

- **Nächste Schritte (Part 4 - Finalisierung):** 
  - InputManager Refactoring: Device-Verwaltung statt direkte Silk.NET-Zugriffe
  - RegisterDevice() / UnregisterDevice() Methoden
  - Program.cs Integration und Testing
  - XML-Dokumentation vervollständigen
  - Camera-Integration mit neuem Input-System
  - **Abschluss des Input-Systems** 🎯

## 0.6.3.0 Alpha | Input System - Part 4 - Complete Implementation - 28.02.2026

- **Vollständiges Input-Abstraktions-System implementiert**
  - Unterstützung für Keyboard, Mouse und Gamepad
  - API-agnostische Enums für alle Input-Typen
  - Frame-basierte Input-Verarbeitung (BeginFrame/EndFrame)
  - Zeit wird von Engine injiziert (nicht selbst gezählt)

- **InputManager als zentrale Verwaltung (Singleton)**
  - Thread-safe Initialization mit Lock-Pattern
  - Device-Erkennung: Keyboard, Mouse, Gamepad
  - Cursor-Control: Visible, Hidden, Locked, Confined, ConfinedHidden
  - Direct Query API für Camera/Debug (keine Actions nötig)
  - Action-System für Gameplay (rebindbar)

- **Device-Abstraktions-Layer**
  - IInputDevice Interface für polymorphe Verwaltung
  - KeyboardDevice: Event-basiert (KeyDown/KeyUp)
  - MouseDevice: Events + Cursor-Mode-Control
  - GamepadDevice: Hybrid (Events + Polling für Achsen)
  - Trigger als Buttons mit Threshold (0.5f default)

- **InputState für Zustand-Management**
  - Current/Previous State für Frame-Vergleich
  - IsKeyDown/Pressed/Released/LongPressed Queries
  - Long Press Detection (>0.5s Halten)
  - Double Press Detection (0.3s Fenster)
  - Key-Kombinationen (Ctrl+S, F3+G)

- **InputAnalyzer als Query-Interface**
  - Für Camera, Debug, Editor (Direct Queries erlaubt)
  - GetMovementVector(): WASD + Gamepad Fallback
  - GetLookVector(): Mouse-Delta + RightStick Fallback
  - IsDebugCombo(F3, G): Debug-Kombinations-Prüfung
  - Deadzone-Support für Gamepad-Achsen (0.15 default)

- **InputRegistry für Action-Registrierung**
  - Getrennt vom InputManager (Modder-freundlich)
  - Engine-Defaults: MoveForward/Backward/Left/Right, Sprint, ToggleCursorLock
  - Fluent API: AddKeyBinding(), AddGamepadBinding()
  - ProcessActions(): Automatische Action-Evaluierung

- **InputBinding-System**
  - KeyBinding mit Modifier-Support (Ctrl+Shift+A)
  - MouseButtonBinding für alle 5 Buttons
  - GamepadButtonBinding für 15 Buttons
  - GamepadAxisBinding mit Deadzone
  - Serialization: Serialize()/Deserialize() für Settings

- **InputConverters für Silk.NET-Mapping**
  - KeyConverter: 60+ Tasten-Mappings
  - MouseConverter: 5 Button-Mappings
  - GamepadConverter: 15 Button-Mappings
  - Nullable Returns für unbekannte Inputs

- **API-agnostische Enums**
  - KeyCode: A-Z, 0-9, F1-F12, Modifiers, Numpad, etc.
  - MouseButton: Left, Right, Middle, Button4, Button5
  - GamepadButton: Face (A,B,X,Y), Bumpers, Triggers, D-Pad, Sticks
  - GamepadAxis: LeftStick X/Y, RightStick X/Y (Trigger sind Buttons!)
  - CursorMode: Visible, Hidden, Locked, Confined, ConfinedHidden
  - InputActionType: Pressed, Held, Released, LongPress, DoublePress, Axis

- **Camera-Integration**
  - Camera.Update(deltaTime) statt 8 bool-Parameter
  - Nutzt InputManager.Instance.Analyzer für Direct Queries
  - ProcessMovement(): WASD + Sprint mit Smooth-Acceleration
  - ProcessRotation(): Mouse-Delta mit Smoothing
  - Debug-Combos: F3+G (Chunk borders), F3+B (Hitboxes)

- **Engine-Regeln eingehalten**
  - ✅ Zeit wird injiziert (EndFrame(deltaTime))
  - ✅ Actions für Gameplay, Direct Queries für Camera/Debug
  - ✅ Gameplay darf NUR InputActions benutzen
  - ✅ Camera, Debug, Editor dürfen Direct Queries
  - ✅ Actions sind rebindingfähig

- **Architektur & Design Patterns**
  - Singleton Pattern: InputManager
  - Strategy Pattern: IInputDevice
  - Observer Pattern: Silk.NET Events → InputState
  - Adapter Pattern: Converters für Enum-Mapping
  - Facade Pattern: InputManager als einfache API
  - Factory Pattern: InputBinding.Deserialize()

- **Bugfixes**
  - MouseDevice.OnMouseDown rief SetMouseButtonUp auf (korrigiert)
  - MouseDevice.OnMouseUp fehlte komplett (hinzugefügt)
  - GamepadDevice hatte Tippfehler in Methodennamen (korrigiert)
  - InputBinding fehlten Parse-Methoden (hinzugefügt)

- **Testing & Validation**
  - ✅ Keyboard initialisiert (Silk.NET Keyboard via GLFW)
  - ✅ Mouse initialisiert (Silk.NET Mouse via GLFW)
  - ✅ Gamepad initialisiert (Silk.NET Gamepad via GLFW)
  - ✅ 6 Engine-Actions registriert
  - ✅ Cursor auf Locked gesetzt
  - ✅ Camera initialisiert bei (0, 0, 5)
  - ✅ Proper Disposal in korrekter Reihenfolge

- **Status:** ✅ Input-System vollständig implementiert
- **Nächste Schritte:** 
  - Camera View/Projection Matrizen an Shader übergeben
  - Block visuell rendern (VAO/VBO funktioniert)
  - Chunk-System reaktivieren

## 0.6.3.1 Alpha | Bugfix: Rendering Pipeline & Input Integration - 28.02.2026

- **Kritische Rendering-Bugs behoben**
  - OnRender() übergab keine View/Projection-Matrizen an Shader (Cube war unsichtbar)
    - shader.SetUniform("uView", camera.GetViewMatrix())
    - shader.SetUniform("uProjection", camera.GetProjectionMatrix(...))
    - shader.SetUniform("uModel", Matrix4X4<float>.Identity) hinzugefügt
  - Shader erhielt keine Transformations-Matrizen → Cube wurde nicht gerendert
  - Window-Size wird jetzt korrekt von WindowManager.Instance.Size geholt

- **Input-System Integration behoben**
  - BeginFrame/EndFrame Reihenfolge korrigiert
    - VORHER (falsch): EndFrame → Camera.Update → BeginFrame
    - NACHHER (korrekt): BeginFrame → Camera.Update → EndFrame
  - InputState wurde vor Camera-Update gecleart (Input war immer leer)
  - Input-Events werden jetzt korrekt pro Frame verarbeitet

- **Camera-System Fixes**
  - Camera.Update() rief nicht-existente Methoden auf
    - ApplyMovement() → ProcessMovement() (existiert jetzt)
    - ApplyRotation() → ProcessRotation() (existiert jetzt)
  - Camera nutzt jetzt InputManager.Instance für Direct Queries
    - IsKeyDown(KeyCode.W/A/S/D) für Bewegung
    - GetLookVector() für Maus-Rotation
    - IsDebugCombo(F3, G) für Debug-Ausgaben
  - ProcessMovement() implementiert mit WASD + Sprint + Space/Ctrl
  - ProcessRotation() implementiert mit Maus-Smoothing
  - Rotation-Berechnung Typo behoben (yawRad: 180 → 180f)

- **MeshManager API-Fix**
  - CreateCube() Signatur korrigiert
    - VORHER: CreateCube(string name, float size)
    - NACHHER: CreateCube(string name, float size, AtlasRegion region)
  - UV-Koordinaten werden jetzt korrekt von AtlasRegion übernommen
  - Program.cs ruft MeshManager.CreateCube() mit dirtRegion auf
  - Alle 6 Cube-Flächen nutzen korrekte UV-Mapping

- **InputAnalyzer Integration**
  - InputManager.Analyzer Property wurde nicht initialisiert (null)
  - Analyzer wird jetzt in InputManager.Initialize() erstellt
  - Camera kann jetzt Direct Queries nutzen
  - IsDebugCombo(modifier, key) funktioniert korrekt
    - F3+G logged "[CAMERA] Debug: Chunk borders toggled"
    - F3+B logged "[CAMERA] Debug: Hitboxes toggled"

- **Logging-Verbesserungen**
  - Camera Position-Log nutzt Vector3D-Notation: `<0, 0, 5>`
  - Konsistente Formatierung für alle Vector-Logs
  - Debug-Combos werden in Console ausgegeben

- **Testing & Validation**
  - ✅ Cube 

## 0.6.3.2 Alpha | Input System - Documentation & Code Comments - 01.03.2026

- **Vollständige XML-Dokumentation für Input-System hinzugefügt**
  - Alle Public-Methoden mit `<summary>` Tags dokumentiert
  - Parameter-Dokumentation mit `<param>` für alle Methoden
  - Return-Type-Dokumentation mit `<returns>` wo relevant
  - Exception-Dokumentation mit `<exception>` für InvalidOperationException

- **InputManager - XML-Dokumentation**
  - Initialize(): Beschreibt Device-Erkennung und Event-Registrierung
  - BeginFrame() / EndFrame(deltaTime): Lifecycle-Beschreibung
  - SetCursorMode(CursorMode): Alle 5 Modi erklärt (Visible, Hidden, Locked, Confined, ConfinedHidden)
  - Direct Query API dokumentiert
    - IsKeyDown/Pressed/Released/LongPressed: Unterschiede erklärt
    - GetMousePosition/Delta/ScrollDelta: Position vs. Delta erklärt
    - GetGamepadAxis/LeftStick/RightStick: Deadzone-Parameter dokumentiert
  - GetMovementVector() / GetLookVector(): Keyboard/Gamepad-Fallback erklärt
  - RegisterAction() / IsActionTriggered(): Action-System dokumentiert
  - Properties: HasKeyboard, HasMouse, HasGamepad, IsInitialized, CurrentCursorMode

- **InputState - Interne API dokumentiert**
  - BeginFrame(): Previous-State-Speicherung erklärt
  - EndFrame(deltaTime): Hold-Time-Tracking erklärt
  - Setter-Methoden dokumentiert
    - SetKeyDown/Up: Keyboard-State-Verwaltung
    - SetMouseButtonDown/Up: Mouse-Button-State
    - SetMousePosition/Delta/ScrollDelta: Position- und Delta-Tracking
    - SetGamepadButtonDown/Up: Gamepad-Button-State
    - SetGamepadAxis: Achsen-Wert-Speicherung
    - SetTriggerValue: Analoge Trigger-Werte
  - Field-Dokumentation für State-Dictionaries

- **InputBinding - Serialization-System dokumentiert**
  - InputAction Klasse
    - Name, Type Properties erklärt
    - Triggered / AxisChanged Events dokumentiert
    - AddKeyBinding/GamepadBinding/AxisBinding: Fluent API erklärt
  - InputBinding Abstract Base
    - IsActive(InputState, InputActionType): Aktivierungs-Logik
    - GetAxisValue(InputState): Analog-Wert-Retrieval
    - Serialize() / Deserialize(): Serialization-Format dokumentiert
  - Concrete Implementations
    - KeyBinding: Modifier-Support erklärt (Ctrl+Shift+A)
    - MouseButtonBinding: 5-Button-Unterstützung
    - GamepadButtonBinding: Xbox-Style-Mapping
    - GamepadAxisBinding: Deadzone-Funktionalität

- **InputAnalyzer - Query-Interface dokumentiert**
  - GetMovementVector(): WASD + Gamepad-Fallback erklärt
  - GetLookVector(): Mouse-Delta + RightStick-Fallback erklärt
  - IsDebugCombo(modifier, key): Debug-Kombinations-Prüfung
    - Beispiel: F3+G für Chunk-Borders
  - Deadzone-Parameter dokumentiert (default 0.15f)

- **InputRegistry - Action-System dokumentiert**
  - RegisterAction(name, type): Action-Erstellung erklärt
  - ProcessActions(): Automatische Action-Evaluierung
  - Fluent API dokumentiert
    - AddKeyBinding(key, modifiers): Tastatur-Bindings
    - AddGamepadBinding(button): Gamepad-Bindings
  - Beispiel-Usage in XML-Comments

- **InputConverter - Enum-Konvertierung dokumentiert**
  - KeyConverter Nested Class
    - Convert(Silk.NET.Input.Key): 60+ Key-Mappings erklärt
    - Nullable KeyCode? Return dokumentiert
  - MouseConverter Nested Class
    - Convert(Silk.NET.Input.MouseButton): 5 Button-Mappings
  - GamepadConverter Nested Class
    - Convert(ButtonName): 15 Button-Mappings (Xbox-Layout)
  - Rationale für Silk.NET-Mapping in Comments

- **Device-Klassen - Vollständige Dokumentation**
  - IInputDevice Interface
    - Name Property: Geräte-Identifikation
    - IsConnected Property: Verfügbarkeits-Check
    - Initialize(): Setup-Prozess
    - RegisterEvents(InputState): Event-Handler-Registrierung
    - UnregisterEvents(): Event-Cleanup
    - Poll(InputState): Frame-basiertes Polling (nur Gamepad)
  - KeyboardDevice
    - Event-basierte Architektur erklärt
    - OnKeyDown/OnKeyUp: Key-Konvertierung dokumentiert
    - "Kein Polling nötig" in Summary erwähnt
  - MouseDevice
    - SetCursorMode(CursorMode): Cursor-Steuerung erklärt
    - RawMouse Property: Direkter IMouse-Zugriff
    - lastPosition Tracking: Delta-Berechnung erklärt
    - OnMouseMove: Delta-Kalkulation dokumentiert
  - GamepadDevice
    - Hybrid-Architektur dokumentiert (Events + Polling)
    - TriggerThreshold: Trigger-als-Button erklärt (0.5f default)
    - Poll(): Thumbstick- und Trigger-Polling dokumentiert
    - SetTriggerValue(): Analoge Wert-Speicherung

- **Enum-Dokumentation - Alle Werte beschrieben**
  - KeyCode (60+ Tasten)
    - Letters: A-Z mit Beschreibung
    - Numbers: 0-9 mit Beschreibung
    - Modifiers: LeftShift, RightShift, LeftControl, etc.
    - Navigation: Up, Down, Left, Right, Home, End, etc.
    - Special: Space, Enter, Escape, Tab, Backspace, etc.
    - Numpad: Numpad0-9, Add, Subtract, Multiply, Divide, etc.
  - MouseButton: Left, Right, Middle, Button4, Button5
  - GamepadButton: 15 Buttons mit Xbox-Layout-Referenz
  - GamepadAxis: LeftStickX/Y, RightStickX/Y (Trigger sind Buttons!)
  - CursorMode: 5 Modi mit Verhalten beschrieben
  - InputActionType: 6 Typen mit Use-Cases

- **Inline-Comments - Komplexe Logik erklärt**
  - InputManager.CheckAction(): Action-Evaluierungs-Loop erklärt
  - InputState.BeginFrame/EndFrame: Lifecycle-Flow dokumentiert
  - GamepadDevice.Poll(): Trigger-Threshold-Logik erklärt
  - MouseDevice.OnMouseMove(): Delta-Berechnung kommentiert
  - InputConverter: Silk.NET-Enum-Mapping-Rationale

- **Code-Qualität-Verbesserungen**
  - Konsistente XML-Dokumentation über alle Klassen
  - Beispiele in XML-Comments wo hilfreich
  - Parameter-Beschreibungen für alle Public-Methoden
  - IntelliSense-freundliche Dokumentation

- **Status:** ✅ Input-System vollständig dokumentiert
- **Nächste Schritte:** 
  - World-System Refactoring
  - Chunk-Manager Implementation
  - Block-System Design

## 0.7.0.0 Alpha | Game Loop Architecture - Camera & Chunk System Part 1 (Skeleton) - 02.03.2026 🔄 **IN ENTWICKLUNG**

- **Status:** 🔄 In Entwicklung - Architektur-Skelett implementiert, keine Funktionalität vorhanden
- **Implementiert grundlegende Game-Loop-Architektur mit Chunk-Verwaltung**

- **GameLoop - Zentrale Spiel-Loop mit 5-Phasen-Modell**
  - Phase 1: Early Update
    - TimeManager.BeginFrame(rawDeltaTime): Frame-Zeit initialisieren
    - InputManager.BeginFrame(): Input-State für neuen Frame vorbereiten
  - Phase 2: Fixed Simulation Ticks (60 TPS)
    - time.ConsumeFixedTicks(): Akkumulator-basierte Fixed Timesteps
    - FixedTick(): Deterministische Simulation (Physics, Player, Chunk-Simulation)
    - "Spiral of Death" Prevention: Max 5 Ticks pro Frame
  - Phase 3: World Scheduler (Budget-basiert)
    - scheduler.ProcessFrame(): 8ms Budget für Chunk-Tasks
    - Task-Priorisierung: Loading, Meshing, BlockUpdates, Lightning, Entities
  - Phase 4: Variable Update (Interpolation)
    - Camera.Update(dt, alpha): Kamera mit Interpolation
    - Alpha = simulationAccumulator / FixedDeltaTime
  - Phase 5: Late Update
    - InputManager.EndFrame(dt): Input-State finalisieren
  - OnRender(rawDeltaTime): Rendering mit Interpolation
    - GetInterpolationAlpha() für smooth Rendering zwischen Fixed Ticks
    - Visible-Chunks-Only Rendering (via WorldRelevanceFilter)

- **TimeManager - Zentrales Zeit-Management (Singleton)**
  - Fixed Delta Time: 1/60s (60 Ticks Per Second)
    - Deterministische Simulation für Physics, Player, Chunk-Logic
    - ConsumeFixedTicks(): Akkumulator-basierte Tick-Generierung
  - Frame Delta Time: Variable Render-Framerate
    - SimulationTimeScale: Zeitlupe/Zeitraffer (Slow-Motion/Fast-Forward)
    - FrameCount: Gesamtanzahl Frames seit Start
  - World Time: In-Game Zeit für Tag/Nacht-Zyklus
    - WorldTimeScale: Unabhängige Zeitgeschwindigkeit (1 real sec = X world sec)
    - IsWorldPaused: Pausiert World-Zeit (nicht Chunk-Loading!)
  - Unscaled Total Time: Background-Zeit für Chunk-Loading
    - Läuft auch während Pause weiter (wichtig für Async-Tasks)
  - Interpolation Alpha: Smooth Rendering zwischen Fixed Ticks
    - GetInterpolationAlpha(): simulationAccumulator / FixedDeltaTime
    - Für Camera-Position, Entity-Rendering
  - Spiral of Death Prevention
    - Max 5 Fixed Ticks pro Frame
    - Verhindert Endlos-Loop bei Lag-Spikes

- **ChunkCoord - 64-Bit Chunk-Koordinaten für Infinite World**
  - Struct mit long X, Y, Z (±9,2 Trillionen Chunks pro Achse)
  - DistanceSquaredTo(other): Effiziente Distanz-Berechnung ohne sqrt
  - IEquatable<ChunkCoord>: Proper Equality-Checks
  - GetHashCode(): Hash für Dictionary/HashSet-Usage

- **ChunkState Enum - 7 Chunk-Lifecycle-Zustände**
  - Requested: Chunk wurde angefordert (in Queue)
  - Loading: WorldGen oder Disk-Load läuft
  - Meshing: Mesh-Generierung aktiv
  - Active: Voll geladen, wird gerendert/simuliert
  - Sleeping: Lazy-Mode (nur kritische Updates)
  - Serializing: Wird auf Disk gespeichert
  - Unloaded: Entladen, nicht im RAM

- **ChunkPriority Enum - 5 Priorisierungs-Level**
  - Critical (0): Player steht im Chunk (60 TPS)
  - High (1): Sichtbar & nah <2 Chunks (30 TPS)
  - Medium (2): Sichtbar aber fern <4 Chunks (10 TPS)
  - Low (3): Simulationsbereich <8 Chunks (2 TPS)
  - Background (4): Lazy/Sleeping >8 Chunks (1 TPS)

- **ChunkJob - Chunk-Task-Metadata für Scheduler**
  - ChunkCoord Coord: Chunk-Position
  - ChunkState State: Aktueller Lifecycle-Zustand
  - ChunkPriority Priority: Dynamische Priorisierung
  - TickRate: 1-60 TPS basierend auf Priority
    - Critical: 60 TPS, High: 30 TPS, Medium: 10 TPS, Low: 2 TPS, Background: 1 TPS
  - LastTickFrame: Letzter Tick-Zeitpunkt (für variable Tick-Rates)
  - IsInFrustum: Frustum-Culling-Flag
  - HasPendingChanges: Dirty-Flag für Mesh-Rebuild
  - ChunkMetadata: Lazy-Simulation-Daten (optional)
    - HasWater: Chunk enthält Wasser (braucht Simulation)
    - HasActiveEntities: Entities im Chunk (höhere Priorität)
    - BiomeId: Biom-Typ für Lazy-Updates
    - AverageTemperature: Für Wetter/Schnee-Simulation
    - LastModifiedTick: Für Dirty-Checking
  - UpdatePriority(cameraChunk, isPlayerInside): Dynamische Priorisierung
    - Distanz-basiert mit Squared-Distance (Performance)
    - Frustum-Check für Sichtbarkeit
    - Automatische TickRate-Anpassung

- **WorldRelevanceFilter - Distanz & Frustum Culling**
  - Distanz-Konfiguration (in Chunks)
    - RenderDistance: 16 (was gerendert wird)
    - SimulationDistance: 24 (was simuliert wird)
    - LoadDistance: 32 (was geladen wird)
    - UnloadDistance: 40 (was entladen wird)
  - UpdateFromCamera(cameraPosition, viewProjection)
    - Berechnet cameraChunk aus Kamera-Position
    - Extrahiert Frustum aus View-Projection-Matrix
  - ShouldRender(ChunkJob): Rendering-Relevanz
    - Distanz-Check: Innerhalb RenderDistance?
    - Frustum-Culling: AABB im Sichtfeld?
  - ShouldSimulate(ChunkJob): Simulations-Relevanz
    - Special-Handling für Wasser-Chunks (immer simulieren)
    - Special-Handling für Entity-Chunks (höhere Priorität)
    - Distanz-Check: Innerhalb SimulationDistance?
  - ShouldLoad(ChunkCoord): Lade-Relevanz
    - Distanz-Check: Innerhalb LoadDistance?
  - ShouldUnload(ChunkJob): Entlade-Relevanz
    - Never unload wenn HasActiveEntities
    - Distanz-Check: Außerhalb UnloadDistance?
  - IsWithinDistance(coord, distance): Squared-Distance-Check (Performance)
  - GetChunkAABB(coord): Axis-Aligned Bounding Box für Frustum-Test

- **Frustum Struct - View-Frustum für Visibility-Culling**
  - 6 Planes: Near, Far, Left, Right, Top, Bottom
  - ExtractFromMatrix(viewProjection): Standard-Algorithmus
    - Extrahiert Frustum-Planes aus View-Projection-Matrix
    - Für Frustum-Culling-Tests
  - Intersects(Box3D aabb): AABB-Intersection-Test
    - P-Vertex-Test für alle 6 Planes
    - Returns true wenn AABB im Frustum sichtbar

- **WorldScheduler - Budget-basierter Task-Scheduler (Singleton)**
  - TotalBudgetMs: 8ms pro Frame (~60 FPS Target)
    - Verteilbar auf 5 Task-Typen
  - Budget-Verteilung (Gewichte, Summe = 1.0)
    - ChunkLoading: 30% (2.4ms)
    - ChunkMeshing: 35% (2.8ms)
    - BlockUpdates: 20% (1.6ms)
    - Lightning: 10% (0.8ms)
    - EntitySimulation: 5% (0.4ms)
  - Priority Queues für Tasks
    - loadQueue: PriorityQueue<ChunkJob, ChunkPriority>
    - meshQueue: PriorityQueue<ChunkJob, ChunkPriority>
    - simulationQueue: PriorityQueue<ChunkJob, ChunkPriority>
  - ProcessFrame(): Haupt-Scheduler-Loop
    - Iteriert durch alle Task-Typen
    - Weist Task-Budget zu (TotalBudgetMs × Weight)
    - ProcessTask() mit Deadline-basierter Ausführung
    - Early Exit bei Budget-Erschöpfung (<0.5ms verbleibend)
  - ProcessTask(task, budgetMs): Task-Ausführung mit Zeitlimit
    - Deadline = currentTime + budgetMs
    - Dequeue von Priority Queue bis Deadline erreicht
    - ChunkLoading: ProcessChunkLoad() (WorldGen oder Disk)
    - ChunkMeshing: ProcessChunkMesh() (Mesh-Generierung)
    - BlockUpdates: ProcessBlockUpdatesWithBudget() (Block-Ticks)
    - Lightning: ProcessLightningWithBudget() (Light-Propagation)
    - EntitySimulation: ProcessEntitiesWithBudget() (Entity-Ticks)
  - Telemetry & Profiling
    - LastFrameBudgetUsedMs: Tatsächlich genutzte Zeit
    - LastTaskTimings: Dictionary<SchedulerTask, double> (pro Task)
    - Stopwatch für präzise Zeit-Messung
  - Public API
    - RequestChunkLoad(ChunkJob): Chunk in Load-Queue
    - RequestChunkMesh(ChunkJob): Chunk in Mesh-Queue
    - RequestSimulation(ChunkJob): Chunk in Simulation-Queue

- **SchedulerTask Enum - 5 Task-Typen**
  - ChunkLoading: WorldGen oder Disk-Load
  - ChunkMeshing: Mesh-Generierung
  - BlockUpdates: Block-Tick-Simulation (Wasser, Lava, Pflanzen)
  - Lightning: Light-Propagation und Updates
  - EntitySimulation: Entity-Ticks (AI, Physics)

- **Architektur & Design Patterns**
  - Singleton Pattern: TimeManager, WorldScheduler
  - Fixed Timestep Pattern: Deterministische Simulation
  - Accumulator Pattern: ConsumeFixedTicks() für variable Framerate
  - Priority Queue Pattern: Task-Scheduling nach Priorität
  - Budget-based Scheduling: Frame-Time-Budget für Tasks
  - Frustum Culling: Visibility-Optimierung
  - Distance-based LOD: Chunk-TickRate basierend auf Distanz
  - Lazy Evaluation: ChunkMetadata für Sleeping-Chunks

- **Performance-Konzepte**
  - Squared Distance: Vermeidet sqrt für Distanz-Checks
  - Frustum Culling: Rendert nur sichtbare Chunks
  - Priority-based Scheduling: Wichtige Tasks zuerst
  - Budget-based Processing: Max 8ms pro Frame für Chunk-Tasks
  - Variable TickRate: Critical 60 TPS, Background 1 TPS
  - Spiral of Death Prevention: Max 5 Fixed Ticks pro Frame
  - Interpolation: Smooth Rendering trotz Fixed Ticks

- **Vollständige XML-Dokumentation**
  - ⚠️ XML-Dokumentation fehlt noch komplett
  - TODO: Summaries für alle Klassen
  - TODO: Methoden-Dokumentation
  - TODO: Enum-Werte beschreiben

- **Status:** 🔄 Skelett-Code - NICHT FUNKTIONAL
  - ✅ Architektur definiert
  - ✅ Interfaces und Enums vorhanden
  - ❌ ProcessChunkLoad() nicht implementiert
  - ❌ ProcessChunkMesh() nicht implementiert
  - ❌ Frustum.ExtractFromMatrix() Skelett
  - ❌ Frustum.Intersects() Skelett
  - ❌ GameLoop.FixedTick() Kommentare statt Code
  - ❌ WorldRelevanceFilter.UpdateFromCamera() unvollständig
  - ❌ Keine Integration in Program.cs
  - ❌ Keine Tests vorhanden

- **Nächste Schritte (Part 2):**
  - Frustum-Extraction-Algorithmus implementieren
  - AABB-Intersection-Test implementieren
  - ChunkManager für Chunk-Verwaltung
  - WorldGenerator für Terrain-Generation
  - ChunkMesher für Mesh-Generierung
  - Integration in Program.cs OnUpdate/OnRender
  - Camera-System mit WorldRelevanceFilter verbinden 

  ## 0.7.1.0 Alpha | Camera Manager System - Part 2 (Complete Implementation for Camera) - 03.03.2026 **IN ENTWICKLUNG**

- **Vollständiges modulares Camera-Management-System implementiert**
  - Ersetzt alte monolithische Camera-Klasse durch Subsystem-Architektur
  - Floating Origin Pattern für unendliche Welten
  - Frustum Culling für Chunk-Visibility
  - 5 Motion Modes mit unterschiedlichen Presets

- **CameraManager - Zentrale Verwaltung**
  - **Subsysteme:**
    - CameraTransform: Position, Rotation, Direction Vectors
    - CameraProjection: FOV, Aspect Ratio, Near/Far Planes
    - CameraMotionModel: Velocity, Acceleration, Look Smoothing
    - CameraWorldBinding: Floating Origin, Chunk Tracking
  - **Update-API:**
    - Update(dt, movementInput, lookInput, verticalInput, isSprinting)
    - movementInput: Vector3D<float> (X/Z für horizontal, Y für vertikal in Fly-Mode)
    - lookInput: Vector2D<float> (Mouse Delta)
    - verticalInput: float (Space/Ctrl für Up/Down)
    - isSprinting: bool (Sprint-Modifier)
  - **Movement Processing:**
    - ComputeWorldMovementDirection(): Berücksichtigt Current Motion Mode
      - Walk/Default: Nutzt ForwardFlat (keine Y-Komponente)
      - Fly/Spectator: Nutzt Forward (volle 3D-Bewegung)
    - ApplyMovementInput(): Exponential Decay für smooth Acceleration/Deceleration
    - ApplyVerticalInput(): Separate vertikale Bewegung
  - **Rotation Processing:**
    - ApplyLookInput(): Exponential Smoothing für Maus-Bewegung
    - Yaw += delta.X (unbegrenzt)
    - Pitch -= delta.Y (clamped -89° bis +89°)
    - Roll Return: Automatischer Return zu 0° wenn !AllowRoll
  - **Matrix Caching:**
    - GetViewMatrix(): Cached mit matricesDirty Flag
    - GetProjectionMatrix(): Cached mit matricesDirty Flag
    - UpdateAspectRatio(width, height): Invalidiert Cache
  - **Motion Mode Switching:**
    - SetMotionMode(mode): Lädt entsprechende Presets
    - Walk, Fly, Glider, Cinematic, Spectator Modi
  - **Direct Setters:**
    - TeleportTo(worldX, worldY, worldZ): Reset Position + Velocity
    - SetRotation(yaw, pitch, roll): Direkte Rotation-Änderung
    - SetFovModifier(modifier): Dynamische FOV-Änderung (Sprint, Zoom)
  - **Output:**
    - GetVisibilityContext(): Erzeugt CameraVisibilityContext für Renderer

- **CameraTransform - Position & Rotation**
  - **Position:**
    - LocalPosition: Vector3D<float> (relativ zu OriginChunk, in Blöcken)
    - Nutzt Floating Origin Pattern für Precision
  - **Rotation (Euler Angles in Degrees):**
    - Yaw: -180° bis +180° (horizontal)
    - Pitch: -90° bis +90° (vertikal, clamped)
    - Roll: -180° bis +180° (tilt, nur für Glider/Cinematic)
  - **Direction Vectors (Computed on Demand):**
    - Forward: Volle 3D-Richtung (berücksichtigt Pitch + Yaw)
    - Right: Cross(Forward, WorldUp) mit Roll-Rotation
    - Up: Cross(Right, Forward)
    - ForwardFlat: Forward projected auf XZ-Ebene (für Walk-Mode)
  - **Roll Support:**
    - RotateAroundAxis(vector, axis, angleRad): Rodrigues' Rotation Formula
    - Für Glider-Banking und Cinematic-Shots
  - **Factory:**
    - Default: Position (0, 64, 0), Yaw -90°, Pitch/Roll 0°

- **CameraProjection - Projection Matrix**
  - **FOV System:**
    - BaseFov: Standard Field of View (70° default)
    - FovModifier: Dynamische Änderung (Sprint +10°, Zoom -20°, etc.)
    - EffectiveFov: BaseFov + FovModifier
    - EffectiveFovRadians: Für Matrix-Berechnung
  - **Planes:**
    - NearPlane: 0.05f (sehr nah für First-Person)
    - FarPlane: 2000f (für große Render-Distanzen)
  - **Aspect Ratio:**
    - AspectRatio: width / height
    - UpdateAspect(width, height): Bei Window-Resize
  - **Matrix Generation:**
    - GetProjectionMatrix(): Perspective Projection mit EffectiveFov
  - **Presets:**
    - Standard: 70° FOV, 16:9, Near 0.05f, Far 2000f
    - Cinematic: 50° FOV (engerer Blickwinkel), 21:9, Far 5000f
    - FirstPerson: 90° FOV (Quake-Style), 16:9, Near 0.01f

- **CameraMotionModel - Movement & Look Parameters**
  - **Movement Parameters:**
    - MaxSpeed: Basis-Geschwindigkeit in m/s
    - SprintMultiplier: Sprint-Faktor (2.5x default)
    - VerticalSpeed: Up/Down-Geschwindigkeit
  - **Acceleration & Damping:**
    - AccelerationRate: Geschwindigkeit der Beschleunigung (10.0 default)
    - DecelerationRate: Geschwindigkeit der Verzögerung (8.0 default)
    - DragCoefficient: Luftwiderstand für Glider (0.0 default, 0.02 für Glider)
  - **Rotation Parameters:**
    - LookSensitivity: Maus-Empfindlichkeit (0.1 default)
    - LookSmoothing: Exponential Smoothing Faktor (18.0 default)
    - MaxPitch: Pitch-Begrenzung (89° default)
    - AllowRoll: Roll-Rotation erlaubt? (false für Walk/Fly, true für Glider)
    - RollReturnRate: Return-to-Zero Rate für Roll (5.0 default)
  - **Runtime State:**
    - Velocity: Vector3D<float> (aktuelle Geschwindigkeit)
    - VerticalVelocity: float (separate Y-Achse)
    - SmoothedLookDelta: Vector2D<float> (geglätteter Maus-Input)
  - **Motion Processing:**
    - ApplyMovementInput(inputDir, isSprinting, dt):
      - Berechnet targetVelocity = inputDir × (MaxSpeed × Sprint)
      - ExpDecay zu targetVelocity mit AccelerationRate oder DecelerationRate
    - ApplyVerticalInput(verticalInput, dt):
      - Berechnet targetVert = verticalInput × VerticalSpeed
      - ExpDecay zu targetVert
    - ApplyLookInput(rawDelta, dt):
      - Smoothing: Lerp(SmoothedLookDelta, rawDelta, smoothFactor)
      - smoothFactor = 1 - exp(-LookSmoothing × dt)
      - Returns SmoothedLookDelta × LookSensitivity
    - ComputePositionDelta(dt):
      - Returns (Velocity.X × dt, Velocity.Y × dt + VerticalVelocity × dt, Velocity.Z × dt)
  - **Presets:**
    - Walk: 6 m/s, Sprint 2.5x, Accel 10, Decel 8, Look 0.1
    - Fly: 12 m/s, Sprint 4x, Accel 12, Decel 6, Drag 0.1
    - Glider: 25 m/s, Sprint 1.5x, Accel 3, Decel 1, Drag 0.02, AllowRoll true
    - Cinematic: 2 m/s, Sprint 3x, Accel 4, Decel 3, Look 0.05, AllowRoll true

- **CameraWorldBinding - Floating Origin Pattern**
  - **World Anchor:**
    - OriginChunk: ChunkCoord (aktueller Referenz-Chunk für LocalPosition)
    - CurrentChunk: ChunkCoord (Chunk in dem Kamera aktuell ist)
    - LocalOffset: Vector3D<float> (Position innerhalb CurrentChunk, 0-16)
  - **Origin Shift Detection:**
    - OriginShiftThreshold: 256 Blöcke (16 Chunks)
    - Wenn Distance(Camera, Origin) > 256: Origin Shift zu CurrentChunk
    - Recalculates LocalPosition relativ zu neuem OriginChunk
    - Verhindert Float-Precision-Probleme bei großen Distanzen
  - **Events:**
    - OriginShifted(oldOrigin, newOrigin): Für Renderer-Updates
    - ChunkChanged(newChunk): Wenn Kamera Chunk-Grenze überquert
  - **Coordinate Conversion:**
    - LocalToWorld(localPosition): OriginChunk × 16 + localPosition
    - WorldToLocal(worldX, worldY, worldZ): world - OriginChunk × 16
    - WorldToChunkLocal(x, y, z): Static Helper für (ChunkCoord, offset) Tuple
    - GetChunkLocalPosition(chunk): Chunk-Position relativ zu OriginChunk
  - **Helpers:**
    - FloorDivide(a, b): Korrekte Division für negative Zahlen (C# / tut das nicht!)
    - Mod(a, b): Korrekte Modulo für negative Zahlen (result < 0 ? result + b : result)
  - **Update Flow:**
    1. UpdateFromLocalPosition(localPosition)
    2. Berechnet worldPos = LocalToWorld(localPosition)
    3. Bestimmt newChunk aus worldPos
    4. ChunkChanged Event wenn newChunk != CurrentChunk
    5. Berechnet LocalOffset innerhalb Chunk (Mod 16)
    6. CheckOriginShift(): Shift wenn distance > 256
    7. Returns correctedPosition (wichtig: muss zu LocalPosition geschrieben werden!)

- **CameraVisibilityContext - Output für Rendering**
  - **Position Data:**
    - CameraChunk: ChunkCoord (für Chunk-Priorität)
    - LocalPosition: Vector3D<float> (für Shader-Uniforms)
    - Forward: Vector3D<float> (für Directional Culling)
  - **Matrices:**
    - ViewMatrix: Matrix4X4<float>
    - ProjectionMatrix: Matrix4X4<float>
    - ViewProjectionMatrix: View × Projection (für Frustum)
  - **Frustum:**
    - Frustum: ViewFrustum (6 Planes für Culling)
    - Automatisch extrahiert aus ViewProjectionMatrix
  - **Distances:**
    - RenderDistanceChunks: int (für Chunk-Loading)
    - FarPlane: float (für Fog, LOD)
  - **Constructor:**
    - Nimmt alle Daten von CameraManager
    - Berechnet ViewProjectionMatrix
    - Extrahiert Frustum via ViewFrustum.ExtractFromMatrix()

- **ViewFrustum - Frustum Culling**
  - **Planes:**
    - Left, Right, Bottom, Top, Near, Far (6 Plane3D Structs)
  - **Extraction:**
    - ExtractFromMatrix(viewProjection): Standard-Algorithmus
      - Left: VP.M14 + VP.M11 (alle 4 Komponenten)
      - Right: VP.M14 - VP.M11
      - Bottom: VP.M14 + VP.M12
      - Top: VP.M14 - VP.M12
      - Near: VP.M14 + VP.M13
      - Far: VP.M14 - VP.M13
    - Jede Plane wird normalisiert (.Normalized())
  - **Intersection Tests:**
    - Intersects(Box3D aabb): AABB vs Frustum
      - P-Vertex-Test für alle 6 Planes
      - Returns true wenn AABB komplett oder teilweise sichtbar
    - Intersects(center, radius): Sphere vs Frustum
      - Distance-Check für alle 6 Planes
      - Returns true wenn Sphere >= -radius für alle Planes
  - **TestPlane Helper:**
    - P-Vertex: Positive Vertex relativ zu Plane Normal
    - pVertex.X = plane.Normal.X >= 0 ? aabb.Max.X : aabb.Min.X (analog Y, Z)
    - Returns plane.DistanceTo(pVertex) >= 0

- **Plane3D - Plane Representation**
  - **Components:**
    - Normal: Vector3D<float> (A, B, C Komponenten)
    - Distance: float (D Komponente)
  - **Constructors:**
    - Plane3D(a, b, c, d): Direkte Komponenten
    - Plane3D(normal, distance): Von Vector + Scalar
  - **Normalization:**
    - Normalized(): Returns neue Plane mit unit-length Normal
    - Normal / length, Distance / length
  - **Distance:**
    - DistanceTo(point): Dot(Normal, point) + Distance
    - Signed Distance (positiv = vor Plane, negativ = hinter Plane)

- **MathHelper - Math Utilities**
  - **Constants:**
    - Deg2Rad: π / 180
    - Rad2Deg: 180 / π
  - **Functions:**
    - Clamp(value, min, max): Math.Max(min, Math.Min(max, value))
    - Lerp(a, b, t): Linear Interpolation a + (b - a) × t
    - ExpDecay(current, target, decay, dt): Exponential Damping
      - Returns Lerp(current, target, 1 - exp(-decay × dt))
      - Smooth, frame-independent Acceleration/Deceleration
      - Overloads: float und Vector3D<float>

- **WorldRelevanceFilter - Integration mit CameraVisibilityContext**
  - **Update-Methode geändert:**
    - VORHER: UpdateFromCamera(cameraPosition, viewProjection)
    - NACHHER: UpdateFromCamera(CameraVisibilityContext)
    - Cached: visibility, cameraChunk, frustum
    - Override RenderDistance wenn visibilityContext.RenderDistanceChunks > 0
  - **ShouldRender() nutzt jetzt cached Frustum:**
    - Distanz-Check wie vorher
    - frustum.Intersects(aabb) statt manueller Frustum-Test
  - **UpdateChunkPriority() hinzugefügt:**
    - UpdateChunkPriority(ChunkJob, isPlayerInside)
    - Setzt chunk.IsInFrustum via frustum.Intersects(aabb)
    - Delegiert zu chunk.UpdatePriority(cameraChunk, isPlayerInside)
  - **GetChunkAABB() geändert:**
    - Berechnet localPos relativ zu visibility.CameraChunk
    - Bessere Float-Precision für weit entfernte Chunks

- **Architektur & Design Patterns**
  - Component-based Architecture: CameraManager als Koordinator
  - Floating Origin Pattern: Verhindert Float-Precision-Probleme
  - Exponential Decay: Smooth, frame-independent Damping
  - Factory + Preset Pattern: Vordefinierte Motion Modes
  - Cached Matrix Generation: Dirty Flag Pattern
  - Event-driven Origin Shifts: Observer Pattern
  - Frustum Culling: Standard-Algorithmus für Visibility

- **Performance-Optimierungen**
  - Matrix Caching: Nur neu berechnen wenn matricesDirty
  - Lazy Direction Vectors: Forward/Right/Up nur on-demand
  - Exponential Decay: 1 Exp-Call statt Loop
  - P-Vertex Test: Effizientes AABB-Frustum-Culling
  - Camera-relative AABB: Bessere Float-Precision

- **Vollständige XML-Dokumentation**
  - ⚠️ XML-Dokumentation fehlt noch komplett
  - TODO: Summaries für alle Klassen
  - TODO: Methoden-Dokumentation
  - TODO: Parameter-Beschreibungen

- **Status:** ✅ Camera-Manager vollständig funktional
- **Nächste Schritte:**
  - XML-Dokumentation hinzufügen
  - Integration in GameLoop OnUpdate/OnRender
  - Input-System mit CameraManager verbinden
  - Chunk-System mit WorldRelevanceFilter verbinden
  - Testing mit verschiedenen Motion Modes

## 0.7.1.1 Alpha | Bugfix: Camera & Frustum Culling Critical Fixes - 03.03.2026

- **Kritische Matrix-Berechnungs-Fehler behoben**
  - **CameraVisibilityContext - ViewProjectionMatrix Multiplikations-Reihenfolge:**
    - VORHER (falsch): `viewMatrix * projectionMatrix`
    - NACHHER (korrekt): `projectionMatrix * viewMatrix`
    - **Problem:** Matrix-Multiplikation ist nicht kommutativ!
    - **Ursache:** VP = P × V (Standard in Computer Graphics)
    - **Effekt:** Frustum-Culling funktionierte komplett falsch, alle Chunks wurden falsch gecullt
    - **Fix:** Korrekte Reihenfolge `ProjectionMatrix * ViewMatrix`

- **ViewFrustum-Extraction-Fehler behoben**
  - **ViewFrustum.ExtractFromMatrix() - Right Plane Formula:**
    - VORHER (Copy-Paste-Fehler): `vp.M14 + vp.M11` (gleiche Formel wie Left Plane!)
    - NACHHER (korrekt): `vp.M14 - vp.M11`
    - **Problem:** Right Plane hatte identische Formel wie Left Plane
    - **Ursache:** Copy-Paste-Fehler beim Implementieren
    - **Effekt:** Frustum hatte zwei linke Planes statt Left + Right
    - **Fix:** Korrekte Formel mit Minus-Operator für Right Plane
    - **Erklärung:** Right Plane normal zeigt nach innen (negative X-Richtung)

- **CameraMotionModel-Bewegungs-Fehler behoben**
  - **ComputePositionDelta() - VerticalVelocity Multiplikation:**
    - VORHER (falsch): `Velocity.Y * dt + VerticalVelocity + dt`
    - NACHHER (korrekt): `Velocity.Y * dt + VerticalVelocity * dt`
    - **Problem:** Fehlender Multiplikations-Operator zwischen VerticalVelocity und dt
    - **Ursache:** Tippfehler beim Implementieren
    - **Effekt:** Vertikale Bewegung (Space/Ctrl) war extrem schnell und frame-abhängig
    - **Fix:** `VerticalVelocity * dt` statt `VerticalVelocity + dt`
    - **Erklärung:** Geschwindigkeit muss mit DeltaTime multipliziert werden für frame-unabhängige Bewegung

- **WorldRelevanceFilter-Logik-Fehler behoben**
  - **ShouldSimulate() - Doppelter HasWater-Check:**
    - VORHER (falsch): 
      ```csharp
      if (chunk.Metadata?.HasActiveEntities == true)  // OK
          return true;
      if (chunk.Metadata?.HasWater == true && IsWithinDistance(...))  // Falsch: HasWater statt HasActiveEntities
          return true;
      ```
    - NACHHER (korrekt):
      ```csharp
      if (chunk.Metadata?.HasActiveEntities == true)
          return true;
      if (chunk.Metadata?.HasWater == true && IsWithinDistance(...))
          return true;
      ```
    - **Problem:** Erster Check prüfte HasActiveEntities zweimal (Copy-Paste-Fehler)
    - **Ursache:** Zeile wurde dupliziert statt geändert
    - **Effekt:** Chunks mit Wasser wurden korrekt simuliert, aber Kommentar war irreführend
    - **Fix:** Korrigierter Kommentar und Logik-Überprüfung

- **WorldRelevanceFilter-AABB-Berechnung korrigiert**
  - **GetChunkAABB() - Koordinaten-System-Fehler:**
    - VORHER (falsch):
      ```csharp
      return new Box3D<float>(
          new Vector3D<float>(coord.X * 16f, coord.Y * 16f, coord.Z * 16f),  // Absolute Welt-Koordinaten!
          new Vector3D<float>((coord.X + 1) * 16f, (coord.Y + 1) * 16f, (coord.Z + 1) * 16f)
      );
      ```
    - NACHHER (korrekt):
      ```csharp
      var localPos = new Vector3D<float>(
          (coord.X - visibility.CameraChunk.X) * 16f,
          (coord.Y - visibility.CameraChunk.Y) * 16f,
          (coord.Z - visibility.CameraChunk.Z) * 16f
      );
      return new Box3D<float>(
          localPos,                                              // Camera-relative Min
          localPos + new Vector3D<float>(16f, 16f, 16f)         // Camera-relative Max
      );
      ```
    - **Problem:** AABB wurde in absoluten Welt-Koordinaten berechnet
    - **Ursache:** Floating Origin Pattern nicht korrekt angewendet
    - **Effekt:** Float-Precision-Probleme bei weit entfernten Chunks (>1000 Blöcke)
    - **Fix:** AABB relativ zu CameraChunk berechnen (Floating Origin)
    - **Erklärung:** Frustum arbeitet mit View-Matrizen in Camera-Space, nicht World-Space!

- **Auswirkungen der Fixes:**
  - ✅ Frustum Culling funktioniert jetzt korrekt
  - ✅ Nur sichtbare Chunks werden gerendert (Performance-Gewinn!)
  - ✅ Vertikale Kamera-Bewegung ist frame-unabhängig
  - ✅ Weit entfernte Chunks werden korrekt gecull't (keine Float-Precision-Fehler)
  - ✅ Chunk-Simulation-Logik ist jetzt korrekt

- **Testing & Validation:**
  - ⚠️ Benötigt Testing mit echten Chunks
  - ⚠️ Frustum-Visualisierung für Debug empfohlen
  - ⚠️ Testing bei großen Distanzen (>10000 Blöcke) notwendig

- **Status:** ✅ Kritische Bugs behoben, bereit für Testing
- **Nächste Schritte:**
  - Integration in GameLoop testen
  - Frustum-Debug-Rendering implementieren
  - Chunk-Loading mit Culling testen
  - Performance-Profiling mit korrektem Culling