# Teil 1 vom Camera System (Setup)

## Optimierungen

N	Bereich				Vorschlag
1	CameraTransform		Property-Setter für Yaw/Pitch/Roll einführen, die automatisch UpdateRotation() triggern
								– oder die Felder komplett kapseln
2	CameraTransform		Toten Code (cosPitch, sinYaw etc.) entfernen
3	CameraProjection	FieldOfView entfernen oder klar dokumentieren, dass BaseFov + FovModifier der echte Wert ist
4	CameraProjection	Projektionsmatrix cachen + nur bei Parameteränderung neu berechnen (Dirty-Flag)
5	CameraMotionModel	ApplyLookInput fixen: Rückgabe sollte SmoothedLookDelta * LookSensitivity sein
6	CameraMotionModel	DragCoefficient in ApplyMovementInput implementieren: Velocity *= (1f - DragCoefficient * dt)
7	CameraMotionModel	Fly/Debug-Presets: Velocity, VerticalVelocity, SmoothedLookDelta explizit auf Zero setzen

---

## TODO / Geplante Features

• Roll-Support ist vorbereitet (AllowRoll, RollReturnRate) aber die Rückführungslogik fehlt komplett – muss im Update-Loop implementiert werden.
• Chunk-Offset-System (LocalPosition im Namen angedeutet) – die Verbindung zwischen LocalPosition und einem World-Chunk-Koordinatensystem ist noch nicht sichtbar. 
  Das wird später kritisch für Floating-Point-Präzision bei großen Welten.
• FOV-Transition (Sprint-FOV, Zoom) ist durch FovModifier vorbereitet, aber der Interpolations-Code fehlt noch.
• Glider-Physik – VerticalSpeed = 0 ist ein Platzhalter, echte Lift/Gravity-Logik fehlt.

---

## Checkliste

🔴 Sofort

- [X] ApplyLookInput Bug fixen – Smoothing hat keinen Effekt auf Rückgabewert
- [ ] Toten Code in UpdateRotation() entfernen (cosPitch etc.)
- [ ] FieldOfView in CameraProjection entfernen oder klar abgrenzen
- [ ] DragCoefficient implementieren oder als // NYI kommentieren

🟡 Bald

- [ ] Auto-Invalidierung des Rotation-Cache in CameraTransform (Property-Setter)
- [ ] Projektionsmatrix-Caching in CameraProjection
- [ ] Alle Presets auf vollständige Initialisierung prüfen

🟢 Später

- [ ] Roll-Rückführungslogik implementieren
- [ ] Chunk-Koordinaten-System an LocalPosition anbinden
- [ ] FOV-Interpolation für Sprint / Zoom ausbauen

---

# Teil 2 vom Camera System (Manager)

## Optimierungen
N	Priorität			Problem										Fix
1	🔴 Kritisch			Logger jeden Frame							Entfernen oder #if DEBUG + Frame-Throttle
2	🔴 Kritisch			SetRotation ohne Update						Rotation()Transform.UpdateRotation() am Ende ergänzen
3	🔴 Kritisch			Hardcodiertes 0.016f in ApplyRotation		Echtes dt durchreichen
4	🟡 Wichtig			Konstruktor public beim Singleton			private machen
5	🟡 Wichtig			IsChunkVisible ohne Frustum Culling			Frustum-Test gegen CameraVisibilityContext implementieren
6	🟡 Wichtig			chunkWorldSize Parameter ungenutzt			Entfernen oder in Frustum Culling einbauen
7	🟡 Wichtig			Mode-Switch verliert Velocity				Vor dem Preset-Swap Velocity sichern und übertragen
8	🟢 Mittel			TeleportTo nutzt Initialize					Eigene MoveTo-Methode in WorldBinding



## TODO / Geplante Features

• Frustum Culling ist strukturell vorbereitet (CameraVisibilityContext existiert, chunkWorldSize Parameter vorhanden) aber die eigentliche AABB-vs-Frustum Logik fehlt komplett
• CameraVisibilityContext – die Klasse existiert und trägt ViewMatrix + ProjectionMatrix, was auf einen geplanten Frustum-Test hindeutet
• Spectator-Mode ist im Enum, nutzt aber denselben MotionModel wie Fly – eigene Logik (Collision-Bypass) fehlt noch
• Cinematic-Mode hat kein Keyframe/Spline-System – ist vermutlich bewusst noch offen

---

## Checkliste

🔴 Sofort

- [X] Logger.Info in ApplyRotation entfernen / hinter Debug-Flag
- [X] SetRotation – Transform.UpdateRotation() ergänzen
- [X] Hardcodiertes 0.016f durch echtes dt ersetzen
- [ ] Singleton-Konstruktor auf private setzen

