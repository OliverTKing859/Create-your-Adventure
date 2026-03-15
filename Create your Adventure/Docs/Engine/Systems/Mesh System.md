# 'Mesh System' 'DEUTSCH'

## Zweck

Singleton-Manager-Klasse, die für die Erstellung, das Caching und die Verwaltung von Mesh-Objekten zuständig ist. 
Abstrahiert das zugrunde liegende Graphics-API-Backend (derzeit OpenGL) über ein Factory-Pattern 
und bietet Convenience-Methoden für gängige Primitives (Quad, Cube).

---

## Verantwortlichkeiten

•	Singleton-Zugriff auf eine zentrale Mesh-Verwaltungsinstanz (thread-safe via Lock)
•	Initialisierung mit einem Graphics-Backend über GlContext
•	Erstellung und Caching von Meshes über ein Dictionary<string, IMesh>
•	Bereitstellung von Factory-Methoden für Primitives (CreateQuad(String, float), CreateCube(string, float, AtlasRegion))
•	Abruf und Existenzprüfung gecachter Meshes (GetMesh(string), HasMesh(string))
•	Disposal aller verwalteten Meshes und GPU-Ressourcen

---

## Architektur

// Um welche Art von Klassenarchitektur handelt es sich?

---

## Pipeline

MeshManager.Initialize() -> meshFactory = (name) => new OpenGLMesh(gl, name)
         |
         v
CreateMesh(name) -> Factory erzeugt IMesh -> Cache in meshCache
         |
         v
mesh.Create(vertices, indices, layout)
   -> OpenGLMesh: VAO erstellen & binden
   -> OpenGLVertexBuffer.SetData() [VBO: GenBuffer → BindBuffer → BufferData]
   -> SetupVertexAttributes() [EnableVertexAttribArray → VertexAttribPointer pro Attribut]
   -> OpenGLIndexBuffer.SetData() [EBO: GenBuffer → BindBuffer → BufferData]
   -> VAO unbinden (State gespeichert)
         |
         v
mesh.Bind() -> gl.BindVertexArray(vao)
         |
         v
mesh.Draw() -> gl.DrawElements / gl.DrawArrays (oder Instanced-Varianten)
         |
         v
MeshManager.Dispose() -> Alle gecachten Meshes disposen → VAO/VBO/EBO löschen

---

## Abhängigkeiten

•	IMesh – Interface-Abstraktion für Mesh-Objekte (IMesh.cs)
•	IVertexBuffer / IIndexBuffer – Interface-Abstraktionen für GPU-Buffer (Source/Engine/Mesh/)
•	OpenGLMesh – Konkrete OpenGL-Implementierung mit VAO/VBO/EBO (OpenGLMesh.cs)
•	OpenGLVertexBuffer – OpenGL Vertex Buffer Object (VBO) Implementierung (OpenGLVertexBuffer.cs)
•	OpenGLIndexBuffer – OpenGL Element Buffer Object (EBO) Implementierung (OpenGLIndexBuffer.cs)
•	VertexLayout / VertexAttribute / VertexAttributeType – Vertex-Datenformat-Beschreibung (VertexLayout.cs)
•	AtlasRegion – UV-Koordinaten aus Texture-Atlas für Cube-Erstellung (AtlasRegion.cs)
•	windowManager – Bereitstellung des OpenGL-Kontexts (GlContext)
•	Logger – Logging über alle Mesh-Operationen hinweg

---

## Geplante Implementierungen

•	Nur OpenGL als Backend implementiert; die Factory-Logik in Initialize() 
    enthält einen else-Branch mit InvalidOperationException für fehlende APIs – weitere Backends (DirectX, Vulkan) sind vorbereitet aber nicht vorhanden
•	VertexLayout.PositionTexCoordNormal() ist definiert, wird aber in MeshManager noch nicht aktiv verwendet (nur PositionTexCoord())
•	IMesh.DrawInstanced() ist spezifiziert und in OpenGLMesh implementiert, wird aber von MeshManager nicht direkt exponiert
•	IVertexBuffer.UpdateData() für partielle Buffer-Updates ist implementiert, wird aber nirgends im Mesh-Erstellungsfluss aufgerufen

---

## Zukünftige Ideen

•	Weitere Graphics-Backends – Die Interface-Abstraktion (IMesh, IVertexBuffer, IIndexBuffer) ist bereits vorhanden; Implementierungen für Vulkan/DirectX könnten ergänzt werden
•	Mesh-Loading aus Dateien – Aktuell werden nur programmatische Primitives unterstützt; ein Loader (z.B. OBJ/glTF) könnte CreateMesh(string) + mesh.Create() nutzen
•	Dynamische Meshes – UpdateData() im OpenGLVertexBuffer ist bereit, wird aber nicht über IMesh exponiert; ein IMesh.UpdateVertices() könnte hinzugefügt werden
•	Material/Shader-Zuordnung – Meshes haben aktuell keine Shader-/Material-Referenz; eine Kopplung würde den Render-Workflow vereinfachen
•	Batch-/Merge-Rendering – Mehrere Meshes könnten in einen gemeinsamen Buffer gemergt werden, um Draw Calls zu reduzieren
•	Vertex-Layout mit Farb-Attribut – VertexAttribute bietet keine Factory-Methode für Farben (aColor); dies könnte als Erweiterung hinzugefügt werden

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