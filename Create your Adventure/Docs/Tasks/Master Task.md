# Kings Engine — Master Task List

> Prioritäten: 🔴 Sofort (Bugs/Crashes/Leaks) · 🟡 Bald (Stabilität/Korrektheit) · 🟢 Später (Features/Erweiterungen)

---

## 🔴 SOFORT — Blocking Bugs & Leaks

### Window
- [X] `GlContext?.Dispose()` in `Dispose()` ergänzen (GPU-Leak)
- [X] `instance = null` nach `Dispose()` im Lock setzen
- [X] `ObjectDisposedException`-Guard in `Run()`, `Close()`, `CenterWindow()`

### Renderer
- [ ] `DebugOutput` hinter `#if DEBUG` Guard
- [ ] `BeginFrame()`/`EndFrame()` mit `IsInitialized`-Check absichern
- [ ] `Dispose()` setzt `instance = null`
- [ ] `OnWindowResize` auf `private` setzen

### Shader
- [ ] Falsche Exception-Message beim Fragment-Shader-Fehler korrigieren
- [ ] `instance = null` in `Dispose()` setzen

### Texture
- [ ] **State-Bug in `BindTexture()` fixen** — `boundAtlas`/`boundTexture` sind vertauscht
- [ ] **Atlas Row-Wrap-Bug** — `currentY *= textureSize` → `currentY += textureSize`
- [ ] `ThrowIfDisposed()` in alle Public-Methoden
- [ ] `Directory.Exists`-Guard in `BuildAtlasFromFolder()`

### Mesh
- [ ] `SetData()` in `OpenGLVertexBuffer`: Guard gegen doppelten Aufruf (GPU-Leak)
- [ ] `UpdateData()`: Null-Handle-Guard
- [ ] `Dispose()` in `MeshManager`: `instance = null` + `meshFactory = null`
- [ ] `isDisposed`-Check in `CreateMesh()`, `GetMesh()` etc.

### Camera
- [X] **VP-Matrix Reihenfolge prüfen** — `proj * view` vs. `view * proj` (Fundament für Chunks)
- [X] **`newChunk`-Berechnung**: `(float)` Cast entfernen, `Math.Floor(double)` nutzen
- [X] `Logger.Info` in `ApplyRotation()` entfernen / hinter `#if DEBUG`
- [X] `SetRotation()` — `Transform.UpdateRotation()` am Ende ergänzen
- [X] **`ApplyLookInput` Smoothing-Bug** — Rückgabe auf `SmoothedLookDelta` umstellen
- [X] Hardcodiertes `0.016f` durch echtes `dt` ersetzen
- [ ] Singleton-Konstruktor auf `private` setzen

### Input
- [X] **`skipNextDelta` in `OnMouseMove` auswerten** (Delta-Spike nach Cursor-Moduswechsel)
- [X] **Tippfehler `NumpadSubstract` → `NumpadSubtract`**
- [X] `DoublePress` implementieren oder mit `NotSupportedException` kennzeichnen
- [X] `LongPressThreshold` aus `KeyBinding` + `InputAnalyzer` in `InputConstants` zusammenführen
- [X] LINQ-Allokation in `EndFrame()` durch reusable `List<KeyCode>` ersetzen

### GameLoop
- [ ] Shader-Referenz in `OnLoad()` cachen — String-Lookup aus `OnRender()` entfernen
- [ ] Early-Return in `OnRender()` bei `null`-Shader oder `null`-Mesh

### KingsEngine
- [ ] `try/finally` um `windowManager.Run()` — Dispose-Garantie bei Crashes
- [ ] Top-Level `catch` mit `Logger.Error` für Fatal-Crashes

### Time
- [ ] `SimulationTimeScale` und `WorldTimeScale` mit `Math.Clamp(0, 10)` absichern
- [ ] `maxTicksPerFrame` als `private const int` herausziehen

### World / Chunk
- [ ] **Tippfehler `UpdatePrivority` → `UpdatePriority`**
- [ ] `long` → `int` in `ChunkCoord.X/Y/Z`

