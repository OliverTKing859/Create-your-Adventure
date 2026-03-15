# Teil 1 vom Gameloop ()

## Optimierungen

Sofort umsetzbar:

Der Shader sollte nach OnLoad gecacht werden, nicht per Frame-Lookup:
csharp

// In OnLoad:
private IShader? activeShader;
activeShader = ShaderManager.Instance.LoadFromFiles("basic", vertPath, fragPath);

// In OnRender — kein String-Lookup mehr:
activeShader?.Use();

---

testCube sollte bei null in OnRender früh returnen:
csharp

if (testCube is null || shader is null) return;

Mittelfristig:
rawDeltaTime in OnRender sollte für State-Interpolation genutzt werden. 
Ein Kommentar // TODO: interpolation alpha = rawDeltaTime / fixedDeltaTime würde die Absicht dokumentieren.

Die Magic-Strings "basic", "dirt", "blocks" sollten in eine Constants- 
oder AssetKeys-Klasse ausgelagert werden — jetzt, bevor sie sich vermehren.  

---

## TODO / Geplante Features

Feature                             Status                              Realistisch?
PhysicsSystem.Tick(fixedDt)         Auskommentiert, Stub vorhanden      ✅ Struktur passt
Chunk-Simulation im Fixed-Tick      Nur Kommentar                       ⚠️ Benötigt Thread-Konzept
UI-System                           Erwähnt in Kommentar, kein Code     ⚠️ Unklar, wo es eingehängt wird
Render-Interpolation                rawDeltaTime ignoriert              ⚠️ Bewusste Lücke, sollte dokumentiert sein

---

## Checkliste

Sofort angehen:

- [ ] Shader-Referenz in OnLoad cachen, String-Lookup aus OnRender entfernen
- [ ] Early-Return in OnRender bei null-Shader oder null-Mesh
- [ ] Magic-Strings in AssetKeys-Klasse auslagern

Kann warten:

- [ ] rawDeltaTime in OnRender für Interpolations-Alpha vorbereiten
- [ ] Thread-Safety-Konzept dokumentieren, bevor Background-Loading kommt
- [ ] UI-Einhängepunkt in OnUpdate / OnRender definieren und als Stub anlegen