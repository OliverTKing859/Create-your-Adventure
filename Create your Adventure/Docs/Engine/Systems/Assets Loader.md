# 'Asset' 'DEUTSCH'

## Zweck

Statische Hilfsklasse zur Lokalisierung von Game-Assets (Audio, Models, Shaders, Textures) im Dateisystem. 
Unterstützt eine zweistufige Verzeichnissuche (base → modded) mit integriertem Path-Caching für wiederholte Lookups.

---

## Verantwortlichkeiten

•	Bereitstellung typisierter Zugriffsmethoden pro Asset-Kategorie (GetAudioPath(string), GetModelPath(string), GetShaderPath(string), GetTexturePath(string))
•	Auflösung von Asset-Dateinamen zu vollständigen Dateipfaden
•	Priorisierte Suche: zuerst assets/base/{subfolder}/, dann assets/modded/{subfolder}/
•	Rekursive Dateisuche in allen Unterverzeichnissen eines Asset-Ordners
•	Caching aufgelöster Pfade in einem Dictionary<string, string> zur Vermeidung wiederholter Dateisystemzugriffe
•	Logging von Suchstatus (gefunden, modded, nicht gefunden) über Logger

---

## Architektur

---

## Pipeline

[API-Aufruf (z.B. GetShaderPath)] -> 
[Cache-Lookup (Dictionary)] -> 
[Suche in assets/base/{subfolder}/] -> 
[Suche in assets/modded/{subfolder}/] -> 
[Ergebnis cachen & zurückgeben]

---

## Abhängigkeiten

•	Logger (Logger) — Farbkodierte Konsolenausgabe für Info-/Error-Meldungen bei Asset-Suche
•	System.IO (Path, Directory) — Dateisystem-Operationen (Pfadkombination, Verzeichnisexistenz, rekursive Dateiauflistung)
•	Aufrufer: GameLoop (Shader-Pfade laden), LoadTextureFromAssets(string, string, TextureSettings?) (Textur-Pfade auflösen)

---

## Geplante Implementierungen

•	Keine expliziten TODO-Kommentare oder unfertige Stubs im Code sichtbar.
•	ClearCache() existiert, wird aber im aktuellen Codebase nicht aufgerufen — deutet auf geplante Runtime-Hot-Reload-Unterstützung hin.

---

## Zukünftige Ideen

•	Thread-Safety: PathCache ist ein reguläres Dictionary — bei parallelem Zugriff wäre ConcurrentDictionary<string, string> sicherer.
•	Asset-Validierung: Aktuell wird nur der Pfad zurückgegeben (string.Empty bei Fehler). Ein Result<T>-Pattern oder eine dedizierte AssetNotFoundException würde Fehlerbehandlung für Aufrufer vereinfachen.
•	Mod-Priorisierung: Derzeit überschreibt das Modded-Verzeichnis das Base-Asset nicht — es wird nur als Fallback gesucht. Eine konfigurierbare Priorisierung (Mod-Override vor Base) wäre für Modding-Support nützlich.
•	Asynchrone Suche: Directory.EnumerateFiles blockiert synchron — eine async-Variante könnte bei großen Asset-Verzeichnissen den Ladevorgang parallelisieren.
•	Unterstützung für Asset-Packs/Archive: Erweiterung auf .zip- oder .pak-Archive statt reiner Dateisystem-Suche.

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