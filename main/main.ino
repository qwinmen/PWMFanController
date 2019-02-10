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

volatile unsigned long pulses = 0;
unsigned long lastRPMmillis = 0;

// Режимы индикации бесперебойного блока питания:
// Все индикаторы выключены, АКБ не задействован, обороты вентилятора на минимум.
// green - индикатор постоянно горит, стандартный режим работы, понижаем обороты вентилятора.
// green - индикатор мигает, зарядка АКБ, обороты вентилятора на максимум.
// yellow - индикатор постоянно горит, работа от АКБ, обороты вентилятора на максимум.
// --yellow - индикатор мигает раз в 500 мс, работа от АКБ (low_batt).
// --yellow - индикатор мигает раз в 1000 мс, АКБ разряжен.

long prevMillis = 0;   // здесь будет храниться время последнего изменения состояния светодиода
long intrvMillis = 5000;      // интервал мигания в миллисекундах
int blinkG_On_cnt = 0;
int blinkG_Off_cnt = 0;
bool IsBlinkingG = false;

void inputData() {
  bool readG = digitalRead(Dg);
  bool readY = digitalRead(Dy);
  bool readR = digitalRead(Dr);
  delay(520);
  Serial.print("Pin Dg -");
  delay(120);
  Serial.println(readG);
  if (IsBlinkingG == false && readG == false && readY == false && readR == false)
  {
    Serial.println("֍۞ Все индикаторы выключены, АКБ не задействован, обороты вентилятора на минимум.");
    handleSerial('=');
  }
  if (IsBlinkingG == false && readG == true && readY == false && readR == false)
  {
    Serial.println("֍ green - индикатор постоянно горит, стандартный режим работы, понижаем обороты вентилятора.");
    handleSerial('=');
  }
  else if (readG == false && readY == true && readR == false)
  {
    Serial.println("۞ yellow - индикатор постоянно горит, работа от АКБ, обороты вентилятора на максимум.");
    handleSerial('!');
  }
//--определить мигание:
  if (readG == false && readY == false && readR == false)
  {
    blinkG_Off_cnt++;
  }
  if (readG == true && readY == false && readR == false)
  {
    blinkG_On_cnt++;
  }
  if (millis() - prevMillis > intrvMillis)
  {
    prevMillis = millis();   // запоминаем текущее время
    long countBlnkPerIntrvl = intrvMillis/1000;
    if(blinkG_Off_cnt < countBlnkPerIntrvl && blinkG_On_cnt < countBlnkPerIntrvl)
    {
      Serial.println("֍ green - индикатор мигает, зарядка АКБ, обороты вентилятора на максимум.");
      handleSerial('!');
      IsBlinkingG = true;
    }else
    {
      IsBlinkingG = false;
      Serial.println("IsBlinkingG == false.");
    }
    Serial.println(blinkG_Off_cnt);
    Serial.println(blinkG_On_cnt);

    blinkG_Off_cnt = 0;
    blinkG_On_cnt = 0;
  }
}

void countPulse() {
  // just count each pulse we see
  // ISRs should be short, not like
  // these comments, which are long.
  pulses++;
}

void setup() {
  TCCR1B = TCCR1B & 0b11111000 | 0x01;
  TCCR0B = TCCR0B & 0b11111000 | 0x03; //TCCR0B = TCCR0B & 0b11111000 |0x01;
  Serial.begin(9600);
  pinMode(reedPin, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(reedPin), countPulse, FALLING);
  pinMode(fanPin, OUTPUT);
  pinMode(Dg, INPUT);
}

unsigned long calculateRPM() {
  unsigned long RPM;
  noInterrupts();
  float elapsedMS = (millis() - lastRPMmillis) / 1000.0f;
  unsigned long revolutions = pulses / 2;
  float revPerMS = revolutions / elapsedMS;
  RPM = revPerMS * 60.0;
  lastRPMmillis = millis();
  pulses = 0;
  interrupts();
  /*Serial.print(F("elpasedMS = ")); Serial.println(elapsedMS);
    Serial.print(F("revolutions = ")); Serial.println(revolutions);
    Serial.print(F("revPerMS = ")); Serial.println(revPerMS); */
  return RPM;
}

void loop() {
  //handleSerial('=');
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

void handleSerial(char arg) {
  boolean printValue = false;
  //while (Serial.available()) 
  {
    switch (arg) {
      case '+':
        pwmValue = (byte)(pwmValue + pwmInterval);
        printValue = true;
        break;

      case '-':
        pwmValue = (byte)(pwmValue - pwmInterval);
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
