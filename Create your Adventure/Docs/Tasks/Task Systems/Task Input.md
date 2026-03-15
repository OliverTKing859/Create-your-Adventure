# Teil 1 vom Input System (Device Phase)

## TODO / Geplante Features
Feature											Status								Einschätzung

skipNextDelta für Cursor-Mode-Wechsel			Bug — nicht implementiert			Sofort fixen

CursorMode.Confined								Stub vorhanden, nicht funktional	Realistisch, braucht GLFW-Clip

IsEnabled-Toggle pro Device						Fehlt im Interface					Sinnvoll, mittelfristig

Rumble/Haptics für Gamepad						Nicht erwähnt, aber typisch			Später, braucht eigene API

Hotplug-Erkennung (Gamepad connect/disconnect)	Nicht vorhanden						Wichtig für Produktionsqualität

Multi-Gamepad-Support							Hardcoded [0]						Bewusste Einschränkung oder TODO?

---

## Checkliste
Sofort angehen:

- [ ] skipNextDelta in OnMouseMove tatsächlich auswerten (Bug)
- [ ] OnButtonDown/Up auf explizites if (state is null) return umstellen

Mittelfristig:

- [ ] IsEnabled-Property ins IInputDevice-Interface aufnehmen
- [ ] CursorMode.Confined korrekt implementieren oder mit NotSupportedException kennzeichnen
- [ ] Hotplug-Erkennung für Gamepad (connect/disconnect events)

Langfristig / Optional:

- [ ] Multi-Gamepad-Support überdenken (jetzt [0] hardcoded)
- [ ] Thread-Safety vorbereiten wenn Input-Thread geplant ist
- [ ] Haptic/Rumble-API für GamepadDevice

---

# Teil 2 vom Input System (Classes)

## TODO / Geplante Features
Feature												Status									Einschätzung

DoublePress Implementierung							Definiert, nicht implementiert			Braucht Timer pro Action/Binding, machbar

LongPress für Mouse/Gamepad							Fehlt in deren Bindings					Inkonsistenz, mittelfristig fixen

Keybinding-Rebinding zur Laufzeit					Serialisierung vorhanden				Grundlage ist da, UI fehlt noch

InputConstants als zentrale Quelle					Zwei duplizierte Konstanten				Kleiner Refactor, sofort möglich
Mouse-Binding in AddMouseBinding					MouseButtonBinding existiert,			Fehlt im Fluent API
auf InputAction										aber kein AddMouseBinding()-Shortcut 
													auf InputAction 

Tippfehler NumpadSubstract							Bug										Sofort fixen

---

## Checkliste
Sofort angehen:

- [ ] Tippfehler NumpadSubstract → NumpadSubtract korrigieren
- [ ] DoublePress in allen IsActive()-Implementierungen behandeln oder mit NotSupportedException kennzeichnen
- [ ] LongPressThreshold aus KeyBinding und InputAnalyzer in eine gemeinsame Konstante (InputConstants) zusammenführen
- [ ] LINQ-Allokation in EndFrame durch reusable List<KeyCode> ersetzen

Mittelfristig:

- [ ] AddMouseBinding() Shortcut auf InputAction hinzufügen (Fluent API ist unvollständig)
- [ ] LongPress für MouseButtonBinding und GamepadButtonBinding nachziehen
- [ ] GetLookVector() Dead-Zone-Check für Mouse-Delta ergänzen

Langfristig / Optional:

- [ ] BeginFrame-Kopie auf UnionWith umstellen (Lesbarkeit)
- [ ] DoublePress-Timer-System designen (braucht State pro Binding)
- [ ] Keybinding-Rebinding UI auf Basis der vorhandenen Serialisierung aufbauen

---

# Teil 3 vom input System (Manager)

## TODO / Geplante Features
Feature												Status									Einschätzung
XML-Summary auf InputManager						Leer									Sofort, 2 Zeilen
EnsureInitialized()-Guard							Fehlt									Sofort,sicherer als !
isDisposed-Check in BeginFrame/EndFrame				Fehlt									Sofort
Singleton-Reset für Tests							Fehlt									Mittelfristig
Gamepad Hotplug										Nicht vorhanden							Wie in Teil 1 erwähnt
SetCursorMode No-Op wenn gleicher Modus				Fehlt									Kleine Verbesserung