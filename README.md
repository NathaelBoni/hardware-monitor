# Hardware Monitor

Project to monitor hardware sensors

## What will be needed

- Arduino Nano
- LCD Display 20x4
- I2C Module

## Arduino Setup

- Using your Arduino, execute the `I2CScanner.ino` program, check the Serial Output. The hex code will be necessary for the monitor program.
- Edit `HardwareMonitor.ino`, setting the `scannerCode` variable the value from the last step.
- Upload `HardwareMonitor.ino` to your Arduino.
- Schematics:

![schamatics](https://github.com/NathaelBoni/hardware-monitor/blob/main/circuit.png)

## Installing the service

- Download the latest release, the service is in `MonitorService` folder.
- Using cmd, run:
```
sc create HardwareMonitor binPath= full\path\to\folder\MonitorService.exe start= auto
```

## Uninstalling
- To stop the service:
```
sc stop HardwareMonitor
```
- To uninstall:
```
sc delete HardwareMonitor
```
