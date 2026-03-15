# Teil 1 Mesh & Buffer System (OpenGL Setup)

## Optimierungen

Sofort beheben:
SetData() im OpenGLVertexBuffer braucht einen Guard gegen doppelten Aufruf:
csharp

if (Handle != 0)
{
    gl.DeleteBuffer(Handle);
    Handle = 0;
}

UpdateData() braucht eine Prüfung:
csharp

if (Handle == 0) throw new InvalidOperationException("Buffer not initialized.");

Das doppelte Binding in Draw() beseitigen. Entweder Draw() bindet immer selbst und Bind()/Unbind() aus der öffentlichen API entfernen, oder Draw() setzt voraus, dass der Aufrufer gebunden hat. Empfehlung: Draw() bindet selbst, Bind()/Unbind() bleiben für manuelle Nutzung (z.B. Compute Shaders) erhalten, aber das Interface-Kommentar muss angepasst werden.

Mittelfristig:

BufferUsageARB als Parameter in SetData() durchreichen:
csharp

void SetData<T>(T[] vertices, VertexLayout layout, 
    BufferUsageARB usage = BufferUsageARB.StaticDraw) where T : unmanaged;

Logger-Calls in SetupVertexAttributes() auf Logger.Debug oder hinter ein #if DEBUG schieben.

VertexLayout könnte als readonly struct oder zumindest als sealed class mit einem Cache für den Stride implementiert werden — aktuell wird der Stride bei jedem Add() neu berechnet, was bei häufig erstellten Layouts minimal overhead erzeugt.

---

## TODO / Geplante Features

Feature                                     Status                  Realistisch?
Dynamic Buffer Support (DynamicDraw)        Fehlt                   Ja, einfach nachzurüsten
Multi-API-Unterstützung (DirectX/Vulkan)    Interface vorbereitet   Ja, Interfaces sind sauber
Instanced Rendering                         Implementiert           ✅ Vorhanden
Partial Buffer Updates (UpdateData)         Implementiert           ✅, aber fehlt Guard
Buffer-Usage-Hint als Parameter             Fehlt                   Ja, geringer Aufwand
Thread-safe Asset Loading                   Nicht angedacht         Komplex, erst bei Bedarf

---

## Checkliste

Sofort angehen:

- [ ] SetData() in OpenGLVertexBuffer: Guard gegen doppelten Aufruf / GPU-Leak beheben
- [ ] UpdateData(): Null-Handle-Guard hinzufügen
- [ ] Draw() vs. manuelles Bind(): Inkonsistenz klären und dokumentieren
- [ ] Logger-Calls in SetupVertexAttributes() auf Debug-Level reduzieren

Später / Nice-to-have:

- [ ] BufferUsageARB als Parameter in SetData() exponieren
- [ ] InvalidOperationException statt bloßer Warnung bei doppeltem Create()
- [ ] Thread-Safety-Konzept dokumentieren (auch wenn nicht implementiert)
- [ ] VertexLayout auf Immutabilität prüfen / als sealed class markieren

# Teil 2 Mesh Manager

## Optimierungen

Sofort beheben:
Dispose() muss instance zurücksetzen und den Initialized-State aufräumen:
csharp

public void Dispose()
{
    if (isDisposed) return;
    // ... cleanup ...
    meshFactory = null;
    isDisposed = true;
    instance = null; // Singleton zurücksetzen
}

CreateQuad()/CreateCube() sollten prüfen, ob das zurückgegebene Mesh bereits initialisiert ist, bevor Create() aufgerufen wird:
csharp

if (mesh.IsInitialized)
{
    Logger.Warn($"[MESH] Mesh '{name}' already initialized, returning cached");
    return mesh;
}

Mittelfristig:
Vertex-Structs statt roher float[] für CreateQuad() und CreateCube() einführen:
csharp

private readonly struct VertexPosTexCoord
{
    public float X, Y, Z, U, V;
}

Das stellt sicher, dass das Vertex-Layout mit den tatsächlichen Daten übereinstimmt.
RemoveMesh(string name) fehlt komplett. Einzelne Meshes können nicht aus dem Cache entfernt werden ohne alles zu disposen.

## TODO / Geplante Features

Feature                             Status                  Realistisch?
Face-spezifische UVs für Cube       Nicht vorhanden         Ja, mittlerer Aufwand
RemoveMesh()                        Fehlt                   Ja, geringer Aufwand
Vertex-Structs statt float[]        Fehlt                   Ja, mittlerer Aufwand
Thread-safe Cache                   Nicht vorhanden         Bei Bedarf mit Concurrent
DictionarySphere/Plane Primitives   Fehlt                   Realistisch, wenn nötig

## Checkliste

Sofort angehen:

- [ ] Dispose(): instance = null und meshFactory = null setzen
- [ ] CreateQuad()/CreateCube(): Guard für bereits initialisierte Meshes hinzufügen
- [ ] isDisposed-Check in CreateMesh(), GetMesh() etc. ergänzen

Später / Nice-to-have:

- [ ] RemoveMesh(string name) implementieren
- [ ] Vertex-Structs statt roher float[] einführen
- [ ] Face-spezifische UV-Unterstützung im Cube vorbereiten
- [ ] Thread-Safety dokumentieren oder mit ConcurrentDictionary absichern