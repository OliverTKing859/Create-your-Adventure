# Changelog

---

## 0.0.0.0 Alpha | Projekt Start - 10.01.2026

Initialer Projekt-Start. Repository angelegt, Grundstruktur erstellt.

### Hinzugefügt:

+ Projekt gestartet
+ Repository erstellt

---

## 0.1.0.0 Alpha | Hello World - 20.01.2026

Erster lauffähiger Code.

### Hinzugefügt:

+ Hello World Ausgabe

---

## 0.1.1.0 Alpha | Create Window - 20.01.2026

Erstellt ein Fenster in C# mit Silk.NET auf OpenGL-Basis.

### Hinzugefügt:

+ Fenster in HD-Auflösung mit OpenGL 4.6
+ Methoden: OnLoad, OnRender, OnUpdate, OnClose
+ Hintergrundfarbe (fast Schwarz, RGB)

---

## 0.1.2.0 Alpha | KHR Debug - 24.01.2026

Debug-Unterstützung für OpenGL hinzugefügt.

### Hinzugefügt:

+ Unsafe Code Unterstützung (Unsafe OnLoad)
+ KHR Debug Fenster mit Konsolen-Ausgabe für Debug-Nachrichten

---

## 0.1.3.0 Alpha | Create Triangle - 30.01.2026

Erstes gerendetes Objekt: ein farbiges Dreieck.

### Hinzugefügt:

+ Dreieck-Rendering mit VBO, VAO und EBO
+ Vertex & Fragment Shader (GLSL 4.6)
+ RGB-Farben für das Dreieck

---

## 0.1.4.0 Alpha | Create Cube - 26.01.2026

Erweiterung des Renderings auf einen Würfel.

### Hinzugefügt:

+ Würfel-Rendering mit VBO, VAO und EBO
+ Vertex & Fragment Shader (GLSL 4.6)
+ RGB-Farben für den Würfel

---

## 0.1.5.0 Alpha | Create Camera - 26.01.2026

Erste Kamera mit Matrix-Transformationen.

### Hinzugefügt:

+ Kamera mit Model-, View- und Projection-Matrix
+ Yaw und Pitch Rotation

---

## 0.1.6.0 Alpha | Camera Controls - 27.01.2026

Interaktive Kamerasteuerung über Maus und Tastatur.

### Hinzugefügt:

+ Maussteuerung (Gyro) und Tastatursteuerung (WASD, Space, Left Shift)
+ Delta Time für frame-unabhängige Bewegung
+ Pitch-Begrenzung auf -89° bis +89°

---

## 0.1.7.0 Alpha | Acceleration / Deceleration, Smooth - 29.01.2026

Flüssigere Kamerabewegung durch Beschleunigung und Glättung.

### Hinzugefügt:

+ Beschleunigung und Verzögerung für Kamerabewegung
+ Smooth Mouse Movement
+ Erhöhte maximale Kamerageschwindigkeit

---

## 0.1.8.0 Alpha | Mouse Sensitivity / Smoothing Factor - 29.01.2026

Feinabstimmung der Maussteuerung.

### Hinzugefügt:

+ Maus-Sensitivität einstellbar
+ Maus-Smoothing-Faktor einstellbar

### Verändert:

~ Left Shift ersetzt durch Left STRG

---

## 0.1.9.0 Alpha | Revision of Camera Functions - 30.01.2026

Überarbeitung der Kamerabewegungslogik für horizontale und vertikale Trennung.

### Verändert:

~ Beschleunigung/Verzögerung aufgeteilt auf horizontale und vertikale Achse
~ Aufbauende und abbauende Geschwindigkeit neu implementiert
~ Smoothing Factor überarbeitet

---

## 0.1.10.0 Alpha | Fine-tuning the Camera - 30.01.2026

Code-Qualität und Vorbereitung auf Abstraktion verbessert.

### Verändert:

~ Variablennamen überarbeitet und vereinheitlicht
~ Code-Optimierungen durchgeführt
~ Vorbereitung auf Kamera-Klassen-Abstraktion

---

## 0.1.11.0 Alpha | Smoothing Reworked Again - 30.01.2026

