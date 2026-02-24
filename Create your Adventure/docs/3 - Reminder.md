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

- InputState Refract to InputState and InputAnalyzer
- Input -> List<IGamepad>
	- InputActionType can be a "Action = Intent | Behavior = Policy"

	- GamepadButton.LeftTrigger | GamepadAxis.LeftTrigger -> Nur in GamepadAxis

	"GamepadButton enthält Trigger UND Axis
GamepadButton.LeftTrigger
GamepadAxis.LeftTrigger

Das wird dir früher oder später explodieren 💣

Warum?

Trigger sind analog

Button ist digital

Empfehlung

❌ Entfernen aus GamepadButton:

LeftTrigger, RightTrigger

✔ Nur in GamepadAxis

Wenn du „Trigger als Button“ willst → Threshold-Binding, nicht Enum-Dopplung."

ENGINE INPUT RULES

1. Gameplay darf NUR InputActions benutzen
2. Kein Gameplay-Code fragt direkt Keys ab
3. Kamera, Debug, Editor dürfen Direct Queries
4. UI nutzt eigenen InputContext
5. Actions sind rebindingfähig, Direct Input nicht

Zeit injizieren

❌ InputManager zählt Zeit selbst

✅ Engine gibt Zeit rein

Ergebnis: Pause, Replay, FixedUpdate funktionieren

Actions vs Direct

Actions = Spielregeln

Direct Input = Technik

Ohne Regel → Chaos

Mit Regel → saubere Engine

