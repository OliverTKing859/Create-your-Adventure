# Teil 1 vom KingsEngine ()

## Optimierungen

Sofort — try/finally für Dispose-Garantie:

csharp

windowManager.Initialize(...);
var gameLoop = new GameLoop(windowManager);

try
{
    windowManager.Run();
}
finally
{
    windowManager.Dispose();
    Logger.Info("[ENGINE] Cleanup complete");
}

---

Das garantiert Dispose auch bei Crashes — unabhängig davon, was in der GameLoop passiert.
Mittelfristig — Exception-Logging am Top-Level:

csharp

try
{
    windowManager.Run();
}
catch (Exception ex)
{
    Logger.Error($"[ENGINE] Fatal crash: {ex}");
    throw; // Re-throw für OS-Level Crash-Report
}
finally
{
    windowManager.Dispose();
}

Später — WindowSettings aus Config:

csharp

var settings = ConfigLoader.Load<WindowSettings>("config/window.json");
windowManager.Initialize(settings);

---

## TODO / Geplante Features

Feature                                 Status              Realistisch?
WindowSettings aus Config/File          Hardcoded           ✅ Einfach nachrüstbar
Top-Level Exception-Handler             Fehlt komplett      ✅ Sollte sofort rein
Mehrere Engine-Phasen / Scenes          Nicht angedeutet    ⚠️ Benötigt größeres Refactor
Engine-Version / Build-Info im Log      Fehlt               ✅ Trivial, aber nützlich

---

## Checkliste

Sofort angehen:

- [ ] try/finally um windowManager.Run() — Dispose-Garantie bei Crashes
- [ ] Top-Level catch mit Logger.Error für Fatal-Crashes

Kann warten:

- [ ] WindowSettings aus einer Config-Datei laden
- [ ] Engine-Version / Build-Timestamp im Startup-Log ergänzen