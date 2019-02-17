using System;

namespace main
{
	public partial class Libs
	{
		protected static void analogWrite(int pin, byte value){}
		protected static bool digitalRead(int pin) => true;
		protected static void delay(int ms){}
		protected static int TCCR1B, TCCR0B;
		protected static void pinMode(int pin, byte constant){}
		protected static readonly byte INPUT_PULLUP = 0, INPUT = 1, OUTPUT = 2;
		protected static void attachInterrupt(int pin, Action action, byte mode){}
		protected static readonly byte LOW = 0, CHANGE = 1, RISING = 2, FALLING = 3;
		protected static int digitalPinToInterrupt(int pin) => 0;
		protected static void noInterrupts(){}
		protected static void interrupts(){}
		protected static long millis() => 0L;
		protected static string F(string msg) => String.Empty;
		
		protected struct Serial
		{
			public static void println(object msg){}
			public static void print(object msg){}
			public static void begin(int boud){}
			public static bool available() => true;
			public static char read() => Char.MinValue;
			public static void flush(){}
		}

		protected interface IInitialize
		{
			void setup();
			void loop();
		}
	}
}