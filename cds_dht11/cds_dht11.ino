#include "DHT.h"

#define DHTPIN 2  
#define DHTTYPE DHT11
DHT dht(DHTPIN, DHTTYPE);
int A = 8;
int B = 9;
int led = 4;
int led_status = 0;
int motor_status = 0;
int cds;
float h;
float t;

void setup() {

  dht.begin();
  pinMode(A,OUTPUT);
  pinMode(B,OUTPUT);
  pinMode(led,OUTPUT);

  Serial.begin(9600);
  Serial.end();
  Serial.begin(9600);
}

void loop() {
	motor_status = 0;
  led_status = 0; 
  cds = analogRead(A0); 
  h = dht.readHumidity();
  t = dht.readTemperature();

  if(t >= 30.0)   // 온도가 25도보다 높으면 선풍기 켜기
  {
    motor_status = 1;
    
    digitalWrite(A,LOW); //팬모터가 A방향으로 회전
    digitalWrite(B,HIGH);
    delay(1000); //1초 후
  
    digitalWrite(A,HIGH); // 전체 정지
    digitalWrite(B,HIGH);
    delay(2000); //2초 후    
  }

  if(cds <= 500)  // 조도센서값이 800이하면 Led 켜기
  {
    led_status = 1; 

    digitalWrite(led,HIGH);
    delay(5000);
    digitalWrite(led,LOW);
  }

  if(cds <= 1023 && t >=0 && h >=0 && led_status >=0 && motor_status >= 0)
  {
    Serial.println(String(cds) + "/" + String(h) + "/" + String(t) + "/" + String(led_status)+ "/" + String(motor_status) );
    delay(1000);

  }
  

}