Weiterer Durchgang der Smoothing-Implementierung.

### Verändert:

~ Smoothing-Logik erneut überarbeitet

---

## 0.1.12.0 Alpha | Abstraction Camera To Camera Class - 01.02.2026

Kamera in eine eigene Klasse ausgelagert.

### Hinzugefügt:

+ Eigenständige Camera-Klasse

---

## 0.1.13.0 Alpha | Added Commentary Improvements - 01.02.2026

Dokumentation der Kamera-Klasse verbessert.

### Hinzugefügt:

+ Kommentare zur Camera-Klasse hinzugefügt

---

## 0.1.14.0 Alpha | Texture Support - 02.02.2026

Textur-Unterstützung für den Würfel implementiert.

### Hinzugefügt:

+ StbImageSharp Integration für Textur-Dekodierung
+ Textur-Loader mit dirt.png (RGBA, Mipmaps, Nearest Filtering)
+ Überarbeitete Shader mit Textur-Unterstützung (vec2 aTexCoord, sampler2D uTexture)
+ 24 Vertices (4 pro Seite) für korrektes UV-Mapping

---

## 0.1.15.0 Alpha | GPU Instancing - 02.02.2026

Performantes Rendering vieler Blöcke durch GPU-Instancing.

### Hinzugefügt:

+ GPU-Instancing mit 1 Draw Call für 4096 Blöcke (16×16×16 Grid)
+ Instance VBO mit Model-Matrizen (Matrix4X4[])
+ Vertex Attribute Divisor für per-instance Daten
+ Shader-Fehlerprüfung: CheckShaderCompileErrors() und CheckProgramLinkErrors()
+ CreateInstanceData() für Instanz-Matrizen-Generierung (zentriertes Grid)

### Verändert:

~ Vertex Shader: Explizite vec4-Attribute (aInstanceMatrix0-3) für Treiber-Kompatibilität
~ Fragment Shader: Konsistente vec2 vTexCoord

### Entfernt:

- uModel Uniform (ersetzt durch per-instance aInstanceMatrix)

---

## 0.1.16.0 Alpha | Chunk Coordinate System - 03.02.2026

Grundlegendes Koordinatensystem für Chunks implementiert.

### Hinzugefügt:

+ ChunkPosition (Chunk-Koordinaten in 3D)
+ LocalPosition (relative Koordinaten 0–15 innerhalb eines Chunks)
+ WorldPosition (absolute Welt-Koordinaten)
+ Konvertierungsfunktionen: LocalToWorld(), WorldToChunkPosition(), WorldToLocal()
+ Instance-Daten basierend auf Chunk-Position berechnet

---

## 0.1.17.0 Alpha | Chunk Class Abstraction & Naming Conventions - 04.02.2026

Chunk-Logik in eine eigene Klasse abstrahiert, Naming Conventions eingeführt.

### Hinzugefügt:

+ Chunk-Klasse mit InstanceMatrices, InstanceCount und RebuildInstanceData()

### Verändert:

~ C# Naming Conventions eingeführt (PascalCase für Constants/Properties, camelCase für Parameter)
~ Program.cs liest nun chunk.InstanceMatrices und chunk.InstanceCount

### Gefixt:

* Typo: RebuildInsanceData() → RebuildInstanceData()

---

## 0.2.0.0 Alpha | XML Documentation & AssetLoader - 04.02.2026

Dokumentation und Asset-Verwaltung eingeführt.

### Hinzugefügt:

+ XML-Dokumentation (Summaries) für alle Public-Methoden in Chunk.cs, Camera.cs und AssetLoader.cs
+ AssetLoader-Klasse mit Asset-Suche in assets/base/ und assets/modded/
+ Caching für schnelle wiederholte Asset-Lookups
+ Methoden: GetAudioPath, GetModelPath, GetShaderPath, GetTexturePath

### Gefixt:

* Typo: RebuildInsanceData() → RebuildInstanceData()
* Typo: WorldTolocal() → WorldToLocal()

---

## 0.2.1.0 Alpha | Logger System Implementation - 05.02.2026

Strukturiertes Logging-System für alle Hauptklassen implementiert.

