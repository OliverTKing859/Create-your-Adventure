# KingsEngine — Alle Pipelines

> Übersicht aller Systemflüsse in der KingsEngine. Jede Pipeline zeigt den exakten Datenpfad von Eingabe bis Ausgabe.

---

## 1. Engine Bootstrap (Startup → Shutdown)

```
KingsEngine.Main()
  → WindowManager.Initialize()          [Silk.NET Window + GL + Input Context]
  → GameLoop erstellen                   [verdrahtet alle Events automatisch]
  → windowManager.Run()                  [blockierender Main-Loop]
      → OnLoad()                         [Initialisierung aller Manager]
      → OnUpdate() × N                   [Frame-Loop]
      → OnRender() × N                   [Render-Loop]
      → OnClose()                        [LIFO Dispose]
  → windowManager.Dispose()
```

---

## 2. Manager-Initialisierungsreihenfolge (OnLoad)

```
[01] WindowManager       → Fenster + GL-Kontext + Input-Kontext bereit
[02] RendererManager     → IRenderContext (OpenGL) + Viewport init
[03] ShaderManager       → Factory registrieren (GL-Backend erkennen)
[04] TextureManager      → Factory registrieren + BlockAtlas bauen
[05] MeshManager         → Factory registrieren + testCube erstellen
[06] InputManager        → KeyboardDevice + MouseDevice + GamepadDevice init
[07] TimeManager         → Akkumulatoren, WorldTime reset
[08] CameraManager       → Transform + Projection + MotionModel init

Dispose (LIFO): [08] → [07] → [06] → [05] → [04] → [03] → [02] → [01]
```

---

## 3. Frame-Loop (OnUpdate)

```
OnUpdate(rawDelta)
  │
  ├─ [Phase 1] TimeManager.BeginFrame(rawDelta)
  │               → DeltaTime berechnen (skaliert + unskaliert)
  │               → FixedTick-Akkumulator befüllen
  │               → WorldTime fortschreiben
  │
  ├─ [Phase 1] InputManager.BeginFrame()
  │               → InputState zurücksetzen (FrameStart-Flags)
  │
  ├─ [Phase 2] TimeManager.ConsumeFixedTicks()
  │               → Schleife: solange Akkumulator ≥ FixedDeltaTime (1/60s)
  │                   → [STUB] PhysicsSystem.Tick(fixedDt)
  │                   → [STUB] Player Logic
  │                   → [STUB] Chunk Simulation
  │               → Max. 5 Ticks pro Frame (Spiral-of-Death-Schutz)
  │
  ├─ [Phase 3] CameraManager.Update(deltaTime)
  │               → InputManager: Movement + Look Vektoren lesen
  │               → CameraMotionModel: Beschleunigung + Smoothing
  │               → CameraTransform: Position + Rotation aktualisieren
  │               → CameraWorldBinding: Chunk-Tracking + Origin-Shift
  │               → View-/Projektionsmatrix invalidieren (lazy cache)
  │
  └─ [Phase 4] InputManager.EndFrame(deltaTime)
                  → GamepadDevice.Poll()
                  → InputRegistry.ProcessActions()
                  → InputState.EndFrame() (PreviousFrame ← CurrentFrame)
```

---

## 4. Render-Loop (OnRender)

```
OnRender(deltaTime)
  │
  ├─ RendererManager.BeginFrame()
  │     → gl.Clear(Color | Depth)
  │
  ├─ ShaderManager.UseProgram("block")
  │     → State-Check (bereits aktiv? → kein redundanter GL-Call)
  │     → gl.UseProgram(id)
  │
  ├─ CameraManager → Uniforms setzen
  │     → GetVisibilityContext()          [lazy: berechnet View/Proj nur wenn dirty]
  │     → SetUniform("uView", viewMatrix)
  │     → SetUniform("uProjection", projMatrix)
  │
  ├─ TextureManager.BindAtlas("blocks")
  │     → State-Check (bereits gebunden?)
  │     → gl.BindTexture(atlasId)
  │
  ├─ mesh.Bind() → mesh.Draw()
  │     → gl.BindVertexArray(vao)
  │     → gl.DrawElements / DrawArrays
  │
  └─ RendererManager.EndFrame()
        → [Swap Buffers via Silk.NET]
```

