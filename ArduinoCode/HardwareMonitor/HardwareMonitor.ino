#include <Arduino_JSON.h>
#include <LiquidCrystal_I2C.h>

//Display connection
//SDA -> A4
//SLC -> A5

byte scannerCode = 0x27;
LiquidCrystal_I2C lcd(scannerCode, 20, 4);
unsigned long lastUpdate;
bool isDisplayOn;

void setup() {
  Serial.begin(9600);
  while (!Serial);

  lcd.init();
  lcd.backlight();
  isDisplayOn = true;

  byte celsius[8] = { B10000, B00111, B00100, B00100, B00100, B00100, B00100, B00111 };
  byte fan[8] = { B00000, B10011, B11010, B00100, B01011, B11001, B00000, B00000 };
  lcd.createChar(0, celsius);
  lcd.createChar(1, fan);
}

void loop() {
  if (!isSerialAvailable()) {
    if (millis() - lastUpdate > 5000)
      displayTurnOff();
    return;
  }

  lastUpdate = millis();
  JSONVar sensorObject = readFromSerial();
  writeToLCD(sensorObject);
}

String getValue(JSONVar object, char sensorName[]){
  if (object.hasOwnProperty(sensorName)) {
    if (sensorName == "MemoryUsage"){
      return String((double)object[sensorName]);
    }else{
      return String((int)object[sensorName]);
    }
  }
  return "";
}

bool isSerialAvailable(){
  return Serial.available() > 0;
}

JSONVar readFromSerial(){
  String serialMessage = Serial.readStringUntil('\n');
  JSONVar sensorObject = JSON.parse(serialMessage);
  return sensorObject;
}

void writeToLCD(JSONVar sensorObject){
  displayTurnOn();

  if (JSON.typeof(sensorObject) == "undefined") {
    lcd.setCursor(0, 0);
    lcd.print("Invalid message");
    return;
  }

  String cpuLoad = getValue(sensorObject, "CPULoad");
  String cpuTemp = getValue(sensorObject, "CPUTemp");
  String cpuFanSpeed = getValue(sensorObject, "CPUFanSpeed");
  String gpuLoad = getValue(sensorObject, "GPULoad");
  String gpuTemp = getValue(sensorObject, "GPUTemp");
  String gpuHotSpotTemp = getValue(sensorObject, "GPUHotSpotTemp");
  String gpuFanSpeed = getValue(sensorObject, "GPUFanSpeed");
  String memoryUsage = getValue(sensorObject, "MemoryUsage");
  
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("CPU " + cpuLoad + "% " + cpuTemp);
  lcd.write(0);
  lcd.print(" ");
  lcd.write(1);
  lcd.print(cpuFanSpeed);
  lcd.setCursor(0, 1);
  lcd.print("GPU " + gpuLoad + "% ");
  lcd.write(1);
  lcd.print(gpuFanSpeed);
  lcd.setCursor(0, 2);
  lcd.print("GPU " + gpuTemp);
  lcd.write(0);
  lcd.print(" " + gpuHotSpotTemp);
  lcd.write(0);
  lcd.setCursor(0, 3);
  lcd.print("RAM " + memoryUsage + " Gb");
}

void displayTurnOff(){
  if (!isDisplayOn)
    return;

  lcd.noDisplay();
  lcd.noBacklight();
  isDisplayOn = false;
}

void displayTurnOn(){
  if (isDisplayOn)
    return;

  lcd.display();
  lcd.backlight();
  isDisplayOn = true;
}