---

## 🟡 BALD — Stabilität & Korrektheit

### Window
- [ ] `WindowSettings.Validate()` implementieren und in `Initialize()` aufrufen
- [X] Events in `Dispose()` auf `null` setzen

### Renderer
- [ ] Factory Pattern für `IRenderContext`-Erzeugung einbauen
- [ ] OpenGL Debug Callback (`DebugMessageCallback`) registrieren
- [ ] `EndFrame`/`SwapBuffers`-Verantwortung klar dokumentieren

### Shader
- [ ] `shaderCache` → `ConcurrentDictionary` für Thread-Safety
- [ ] `File.Exists()`-Guard in `LoadFromFiles()`

### Texture
- [ ] Bind-Guard um `(name, unit)`-Tupel erweitern
- [ ] `BuildBlockAtlas()` auf `AssetLoader` umstellen
- [ ] `singleton` nach `Dispose()` auf `null` zurücksetzen

### Mesh
- [ ] `Draw()` vs. manuelles `Bind()`: Inkonsistenz klären und dokumentieren
- [ ] Logger-Calls in `SetupVertexAttributes()` auf Debug-Level
- [ ] `CreateQuad()`/`CreateCube()`: Guard für bereits initialisierte Meshes

### Camera
- [ ] `LengthSquared` statt `Length` für Threshold-Check (Origin-Shift)
- [ ] `DragCoefficient` implementieren: `Velocity *= (1f - DragCoefficient * dt)`
- [ ] `FieldOfView` in `CameraProjection` entfernen / mit `BaseFov` konsolidieren
- [ ] Velocity bei `SetMotionMode()` erhalten (Übergang ohne Sprung)
- [ ] `TeleportTo` vs. `Initialize` in `WorldBinding` trennen (`Reset()`/`MoveTo()`)
- [ ] Toten Code in `UpdateRotation()` entfernen (`cosPitch`, `sinYaw` etc.)
- [ ] Alle Motion-Presets auf vollständige Initialisierung prüfen

### Input
- [ ] `isDisposed`-Check in `BeginFrame()`/`EndFrame()` ergänzen
- [ ] `AddMouseBinding()` Shortcut auf `InputAction` hinzufügen (Fluent API unvollständig)
- [ ] `LongPress` für `MouseButtonBinding` + `GamepadButtonBinding` nachziehen
- [ ] `CursorMode.Confined` korrekt implementieren oder mit `NotSupportedException` kennzeichnen
- [ ] `IsEnabled`-Property ins `IInputDevice`-Interface aufnehmen

### GameLoop
- [ ] Magic-Strings `"basic"`, `"dirt"`, `"blocks"` in `AssetKeys`-Klasse auslagern

### Time
- [ ] Singleton `Reset()`-Methode für Tests/Reloads
- [ ] `IsWorldPaused` vs. Simulation-Pause explizit dokumentieren oder trennen

### World / Chunk
- [ ] Distanz-Schwellen in `UpdatePriority()` dynamisch / konfigurierbar machen
- [ ] `TickRate` im `WorldScheduler` tatsächlich auswerten
- [ ] `WorldRelevanceFilter.GetChunkAABB()` — `localPos` vs. absolute Koordinaten prüfen
- [ ] `RequestChunkMesh()` auf `public`/`internal` setzen und Load→Mesh-Übergang anschließen

### Assets
- [X] Mod-Priorität umkehren — Modded zuerst suchen, Base als Fallback
- [X] `PathCache` auf `ConcurrentDictionary` umstellen
- [X] Ungenutzte `using`-Direktiven entfernen

---

## 🟢 SPÄTER — Features & Erweiterungen

### Window
- [X] `DebugContext` mit `#if DEBUG` automatisieren
- [ ] Fullscreen-Toggle zur Laufzeit vorbereiten

### Renderer
- [ ] Multi-Viewport Support (SetViewport ist bereits parametrisiert)
- [ ] Render-Stats / Profiling-Hooks