🟡 Bald

- [ ] Frustum Culling in IsChunkVisible implementieren
- [ ] chunkWorldSize nutzen oder entfernen
- [ ] Velocity bei Mode-Switch erhalten
- [ ] TeleportTo vs. Initialize in WorldBinding klären

🟢 Später

- [ ] UpdateFromInput von InputManager.Instance entkoppeln
- [ ] Spectator-Mode eigene Logik
- [ ] Cinematic Keyframe-System

---

# Teil 3 vom Camera System (Visible und World)

## Optimierungen

N	Priorität		Problem												Fix
1	🔴 Kritisch		VP-Matrix Multiplikationsreihenfolge				Gegen Standard-Convention prüfen und konsistent halten
2	🔴 Kritisch		newChunk-Berechnung castet double zu float			Math.Floor (double) statt MathF.Floor(float) nutzen
3	🟡 Wichtig		Length statt LengthSquared im Threshold-Check		LengthSquared > threshold² verwenden
4	🟡 Wichtig		Initialize für Teleport semantisch falsch			Reset()/MoveTo() trennen, Event feuern
5	🟡 Wichtig		LocalOffset ungenutzt								Dokumentieren oder entfernen
6	🟢 Mittel		Origin-Shift Trigger unabhängig von Chunk-Wechsel	Shift an Chunk-Grenze koppeln statt an Float-Distanz

## TODO / Geplante Features
Angemerkung:	Chunks sind noch nicht implementiert. 
				Folgende Teile des Kamera-Systems sind deshalb vorbereitet aber noch nicht funktionierend:

• IsChunkVisible in CameraManager – Frustum ist bereit, Chunk-Daten fehlen
• ChunkChanged-Event hat keine Subscriber
• OriginShifted-Event hat keine Subscriber – kein System reagiert noch auf Origin-Shift
• GetChunkLocalPosition – nützlich für Rendering, aber ohne Chunk-System ohne Abnehmer
• RenderDistanceChunks – konfiguriert aber wirkungslos

Das ist kein Fehler – es ist der richtige Ansatz,
das Kamera-System zuerst stabil zu machen. 
Aber es bedeutet: Bevor Chunks kommen, 
müssen die kritischen Bugs oben (VP-Matrix, double→float Cast) gefixt sein, 
sonst baut das Chunk-System auf einem falschen Fundament auf.

---

---

## Gesamtfazit – Kamera-System
Das System ist architektonisch gut aufgestellt. 
Die Trennung in Transform / Projection / MotionModel / WorldBinding / VisibilityContext ist sauber und erweiterbar. 
Der Origin-Shift Ansatz zeigt, dass du das Floating-Point-Problem 
für große Welten von Anfang an bedacht hast – das ist nicht selbstverständlich.
Die kritischen Probleme sind keine strukturellen Fehler sondern Implementierungsdetails die sich aufgestaut haben:

---

## Gesamtcheckliste (alle 3 Teile)

🔴 Sofort – Blocking Bugs

- [X] VP-Matrix Reihenfolge prüfen (proj * view vs view * proj)
- [X] newChunk-Berechnung: (float) Cast entfernen, Math.Floor (double) nutzen
- [X] Logger.Info in ApplyRotation entfernen
- [X] SetRotation – Transform.UpdateRotation() ergänzen
- [X] ApplyLookInput – Smoothing-Bug fixen (Rückgabe auf SmoothedLookDelta umstellen)
- [X] Hardcodiertes 0.016f durch echtes dt ersetzen

🟡 Bald – Stabilität & Korrektheit

- [ ] LengthSquared statt Length für Threshold-Check
- [ ] DragCoefficient implementieren
- [ ] FieldOfView in CameraProjection entfernen / konsolidieren
- [ ] Singleton-Konstruktor private machen
- [ ] Velocity bei SetMotionMode erhalten
- [ ] TeleportTo / Initialize in WorldBinding trennen
- [ ] Toten Code in UpdateRotation entfernen
- [ ] Alle Presets vollständig initialisieren

🟢 Später – Features & Erweiterungen

- [ ] Frustum Culling in IsChunkVisible aktivieren (nach Chunk-Implementierung)
- [ ] OriginShifted / ChunkChanged Events an Chunk-System anbinden
- [ ] Projektionsmatrix cachen (Dirty-Flag)
- [ ] Auto-Invalidierung Rotation-Cache (Property-Setter)
- [ ] Roll-Rückführungslogik
- [ ] FOV-Interpolation für Sprint / Zoom
- [ ] Spectator-Mode Collision-Bypass
- [ ] UpdateFromInput von InputManager.Instance entkoppeln