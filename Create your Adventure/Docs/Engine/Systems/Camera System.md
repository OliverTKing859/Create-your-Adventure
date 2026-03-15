# 'Camera System' 'DEUTSCH'

## Zweck

Zentraler Singleton-Orchestrator für das gesamte Kamerasystem. 
Verwaltet Transform, Projektion, Bewegungsmodell und Weltkoordinaten-Bindung. 
Verarbeitet Spieler-Input und erzeugt View-/Projektionsmatrizen sowie Sichtbarkeitskontext für das Rendering.

---

## Verantwortlichkeiten

// Welche Verantwortlichkeiten bringt dieser Prozess in Bezug auf die wichtigsten Punkte mit sich?

•	Initialisierung der Kamera an absoluten Weltkoordinaten
•	Verarbeitung von Bewegungs- und Blick-Input (direkt oder via InputManager)
•	Delegation an Subsysteme (CameraTransform, CameraMotionModel, CameraWorldBinding)
•	Lazy-Berechnung und Caching von View- und Projektionsmatrizen
•	Bereitstellung von CameraVisibilityContext für Frustum-/Distanz-basiertes Culling
•	Umschaltung zwischen Bewegungsmodi (Walk, Fly, Glider, Cinematic, Spectator, Debug)
•	Teleportation und direkte Rotation-/FoV-Setter für Cutscenes etc.

---

## Architektur

// Um welche Art von Klassenarchitektur handelt es sich?

---

## Pipeline

// Wie sieht der Prozess in den einzelnen Phasen genau aus?
[Input (Move/Look)] -> 
[CameraMotionModel (Acceleration/Smoothing)] -> 
[CameraTransform (Position + Rotation)] -> 
[CameraWorldBinding (Chunk-Tracking + Origin-Shift)] -> 
[Matrix Generation (View/Projection, cached)] -> 
[CameraVisibilityContext (Frustum Extraction)]			

---

## Abhängigkeiten

// Welche Abhängigkeiten erfordert dieser Prozess? (Aufgelistet nach Wichtigkeit)

•	CameraTransform (struct)						— Position, Euler-Winkel, Quaternion-basierte Richtungsvektoren
•	CameraProjection (struct)						— FoV, Near/Far-Plane, Aspect Ratio, Projektionsmatrix
•	CameraMotionModel (class)						— Geschwindigkeit, Beschleunigung, Look-Smoothing, Presets pro Modus
•	CameraWorldBinding (class)						— Chunk-Koordinaten, Origin-Shift, Koordinatenkonvertierung, Events
•	CameraVisibilityContext (readonly struct)		— Snapshot für Rendering: Matrizen, Frustum, Render-Distanz
•	ViewFrustum (readonly struct)					— 6-Ebenen-Frustum aus ViewProjection-Matrix, AABB-/Sphere-Tests
•	CameraMotionMode (enum)							— Walk, Fly, Glider, Cinematic, Spectator, Debug
•	InputManager									— Liefert Movement-/Look-Vektoren und Action-Triggers
•	Logger											— Diagnoseausgaben
•	MathHelper										— ExpDecay(float, float, float, float), Clamp(float, float, float), Deg2Rad
•	ChunkCoord, Plane3D, Box3D<float>				— Geometrie-Typen
•	Silk.NET Maths									— Matrix4X4<float>, Vector3D<float>, Quaternion<float>

---

## Geplante Implementierungen

•	IsChunkVisible(ChunkCoord, float) prüft aktuell nur Chunk-Distanz — Frustum-Culling via ViewFrustum.Intersects() ist vorbereitet aber nicht angebunden
•	DragCoefficient ist in CameraMotionModel definiert (z.B. 0.1f für Fly), wird aber in ApplyMovementInput(Vector3D<float>, bool, float) / ComputePositionDelta(float) nie angewendet
•	SmoothedLookDelta wird in ApplyLookInput(Vector2D<float>, float) berechnet, aber der geglättete Wert wird verworfen — es wird rawDelta * LookSensitivity zurückgegeben
•	FieldOfView existiert in CameraProjection neben BaseFov, wird aber nicht in der Matrix-Berechnung verwendet (EffectiveFov = BaseFov + FovModifier)																						

---

## Zukünftige Ideen

•	Frustum-Culling in IsChunkVisible(ChunkCoord, float) vollständig aktivieren (AABB-Test gegen ViewFrustum)
•	Drag/Reibung tatsächlich in die Bewegungssimulation integrieren für realistischeres Fly/Glider-Feeling
•	Look-Smoothing korrekt nutzen (SmoothedLookDelta statt Raw-Delta zurückgeben)
•	Redundantes FieldOfView-Feld bereinigen oder als Runtime-Override einsetzen
•	Kamera-Shake-System (z.B. Explosionen, Laufen) als optionale Komponente
•	Interpolation zwischen Kamera-Modi bei SetMotionMode(CameraMotionMode) für weiche Übergänge
•	Third-Person / Orbit-Kamera als zusätzlicher CameraMotionMode
•	Serialisierung des Kamerazustands für Savegames (Position, Rotation, Modus)

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