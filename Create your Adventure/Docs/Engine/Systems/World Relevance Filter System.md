# 'WorldRelevanceFilter' 'DEUTSCH'

## Zweck

Kamera-basierter Relevanz-Filter, der bestimmt, welche Chunks gerendert, simuliert, geladen oder entladen werden sollen. 
Nutzt Distanz-Checks und Frustum-Culling.

---

## Verantwortlichkeiten

•	Konfigurierbare Distanz-Zonen: Render (16), Simulation (24), Load (32), Unload (40) in Chunk-Einheiten
•	Frustum-Culling gegen Chunk-AABBs für Render-Entscheidungen
•	Metadata-basierte Ausnahmen (aktive Entities verhindern Unload, Wasser erzwingt Simulation)
•	Prioritäts-Update: kombiniert Frustum-Visibility mit Distanz-basierter Priorität
•	Kamera-Synchronisation über CameraVisibilityContext

---

## Architektur

---

## Pipeline

[UpdateFromCamera (CameraVisibilityContext)] → 
[ShouldLoad / ShouldRender / ShouldSimulate / ShouldUnload Checks] → 
[Frustum-Culling + Distanz-Check] → 
[UpdateChunkPriority]

---

## Abhängigkeiten

•	CameraVisibilityContext (struct) — Kamera-Position, Matrizen, Frustum, Render-Distanz
•	ViewFrustum (struct) — 6-Plane-Frustum mit AABB-Intersection-Test
•	ChunkJob — Chunk-Daten und Prioritäts-Logik
•	ChunkCoord — Distanzberechnung
•	ChunkMetadata — Metadata-Checks (HasWater, HasActiveEntities)
•	Silk.NET.Maths — Box3D<float>, Vector3D<float>, Matrix4X4<float>

---

## Geplante Implementierungen

•	GetChunkAABB(ChunkCoord) berechnet localPos relativ zur Kamera, verwendet aber absolute Koordinaten für die zurückgegebene AABB — potentieller Bug
•	Keine Integration in eine aktive Game-Loop sichtbar (WorldRelevanceFilter wird nirgends referenziert außer intern)

---

## Zukünftige Ideen

•	Octree/Spatial-Index für effizientere Relevanz-Prüfung bei vielen Chunks
•	LOD-Integration basierend auf Distanz-Zonen
•	Streaming-Priorität aus Relevanz-Filter ableiten

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