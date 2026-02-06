# Design-Notizen - Create your Adventure

Dieses Dokument beschreibt wichtige Architektur-Entscheidungen und die Begründung dahinter.

---

## Unendliche Weltgenerierung (06.02.2026)

### Problem
Minecraft hat ein hartes Limit von ca. 30 Millionen Blöcken pro Achse (±29.999.984 Blöcke), bedingt durch die Verwendung von 32-Bit Integer (`int`) für Chunk-Koordinaten. Bei großen Distanzen treten zudem Floating-Point-Präzisionsprobleme auf, die zu visuellen Artefakten führen ("Far Lands").

### Lösung: 64-Bit Architektur

**Chunk-Positionen: `Vector3D<long>` (64-Bit)**
- Ermöglicht ±9.223.372.036.854.775.807 Chunks pro Achse
- Bei 16 Blöcken pro Chunk: ±147.573.952.589.676.412.912 Blöcke pro Achse
- Praktisch unbegrenzt für jedes realistische Gameplay-Szenario

**Lokale Block-Positionen: `int` (32-Bit)**
- Blöcke innerhalb eines Chunks verwenden weiterhin `int` (0-15 Range)
- Keine Verschwendung von Speicher für kleine Koordinaten
- Perfekt ausreichend für ChunkSize = 16

**Rendering: `float` mit World-Offset**
- GPU arbeitet mit `float` (32-Bit Floating-Point) für Matrizen
- Bei großen Distanzen: Kamera-relatives Rendering
  - Berechne Position relativ zur Kamera-Chunk-Position
  - Verhindert Präzisionsverlust bei `float` Konvertierung
  - Technisch: `(ChunkPos - CameraChunkPos) * ChunkSize + LocalPos`

### Implementierung

---

### Zukünftige Optimierungen

Die aktuelle Implementierung rendert **alle** Blöcke in einem Chunk (4096 Instance-Matrizen). Dies ist für die frühe Entwicklungsphase akzeptabel, muss aber später optimiert werden:

1. **Block-Daten-Speicherung**
   - Aktuell: Keine Block-Daten, nur leere Matrizen
   - Zukünftig: `Block[,,]` Array mit BlockType (Luft, Stein, Gras, etc.)

2. **Visibility Culling**
   - Nur sichtbare, nicht-leere Blöcke rendern
   - Luft-Blöcke überspringen

3. **Face Culling**
   - Flächen zwischen zwei festen Blöcken nicht rendern
   - Reduziert Geometrie drastisch (bis zu 80% bei vollen Chunks)

4. **Greedy Meshing**
   - Kombiniere benachbarte identische Flächen zu größeren Quads
   - Reduziert Draw Calls und Vertex Count
   - Algorithmus: Iteriere über Schichten, finde rechteckige Regionen

5. **Separation von Logik und Rendering**
   - Chunk-Klasse sollte reiner Daten-Container sein
   - Rendering-Logik in separates `ChunkRenderer` System auslagern
   - Ermöglicht Multi-Threading (Chunk-Daten auf Background-Thread, Rendering auf Main-Thread)

### Vorteile gegenüber Minecraft

| Aspekt | Minecraft (Java) | CyA (Create your Adventure) |
|--------|------------------|------------------------------|
| Max. Distanz | ±30 Mio. Blöcke | ±147 Trillionen Blöcke |
| Chunk-Koordinaten | 32-Bit `int` | 64-Bit `long` |
| Float-Präzision | Problem ab ~16 Mio. | Gelöst durch World-Offset |
| Far Lands | Ja (Artefakte) | Nein (durch Architektur) |

### Referenzen
- Minecraft Wiki: World Boundary (30.000.000 Block Limit)
- IEEE 754 Floating Point Precision
- Greedy Meshing: https://0fps.net/2012/06/30/meshing-in-a-minecraft-game/

---

## Weitere Design-Entscheidungen

*(Zukünftige Architektur-Entscheidungen hier dokumentieren)*