### Hinzugefügt:

+ Logger-Klasse mit Info, Warn, Error und Debug Methoden
+ Farbcodierte Konsolen-Ausgabe (Weiß, Gelb, Rot, Grau)
+ EnableDebug Toggle für Debug-Nachrichten
+ Kategorisierte Log-Nachrichten: [ENGINE], [OPENGL], [SHADER], [TEXTURE], [CHUNK], [ASSETLOADER], [CAMERA], [INPUT]
+ OpenGL Debug Callback kategorisiert nach Severity (High/Medium/Low)

### Verändert:

~ Console.WriteLine() durch Logger-Aufrufe ersetzt

---

## 0.2.2.0 Alpha | Infinite World Architecture - 06.02.2026

Architektur für eine unendliche Welt vorbereitet.

### Hinzugefügt:

+ 64-Bit Chunk-Koordinaten (Vector3D<long>) für ±9,2 Trillionen Chunks pro Achse
+ TODO-Kommentare für Block-Daten, Visibility Culling, Face Culling, Greedy Meshing
+ Kommentierte Beispiel-Methode LocalToWorldRelative() für World-Offset Rendering

### Verändert:

~ WorldToChunkPosition() und WorldToLocal() zu public static geändert
~ XML-Dokumentation mit Architektur-Hinweisen erweitert

### Gefixt:

* Typo: WorldTolocal() → WorldToLocal()

---

## 0.2.3.0 Alpha | ImGui Debug Display - 08.02.2026

Debug-Overlay mit FPS-Anzeige und Performance-Statistiken.

### Hinzugefügt:

+ DebugDisplay.cs als Static-Klasse mit Update() und RenderImGui()
+ FPS-Tracking mit Min/Max-Statistiken und Frame-Time in Millisekunden
+ Dynamische FPS-Farbcodierung: Grün ≥60, Gelb ≥30, Rot <30
+ ImGui-Fenster: transparenter Hintergrund (Alpha 0.35), oben links (10, 10), Auto-Resize
+ Reset-Stats-Button

---

## 0.2.3.1 Alpha | Fix: DebugDisplay General Fixes - 08.02.2026

Allgemeine Bugfixes und Verbesserungen am Debug-Display.

### Verändert:

~ FPS-Farbcodierung zuverlässiger implementiert
~ ImGui-Layout und Abstände optimiert
~ Floating-Point-Formatierung vereinheitlicht

### Gefixt:

* Reset Stats Funktionalität stabilisiert

---

## 0.2.4.0 Alpha | ImGui Debug Display Improvements - 08.02.2026

Stabilitäts- und Layout-Verbesserungen am Debug-Display.

### Verändert:

~ Stabile FPS-Berechnung mit Akkumulator und 1s-Updateintervall
~ ImGui-Fensterflags, Positionierung und Transparenz optimiert

### Gefixt:

* Min-/Max-FPS-Tracking und Frame-Time-Anzeige korrigiert
* Reset-Stats-Button repariert

---

## 0.3.0.0 Alpha | Refactor: WindowManager Abstraction - 08.02.2026 ⚠️ BROKEN

Beginn der Architektur-Refaktorierung. Programm läuft aktuell nicht.

### Hinzugefügt:

+ WindowManager als Singleton (zentralisiert Fenster, GL Context und Input)
+ WindowSettings für flexible Fensterkonfiguration (Title, Width, Height, GL Version, Debug Context)
+ Event-basierte Architektur: OnLoad, OnUpdate, OnRender, OnClose

### Verändert:

~ Program.Main() auf ~10 Zeilen vereinfacht
~ Globale statische Variablen für Window/GL/Input entfernt

---

## 0.3.1.0 Alpha | Refactor: RenderManager Abstraction - 09.02.2026

Zentrale Render-Verwaltung implementiert, Integration ausstehend.

### Hinzugefügt:

+ RenderManager Singleton als zentrale Rendering-Verwaltung
+ IRenderContext Interface (Graphics API agnostisch): BeginFrame, EndFrame, SetViewport, etc.
+ OpenGLRenderContext als konkrete IRenderContext-Implementierung
+ Viewport-Management mit automatischem Window-Resize-Handling

