using System;

namespace main
{
	public class OtherProj : Libs
	{
		private class MainC : IInitialize
		{
			const int Dg = 3; //green
			const int Dy = 4; //yellow
			const int Dr = 5; //red
			long _previousMillis;
			long _interval = 1000;
			byte _pwmValue = 125;
			byte _pwmInterval = 5;
			const byte PwmMax = 255;
			const byte PwmMin = 0;

			const int ReedPin = 2;
			const int FanPin = 9;
			volatile float _pulses = 0;
			long _lastRpMmillis = 0;

			void InputData()
			{
				bool readG = digitalRead(Dg);
//				bool readY = digitalRead(Dy);
//				bool readR = digitalRead(Dr);
				delay(520);
				Serial.println("Pin Dg -");
				delay(120);
				Serial.println(readG);
			}

			void CountPulse()
			{
				// just count each pulse we see
				// ISRs should be short, not like
				// these comments, which are long.
				_pulses++;
			}

			void IInitialize.setup()
			{
				TCCR1B = TCCR1B & 0b11111000 | 0x01;
				TCCR0B = TCCR0B & 0b11111000 | 0x03; //TCCR0B = TCCR0B & 0b11111000 |0x01;
				Serial.begin(9600);
				pinMode(ReedPin, INPUT_PULLUP);
				attachInterrupt(digitalPinToInterrupt(ReedPin), CountPulse, FALLING);
				pinMode(FanPin, OUTPUT);
				pinMode(Dg, INPUT);
			}

			float CalculateRpm()
			{
				noInterrupts();
				float elapsedMs = (millis() - _lastRpMmillis) / 1000.0f;
				float revolutions = _pulses / 2;
				float revPerMs = revolutions / elapsedMs;
				float rpm = revPerMs * 60.0f;
				_lastRpMmillis = millis();
				_pulses = 0;
				interrupts();
				/*Serial.print(F("elpasedMS = ")); Serial.println(elapsedMS);
				Serial.print(F("revolutions = ")); Serial.println(revolutions);
				Serial.print(F("revPerMS = ")); Serial.println(revPerMS); */
				return rpm;
			}

			void IInitialize.loop()
			{
				HandleSerial();
				analogWrite(FanPin, _pwmValue);
				if (millis() - _previousMillis > _interval)
				{
					Serial.print("RPM=");
					Serial.print(CalculateRpm());
					Serial.print(F(" @ PWM="));
					Serial.println(_pwmValue);
					_previousMillis = millis();

					InputData();
				}
			}

			void HandleSerial()
			{
				bool printValue = false;
				while (Serial.available())
				{
					switch (Serial.read())
					{
						case '+':
							_pwmValue = (byte) (_pwmValue + _pwmInterval);
							printValue = true;
							break;

						case '-':
							_pwmValue = (byte) (_pwmValue - _pwmInterval);
							printValue = true;
							break;

						case '!':
							_pwmValue = PwmMax;
							printValue = true;
							break;

						case '=':
							_pwmValue = 125;
							printValue = true;
							break;

						case '0':
						case '@':
							_pwmValue = PwmMin;
							printValue = true;
							break;

						case '?':
							Serial.println(F("+ Increase"));
							Serial.println(F("- Decrease"));
							Serial.print(F("! PWM to "));
							Serial.println(PwmMax);
							Serial.print(F("@ PWM to "));
							Serial.println(PwmMin);
							Serial.println(F("= PWM to 125"));
							printValue = true;
							break;
					}
				}

				if (printValue)
				{
					Serial.print(F("Current PWM = "));
					Serial.println(_pwmValue);
					Serial.flush();
				}
			}
		}
	}
}