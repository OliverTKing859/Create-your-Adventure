# Create your Adventure

## For now

-
-
-

## Must implementation

- Anisotropic Filterung einbauen
- Floating Origin implementieren
- Relative Chunks korrekt machen

- Windows Manager Modes
	- Windowed
	- Fullscreen
	- Borderless

- Atlas more Categories
	- Blocks
	- Items
	- Entitys
	- Particles
	- Spezial?

- Ist die Camera und Input Manager und alles mögliche, mit dem GameLoop verbunden? Anstatt sie ihr eigenes Ding machen?

Kleiner Hinweis zu deinem Code

Hier:

float distanceFromOrigin = localPosition.Length;

Das misst nur Entfernung zum lokalen Ursprung.

Wenn du diagonal gehst:

(200,0,200)

ist die Länge:

≈ 282

→ OriginShift früher als erwartet.

Viele Engines nutzen stattdessen:

max(abs(x), abs(y), abs(z))

z.B.

float maxAxis = MathF.Max(MathF.Abs(localPosition.X),
               MathF.Max(MathF.Abs(localPosition.Y),
                         MathF.Abs(localPosition.Z)));

Das verhindert unnötige Shifts.

Aber dein Code funktioniert trotzdem.