# Asset

## Optimierungen

Sofort:
Modded-Priorität umkehren — Mods sollten Base überschreiben können:
csharp

// Erst Modded suchen, dann Base als Fallback
string moddedRoot = Path.Combine(AssetsRoot, "modded", subfolder);
result = SearchInFolder(moddedRoot, filename);
if (result is null)
{
    string baseRoot = Path.Combine(AssetsRoot, BaseFolder, subfolder);
    result = SearchInFolder(baseRoot, filename);
}

Thread-sicheren Cache verwenden:
csharp

private static readonly ConcurrentDictionary<string, string> PathCache = new();

Ungenutzte using-Direktiven entfernen (System.Text, System.Text.StringBuilder, System.Collections.Generic — letzteres wird durch den Dictionary-Typ implizit benötigt, aber prüfen).

Mittelfristig:
Einen asynchronen Preload-Mechanismus einführen (PreloadAllAsync()), 
damit der erste Frame keine teuren Filesystem-Reads auslöst. 
Gerade für Audio und Texturen ist das relevant.

string.Empty als Fehler-Rückgabe ist ein schwaches Fehlersignal — ein bool TryGetAudioPath(string filename, out string path) 
Pattern wäre robuster und verhindert stille Fehler im Caller-Code.

---

## TODO / Geplante Features

Feature                                 Status                              Einschätzung
Async Asset Loading                     Fehlt                               Realistisch, mittlerer Aufwand
Mod-Override-Logik                      Fehlerhaft (Priorität falsch)       Sofort korrigierbar
Asset-Validierung (Format, Größe)       Fehlt                               Sinnvoll vor Produktion
Cache-Persistenz über Sessions          Fehlt                               Optional, Low Priority

---

## Checkliste

Sofort angehen:

- [ ] Mod-Priorität umkehren (Modded überschreibt Base)
- [ ] PathCache auf ConcurrentDictionary umstellen
- [ ] Ungenutzte using-Direktiven entfernen

Kann warten:

- [ ] Async Preloading einführen
- [ ] Rückgabe auf TryGet-Pattern umstellen
- [ ] Asset-Validierung (Existenz, Dateiformat) ergänzen