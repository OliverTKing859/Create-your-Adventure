# Create your Adventure

## For now

-
-
-

## Later

- KingsEngine Master Docs öffentlich und Dev Version
- Rendering Prompt Header erstellen
- Alle Klassen die ausgegraulten using's entfernen
- InputBinding IsActive Method:
	- InputActionType.DoublePress => false, // TODO: implement DoublePress timer

- HotReload einprogrammieren

- AssetLoader muss noch mal verbessert werden

- Singleton upgraden to Systems: Alle Manager zu Systemen umwandeln - raus aus Singleton+
-	- Systems erstellen
	- DI (Dependency Injection) verwenden und Singletons entfernen.
    - Optional: Mini Service Locator
	- Als Beispiel (
    
 class KingsEngine
{
    public TextureSystem Textures { get; }
    public RenderSystem Renderer { get; }

    public KingsEngine()
    {
        Textures = new TextureSystem();
        Renderer = new RenderSystem(Textures); // DI!
    }
}

)