---

## 5. Camera-Pipeline

```
Input (Move/Look Vector)
  → CameraMotionModel
  │     → Geschwindigkeit + Beschleunigung berechnen
  │     → Look-Smoothing (SmoothedLookDelta — derzeit raw, Bug)
  │     → Preset je CameraMotionMode (Walk/Fly/Glider/Cinematic/Spectator/Debug)
  │
  → CameraTransform
  │     → Position += PositionDelta
  │     → Euler-Winkel aktualisieren (Yaw/Pitch, clamped)
  │     → Quaternion + Richtungsvektoren (Forward/Right/Up) ableiten
  │
  → CameraWorldBinding
  │     → ChunkCoord aus Weltposition ableiten
  │     → Origin-Shift bei Chunk-Wechsel (Event feuern)
  │     → Koordinatenkonvertierung Welt ↔ Render-Space
  │
  → Matrix-Generierung (lazy cached)
  │     → ViewMatrix = Matrix4X4.CreateLookAt(pos, pos+forward, up)
  │     → ProjectionMatrix = Matrix4X4.CreatePerspective(fov, aspect, near, far)
  │
  → CameraVisibilityContext (readonly snapshot für Rendering)
        → Position, ViewMatrix, ProjectionMatrix
        → ViewFrustum (6-Plane-Extraktion aus ViewProjection)
        → RenderDistance
```

---

## 6. Input-Pipeline

```
Engine-Start
  WindowManager.Initialize()
    → InputManager.Initialize(IInputContext)
        → KeyboardDevice.Initialize()
        → MouseDevice.Initialize()
        → GamepadDevice.Initialize()

Pro Frame
  BeginFrame()
    → InputState.BeginFrame()          [FrameStart-Snapshots]
    │
    → [Events feuern während Frame]
    │   KeyboardDevice  → InputState.SetKey(key, pressed)
    │   MouseDevice     → InputState.SetMouseButton / SetMouseDelta / SetScroll
    │   GamepadDevice   → InputState.SetAxis / SetButton
    │
  EndFrame(deltaTime)
    → GamepadDevice.Poll()             [Gamepad: kein Event-Modell, Polling]
    → InputRegistry.ProcessActions()   [benannte Actions prüfen + Events feuern]
    → InputState.EndFrame()            [Current → Previous schieben]

Abfrage (jederzeit im Frame)
  InputAnalyzer.IsKeyHeld / IsKeyPressed / IsKeyReleased
  InputAnalyzer.GetMovementVector / GetLookVector
  InputRegistry.Action("Jump").IsActive()
```

---

## 7. Shader-Pipeline

```
WindowManager.Initialize()
  → ShaderManager.Initialize()
  │     → GL-Kontext erkennen → OpenGL-Factory registrieren
  │
  → ShaderManager.LoadFromFiles("block", vertPath, fragPath)
  │     → AssetLoader.GetShaderPath(name)    [Cache → base → modded]
  │     → File.ReadAllText(vertPath)
  │     → File.ReadAllText(fragPath)
  │     → IShaderProgram.Compile(vertSrc, fragSrc)
  │           → gl.CreateShader(Vertex)  → CompileShader → CheckError
  │           → gl.CreateShader(Fragment) → CompileShader → CheckError
  │           → gl.CreateProgram() → AttachShader × 2 → LinkProgram
  │           → gl.DeleteShader × 2
  │     → Cache: shaderCache[name] = program
  │
  → ShaderManager.UseProgram("block")
  │     → State-Check: currentProgram == "block"? → skip
  │     → gl.UseProgram(program.Id)
  │
  → ShaderManager.SetUniform("uView", matrix)
        → program.SetUniform(name, value)
        → gl.UniformMatrix4fv(location, ...)

  Dispose()
    → Alle IShaderProgram.Dispose()
    → gl.DeleteProgram(id) pro Shader
```

---

## 8. Texture & Atlas-Pipeline

