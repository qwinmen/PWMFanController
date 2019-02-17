#define Dg 3  //green
#define Dy 4  //yellow
#define Dr 5  //red

unsigned long previousRPMMillis;
unsigned long previousMillis;
float RPM;
 
unsigned long interval = 1000;
byte pwmValue = 125;
byte pwmInterval = 5;
const byte pwmMax = 255;
const byte pwmMin = 0;
 
const int reedPin = 2;
const int fanPin = 9;
 
volatile unsigned long pulses=0;
unsigned long lastRPMmillis = 0;

void inputData(){
  bool readG = digitalRead(Dg);
  bool readY = digitalRead(Dy);
  bool readR = digitalRead(Dr);
  delay(520);
  Serial.println("Pin Dg -");
  delay(120);
  Serial.println(readG);
  } 
 
void countPulse() {
  // just count each pulse we see
  // ISRs should be short, not like
  // these comments, which are long.
  pulses++;
}
 
void setup() {
  TCCR1B = TCCR1B & 0b11111000 |0x01;
  TCCR0B = TCCR0B & 0b11111000 |0x03;//TCCR0B = TCCR0B & 0b11111000 |0x01;
  Serial.begin(9600);
  pinMode(reedPin,INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(reedPin), countPulse, FALLING);
  pinMode(fanPin, OUTPUT);
  pinMode(Dg, INPUT);
}
 
unsigned long calculateRPM() {
  unsigned long RPM;
  noInterrupts();
  float elapsedMS = (millis() - lastRPMmillis)/1000.0;
  unsigned long revolutions = pulses/2;
  float revPerMS = revolutions / elapsedMS;
  RPM = revPerMS * 60.0;
  lastRPMmillis = millis();
  pulses=0;
  interrupts();
  /*Serial.print(F("elpasedMS = ")); Serial.println(elapsedMS);
  Serial.print(F("revolutions = ")); Serial.println(revolutions);
  Serial.print(F("revPerMS = ")); Serial.println(revPerMS); */
  return RPM;
}
 
void loop() {
  handleSerial();
  analogWrite(fanPin, pwmValue);
   
  if (millis() - previousMillis > interval) {
    Serial.print("RPM=");
    Serial.print(calculateRPM());
    Serial.print(F(" @ PWM="));
    Serial.println(pwmValue);
    previousMillis = millis();  
	
	inputData();
  }
}
 
void handleSerial() {
boolean printValue = false;
  while(Serial.available()) {
    switch (Serial.read()) {
      case '+':
        pwmValue = pwmValue+pwmInterval;
        printValue = true;
      break;      
       
      case '-':
        pwmValue = pwmValue-pwmInterval;
        printValue = true;
      break;
   
      case '!':
        pwmValue = pwmMax;
        printValue = true;
      break;
 
      case '=':
        pwmValue = 125;
        printValue = true;
     break;
 
      case '0':
      case '@':
        pwmValue = pwmMin;
        printValue = true;
      break;
 
      case '?':
        Serial.println(F("+ Increase"));
        Serial.println(F("- Decrease"));
        Serial.print(F("! PWM to ")); Serial.println(pwmMax);
        Serial.print(F("@ PWM to ")); Serial.println(pwmMin);
        Serial.println(F("= PWM to 125"));
        printValue = true;
      break;
    }
  }
  if (printValue) {
    Serial.print(F("Current PWM = "));
    Serial.println(pwmValue);
    Serial.flush();
  }
}