---

## 0.3.2.0 Alpha | ShaderProgram Class Finalization - 13.02.2026

Vollständige ShaderProgram-Klasse mit Compile-, Link- und Uniform-Management.

### Hinzugefügt:

+ ShaderProgram-Klasse: Vertex & Fragment Shader Kompilierung mit Fehlerprüfung
+ Uniform-Caching mit GetUniformLocation() und Dictionary-Cache
+ SetUniform() Überladungen für int, float, Vector2D, Vector3D, Vector4D, Matrix4X4
+ CheckShaderCompileStatus() und CheckProgramLinkStatus() Validierung
+ Dispose Pattern mit isDisposed Flag

---

## 0.3.3.0 Alpha | ShaderManager Implementation & Documentation - 15.02.2026

Zentrale Shader-Verwaltung als Singleton implementiert.

### Hinzugefügt:

+ ShaderManager Singleton mit Thread-safe Initialization (Lock-Pattern)
+ Shader-Caching: LoadProgram(), GetProgram()
+ Aktive Program-Verwaltung: UseProgram(), UnbindProgram()
+ XML-Dokumentation für alle Public Methods
+ Logger-Integration mit [SHADER]-Kategorie

### Verändert:

~ Program.cs nutzt ShaderManager statt direkter ShaderProgram-Verwaltung

---

## 0.3.4.0 Alpha | API Abstraction: Graphics API Agnostic Shader System - 15.02.2026

Vollständige Graphics-API-Abstraktionsschicht für Shader.

### Hinzugefügt:

+ IShaderProgram Interface (API-agnostisch): Compile, Use, SetUniform
+ Factory Pattern im ShaderManager für automatische Backend-Erkennung
+ OpenGLShaderProgram als konkrete OpenGL-Implementierung mit injiziertem GL-Context

### Verändert:

~ ShaderManager arbeitet ausschließlich mit IShaderProgram Interface

---

## 0.3.5.0 Alpha | Shader System Abstraction & File Loading - 15.02.2026

Shader-System vollständig funktional mit Datei-Loading und Performance-Optimierungen.

### Hinzugefügt:

+ Shader-Dateisystem: .vert und .frag aus assets/base/shaders/opengl/ laden
+ basic.vert (Vertex Shader mit Instancing-Support) und basic.frag (Fragment Shader)
+ State-Tracking in UseProgram() (bindet nur wenn nicht bereits aktiv)
+ Initialisierungs-Reihenfolge: OnLoad: WindowManager → RendererManager → ShaderManager
+ Vollständige XML-Dokumentation für IShaderProgram, ShaderManager, OpenGLShaderProgram

### Verändert:

~ Program.cs Shader-Code von >100 auf ~15 Zeilen reduziert

---

## 0.4.0.0 Alpha | Texture System Part 1: API Abstraction - 16.02.2026 ⚠️ IN ENTWICKLUNG

Interfaces und Manager für das Textur-System fertig, noch nicht startbereit.

### Hinzugefügt:

+ ITexture Interface: LoadFromFile(), LoadFromData(), Bind(), Unbind()
+ ITextureAtlas Interface: AddTexture(), Build(), GetRegion()
+ AtlasRegion Struct mit Pixel- und normalisierten UV-Koordinaten
+ TextureSettings Record mit Presets: PixelArt, Smooth, Atlas
+ TextureManager Singleton mit Factory Pattern, Texture-Caching und Atlas-Caching

---

## 0.4.1.0 Alpha | Texture System Part 2: Implementation - 16.02.2026

Textur-Management vollständig implementiert und getestet.

### Hinzugefügt:

+ OpenGLTexture2D: LoadFromFile() mit StbImageSharp, LoadFromData(), Mipmaps/Filtering/Wrapping
+ OpenGLTextureAtlas: Grid-Packing, automatische Größenberechnung (Potenz von 2), normalisierte UV-Koordinaten
+ TexWrapMode Enum: Repeat, ClampToEdge, MirroredRepeat
+ TexMinFilter Enum: 6 Optionen inkl. Mipmaps
+ TexMagFilter Enum: Nearest, Linear
+ BuildBlockAtlas() für schnelles Laden aus assets/base/textures/blocks/
+ BindTexture() / BindAtlas() mit State-Tracking
+ GetBlockUV() für UV-Zugriff