### Shader
- [ ] `Reload(name)` / Hot-Reload Mechanismus
- [ ] `bool`-Uniform-Overload im Interface ergänzen

### Texture
- [ ] `UnloadTexture(string name)` implementieren
- [ ] Dateiname-Kollisions-Handling im Atlas-Builder
- [ ] Async-Loading für Texturen (Hintergrund-Thread für Decode)

### Mesh
- [ ] `RemoveMesh(string name)` implementieren
- [ ] Vertex-Structs statt roher `float[]` einführen
- [ ] Face-spezifische UV-Unterstützung im Cube vorbereiten
- [ ] `BufferUsageARB` als Parameter in `SetData()` exponieren

### Camera
- [ ] **Frustum Culling in `IsChunkVisible()` aktivieren** (nach Chunk-Implementierung)
- [ ] `OriginShifted` / `ChunkChanged` Events an Chunk-System anbinden
- [ ] Projektionsmatrix cachen (Dirty-Flag)
- [ ] Roll-Rückführungslogik implementieren
- [ ] FOV-Interpolation für Sprint / Zoom
- [ ] Spectator-Mode Collision-Bypass
- [ ] `UpdateFromInput()` von `InputManager.Instance` entkoppeln

### Input
- [ ] Hotplug-Erkennung für Gamepad
- [ ] Singleton-Reset für Tests
- [ ] `GetScrollDelta()`, `IsKeyReleased()`, `IsMouseButtonPressed/Released` direkt auf `InputManager` exponieren

### GameLoop
- [ ] `rawDeltaTime` in `OnRender()` für Interpolations-Alpha vorbereiten
- [ ] UI-Einhängepunkt in `OnUpdate()`/`OnRender()` als Stub anlegen

### KingsEngine
- [ ] `WindowSettings` aus Config-Datei laden (JSON/YAML)
- [ ] Engine-Version / Build-Timestamp im Startup-Log

### Time
- [ ] Thread-safe Setter für `TimeScale` + `Pause` wenn Multithreading eingeführt wird
- [ ] Time-Events / Callbacks bei WorldTime-Schwellen (Sonnenaufgang, Mitternacht)

### World / Chunk
- [ ] WorldGen + Disk-Load in `ProcessChunkLoad()` implementieren
- [ ] Mesh-Generierung in `ProcessChunkMesh()` implementieren
- [ ] `WorldRelevanceFilter` in GameLoop integrieren
- [ ] Asynchrone Chunk-Verarbeitung (Worker-Threads)
- [ ] `IsGenerated`-Flag in `ChunkMetadata`
- [ ] `NeedsRemesh`-State im `ChunkState`-Enum
- [ ] `LastTaskTimings` Bug fixen — speichert Budget statt tatsächlich verbrauchter Zeit

### Assets
- [ ] Async Preloading (`PreloadAllAsync()`) einführen
- [ ] Rückgabe auf `TryGet`-Pattern umstellen
- [ ] Asset-Validierung (Existenz, Dateiformat)

---

## Schnellübersicht — Kritische Bugs auf einen Blick

| Bug | System | Impact |
|-----|--------|--------|
| `boundTexture`/`boundAtlas` vertauscht in `BindTexture()` | TextureManager | Falsche Textur-Binds |
| Atlas `currentY *= size` statt `+=` | OpenGLTextureAtlas | Korrupte Atlas-Textur |
| VP-Matrix Reihenfolge unklar | CameraManager | Falsches Rendering |
| `SmoothedLookDelta` wird verworfen | CameraMotionModel | Look-Smoothing wirkungslos |
| `skipNextDelta` nie ausgewertet | MouseDevice | Delta-Spike bei Cursor-Lock |
| `GlContext` nicht disposed | WindowManager | GPU-Ressourcen-Leak |
| `newChunk` float-Cast | CameraWorldBinding | Präzisionsfehler bei Origin-Shift |
| `RequestChunkMesh()` private | WorldScheduler | Load→Mesh nie getriggert |