```
Initialize(GlContext)
  → GL-Kontext erkennen
  → textureFactory  = () => new OpenGLTexture2D(gl)
  → atlasFactory    = () => new OpenGLTextureAtlas(gl)

Einzeltextur laden
  LoadTextureFromAssets(category, name, settings?)
    → AssetLoader.GetTexturePath(name)
    → Cache-Check: textures[name]?
    → textureFactory() → ITexture
    → ITexture.LoadFromFile(path, settings)
          → StbImage.LoadFromFile() → RGBA-Buffer
          → gl.GenTexture() → BindTexture → TexImage2D
          → Sampler-Parameter (Filter, Wrap) setzen
          → GenerateMipmap (falls settings.Mipmap)
    → Cache speichern

Atlas erstellen
  BuildBlockAtlas()
    → CreateAtlas("blocks")
    → BuildAtlasFromFolder("textures/blocks", "blocks", TextureSettings.Atlas)
          → Alle Dateien im Ordner laden (LoadTexture × N)
          → OpenGLTextureAtlas.Build(textures, settings)
                → [1] StbImage alle Images laden → Größen prüfen
                → [2] Power-of-2 Atlas-Größe berechnen (256–4096)
                → [3] Grid-Packing: pixel-copy in Atlas-Buffer
                       (currentY += textureSize — BUG: derzeit *=)
                → [4] gl.TexImage2D(atlasBuffer) + Mipmap
          → AtlasRegion pro Textur berechnen (UV-Koordinaten)
    → Cache: atlases["blocks"]

Binden (State-optimiert)
  BindAtlas("blocks")
    → boundAtlas == "blocks"? → skip
    → gl.BindTexture(atlas.Id)
    → boundAtlas = "blocks"
    → boundTexture = null   (BUG: Zuweisung derzeit vertauscht)

UV-Zugriff
  GetBlockUV("grass_top")
    → activeAtlas.GetRegion("grass_top")
    → AtlasRegion { PixelX, PixelY, Width, Height, UVMin, UVMax }
```

---

## 9. Mesh-Pipeline

```
MeshManager.Initialize(GlContext)
  → GL-Kontext erkennen
  → meshFactory = (name) => new OpenGLMesh(gl, name)

CreateCube("testCube", size, atlasRegion)
  → meshFactory("testCube") → OpenGLMesh
  → Vertices berechnen (Position + TexCoord aus AtlasRegion)
  → Indices definieren (6 Flächen × 2 Dreiecke = 12 Indizes)
  → mesh.Create(vertices, indices, VertexLayout.PositionTexCoord())

mesh.Create(vertices, indices, layout)
  → gl.GenVertexArray() → BindVertexArray(vao)
  → OpenGLVertexBuffer.SetData(vertices)
  │     → gl.GenBuffer() → BindBuffer(Array) → BufferData
  → SetupVertexAttributes(layout)
  │     → gl.EnableVertexAttribArray(i)
  │     → gl.VertexAttribPointer(i, size, type, stride, offset)  pro Attribut
  → OpenGLIndexBuffer.SetData(indices)
  │     → gl.GenBuffer() → BindBuffer(ElementArray) → BufferData
  → gl.BindVertexArray(0)   [VAO-State gespeichert, unbinden]

Render
  mesh.Bind()  → gl.BindVertexArray(vao)
  mesh.Draw()  → gl.DrawElements(Triangles, indexCount, UnsignedInt, 0)

Dispose
  MeshManager.Dispose()
    → alle meshCache.Values → mesh.Dispose()
          → gl.DeleteVertexArray(vao)
          → gl.DeleteBuffer(vbo)
          → gl.DeleteBuffer(ebo)
```

---

## 10. Asset-Auflösung (AssetLoader)

```
GetShaderPath("block")   /   GetTexturePath("grass") etc.
  │
  ├─ PathCache["block"]? → return cached path
  │
  ├─ Suche: assets/base/shaders/ (rekursiv, alle Unterordner)
  │     → Directory.EnumerateFiles(basePath, "*", Recursive)
  │     → Dateiname match? → Logger.Info("Found") → Cache → return
  │
  ├─ Suche: assets/modded/shaders/ (rekursiv)
  │     → match? → Logger.Info("Found modded") → Cache → return
  │
  └─ Nicht gefunden → Logger.Error("Not found") → return string.Empty
```

---

## 11. World-Scheduler (Frame-Budget)