---

## 0.5.0.0 Alpha | Mesh System Part 1: VAO/VBO/EBO Abstraction - 20.02.2026 🔄 IN ARBEIT

Interfaces und OpenGL-Implementierung abgeschlossen, Integration ausstehend.

### Hinzugefügt:

+ IMesh Interface: Create(), Bind/Unbind, Draw(), DrawInstanced()
+ IVertexBuffer Interface: SetData<T>(), UpdateData<T>()
+ IIndexBuffer Interface: SetData()
+ VertexAttribute Struct mit Factory Methods: Position(), TexCoord(), Normal()
+ VertexLayout Klasse mit Fluent API (Add()) und automatischer Stride-Berechnung
+ VertexAttributeType Enum: Float, Int, UInt, Byte
+ OpenGLMesh, OpenGLVertexBuffer, OpenGLIndexBuffer Implementierungen
+ MeshManager Singleton mit Factory Pattern und Mesh-Caching
+ [MESH], [VBO], [EBO] Logger-Kategorien

---

## 0.5.1.0 Alpha | Project Documentation: LICENSE & README - 20.02.2026

Projekt-Dokumentation für Open-Source-Veröffentlichung erstellt.

### Hinzugefügt:

+ MIT License
+ README.md mit Projekt-Übersicht, Feature-Liste, Setup-Anleitung, Architektur-Dokumentation, Roadmap und Contributing Guidelines

---

## 0.5.2.0 Alpha | Mesh System Part 2: Complete Implementation - 22.02.2026

Mesh-Abstraktionssystem vollständig implementiert und validiert.

### Hinzugefügt:

+ OpenGLMesh: Create<T>() für indizierte und nicht-indizierte Meshes, VAO State Management
+ CreateQuad() und CreateCube() Convenience-Methoden im MeshManager
+ Vollständige XML-Dokumentation für IMesh, IVertexBuffer, IIndexBuffer

### Verändert:

~ VAO State Pattern: 1 Bind statt VBO + EBO + Attributes (reduziert OpenGL State Changes)

---

## 0.6.0.0 Alpha | Input System Part 1: Enums, State & Bindings - 24.02.2026 🔄 IN ENTWICKLUNG

Vollständiges Input-Abstraktionssystem mit Enums, State-Management und Binding-System.

### Hinzugefügt:

+ CursorMode Enum: Visible, Hidden, Locked, Confined, ConfinedHidden
+ KeyCode Enum: 60+ Tasten (A–Z, 0–9, F1–F12, Modifiers, Navigation, Numpad, etc.)
+ MouseButton Enum: Left, Right, Middle, Button4, Button5
+ GamepadButton Enum: 15 Buttons (Face, Bumpers, Triggers, D-Pad, Sticks, Special)
+ GamepadAxis Enum: LeftStickX/Y, RightStickX/Y, LeftTrigger, RightTrigger
+ InputActionType Enum: Pressed, Held, Released, LongPress, DoublePress, Axis
+ InputState Klasse: Keyboard/Mouse/Gamepad State-Tracking, Long Press (0.5s), Double Press (0.3s), Key-Kombinationen
+ InputAction Klasse: Name, Type, Triggered/AxisChanged Events, Fluent Binding API
+ InputBinding System: KeyBinding, MouseButtonBinding, GamepadButtonBinding, GamepadAxisBinding mit Serialization

---

## 0.6.1.0 Alpha | Input System Part 2: InputManager & Silk.NET Integration - 25.02.2026 🔄 IN ENTWICKLUNG

InputManager implementiert, Integration in Program.cs ausstehend.

### Hinzugefügt:

