# Teil 1 vom Window ()

## Optimierungen

🔴 Sofort: GL-Kontext disposen
csharp

public void Dispose()
{
    if (isDisposed) return;

    InputContext?.Dispose();
    GlContext?.Dispose();   // ← fehlt komplett
    window?.Dispose();

    isDisposed = true;
}

Ein nicht-disposed GL-Kontext hält GPU-Ressourcen. In einer Game Engine ist das ein echter Leak.

---

🔴 Sofort: Singleton nach Dispose zurücksetzen
csharp

public void Dispose()
{
    if (isDisposed) return;

    InputContext?.Dispose();
    GlContext?.Dispose();
    window?.Dispose();

    isDisposed = true;

    lock (instanceLock)
    {
        instance = null;  // ← erlaubt saubere Neuinitialisierung
    }

    Logger.Info("[WINDOW] WindowManager disposed");
}

Ohne das führt jeder Zugriff auf Instance nach Dispose() zum selben kaputten Objekt.

---

🟡 Mittelfristig: isDisposed-Guards in public Methoden
csharp

public void Run()
{
    ObjectDisposedException.ThrowIf(isDisposed, this);
    if (window is null)
        throw new InvalidOperationException("...");
    // ...
}
Close() und CenterWindow() brauchen dasselbe. Sonst sind Aufrufe nach Dispose() stumme No-Ops oder Crashes.

---

🟡 Mittelfristig: WindowSettings-Validierung
csharp

public void Validate()
{
    if (Width <= 0 || Height <= 0)
        throw new ArgumentException("Window dimensions must be positive.");
    if (GLMajorVersion < 3)
        throw new ArgumentException("OpenGL 3.0+ required.");
}
Oder als Guard direkt in WindowManager.Initialize() aufgerufen.

---

🟢 Optional: Events nach Dispose nullen
csharp

public void Dispose()
{
    if (isDisposed) return;

    Loaded = null;
    Updated = null;
    Rendered = null;
    OnClose = null;
    OnResize = null;
    // ...dann dispose
}
Verhindert, dass Subscriber-Callbacks nach Shutdown noch feuern, falls das Fenster asynchron schließt.

---

🟢 Optional: DebugContext per Build-Konfiguration automatisieren
csharp

public bool DebugContext { get; init; } =
#if DEBUG
    true;
#else
    false;
#endif

Entlastet den Aufrufer und macht das Verhalten explizit klar.

---

## TODO / Geplante Features

Feature                                 Status                                      Einschätzung
Fullscreen-Toggle zur Laufzeit          Nicht vorhanden                             Realistisch, aber braucht Resize-Logik
Window-Icon setzen                      Nicht vorhanden                             Kleines Feature, einfach ergänzbar
Mehrere Monitore / Monitor-Auswahl      Nur Primary Monitor in CenterWindow()       Für MVP ausreichend
Frame-Rate Cap (ohne VSync)             Nicht vorhanden                             Nützlich für Dev-Tools
GlContext null-Safety nach Dispose      Fehlt                                       Sollte mit Guard abgesichert werden

---

## Checkliste

🔴 Sofort angehen

- [ ] GlContext?.Dispose() in Dispose() ergänzen
- [ ] instance = null nach Dispose im Lock setzen
- [ ] ObjectDisposedException-Guard in Run(), Close(), CenterWindow()

🟡 Bald angehen

- [ ] WindowSettings.Validate() implementieren und in Initialize() aufrufen
- [ ] Events in Dispose() auf null setzen

🟢 Später / Optional

- [ ] DebugContext mit #if DEBUG automatisieren
- [ ] Fullscreen-Toggle zur Laufzeit vorbereiten
- [ ] Frame-Cap-Option in WindowSettings ergänzen