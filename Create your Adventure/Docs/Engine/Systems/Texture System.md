# 'Texture' 'DEUTSCH'

## Zweck

Zentraler Singleton-Manager für das Laden, Caching, Erstellen und Binden von Texturen und Texturatlanten. 
Abstrahiert die Grafik-API über ein Factory-Pattern und unterstützt aktuell OpenGL als Backend.

---

## Verantwortlichkeiten

•	Singleton-Bereitstellung – Thread-sichere Singleton-Instanz über Double-Check-Locking
•	API-Abstraktion – Erzeugt ITexture- und ITextureAtlas-Instanzen über konfigurierbare Factory-Delegates, entkoppelt vom konkreten Grafik-Backend
•	Textur-Laden – Laden einzelner Texturen aus Dateipfad oder Asset-Ordner (LoadTexture(string, string, TextureSettings?), LoadTextureFromAssets(string, string, TextureSettings?))
•	Atlas-Erstellung – Erzeugen, Befüllen und Bauen von Texturatlanten aus Einzeltexturen oder ganzen Ordnern (CreateAtlas(string), BuildAtlasFromFolder(string, string, TextureSettings?), BuildBlockAtlas())
•	Caching – Dictionary-basiertes Caching aller geladenen Texturen und Atlanten nach Name
•	State-Tracking & Bind-Optimierung – Vermeidet redundante Bind-Aufrufe durch Tracking der aktuell gebundenen Textur/Atlas
•	UV-Zugriff – Convenience-Methode GetBlockUV(string) für schnellen Block-UV-Zugriff
•	Ressourcen-Freigabe – IDisposable-Implementierung zur sauberen GPU-Ressourcenfreigabe

---

## Architektur

// Um welche Art von Klassenarchitektur handelt es sich?

---

## Pipeline

### Texture

Initialize(detect GL context)
    -> Factory-Delegates setzen (textureFactory / atlasFactory)
        -> LoadTexture / CreateAtlas (Factory erstellt ITexture / ITextureAtlas)
            -> Cache prüfen → ggf. LoadFromFile / AddTexture + Build
                -> BindTexture / BindAtlas (State-Check → GPU Bind)
                    -> GetBlockUV / GetRegion (UV-Koordinaten für Rendering)
                        -> Dispose (GPU-Ressourcen freigeben, Caches leeren)

### Atlas
AddTexture(name, path) -> Build(settings) :
  [1. Load all images via StbImage]
    -> [2. Calculate Power-of-2 atlas size (256–4096)]
      -> [3. Grid-based packing + pixel copy into atlas buffer]
        -> [4. GL TexImage2D upload + parameter config + mipmap generation]

---

## Abhängigkeiten

Abhängigkeit	        Pfad / Typ	                Rolle
windowManager	        Engine (Singleton)	        Liefert den GlContext für die API-Erkennung bei Initialize()
ITexture	            ITexture.cs	                Interface-Vertrag für einzelne Texturen
ITextureAtlas	        ITextureAtlas.cs	        Interface-Vertrag für Texturatlanten
TextureSettings	        TextureSettings.cs	        Immutable Record mit Filter-/Wrap-/Mipmap-Konfiguration und Presets (PixelArt, Smooth, Atlas)
AtlasRegion	            AtlasRegion.cs	            Readonly-Struct mit Pixel- und UV-Koordinaten einer Atlas-Region
OpenGLTexture2D	        OpenGLTexture2D.cs	        OpenGL-Implementierung von ITexture (StbImage-Laden, GPU-Upload)
OpenGLTextureAtlas	    OpenGLTextureAtlas.cs	    OpenGL-Implementierung von ITextureAtlas (Grid-Packing, GPU-Upload)
AssetLoader	            AssetLoader.cs	            Auflösung relativer Asset-Pfade (base/modded) mit Cache
Logger	                Engine (statisch)	        Logging aller Lade-/Bind-/Fehler-Ereignisse
SamplerMinFilter /      TextureSettings.cs	        API-agnostische Enums für Textur-Sampling-Parameter
SamplerMagFilter / 
SamplerWrapMode	        


---

## Geplante Implementierungen

•	Weitere Grafik-Backends – Code enthält expliziten Kommentar: // Future extension point: else if (vulkanContext is not null) { ... } — Vulkan- und DirectX-Support sind vorgesehen aber nicht implementiert
•	Bug in BindTexture(string, uint) – In BindTexture() wird boundAtlas statt boundTexture gesetzt und boundTexture genullt. Die Zuweisungen von boundTexture und boundAtlas sind vertauscht (vermutlich Copy-Paste-Fehler)
•	Bug in Atlas-Packing – In OpenGLTextureAtlas.Build() wird currentY *= textureSize statt currentY += textureSize verwendet beim Zeilenumbruch – dadurch werden Texturen bei Zeilenumbruch falsch positioniert

---

## Zukünftige Ideen

•	Erweiterter Packing-Algorithmus – Der aktuelle Grid-Packer setzt gleich große Texturen voraus. Ein Bin-Packing-Algorithmus (z.B. MaxRects) würde unterschiedlich große Texturen effizienter unterstützen
•	Async-Loading – Texturen könnten asynchron von Disk geladen und auf einem Hintergrund-Thread dekodiert werden, bevor der GPU-Upload auf dem Render-Thread erfolgt
•	Texture-Streaming / LOD – Dynamisches Laden/Entladen von Texturen basierend auf Sichtbarkeit und Distanz
•	Atlas-Resize / Rebuild – Möglichkeit, zur Laufzeit Texturen zum Atlas hinzuzufügen und neu zu packen
•	Textur-Kompression – Unterstützung für GPU-komprimierte Formate (BC/DXT, ASTC) zur Reduzierung des VRAM-Verbrauchs
•	Vulkan/DirectX Backends – Implementierung von ITexture/ITextureAtlas für weitere APIs gemäß dem bestehenden Factory-Pattern
•	Hot-Reload – Erkennen geänderter Textur-Dateien zur Laufzeit und automatisches Neuladen für schnellere Iteration

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