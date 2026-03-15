# 'Kings Engine' 'DEUTSCH'

## Zweck

Statischer Einstiegspunkt der Engine. Bootstrapped das gesamte System in definierter Reihenfolge und blockiert bis zum Fenster-Close.

---

## Verantwortlichkeiten

•	Erstellung und Konfiguration des windowManager mit WindowSettings
•	Instanziierung der GameLoop (verdrahtet automatisch alle Events)
•	Start der Hauptschleife via windowManager.Run() (blockierend)
•	Finales Dispose des windowManager nach Beendigung

---

## Architektur

// Um welche Art von Klassenarchitektur handelt es sich?

---

## Pipeline

[Phase 1: WindowManager.Initialize]
   -> [Phase 2: GameLoop erstellen (Events verdrahtet)]
   -> [Phase 3: windowManager.Run() — blockiert]
   -> [Phase 4: windowManager.Dispose() — Cleanup]

---

## Abhängigkeiten

•	windowManager — Fenster-Erstellung und Hauptschleife
•	WindowSettings — Konfigurationsobjekt (Titel, Auflösung)
•	GameLoop — Event-gesteuerter Spiel-Loop
•	Logger — Start-/Shutdown-Logging

---

## Geplante Implementierungen

•	Keine expliziten TODOs im Code sichtbar

---

## Zukünftige Ideen

•	Konfiguration über externe Datei (JSON/YAML) statt Hardcoded WindowSettings
•	Kommandozeilen-Argumente für Auflösung, Fenstermodus, Debug-Flags
•	Rückgabe eines Exit-Codes für CI/CD-kompatibles Lifecycle-Management

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