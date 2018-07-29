#define Dg 2  //green
#define Dy 3  //yellow
#define Dr 4  //red
#define Dpwm 5  //PWM

void setup() {
  Serial.begin(9600);
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(Dg, INPUT);
  pinMode(Dy, INPUT);
  pinMode(Dr, INPUT);
  pinMode(Dpwm, OUTPUT);
}

void loop() {
  digitalWrite(LED_BUILTIN, LOW);   
  delay(1000);                      
  digitalWrite(LED_BUILTIN, HIGH);  
  delay(1000); 
  
  inputData();
}

void inputData(){
  bool readG = digitalRead(Dg);
  bool readY = digitalRead(Dy);
  bool readR = digitalRead(Dr);
  //Serial.println(rG);
  setPWM(8);
  }

void setPWM(byte range){
  analogWrite(Dpwm, range);
  }