+ InputManager Singleton mit Thread-safe Initialization
+ Cursor-Control-System: SetCursorMode(), LockCursor(), UnlockCursor(), ToggleCursorLock()
+ Direct Input Query API: IsKeyDown/Pressed/Released/LongPressed, GetMousePosition/Delta, GetGamepadAxis, etc.
+ GetMovementVector(): WASD + Gamepad Fallback, normalisierte Diagonalbewegung
+ GetLookVector(): Mouse-Delta + Gamepad Fallback
+ RegisterAction(), IsActionTriggered(), CheckAction()
+ Default Action Bindings: MoveForward/Backward/Left/Right, MoveUp/Down, Sprint, ToggleCursorLock
+ Event Handler für Keyboard, Mouse und Gamepad (Silk.NET Integration)
+ InputConverter: ConvertKey(), ConvertMouseButton(), ConvertGamepadButton()

---

## 0.6.2.0 Alpha | Input System Part 3: Device Abstraction & Converters - 26.02.2026 🔄 IN ENTWICKLUNG

Device-Abstraktionsschicht implementiert, Finalisierung ausstehend.

### Hinzugefügt:

+ IInputDevice Interface: Name, IsConnected, Initialize(), RegisterEvents(), UnregisterEvents(), Poll()
+ KeyboardDevice: Event-basierter Silk.NET Keyboard-Wrapper (OnKeyDown/OnKeyUp)
+ MouseDevice: Event-basierter Silk.NET Mouse-Wrapper inkl. SetCursorMode() und RawMouse Property
+ GamepadDevice: Hybrid-Wrapper (Events + Polling für Achsen), TriggerThreshold (0.5f)
+ InputConverter als separate Static-Klasse mit KeyConverter, MouseConverter, GamepadConverter
+ InputAnalyzer Placeholder-Klasse für zukünftige Pattern-Erkennung

### Verändert:

~ InputState Fields auf internal geändert für direkten Device-Zugriff
~ TriggerValues Dictionary und SetTriggerValue() zu InputState hinzugefügt

---

## 0.6.3.0 Alpha | Input System Part 4: Complete Implementation - 28.02.2026

Input-System vollständig implementiert und validiert.

### Hinzugefügt:

+ InputRegistry: Getrennt vom InputManager, Engine-Defaults, ProcessActions()
+ InputAnalyzer als Query-Interface für Camera/Debug/Editor (GetMovementVector, GetLookVector, IsDebugCombo)
+ Vollständige Camera-Integration: Camera.Update(deltaTime) statt 8 bool-Parameter

### Verändert:

~ InputManager auf Device-Abstraktion refaktoriert
~ Zeit-Injektion: EndFrame(deltaTime) statt selbst zählen

### Gefixt:

* MouseDevice.OnMouseDown rief fälschlicherweise SetMouseButtonUp auf
* MouseDevice.OnMouseUp fehlte komplett
* GamepadDevice Tippfehler in Methodennamen
* InputBinding fehlende Parse-Methoden ergänzt

---

## 0.6.3.1 Alpha | Bugfix: Rendering Pipeline & Input Integration - 28.02.2026

Kritische Rendering- und Input-Bugs behoben.

### Gefixt:

* OnRender() übergab keine View/Projection-Matrizen an Shader → Cube war unsichtbar
* BeginFrame/EndFrame-Reihenfolge korrigiert (vorher: EndFrame → Camera → BeginFrame, jetzt: BeginFrame → Camera → EndFrame)
* Camera.Update() rief nicht-existente Methoden auf (ApplyMovement/ApplyRotation → ProcessMovement/ProcessRotation)
* MeshManager.CreateCube() Signatur korrigiert (size → size + AtlasRegion)
* InputAnalyzer wurde in InputManager.Initialize() nicht initialisiert (null)
* Rotation-Berechnung Typo: yawRad 180 → 180f

---

## 0.6.3.2 Alpha | Input System: Documentation & Code Comments - 01.03.2026

Vollständige XML-Dokumentation für das gesamte Input-System.

### Hinzugefügt:

+ XML-Dokumentation für InputManager, InputState, InputBinding, InputAnalyzer, InputRegistry, InputConverter und alle Device-Klassen
+ Enum-Dokumentation für alle Werte (KeyCode, MouseButton, GamepadButton, etc.)
+ Inline-Comments für komplexe Logik (Action-Evaluierung, Lifecycle, Trigger-Threshold, Delta-Berechnung)

