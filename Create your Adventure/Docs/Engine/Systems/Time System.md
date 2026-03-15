# 'Time Manager' 'DEUTSCH'

## Zweck

Zentrale Zeitverwaltung der Engine. Verwaltet Frame-Timing, fixe Simulationsticks und In-Game-Weltzeit als thread-sicherer Singleton.

---

## Verantwortlichkeiten

•	Berechnung von skaliertem und unskaliertem Frame-Delta
•	Akkumulation und Abgabe fixer Simulationsticks (Fixed Timestep Pattern, 60 TPS)
•	Verhinderung der „Spiral of Death" durch Tick-Cap pro Frame (max. 5)
•	Verwaltung einer unabhängigen Weltzeit (Tag/Nacht, Wetter, Jahreszeiten) mit eigener Skalierung und Pause-Funktion
•	Bereitstellung von unskalierter Gesamtzeit für hintergrundunabhängige Systeme (z. B. Chunk Loading)
•	Berechnung des Interpolations-Alphas für Render-Smoothing

---

## Architektur

---

## Pipeline

BeginFrame(rawDelta) -> ConsumeFixedTicks() -> [FixedTick-Schleife] -> GetInterpolationAlpha()

---

## Abhängigkeiten

•	Keine externen Abhängigkeiten – eigenständiger Singleton
•	Konsumiert von:
•	GameLoop (ruft BeginFrame(double), ConsumeFixedTicks(), FrameDeltaTime, FixedDeltaTime auf)

---

## Geplante Implementierungen

•	Physik & Spielerlogik: Die Fixed-Tick-Schleife in GameLoop (Zeile 117–118) enthält nur Platzhalter-Kommentare (PhysicsSystem.Tick(fixedDt) ist auskommentiert).
•	Weltzeit-Konsumenten: WorldTime, WorldTimeScale und IsWorldPaused sind exponiert, aber es gibt noch keine sichtbaren Konsumenten (Tag/Nacht-Zyklus, Wetter, Jahreszeiten).

---

## Zukünftige Ideen

•	Time-Events / Callbacks: Registrierbare Events bei bestimmten WorldTime-Schwellen (z. B. Sonnenaufgang, Mitternacht).
•	Slow-Motion / Time-Freeze: SimulationTimeScale wird bereits exponiert – ein dediziertes API für temporäre Zeiteffekte (Ease-In/Out) wäre eine natürliche Erweiterung.
•	Diagnostik-Properties: AverageFrameTime, FPS-Counter oder TicksPerSecond für Debug-Overlays.
•	Testbarkeit: Abstraktion hinter einem ITimeProvider-Interface, um den Singleton in Unit-Tests ersetzbar zu machen.
•	Persistenz: Serialisierung von WorldTime für Save/Load-Funktionalität.

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