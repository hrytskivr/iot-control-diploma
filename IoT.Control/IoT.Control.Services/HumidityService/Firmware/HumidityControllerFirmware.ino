#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <DHT.h>
#include <ArduinoJson.h>

// sensor config
#define SENSOR_PIN D7
#define SENSOR_TYPE DHT11

// WiFi config
#define NETWORK_NAME ""
#define NETWORK_PASS ""

#define HUMIDITY_UPDATED_ENDPOINT ""
#define AWS_FINGERPRINT ""

// initialize dht sensor & http client
DHT dht(SENSOR_PIN, SENSOR_TYPE);
HTTPClient http;

// global variables
int humidifierState = 0;

void setup()
{
  Serial.begin(115200); // TODO: replace with AWS logs

  // set-up sensor
  pinMode(SENSOR_PIN, INPUT);
  dht.begin();
  
  // set-up humidifier
  pinMode(15, OUTPUT); // живлення резонатора D8
  pinMode(12, OUTPUT); // вкл. резонатора D6
  pinMode(2, OUTPUT);  // flashlight

  digitalWrite(15, HIGH);
  delay(4000);
  digitalWrite(12, HIGH);

  // set-up WiFi
  Serial.print("Establishing network connection");
  WiFi.begin(NETWORK_NAME, NETWORK_PASS);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }
  
  Serial.println("\nWorking!");
}

void loop()
{
  delay(3000);
  
  Serial.println("Getting humidity information...");
  float currentHumidityLevel = dht.readHumidity();
  String currentHumidifierState = Get_Current_Humidifier_State();

  Serial.println("Sending request...");
  String requestBody = Create_Humidity_Updated_Request_Body(currentHumidityLevel, currentHumidifierState);
  int responseCode = Submit_Humidity_Updated_Request(requestBody);
  if (responseCode == 202)
  {
    Toggle_Humidifier();
  }
  
  Serial.println(responseCode);
}

int Submit_Humidity_Updated_Request(String requestBody)
{
  int responseCode;
  
  http.begin(HUMIDITY_UPDATED_ENDPOINT, AWS_FINGERPRINT);
  http.addHeader("Content-Type", "application/json");

  responseCode = http.POST(requestBody);
  http.end();

  return responseCode;
}

String Create_Humidity_Updated_Request_Body(float currentHumidityLevel, String currentHumidifierState)
{
  String response;

  StaticJsonDocument<100> json;
  json["current_humidity_level"] = currentHumidityLevel/100;
  json["current_humidifier_state"] = currentHumidifierState;
  serializeJson(json, response);

  return response;
}

String Get_Current_Humidifier_State()
{
  if (humidifierState == 1)
  {
    return "enabled";
  }
  else if (humidifierState == 0)
  {
    return "disabled";
  }
}

void Toggle_Humidifier()
{
  if (humidifierState == 0)
  {
    digitalWrite(12, LOW);
    delay(100);
    digitalWrite(12, HIGH);

    humidifierState = 1;
    digitalWrite(2, LOW); // flashlight off
  }
  else if (humidifierState == 1)
  {
    digitalWrite(12, LOW);
    delay(100);
    digitalWrite(12, HIGH);
    delay(100);
    digitalWrite(12, LOW);
    delay(100);
    digitalWrite(12, HIGH);

    humidifierState = 0;
    digitalWrite(2, HIGH); // flashlight on
  }
}
