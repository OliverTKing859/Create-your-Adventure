# Render

## Optimierungen

Sofort umsetzbar
Debug-Guard in OpenGLRenderContext:
csharp

#if DEBUG
gl.Enable(EnableCap.DebugOutput);
gl.Enable(EnableCap.DebugOutputSynchronous);
#endif

OnWindowResize als private Handler:
csharp

private void OnWindowResize(Vector2D<int> size) { ... }

Guard in BeginFrame/EndFrame:
csharp

public void BeginFrame()
{
    if (renderContext is null || !renderContext.IsInitialized)
        throw new InvalidOperationException("RenderContext not initialized.");
    renderContext.BeginFrame();
}

Singleton-Reset nach Dispose:
csharp

public void Dispose()
{
    if (isDisposed) return;
    WindowManager.Instance.OnResize -= OnWindowResize;
    renderContext?.Dispose();
    isDisposed = true;
    lock (instanceLock) { instance = null; }
    Logger.Info("[RENDER] RenderManager disposed");
}

Mittelfristig

Factory Pattern für RenderContext ist im Code als Kommentar erwähnt – das sollte früh eingebaut werden, bevor mehr OpenGL-spezifischer Code entsteht.
SetClearColor / SetDepthTestEnabled aus IRenderContext in ein IRenderStateManager-Interface auslagern, sobald mehr State-Verwaltung hinzukommt (Blending, Culling, Stencil etc.).
EndFrame Contract klären: Entweder SwapBuffers hier aufrufen oder explizit dokumentieren, dass WindowManager dafür verantwortlich ist.

---

## TODO / Geplante Features

Feature                             Realistisch?             Anmerkung
Factory Pattern für Backends        ✅ Ja                    Kommentar ist schon im Code – früh angehen
DirectX / Vulkan Backend            ⚠️ Aufwand               hochInterface ist vorbereitet, aber IRenderContext reicht für Vulkan nicht aus
Multi-Viewport Support              ✅ Machbar               SetViewport ist bereits parametrisiert
Debug Callback registrieren         ✅ Ja                    gl.DebugMessageCallback fehlt noch, wäre sinnvoll
Render Stats (Frame Time etc.)      ✅ Ja                    Guter nächster Schritt nach stabiler Basis

---

## Checkliste

🔴 Sofort angehen

- [ ] DebugOutput hinter #if DEBUG Guard
- [ ] OnWindowResize auf private setzen
- [ ] BeginFrame/EndFrame mit IsInitialized-Check absichern
- [ ] Dispose() setzt instance = null (Singleton-Reset)
- [ ] EndFrame/SwapBuffers-Verantwortung klar dokumentieren

🟡 Bald angehen

- [ ] Factory Pattern für IRenderContext-Erzeugung einbauen
- [ ] OpenGL Debug Callback (DebugMessageCallback) registrieren
- [ ] IRenderState für Render-Konfiguration (ClearColor, DepthTest, etc.) auslagern

🟢 Langfristig / Optional

- [ ] GetCapabilities() auf Interface für Multi-Backend-Support
- [ ] Render Stats / Profiling-Hooks
- [ ] Vulkan-/DirectX-Backend vorbereiten (Interface erst erweitern wenn nötig)