```
RequestChunkLoad(coord)   →  chunkQueue.Enqueue(job, priority)
RequestSimulation(job)    →  simulationQueue.Enqueue(job, priority)

ProcessFrame(deltaTime)
  │
  ├─ Budget = ~8ms (konfigurierbar)
  ├─ Deadline = Stopwatch.Now + Budget
  │
  ├─ ProcessTask(ChunkLoad,   budgetSlice)   → ProcessChunkLoad()    [STUB]
  ├─ ProcessTask(ChunkMesh,   budgetSlice)   → ProcessChunkMesh()    [STUB]
  ├─ ProcessTask(BlockUpdate, budgetSlice)   → ProcessBlockUpdates() [STUB]
  ├─ ProcessTask(Lightning,   budgetSlice)   → ProcessLightning()    [STUB]
  └─ ProcessTask(Entities,    budgetSlice)   → ProcessEntities()     [STUB]

ProcessTask(type, budget)
  → stopwatch.Restart()
  → while queue.Count > 0 && verbleibend > 0.5ms
  │     → job = queue.Dequeue()
  │     → Process*(job)
  │     → Early-Exit bei Budget-Erschöpfung
  └─ Telemetrie: LastTaskTimings[type] = elapsed  (BUG: speichert budget statt used)
```

---

## 12. WorldRelevanceFilter

```
UpdateFromCamera(CameraVisibilityContext)
  → cameraPos = context.Position
  → frustum   = context.ViewFrustum

ShouldLoad(chunk)
  → dist = ChunkCoord.SquaredDistance(chunk, cameraChunk)
  → dist ≤ LoadRadius²  →  true

ShouldRender(chunk)
  → dist ≤ RenderRadius²
  → AND frustum.Intersects(GetChunkAABB(chunk))   [AABB-Frustum-Test]
  → true

ShouldSimulate(chunk)
  → dist ≤ SimulationRadius²
  → OR chunk.Metadata.HasWater                    [Wasser erzwingt Simulation]
  → true

ShouldUnload(chunk)
  → dist > UnloadRadius²
  → AND NOT chunk.Metadata.HasActiveEntities       [Entities verhindern Unload]
  → true

UpdateChunkPriority(job)
  → isVisible = frustum.Intersects(aabb)
  → job.UpdatePriority(cameraChunk, isVisible)     [ChunkJob.UpdatePriority()]
```

---

## 13. Time-Pipeline

```
BeginFrame(rawDelta)
  → rawDelta clamp (max 0.1s, verhindert Spike-Runaway)
  → DeltaTime         = rawDelta × SimulationTimeScale
  → UnscaledDeltaTime = rawDelta
  → TotalTime        += UnscaledDeltaTime
  → WorldTime        += rawDelta × WorldTimeScale  (falls !IsWorldPaused)
  → FixedAccumulator += rawDelta

ConsumeFixedTicks()
  → tickCount = 0
  → while FixedAccumulator ≥ FixedDeltaTime (1/60s) && tickCount < 5
  │     → FixedAccumulator -= FixedDeltaTime
  │     → tickCount++
  │     → yield return FixedDeltaTime   [Caller führt Physik-Tick aus]
  └─ InterpolationAlpha = FixedAccumulator / FixedDeltaTime
```

---

## 14. Window-Lifecycle

```
WindowSettings (Titel, Auflösung, VSync, Fullscreen, GL-Version)
  → WindowManager.Initialize()
  → Silk.NET Window.Create(WindowOptions)
  → window.Load    → HandleLoad()
  │                    → GL.GetApi(window)       → GlContext bereit
  │                    → window.CreateInput()    → IInputContext bereit
  │                    → CenterWindow()
  │                    → Loaded?.Invoke()        → GameLoop.OnLoad()
  │
  → window.Update  → Updated?.Invoke(dt)         → GameLoop.OnUpdate()
  → window.Render  → Rendered?.Invoke(dt)        → GameLoop.OnRender()
  → window.Resize  → OnResize?.Invoke(size)      → RendererManager.SetViewport()
  → window.Closing → OnClose?.Invoke()           → GameLoop.OnClose()
  → windowManager.Run()   [blockiert]
  → windowManager.Dispose()
```

---

*KingsEngine · C# .NET 10 · Silk.NET · OpenGL 4.6 · Vulkan abstraction layer prepared*