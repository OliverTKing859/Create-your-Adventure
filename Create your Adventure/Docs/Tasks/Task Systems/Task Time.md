# Teil 1 vom Time System (Setup)

## Optimierungen

① maxTicksPerFrame als Konstante auslagern

Aktuell hardcodiert in der Methode. 
Besser als konfigurierbare Property oder zumindest als Klassenkonstante, 
damit sie zentral angepasst werden kann.

csharp
private const int MaxTicksPerFrame = 5;

② SimulationTimeScale mit Clamping absichern

Ein negativer oder extrem hoher Wert würde den Accumulator destabilisieren. Minimaler Schutz:

csharp
private double simulationTimeScale = 1.0;

public double SimulationTimeScale
{
    get => simulationTimeScale;
    set => simulationTimeScale = Math.Clamp(value, 0.0, 10.0);
}

Gleiches gilt für WorldTimeScale.

③ FrameCount und TickCount – Overflow-Verhalten dokumentieren

ulong läuft bei 60 FPS nach ~9,7 Milliarden Jahren über – kein echtes Problem. 
Trotzdem sollte das kurz kommentiert sein, damit kein zukünftiger Entwickler unnötig einen Reset einbaut.

④ Singleton Reset für Tests

Aktuell gibt es keine Möglichkeit, die Instanz zurückzusetzen. 
Für Unit Tests oder Scene Reloads kann das zum Problem werden. 
Ein internes Reset()-Method oder ein ResetForTesting()-Hook wäre hilfreich.

## TODO / Geplante Features

Feature                                         Bewertung                                                       Priorität
Thread-safe Properties (TimeScale, Pause)       Fehlt, sobald Multithreading kommt                              Mittel
TimeScale-Clamping                              Kleines Stabilitätsrisiko                                       Niedrig
maxTicksPerFrame konfigurierbar                 Wartbarkeit                                                     Niedrig
Singleton Reset / Test-Hook                     Für Testbarkeit wichtig                                         Mittel
Dokumentation der Slow-Motion-Entscheidung      Für Teamverständnis                                             Niedrig
Pause für Simulation (nicht nur WorldTime)      Unklar ob gewollt – SimulationTimeScale = 0 wäre Workaround     Offen

---

## Checkliste

🔴 Sofort angehen

- [ ] SimulationTimeScale und WorldTimeScale mit Math.Clamp absichern
- [ ] maxTicksPerFrame als Klassenkonstante herausziehen

🟡 Bald angehen

- [ ] Singleton Reset()-Methode für Tests/Reloads ergänzen
- [ ] Verhalten von IsWorldPaused vs. Simulation-Pause explizit kommentieren oder trennen

🟢 Langfristig / Bei Bedarf

- [ ] Thread-safe Setter ergänzen, wenn Multithreading eingeführt wird
- [ ] FrameCount/TickCount Overflow-Kommentar hinzufügen
- [ ] Slow-Motion-Verhalten bei maxTicksPerFrame-Grenze dokumentieren