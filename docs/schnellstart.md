# incluMOVE Schnellstart
Hier gibt es grundsätzliche Informationen über Ersteinrichtung und Konfiguration der incluMOVE Software.
Die Software ist in zwei Versionen vorhanden. Beide sind als fertiges Release über die [GitHub Seite](https://github.com/AffectiveCognitiveInstitute/inclumove) herunterzuladen.

#### incluMOVE Light
Diese Version benötigt zusätzlich zum Installationsrechner in der Minimalkonfiguration nur einen Beamer. Hier gibt es dann keine automatisierte Überprüfung der Montageabläufe.
Eine vereinfachte Überprüfung kann über ein simples MQTT-Netzwerkprotokoll hinzugefügt werden. Eine Softwarelösung für die Überprüfung ist nicht enthalten.

#### incluMOVE Extended
Diese Version ist in Verbindung mit dem im Projekt entstandenen Gesamttischsystem vorgesehen. Es besteht eine feste Abhängigkeit zu mehreren Systemkomponenten die Über MQTT miteinander kommunizieren.

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
Hier gibts es eine Übersicht über Aufbau des Installationsordner und der wichtigen Konfigurationsdateien.

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
Über die diese Datei werden bestimmte Einstellungen der incluMOVE Software konfiguriert.

| Konfigurationswert        | Werte          | Beispiel/ *Zulässige Werte*              | Funktion                                                                              |
| -------------             | -------------  | -------------                            | -------------                                                                         |
| webcamDevice              | Textstring     | "Logitech C920"                          | Name des Webcam-Device                                                                |
| resolutionWidth           | Ganzzahl       | 1280                                     | Webcam Auflösung Breite (Geräteabhängig)                                              |
| resolutionHeight          | Ganzzahl       | 720                                      | Webcam Auflösung Höhe (Geräteabhängig)                                                | 
| fps                       | Ganzzahl       | 30                                       | Webcam Bildwiederholrate (Geräteabhängig)                                             |
| assetsUrl                 | Textstring     | "D://incluMove/workflows/workflow-files" | Absoluter Pfad zum Standardassetordner                                                |
| workflowDirectory         | Textstring     | "D://incluMove/workflows"                | Absoluter Pfad zum Workflowordner                                                     |
| mode                      | Textstring     | *"kiosk", "guest"*                       | Betriebsmodus (kiosk = Standard, guest = Modus ohne Nutzerprofile)                    |
| UseUsbProfile             | boolean        | *true, false*                            | Nutzerprofile über USB-Stick (Nutzt ansonsten user.prf Datei im Installationsordner)  |
| SkipIntro                 | boolean        | *true, false*                            | Springt direkt in Workflow ohne Begrüßung                                             |
| adaptivityTimeThreshold   | Gleitkommazahl | 30.75                                    | Mindestdauer zwischen Chatbot-Anfragen zur Änderung der Adaptivität in Sekunden       |
| adaptivityRepeatThreshold | Ganzzahl       | 2                                        | Mindestschritte über dem Grenzwert bevor eine Adaptivitäts-Änderung vorgschlagen wird |
| standardWorkflow          | Textstring     | "example.work"                           | Dateiname des Worfklows der bei neuen Nutzern standardmäßig hinzugefügt wird          |

Um die Konfigurationsdatei zurückzusetzen kann sie einfach aus dem Ordner gelöscht werden. Die Software generiert dann automatisch eine neue Datei.

### Nutzerdateien
Nutzerdateien speichern den Arbeitsverlauf von Nutzern und legen fest welchen Workflow sie bearbeiten können.

| Konfigurationswert | Werte            | Beispiel/ *Zulässige Werte* | Funktion                                        |
| -------------      | -------------    | -------------               | -------------                                   |
| name               | Textstring       | "Musterperson"              | Anzeigename des Nutzers                         |
| profilePicture     | Bytearray        | [140,138,142,255,...]       | Profilbilddaten                                 |
| isAdmin            | boolean          | *true, false*               | Adminutzer (Schaltet Workfloweditor frei)       | 
| workflows          | Textstring Array | ["example.work"]            | Workflows auf die der Nutzer Zugriff hat        |
| badgeHistory       | Verlaufsdaten    | --------------              | Verlaufsdaten des Nutzers                       |
| adaptivityLevel    | Ganzahl          | *0,1,2*                     | Letzter gewählter Adaptivitätslevel des Nutzers |
| selectedReward     | Textstring       | "Balloons"                  | Letzte gewählte Belohnung des Nutzers           |