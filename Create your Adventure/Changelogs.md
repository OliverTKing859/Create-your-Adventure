ChangeLogs

# Alpha Version

## 0.0.0.0 Alpha | Projekt Start - 10.01.2026
- Projekt start
- Repository erstellt

## 0.0.0.1 Alpha | Hello World - 20.01.2026
- Hello World

## 0.0.1.0 Alpha | Create Window - 20.01.2026
- Erstelle ein Fenster in C# mit Silk.NET auf OpenGL Basis
	- Fenster ist auf HD Auflösung eingestellt
	- Nutzt 4.6 OpenGL
	- Hat vier Methoden
		- OnLoad
		- OnRender
		- OnUpdate
		- OnClose
	- aktuelle Hintergrundfarbe ist fast Schwarz (RGB)

## 0.0.1.1 Alpha | KHR Debug - 24.01.2026
- Erlaube es Unsafe Code zu nutzen
	- Unsafe OnLoad
- Erstellt ein KHR Debug Fenster
	- Debug Nachrichten werden in der Konsole ausgegeben

## 0.0.2.0 Alpha | Create Triangle - 30.01.2026
- Erstellt ein Dreieck
	- Nutzt VBO, VAO und EBO
	- Nutzt Shader (Vertex & Fragment)
	- Nutzt GLSL 4.6
	- Nutzt Farben für das Dreieck (RGB)

## 0.0.3.0 Alpha | Create Cube - 26.01.2026
- Erstellt einen Würfel
	- Nutzt VBO, VAO und EBO
	- Nutzt Shader (Vertex & Fragment)
	- Nutzt GLSL 4.6
	- Nutzt Farben für den Würfel (RGB)

## 0.0.4.0 Alpha | Create Camera - 26.01.2026
- Erstellt eine Kamera
	- Nutzt Matrix Transformationen (Model, View, Projection)
	- Nutzt Yaw, Pitch

## 0.0.4.1 Alpha | Camera Controls - 27.01.2026
- Erstellt Kamera Steuerung
	- Nutzt Maus (Gyro) und Tastatur (WASD, Space, Left Shift) Eingaben
	- Nutzt Delta Time für Bewegung
	- Pitch begrenzt auf -89 bis 89 Grad

## 0.0.4.2 Alpha | Acceleration / Deceleration, Smooth - 29.01.2026
- Erstellt eine Beschleunigung / Verzögerung für die Kamera Bewegung
- Nutzt Smooth Mouse Movement
- Erhöht die Max Speed der Kamera

## 0.0.4.3 Alpha |  - 29.01.2026 Mouse Sensitivity / Smoothing Factor
- Nutzt Mouse Sensitivity
- Nutzt Mouse Smoothing Faktor
- Ändert Left Shift zu Left STRG

## 0.0.4.4 Alpha | Revision of camera functions - 30.01.2026

- Ändert Acceleration / Deceleration zu eine horizontale Achse und eine vertikale Achse
- Nutzt aufbauende Geschwindigkeit und abbauende Geschwindigkeit
- Überarbeitung von Smoothing Factor

## 0.0.4.5 Alpha | Fine-tuning the camera - 30.01.2026

- Überarbeitung der Variablen Namen
- Code Optimierung
- Vorbereitung auf Abstraktion für der Kamera Klasse

## 0.0.4.6 Alpha | Smoothing reworked again - 30.01.2026

## 0.0.5.0 Alpha | Abtraction Camera To Camera Class - 01.02.2026

- Abstraktion der Kamera in eine eigene Kamera Klasse

## 0.0.5.1 Alpha | Added Commentary Improvements - 01.02.2026