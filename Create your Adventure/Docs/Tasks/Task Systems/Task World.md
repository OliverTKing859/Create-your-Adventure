# Teil 1 Mesh & Buffer System (OpenGL Setup)

## Optimierungen

Sofort:
long → int für ChunkCoord.X/Y/Z – kein Vorteil bei long für typische Voxel-Weltgrößen, aber messbarer Overhead in Hash + Vergleich bei hoher Chunk-Dichte.

Tippfehler beheben: UpdatePrivority → UpdatePriority.

Distanz-Schwellen in UpdatePriority dynamisch machen – z.B. als Parameter oder relativ zu einer konfigurierbaren RenderDistance skalieren:
csharp
// statt hardcoded < 4, < 16, < 64
long highThreshSq = (long)(renderDist * 0.25f) * (long)(renderDist * 0.25f);

Später:
ChunkJob ist eine class – bei vielen tausend Chunks entsteht GC-Druck. Für Performance-kritische Systeme wäre ein Pool (ObjectPool<ChunkJob>) oder der Wechsel zu einem struct-basierten ECS-Ansatz sinnvoll.

ChunkMetadata könnte optional als struct ausgeführt werden, wenn sie immer vorhanden ist – spart Heap-Allokation.

---

## TODO / Geplante Features

Feature											Status		Realistisch?
TickRate tatsächlich im Scheduler verwenden		Fehlt		✅ Ja, nächster Schritt
Dirty/NeedsRemesh-State							Fehlt		✅ Sinnvoll
ChunkMetadata.IsGenerated						Fehlt		✅ Notwendig für WorldGen
Pool für ChunkJob								Fehlt		⏳ Später
Dynamische Prioritätsschwellen					Fehlt		✅ Sollte bald

---

## Checkliste

Sofort angehen:

- [ ] UpdatePrivority → UpdatePriority (Tippfehler)
- [ ] long → int in ChunkCoord
- [ ] Distanz-Schwellen in UpdatePriority dynamisch / konfigurierbar machen
- [ ] TickRate im WorldScheduler tatsächlich auswerten

Kann warten:

- [ ] ChunkJob-Pooling gegen GC-Druck
- [ ] IsGenerated-Flag in ChunkMetadata
- [ ] Expliziter NeedsRemesh-State im ChunkState-Enum