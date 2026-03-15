<<<<<<< HEAD
![License](https://img.shields.io/badge/license-CyA--Custom-red)
![Status](https://img.shields.io/badge/status-In%20Development-orange)
![Language](https://img.shields.io/badge/language-C%23-blue)
![Engine](https://img.shields.io/badge/engine-Custom%20Engine-purple)

---

# 🌍 Create Your Adventure (CyA)

> *Du bist nicht der Spieler einer Welt – du bist ihr Autor.*
> *You are not a player in a world – you are its author.*

---

## 🇩🇪 Deutsch

*Create Your Adventure – möglicherweise eines der größten Spiele der Welt,
nicht gemessen an Inhalten, sondern an reiner Fläche. Durch Long-Integer-basierte
Chunk-Koordinaten erstreckt sich die Welt auf eine Größe, die jeden
bisherigen Maßstab sprengt.*

### Was ist CyA?

Ihr kennt es sicherlich: Welten, die statisch und unlebendig sind. Voxel-Spiele, die schon nach 500.000 Blöcken Weltkoordinaten die ersten Mängel aufweisen – ruckelnde Chunks, begrenzte Simulationen, eine Welt die nur am absoluten Nullpunkt wirklich funktioniert.

**Damit ist jetzt ein für alle Mal Schluss.**

**Create Your Adventure** ist ein Voxel-Spiel, dessen Welt **Billionen von Blöcken und Chunks** umfasst – und das nicht nur auf dem Papier. Die Welt funktioniert überall gleich präzise, egal wie weit du dich vom Ursprung entfernst.

---

### 🧠 Das Herzstück: Chunk-Metadaten

Chunks in CyA sind mehr als statische Render- und Save/Load-Einheiten. Sie sind das **Herz des Spiels**.

Jeder Chunk trägt **Metadaten** – ein dynamisches System, das selbst kleinste Veränderungen mit der gesamten Spielwelt verknüpft. Diese Architektur ermöglicht ein realistisches, lebendiges Spielerlebnis in einem Umfang, der bisher nicht existiert hat.

**Was Chunk-Metadaten ermöglichen:**

- 🌊 **Dynamische Flüsse** – Verändere das Gelände, und Flüsse passen ihren Lauf an
- 🌿 **Lebendige Biome** – Eingriffe in ein Biom können benachbarte Biome transformieren
- 🌦️ **Reaktives Wettersystem** – Weltveränderungen beeinflussen Klima und Wetter
- 🐾 **Tierverhalten & Migration** – Tiere verlassen veränderte Gebiete, neue Arten siedeln sich an
- 🔗 **Verkettete Konsequenzen** – Jede noch so kleine Aktion kann große Dinge heraufbeschwören

---

### 💡 Philosophie

In den meisten Spielen ist die Welt ein Sandkasten – du baust, die Welt reagiert nicht wirklich.

In **CyA** bist du **lebendig mit der Welt**. Du bist nicht der Spieler einer statischen Kulisse – du bist der Autor deiner eigenen Weltgeschichte. Deine Entscheidungen formen die Welt. Die Welt formt zurück.

---

### 🛠️ Tech Stack

| Technologie | Verwendung |
|---|---|
| C# / .NET | Hauptprogrammiersprache |
| Silk.NET | Low-Level Grafik & Fenster-API |
| ImGUI | Debug- & Entwickler-Interface |
| Eigene Engine | Keine externe Game-Engine – alles selbst gebaut |

---

### ⚙️ Technische Vision

| Feature | Beschreibung |
|---|---|
| Weltgröße | Billionen von Blöcken und Chunks |
| Koordinatenstabilität | Volle Funktionalität auf weiten Strecken vom Ursprung |
| Chunk-System | Metadaten-basiert, dynamisch und vernetzt |
| Simulation | Wetter, Ökosysteme, Hydrologie, Biome |
| Spielerprinzip | Aktiver Teil der Welt, nicht externer Beobachter |

---

### 🗺️ Roadmap

- [x] Projektkonzept & Architektur
- [ ] Chunk-System mit Metadaten
- [ ] Prozeduale Weltgenerierung
- [ ] Dynamisches Wettersystem
- [ ] Ökosystem & Tiermigration
- [ ] Alpha-Release

---

### 🚀 Getting Started

> Das Projekt befindet sich noch in früher Entwicklung. Eine spielbare Version ist noch nicht verfügbar.
> Sobald eine erste Version veröffentlicht wird, findest du hier die Installationsanleitung.

---

### 📬 Kontakt & Feedback

Du hast Ideen, Anmerkungen oder Fragen? Öffne gerne ein **Issue** in diesem Repository.
Bitte beachte: Direkte Code-Beiträge (Pull Requests) werden gemäß der Lizenz nicht angenommen.

---

### 📄 Lizenz

Dieses Projekt steht unter einer eigenen Lizenz. Siehe [LICENSE](LICENSE) für alle Details.
Kurz zusammengefasst: Einsehen & Lernen erlaubt – Bearbeiten & kommerzielle Nutzung ohne Genehmigung nicht.

---
---

## 🇬🇧 English

*Create Your Adventure – potentially one of the largest games in the world,
not measured by content, but by raw surface area. Through Long Integer-based
chunk coordinates, the world extends to a scale that surpasses anything
seen before.*

### What is CyA?

You're probably familiar with worlds that are static and lifeless. Voxel games that show their first flaws after just 500,000 blocks of world coordinates – jerky chunks, limited simulations, a world that only really works at absolute zero.

**That's now a thing of the past, once and for all.**

**Create Your Adventure** is a voxel game whose world comprises **trillions of blocks and chunks** – and not just on paper. The world functions with the same precision everywhere, no matter how far you stray from the origin.

---

### 🧠 The Heart of the Game: Chunk Metadata

Chunks in CyA are more than static render and save/load units. They are the **heart of the game**.

Each chunk carries **metadata** – a dynamic system that links even the smallest changes to the entire game world. This architecture enables a realistic, vibrant gaming experience on a scale that has never existed before.

**What chunk metadata enables:**

- 🌊 **Dynamic rivers** – Change the terrain, and rivers adjust their course
- 🌿 **Living biomes** – Interventions in one biome can transform neighboring biomes
- 🌦️ **Reactive weather system** – World changes affect climate and weather
- 🐾 **Animal behavior & migration** – Animals leave changed areas, new species settle in
- 🔗 **Chain reactions** – Every little action can conjure up big things

---

### 💡 Philosophy

In most games, the world is a sandbox – you build, the world doesn't really react.

In **CyA**, you are **alive with the world**. You are not a player in a static setting – you are the author of your own world history. Your decisions shape the world. The world shapes you back.

---

### 🛠️ Tech Stack

| Technology | Usage |
|---|---|
| C# / .NET | Primary programming language |
| Silk.NET | Low-level graphics & window API |
| ImGUI | Debug & developer interface |
| Custom Engine | No external game engine – built from scratch |

---

### ⚙️ Technical Vision

| Feature | Description |
|---|---|
| World size | Trillions of blocks and chunks |
| Coordinate stability | Full functionality over long distances from the origin |
| Chunk system | Metadata-based, dynamic, and networked |
| Simulation | Weather, ecosystems, hydrology, biomes |
| Player principle | Active part of the world, not an external observer |

---

### 🗺️ Roadmap

- [x] Project concept & architecture
- [ ] Chunk system with metadata
- [ ] Procedural world generation
- [ ] Dynamic weather system
- [ ] Ecosystem & animal migration
- [ ] Alpha release

---

### 🚀 Getting Started

> The project is still in early development. A playable version is not yet available.
> Once a first version is released, you will find installation instructions here.

---

### 📬 Contact & Feedback

Got ideas, feedback or questions? Feel free to open an **Issue** in this repository.
Please note: Direct code contributions (Pull Requests) are not accepted per the license terms.

---

### 📄 License

This project is licensed under a custom license. See [LICENSE](LICENSE) for full details.
Short summary: Viewing & learning is allowed – modifying & commercial use without permission is not.

---

*© 2026 Oliver Schmidt. All rights reserved.*
=======
![License](https://img.shields.io/badge/license-CyA--Custom-red)
![Status](https://img.shields.io/badge/status-In%20Development-orange)
![Language](https://img.shields.io/badge/language-C%23-blue)
![Engine](https://img.shields.io/badge/engine-Custom%20Engine-purple)

---

# 🌍 Create Your Adventure (CyA)

> *Du bist nicht der Spieler einer Welt – du bist ihr Autor.*
> *You are not a player in a world – you are its author.*

---

## 🇩🇪 Deutsch

*Create Your Adventure – möglicherweise eines der größten Spiele der Welt,
nicht gemessen an Inhalten, sondern an reiner Fläche. Durch Long-Integer-basierte
Chunk-Koordinaten erstreckt sich die Welt auf eine Größe, die jeden
bisherigen Maßstab sprengt.*

### Was ist CyA?

Ihr kennt es sicherlich: Welten, die statisch und unlebendig sind. Voxel-Spiele, die schon nach 500.000 Blöcken Weltkoordinaten die ersten Mängel aufweisen – ruckelnde Chunks, begrenzte Simulationen, eine Welt die nur am absoluten Nullpunkt wirklich funktioniert.

**Damit ist jetzt ein für alle Mal Schluss.**

**Create Your Adventure** ist ein Voxel-Spiel, dessen Welt **Billionen von Blöcken und Chunks** umfasst – und das nicht nur auf dem Papier. Die Welt funktioniert überall gleich präzise, egal wie weit du dich vom Ursprung entfernst.

---

### 🧠 Das Herzstück: Chunk-Metadaten

Chunks in CyA sind mehr als statische Render- und Save/Load-Einheiten. Sie sind das **Herz des Spiels**.

Jeder Chunk trägt **Metadaten** – ein dynamisches System, das selbst kleinste Veränderungen mit der gesamten Spielwelt verknüpft. Diese Architektur ermöglicht ein realistisches, lebendiges Spielerlebnis in einem Umfang, der bisher nicht existiert hat.

**Was Chunk-Metadaten ermöglichen:**

- 🌊 **Dynamische Flüsse** – Verändere das Gelände, und Flüsse passen ihren Lauf an
- 🌿 **Lebendige Biome** – Eingriffe in ein Biom können benachbarte Biome transformieren
- 🌦️ **Reaktives Wettersystem** – Weltveränderungen beeinflussen Klima und Wetter
- 🐾 **Tierverhalten & Migration** – Tiere verlassen veränderte Gebiete, neue Arten siedeln sich an
- 🔗 **Verkettete Konsequenzen** – Jede noch so kleine Aktion kann große Dinge heraufbeschwören

---

### 💡 Philosophie

In den meisten Spielen ist die Welt ein Sandkasten – du baust, die Welt reagiert nicht wirklich.

In **CyA** bist du **lebendig mit der Welt**. Du bist nicht der Spieler einer statischen Kulisse – du bist der Autor deiner eigenen Weltgeschichte. Deine Entscheidungen formen die Welt. Die Welt formt zurück.

---

### 🛠️ Tech Stack

| Technologie | Verwendung |
|---|---|
| C# / .NET | Hauptprogrammiersprache |
| Silk.NET | Low-Level Grafik & Fenster-API |
| ImGUI | Debug- & Entwickler-Interface |
| Eigene Engine | Keine externe Game-Engine – alles selbst gebaut |

---

### ⚙️ Technische Vision

| Feature | Beschreibung |
|---|---|
| Weltgröße | Billionen von Blöcken und Chunks |
| Koordinatenstabilität | Volle Funktionalität auf weiten Strecken vom Ursprung |
| Chunk-System | Metadaten-basiert, dynamisch und vernetzt |
| Simulation | Wetter, Ökosysteme, Hydrologie, Biome |
| Spielerprinzip | Aktiver Teil der Welt, nicht externer Beobachter |

---

### 🗺️ Roadmap

- [x] Projektkonzept & Architektur
- [ ] Chunk-System mit Metadaten
- [ ] Prozeduale Weltgenerierung
- [ ] Dynamisches Wettersystem
- [ ] Ökosystem & Tiermigration
- [ ] Alpha-Release

---

### 🚀 Getting Started

> Das Projekt befindet sich noch in früher Entwicklung. Eine spielbare Version ist noch nicht verfügbar.
> Sobald eine erste Version veröffentlicht wird, findest du hier die Installationsanleitung.

---

### 📬 Kontakt & Feedback

Du hast Ideen, Anmerkungen oder Fragen? Öffne gerne ein **Issue** in diesem Repository.
Bitte beachte: Direkte Code-Beiträge (Pull Requests) werden gemäß der Lizenz nicht angenommen.

---

### 📄 Lizenz

Dieses Projekt steht unter einer eigenen Lizenz. Siehe [LICENSE](LICENSE) für alle Details.
Kurz zusammengefasst: Einsehen & Lernen erlaubt – Bearbeiten & kommerzielle Nutzung ohne Genehmigung nicht.

---
---

## 🇬🇧 English

*Create Your Adventure – potentially one of the largest games in the world,
not measured by content, but by raw surface area. Through Long Integer-based
chunk coordinates, the world extends to a scale that surpasses anything
seen before.*

### What is CyA?

You're probably familiar with worlds that are static and lifeless. Voxel games that show their first flaws after just 500,000 blocks of world coordinates – jerky chunks, limited simulations, a world that only really works at absolute zero.

**That's now a thing of the past, once and for all.**

**Create Your Adventure** is a voxel game whose world comprises **trillions of blocks and chunks** – and not just on paper. The world functions with the same precision everywhere, no matter how far you stray from the origin.

---

### 🧠 The Heart of the Game: Chunk Metadata

Chunks in CyA are more than static render and save/load units. They are the **heart of the game**.

Each chunk carries **metadata** – a dynamic system that links even the smallest changes to the entire game world. This architecture enables a realistic, vibrant gaming experience on a scale that has never existed before.

**What chunk metadata enables:**

- 🌊 **Dynamic rivers** – Change the terrain, and rivers adjust their course
- 🌿 **Living biomes** – Interventions in one biome can transform neighboring biomes
- 🌦️ **Reactive weather system** – World changes affect climate and weather
- 🐾 **Animal behavior & migration** – Animals leave changed areas, new species settle in
- 🔗 **Chain reactions** – Every little action can conjure up big things

---

### 💡 Philosophy

In most games, the world is a sandbox – you build, the world doesn't really react.

In **CyA**, you are **alive with the world**. You are not a player in a static setting – you are the author of your own world history. Your decisions shape the world. The world shapes you back.

---

### 🛠️ Tech Stack

| Technology | Usage |
|---|---|
| C# / .NET | Primary programming language |
| Silk.NET | Low-level graphics & window API |
| ImGUI | Debug & developer interface |
| Custom Engine | No external game engine – built from scratch |

---

### ⚙️ Technical Vision

| Feature | Description |
|---|---|
| World size | Trillions of blocks and chunks |
| Coordinate stability | Full functionality over long distances from the origin |
| Chunk system | Metadata-based, dynamic, and networked |
| Simulation | Weather, ecosystems, hydrology, biomes |
| Player principle | Active part of the world, not an external observer |

---

### 🗺️ Roadmap

- [x] Project concept & architecture
- [ ] Chunk system with metadata
- [ ] Procedural world generation
- [ ] Dynamic weather system
- [ ] Ecosystem & animal migration
- [ ] Alpha release

---

### 🚀 Getting Started

> The project is still in early development. A playable version is not yet available.
> Once a first version is released, you will find installation instructions here.

---

### 📬 Contact & Feedback

Got ideas, feedback or questions? Feel free to open an **Issue** in this repository.
Please note: Direct code contributions (Pull Requests) are not accepted per the license terms.

---

### 📄 License

This project is licensed under a custom license. See [LICENSE](LICENSE) for full details.
Short summary: Viewing & learning is allowed – modifying & commercial use without permission is not.

---

*© 2026 Oliver Schmidt. All rights reserved.*
>>>>>>> 7e31e3b63a4f390660b9390537232913fccbaf6a
