# 'Chunk' 'DEUTSCH'

## Zweck

Datenmodell für Chunk-Koordinaten, Chunk-Aufträge und Chunk-Metadaten. Bildet die Grundlage für das gesamte World-Chunk-System.

---

## Verantwortlichkeiten

•	ChunkCoord — Immutable 3D-Koordinate (long X/Y/Z) mit Squared-Distance-Berechnung und Equality
•	ChunkJob — Chunk-Arbeitspaket mit Zustand, Priorität, Tick-Rate, Frustum-Visibility und Metadata
•	ChunkPriority — Distanzbasierte Priorisierung (Critical/High/Medium/Low/Background) mit angepasster Tick-Rate
•	ChunkMetadata — Optionale Chunk-Eigenschaften (Wasser, aktive Entities, Biome, Temperatur, letzter Änderungs-Tick)

---

## Architektur

---

## Pipeline

[ChunkJob erstellen] → 
[UpdatePriority (Distanz + Frustum)] → 
[TickRate ableiten] → 
[In Scheduler-Queue einreihen]

---

## Abhängigkeiten

•	Keine externen Abhängigkeiten (reine Datenklassen)
•	Verwendet von: WorldScheduler, WorldRelevanceFilter

---

## Geplante Implementierungen

•	Keine TODO-Kommentare sichtbar

---

## Zukünftige Ideen

•	UpdatePrivority(ChunkCoord, bool) enthält Tippfehler (→ UpdatePriority)
•	ChunkMetadata könnte als struct implementiert werden für bessere Cache-Locality

---