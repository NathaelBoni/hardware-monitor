#include <Wire.h>

void setup() {
  Serial.begin (9600);
  Serial.println ();
  Serial.println ("I2C scanner. Scanning ...");
  byte count = 0;
  
  Wire.begin();
  for (byte i = 8; i < 120; i++) {
    Wire.beginTransmission (i);
    if (Wire.endTransmission () == 0) {
      Serial.print ("Address: ");
      Serial.print ("0x");
      Serial.print (i, HEX);
      count++;
      delay (1);
    }
  }
}

void loop() {}
