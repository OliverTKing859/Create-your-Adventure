# 'World' 'DEUTSCH'

## Zweck

Budget-basierter Frame-Scheduler, der World-Aufgaben (Chunk-Loading, Meshing, Block-Updates, Lighting, Entity-Simulation) 
innerhalb eines konfigurierbaren Zeitbudgets pro Frame priorisiert und verarbeitet. Singleton-Pattern.

---

## Verantwortlichkeiten

•	Verteilung eines Frame-Budgets (~8ms bei 60 FPS) auf fünf gewichtete Task-Kategorien
•	Prioritätsbasierte Abarbeitung von Chunk-Load-, Mesh- und Simulations-Queues via PriorityQueue<ChunkJob, ChunkPriority>
•	Early-Exit bei Budget-Erschöpfung (< 0.5ms verbleibend)
•	Telemetrie: Tracking von Task-Timings und gesamtem Budget-Verbrauch pro Frame
•	Öffentliche API zum Einreihen von Chunk-Load- und Simulations-Anfragen

---

## Architektur

---

## Pipeline

[RequestChunkLoad / RequestSimulation] → 
[ProcessFrame (Budget-Verteilung)] → 
[ProcessTask pro SchedularTask] → 
[Queue-Dequeue & Verarbeitung bis Deadline] → 
[Telemetrie-Update]

---

## Abhängigkeiten

•	ChunkJob — Datenträger für Chunk-Zustand, Priorität, Tick-Rate und Metadata
•	ChunkPriority — Enum zur Priorisierung (Critical → Background)
•	ChunkState — Enum für Chunk-Lebenszyklus (Requested → Unloaded)
•	SchedularTask — Enum der fünf Aufgabentypen
•	stopwatch — Zeitmessung

---

## Geplante Implementierungen

•	ProcessChunkLoad(ChunkJob) — nur Stub, setzt ChunkState.Loading, keine echte WorldGen/Disk-Load-Logik
•	ProcessChunkMesh(ChunkJob) — nur Stub, setzt ChunkState.Meshing, keine Mesh-Generierung
•	ProcessBlockUpdatesWithBudget(double) — leerer Body
•	ProcessLightningWithBudget(double) — leerer Body
•	ProcessEntitiesWithBudget(double) — leerer Body
•	RequestChunkMesh(ChunkJob) ist private — wird intern nie aufgerufen, kein Pipeline-Übergang von Load → Mesh

---

## Zukünftige Ideen

•	Asynchrone Chunk-Verarbeitung (Offloading auf Worker-Threads)
•	Dynamische Budget-Umverteilung basierend auf tatsächlicher Last
•	LastTaskTimings speichert aktuell taskBudgetMs statt taskUsedMs — Telemetrie-Bug

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