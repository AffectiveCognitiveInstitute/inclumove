# incluMOVE Schnellstart

---

For the **english version** down below, please click [here](#english).

---

Hier gibt es grundsätzliche Informationen über Ersteinrichtung und Konfiguration der incluMOVE Software.
Die Software ist in zwei Versionen vorhanden. Beide sind als fertiges Release über die [GitHub Seite](https://github.com/AffectiveCognitiveInstitute/inclumove) herunterzuladen.

#### incluMOVE Light
Diese Version benötigt zusätzlich zum Installationsrechner in der Minimalkonfiguration nur einen Beamer. Hier gibt es dann keine automatisierte Überprüfung der Montageabläufe.
Eine vereinfachte Überprüfung kann über ein simples MQTT-Netzwerkprotokoll hinzugefügt werden. Eine Softwarelösung für die Überprüfung ist nicht enthalten.

#### incluMOVE Extended
Diese Version ist in Verbindung mit dem im Projekt entstandenen Gesamttischsystem vorgesehen. Es besteht eine feste Abhängigkeit zu mehreren Systemkomponenten die über MQTT miteinander kommunizieren.

## Ersteinrichtung
Um die incluMOVE Software für den Betrieb startklar zu machen müssen einige Werte in den Konfigurationsdateien angelegt werden.
In der Konfigurationsdatei müssen die korrekten Pfade für die Workflows und Workflow Assets gesetzt werden. Standardmäßig ist der Workflow Ordner mit Beispielen im Installationsordner beigefügt.

```json
...
"assetsUrl": "D://incluMOVE/workflows/workflow-files"
"workflowDirectory": "D://incluMOVE/workflows"
...
```

## Konfiguration
Hier gibts es eine Übersicht über den Aufbau des Installationsordners und der wichtigen Konfigurationsdateien.

### Installationsordner
Im gewählten Installationsordner befinden sich folgende Ordner und Dateien:

| Pfad                    | Beschreibung                              |
| -------------           |-------------                              |
| incluMOVE_Data/         | Anwendungsdaten                           |
| MonoBleedingEdge/       | Scriptbibliotheken                        |
| workflows/              | Standard Workflow Ordner                  |
| config.json             | incluMOVE Konfigurationsdatei             |
| incluMOVE.exe           | Anwendungs-executable                     |
| UnityCrashhandler64.exe | Absturzmanager                            |
| UnityPlayer.dll         | Unity-Engine Bibliothek                   |
| user.prf                | Beispiel Nutzerdatei                      |

### Konfigurationsdatei
Über diese Datei werden bestimmte Einstellungen der incluMOVE Software konfiguriert.

| Konfigurationswert        | Werte          | Beispiel / *Zulässige Werte*              | Funktion                                                                             |
| -------------             | -------------  | -------------                             | -------------                                                                        |
| webcamDevice              | Textstring     | "Logitech C920"                           | Name des Webcam-Device                                                               |
| resolutionWidth           | Ganzzahl       | 1280                                      | Webcam Auflösung Breite (Geräteabhängig)                                             |
| resolutionHeight          | Ganzzahl       | 720                                       | Webcam Auflösung Höhe (Geräteabhängig)                                               | 
| fps                       | Ganzzahl       | 30                                        | Webcam Bildwiederholrate (Geräteabhängig)                                            |
| assetsUrl                 | Textstring     | "D://incluMove/workflows/workflow-files"  | Absoluter Pfad zum Standardassetordner                                               |
| workflowDirectory         | Textstring     | "D://incluMove/workflows"                 | Absoluter Pfad zum Workflowordner                                                    |
| mode                      | Textstring     | *"kiosk", "guest"*                        | Betriebsmodus (kiosk = Standard, guest = Modus ohne Nutzerprofile)                   |
| UseUsbProfile             | boolean        | *true, false*                             | Nutzerprofile über USB-Stick (Nutzt ansonsten user.prf Datei im Installationsordner) |
| SkipIntro                 | boolean        | *true, false*                             | Springt direkt in Workflow ohne Begrüßung                                            |
| adaptivityTimeThreshold   | Gleitkommazahl | 30.75                                     | Mindestdauer zwischen Chatbot-Anfragen zur Änderung der Adaptivität in Sekunden      |
| adaptivityRepeatThreshold | Ganzzahl       | 2                                         | Mindestschritte über dem Grenzwert bevor eine Adaptivitäts-Änderung vorgschlagen wird|
| standardWorkflow          | Textstring     | "example.work"                            | Dateiname des Worfklows der bei neuen Nutzern standardmäßig hinzugefügt wird         |

Um die Konfigurationsdatei zurückzusetzen kann sie einfach aus dem Ordner gelöscht werden. Die Software generiert dann automatisch eine neue Datei.

### Nutzerdateien
Nutzerdateien speichern den Arbeitsverlauf von Nutzern und legen fest welchen Workflow sie bearbeiten können.

| Konfigurationswert | Werte            | Beispiel / *Zulässige Werte* | Funktion                                        |
| -------------      | -------------    | -------------                | -------------                                   |
| name               | Textstring       | "Musterperson"               | Anzeigename des Nutzers                         |
| profilePicture     | Bytearray        | [140,138,142,255,...]        | Profilbilddaten                                 |
| isAdmin            | boolean          | *true, false*                | Adminutzer (Schaltet Workfloweditor frei)       | 
| workflows          | Textstring Array | ["example.work"]             | Workflows auf die der Nutzer Zugriff hat        |
| badgeHistory       | Verlaufsdaten    | --------------               | Verlaufsdaten des Nutzers                       |
| adaptivityLevel    | Ganzahl          | *0,1,2*                      | Letzter gewählter Adaptivitätslevel des Nutzers |
| selectedReward     | Textstring       | "Balloons"                   | Letzte gewählte Belohnung des Nutzers           |




_________________________________________________________________________________________________________________

<a name="english"></a>
# incluMOVE Quick start

Here you will find basic information about the initial setup and configuration of the incluMOVE software.
The software is available in two versions. Both can be downloaded as a finished release via the [GitHub page](https://github.com/AffectiveCognitiveInstitute/inclumove).

#### incluMOVE Light
This version requires only a beamer in addition to the installation computer in the minimum configuration. There is no automated verification of the installation processes then.
A simplified check can be added via a simple MQTT network protocol. A software solution for the check is not included.

#### incluMOVE Extended
This version is intended to be used in conjunction with the overall table system created in the project. There is a fixed dependency on several system components that communicate with each other via MQTT.

## Initial setup
In order to make the incluMOVE software ready for operation, some values must be created in the configuration files.
The correct paths for the workflows and workflow assets must be set in the configuration file. As standard, the workflow folder with examples is included in the installation folder.

```json
...
"assetsUrl": "D://incluMOVE/workflows/workflow-files"
"workflowDirectory": "D://incluMOVE/workflows"
...
```

## Configuration
Here is an overview of the structure of the installation folder and the important configuration files.

### Installation folder
The following folders and files are located in the selected installation folder:

| Path                    | Description                               |
| -------------           |-------------                              |
| incluMOVE_Data/         | Application data                          |
| MonoBleedingEdge/       | Script libraries                          |
| workflows/              | Standard Workflow folder                  |
| config.json             | incluMOVE Configuration file              |
| incluMOVE.exe           | Application executable                    |
| UnityCrashhandler64.exe | Crash manager                             |
| UnityPlayer.dll         | Unity-Engine library                      |
| user.prf                | Example user file                         |

### Configuration file
This file is used to configure certain settings of the incluMOVE software.

| Configuration Value       | Values         | Example / *Allowed Values*               | Function                                                                          |
| -------------             | -------------  | -------------                            | -------------                                                                     |
| webcamDevice              | text string    | "Logitech C920"                          | Name of webcam device                                                             |
| resolutionWidth           | integer        | 1280                                     | Webcam resolution width (device dependent)                                        |
| resolutionHeight          | integer        | 720                                      | Webcam resolution height (device-dependent)                                       |
| fps                       | integer        | 30                                       | Webcam refresh rate (device-dependent)                                            | 
| assetsUrl                 | text string    | "D://incluMove/workflows/workflow-files" | Absolute path to default asset folder                                             |
| workflowDirectory         | text string    | "D://incluMove/workflows"                | Absolute path to the workflow folder                                              |
| mode                      | text string    | *"kiosk", "guest "*                      | Operating mode (kiosk = default, guest = mode without user profiles)              |
| UseUsbProfile             | boolean        | *true, false*                            | User profiles via USB stick (otherwise uses user.prf file in installation folder) |
| SkipIntro                 | boolean        | *true, false*                            | Jumps directly into workflow without greeting                                     |
| adaptivityTimeThreshold   | floating point | 30.75                                    | Minimum time between chatbot requests to change adaptivity in seconds             |
| adaptivityRepeatThreshold | integer        | 2                                        | Minimum steps above threshold before suggesting an adaptivity change              |
| standardWorkflow          | text string    | "example.work"                           | filename of the worfklow that is added by default for new users                   |

To reset the configuration file, simply delete it from the folder. The software then automatically generates a new file.

### User files
User files store the work history of users and define which workflow they can process.

| Configuration Value | Values            | Example / *Allowed Values*  | Function                                        |
| -------------       | -------------     | -------------               | -------------                                   |
| name                | text string       | "sample person"             | display name of user                            |
| profilePicture      | byte array        | [140,138,142,255,...]       | profile picture data                            |
| isAdmin             | boolean           | *true, false*               | admin user (enables workflow editor)            | 
| workflows           | text string array | ["example.work"]            | Workflows to which the user has access          |
| badgeHistory        | history data      | --------------              | user history data                               |
| adaptivityLevel     | integer           | *0,1,2*                     | Last selected adaptivity level of the user      |
| selectedReward      | text string       | "Balloons"                  | Last selected reward of the user                |