---

## 0.7.0.0 Alpha | Game Loop Architecture: Camera & Chunk System Part 1 (Skeleton) - 02.03.2026 🔄 IN ENTWICKLUNG

Architektur-Skelett implementiert, keine Funktionalität vorhanden.

### Hinzugefügt:

+ GameLoop: 5-Phasen-Modell (BeginFrame, Fixed Ticks, World Scheduler, Variable Update, Late Update)
+ TimeManager Singleton: Fixed Delta Time (1/60s), Frame Delta Time, Simulation/World Time Scale, Interpolation Alpha, Spiral of Death Prevention (max 5 Ticks)
+ ChunkCoord Struct: 64-Bit Koordinaten (long X/Y/Z), DistanceSquaredTo(), IEquatable
+ ChunkState Enum: Requested, Loading, Meshing, Active, Sleeping, Serializing, Unloaded
+ ChunkPriority Enum: Critical (60 TPS), High (30 TPS), Medium (10 TPS), Low (2 TPS), Background (1 TPS)
+ ChunkJob Klasse: Metadata, UpdatePriority(), dynamische TickRate-Anpassung
+ WorldRelevanceFilter: Distanz-Konfiguration (Render 16, Simulation 24, Load 32, Unload 40), ShouldRender/Simulate/Load/Unload
+ Frustum Struct: ExtractFromMatrix(), Intersects() für AABB (Skelett)
+ WorldScheduler Singleton: 8ms Budget, 5 Task-Typen, PriorityQueues, Telemetry
+ SchedulerTask Enum: ChunkLoading, ChunkMeshing, BlockUpdates, Lightning, EntitySimulation

---

## 0.7.1.0 Alpha | Camera Manager System Part 2: Complete Camera Implementation - 03.03.2026 🔄 IN ENTWICKLUNG

Vollständiges modulares Camera-Management-System implementiert.

### Hinzugefügt:

+ CameraManager: Zentrale Verwaltung mit Update(), GetVisibilityContext(), SetMotionMode(), TeleportTo()
+ CameraTransform: LocalPosition, Yaw/Pitch/Roll, Forward/Right/Up/ForwardFlat Vektoren, Rodrigues' Rotation für Roll
+ CameraProjection: BaseFov, FovModifier, EffectiveFov, NearPlane (0.05f), FarPlane (2000f), Presets
+ CameraMotionModel: MaxSpeed, SprintMultiplier, AccelerationRate, DecelerationRate, LookSensitivity, LookSmoothing, AllowRoll, ExpDecay-basierte Bewegungsverarbeitung
+ CameraWorldBinding: Floating Origin Pattern, OriginShiftThreshold (256 Blöcke), OriginShifted/ChunkChanged Events, LocalToWorld/WorldToLocal Koordinaten-Konvertierung
+ CameraVisibilityContext: ViewMatrix, ProjectionMatrix, ViewProjectionMatrix, Frustum, RenderDistanceChunks
+ ViewFrustum: ExtractFromMatrix(), Intersects() für AABB und Sphere, Plane3D Struct
+ MathHelper: Deg2Rad, Rad2Deg, Clamp, Lerp, ExpDecay (float + Vector3D Overloads)

### Verändert:

~ WorldRelevanceFilter.UpdateFromCamera() nimmt jetzt CameraVisibilityContext statt separater Parameter
~ GetChunkAABB() berechnet AABB kamera-relativ für Floating Origin

---

## 0.7.1.1 Alpha | Bugfix: Camera & Frustum Culling Critical Fixes - 03.03.2026

Kritische Matrix- und Frustum-Bugs behoben.

### Gefixt:

