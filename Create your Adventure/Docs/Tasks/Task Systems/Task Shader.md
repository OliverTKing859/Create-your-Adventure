# Shader

## Optimierungen

Sofort relevant:
Der Copy-Paste-Fehler in der Exception-Message beim Fragment-Shader sollte direkt gefixt werden, da er Debugging erheblich erschwert.
shaderCache gegen ConcurrentDictionary tauschen ist low-cost und schützt vor späteren Threading-Bugs, wenn Asset-Loading ausgelagert wird.
Nach Dispose() sollte instance = null gesetzt werden (oder zumindest IsInitialized als Guard überall greifen).

Mittelfristig:
LoadFromFiles() sollte eine explizite File.Exists()-Prüfung mit einer klaren Fehlermeldung bekommen, statt eine rohe Exception zu werfen.
Ein Reload(string name)-Mechanismus wäre für Hot-Reload während der Entwicklung sehr wertvoll – die Grundstruktur (Cache + Factory) ist bereits dafür ausgelegt.

---

## TODO / Geplante Features

Der Kommentar // Future extension point: else if (vulkanContext) ist realistisch geplant, 
aber die aktuelle Factory-Struktur (Func<string, IShaderProgram>) unterstützt das bereits vollständig – das ist gut vorgedacht.

Was noch fehlen wird, sobald das Projekt wächst: Shader-Hotreload, 
Shader-Variants/Permutationen (z.B. mit/ohne Skinning), 
und ein explizites ResetActiveProgram() für Frame-Beginn-Cleanup.

---

## Checkliste

Sofort fixen:

- [ ] Falsche Exception-Message bei Fragment-Shader-Fehler korrigieren
- [ ] instance = null in Dispose() des ShaderManagers setzen

Bald angehen:

- [ ] shaderCache → ConcurrentDictionary für Thread-Safety
- [ ] File.Exists()-Guard in LoadFromFiles()

Für später vormerken:

- [ ] Reload(name) / Hot-Reload Mechanismus
- [ ] Vulkan/DirectX Factory implementieren wenn nötig
- [ ] bool-Uniform-Overload im Interface ergänzen