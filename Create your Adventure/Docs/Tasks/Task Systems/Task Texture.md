# Texture

## Optimierungen

Sofort umsetzbar:
csharp

// 1. Presets als static readonly (kein GC-Druck)
public static readonly TextureSettings PixelArt = new() { ... };

// 2. Guard in AtlasRegion.Create
if (atlasWidth <= 0 || atlasHeight <= 0)
    throw new ArgumentException("Atlas dimensions must be positive.");

// 3. IsValid auf AtlasRegion
public bool IsValid => Width > 0 && Height > 0;

Mittelfristig:

Eigene Enums entweder entfernen (direkt Silk.NET nutzen) oder einen zentralen Mapper einführen — nicht beides parallel wachsen lassen.
FlipVertically aus TextureSettings herauslösen in eine separate TextureLoadOptions-Klasse.
ITextureAtlas.GetRegion() Vertrag im Interface dokumentieren (Exception vs. TryGet-Pattern).

---

## TODO / Geplante Features

Feature                                             Bewertung
Hot-Reload für Texturen                             Realistisch, aber braucht separaten FileWatcher
Dynamischer Atlas (nachträgliches Hinzufügen)       Komplex — GPU-Reupload nötig, später angehen
Format-Unterstützung (nicht nur RGBA)               Wichtig für Depth-Texturen / Render Targets
Thread-Safety beim Atlas-Build                      Unklar — Build sollte nur auf Main/Render-Thread laufen

---

## Checkliste

Sofort:

- [ ] static readonly statt static für Presets
- [ ] Division-by-Zero Guard in AtlasRegion.Create
- [ ] GetRegion() Fehlerverhalten im Interface dokumentieren

Später:

- [ ] Enum-Duplizierung auflösen
- [ ] FlipVertically aus Settings herauslösen
- [ ] IsValid auf AtlasRegion
- [ ] Build() Rückgabewert oder Exception-Strategie festlegen

# Teil 2 Mesh Manager

## Optimierungen

Kritisch – Sofort:
csharp

// Fix BindTexture State-Bug:
texture.Bind(unit);
boundTexture = name;  // korrekt
boundAtlas = null;    // korrekt

// Fix Unit-aware Binding:
private (string name, uint unit) boundTextureState;
if (boundTextureState == (name, unit)) return;

Fehlerbehandlung in BuildAtlasFromFolder:
csharp

if (!Directory.Exists(folderPath))
    throw new DirectoryNotFoundException($"Texture folder not found: {folderPath}");

if (!textureFiles.Any())
    Logger.Warn($"[TEXTURE] No PNG files found in '{folderPath}'");

Dispose-Safety:
csharp

private void ThrowIfDisposed()
{
    ObjectDisposedException.ThrowIf(isDisposed, this);
}
// → In EnsureInitialized() und allen Public-Methoden aufrufen

Singleton Reset nach Dispose:
csharp

public void Dispose()
{
    // ... bestehende Logik ...
    instance = null; // Singleton zurücksetzen
}

## TODO / Geplante Features

Feature                             Bewertung
Vulkan/DirectX Backend              Struktur ist vorbereitet ✅ — else if-Erweiterung ist sauber
Hot-Reload                          Fehlt komplett — kein Reload-Pfad auf ITexture
Dateiname-Kollision im Atlas        Kein Handling — bei größeren Asset-Mengen problematisch
Thread-Safety für Cache             Aktuell Single-Thread-only — sollte dokumentiert sein
UnloadTexture()                     Fehlt — kein gezieltes Freigeben einzelner Texturen möglich

## Checkliste

Sofort:

- [ ] State-Bug in BindTexture fixen (boundAtlas/boundTexture vertauscht)
- [ ] Unit in Bind-Guards einbeziehen ((name, unit)-Tupel)
- [ ] Directory.Exists-Guard in BuildAtlasFromFolder
- [ ] ThrowIfDisposed() in alle Public-Methoden
- [ ] BuildBlockAtlas auf AssetLoader umstellen

Später:

- [ ] UnloadTexture(string name) implementieren
- [ ] Thread-Safety dokumentieren oder ConcurrentDictionary einführen
- [ ] Dateiname-Kollisions-Handling im Atlas-Builder
- [ ] Singleton nach Dispose() auf null zurücksetzen