* CameraVisibilityContext: ViewProjectionMatrix Multiplikations-Reihenfolge falsch (viewMatrix * projectionMatrix → projectionMatrix * viewMatrix)
* ViewFrustum.ExtractFromMatrix(): Right Plane hatte identische Formel wie Left Plane (Copy-Paste-Fehler: + → -)
* CameraMotionModel.ComputePositionDelta(): VerticalVelocity Tippfehler (VerticalVelocity + dt → VerticalVelocity * dt)
* WorldRelevanceFilter.ShouldSimulate(): Doppelter HasActiveEntities-Check statt HasWater-Check (Copy-Paste-Fehler)
* WorldRelevanceFilter.GetChunkAABB(): AABB in absoluten Welt-Koordinaten statt kamera-relativ (Float-Precision-Fehler bei >1000 Blöcken)

---

## 0.7.2.0 Alpha | GameLoop Integration & Camera System Refinement - 05.03.2026 🔄 IN ENTWICKLUNG

GameLoop-Architektur nahezu vollständig integriert (~95%).

### Hinzugefügt:

+ 5-Phasen Game Loop vollständig mit CameraManager integriert
+ Spiral of Death Prevention validiert (max 5 Fixed Ticks)
+ Interpolation Alpha für smooth Rendering zwischen Fixed Ticks

### Verändert:

~ Camera.Update() nimmt dt, movementInput, lookInput, verticalInput, isSprinting
~ Input-Integration: BeginFrame/EndFrame Lifecycle korrekt in GameLoop

---

## 0.7.2.1 Alpha | Bugfix: Camera Rotation Axis - 08.03.2026

Kritische Kamera-Rotationsfehler behoben.

### Verändert:

~ CameraTransform: Quaternionen entfernt, trigonometrische Berechnung (Industriestandard für FPS-Kameras) eingeführt
~ Forward-Vektor: X = cos(pitch)*sin(yaw), Y = sin(pitch), Z = -cos(pitch)*cos(yaw)
~ Right-Vektor: Hängt nur von Yaw ab (Y immer 0 für horizontale Bewegung)
~ Ausführliche Code-Kommentare zur sphärisch-kartesischen Umrechnung

### Gefixt:

* Yaw war invertiert: Maus rechts → Kamera drehte nach links (Transform.Yaw += delta.X → -= delta.X)
* Seitwärts-Rotation beim Hoch/Runter-Schauen durch Quaternion-basierte lokale Achsenkombination

---

## 0.7.2.2 Alpha | Mini-Fix: Yaw Positive Direction Clarification - 08.03.2026

Yaw-Konventionen dokumentiert.

### Verändert:

~ CameraTransform: Yaw-Kommentar präzisiert (0° = -Z, 90° = +X, -90° = -X)

---

## 0.7.3.0 Alpha | Documentation Restructure & Changelog Rewrite - 15.03.2026

Dokumentationsstruktur komplett überarbeitet und neu organisiert.

### Hinzugefügt:

+ Neuer Ordner Docs/Changelogs/ als zentrale Dokumentationsablage
+ Changelogs Alpha.md: Vollständig neu geschriebenes Changelog mit sauberer Struktur (### Überschriften, --- Trennlinien, kategorisierte Einträge)
+ Commit Template.txt: Englischsprachige Commit-Vorlage mit standardisiertem Format (Added/Changes/Fixed/Removed)

### Verändert:

~ docs/ Ordner reorganisiert zu Docs/Changelogs/ (PascalCase, klarere Hierarchie)
~ 1 - Changelogs.md → Changelogs Alpha.md (beschreibender Name, kompletter Inhalt neu verfasst)
~ Commit-Vorlage.txt → Commit Template.txt (deutscher → englischer Dateiname)
~ Changelog-Format überarbeitet: Versions-Header mit Titel & Datum, Beschreibungszeilen, kategorisierte Sektionen
~ Alle 30+ Changelog-Einträge für Konsistenz und Lesbarkeit neu formatiert

### Entfernt:

- Alte docs/ Ordnerstruktur (1 - Changelogs.md, notes/Commit-Vorlage.txt)
- Altes unstrukturiertes Changelog-Format

---

## 0.7.3.1 Alpha | Kamera XML-Summaries - 16.03.2026

### Hinzugefügt:

+ XML-Summary-Kommentare für das Kamera-System zur Verbesserung der Intellisense- und Dokumentationsqualität (z. B. CameraManager, CameraTransform, MotionModel, WorldBinding, VisibilityContext, ViewFrustum)

---