# iot-control-diploma

In this repository you will find an example IoT control service that will let you control relative humidity level via ESP8266 module and ultrasonic humidifier.

### Prerequisites:
- AWS account (free tier will do just fine)
- ESP8266 microcontroller NodeMCU v3
- Ultrasonic humidifier UKC YX025S (KPY-25S)
- DHT11 humidity/temperature sensor
- Wirings, relays etc.

### How this will work (the high level diagram):
![alt text](https://drive.google.com/uc?export=view&id=1m-KpzdrXUV4oVPQlxDD9rfBbZ3k2BmIC)

### Set-up instructions:
1. Configure AWS credentials
    - Given you already have AWS account, you now have to configure credentials on the machine that you will use for `serverless deploy` command
    - See this page for details -> https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-configure.html

2. Prepare Lambda deployment archive
    - Now you need to build, publish and package this project so that the Lambda funciton can be deployed
    - You'll have to execute `dotnet publish -c Release` command
    - Then zip everything you find in _publish_ folder, see [serverless configuration file](IoT.Control/IoT.Control.Services/serverless.yml) for expected artifact path

3. Deploy AWS infrastructure
    - Navigate to [serverless folder](IoT.Control/IoT.Control.Services/)
    - Execute `npm install` so that you have required node modules installed
    - If everything went well, you should now have a fresh CloudFormation stack deployed & API Gateway endpoint available

4. Compose the hardware
    - You have to connect the DHT11 sensor to the controller
    - And the controller itself to the humidifier
    - See schemes at [this folder](IoT.Control/IoT.Control.Services/HumidityService/Schemas/)

5. Configure controller firmware
    - Open [controller firmware file](IoT.Control/IoT.Control.Services/HumidityService/Firmware/HumidityControllerFirmware.ino) in the editor
    - Set the values for _NETWORK_NAME_ & _NETWORK_PASS_ so that the controller can connect to the internet trough the WiFi
    - Set the values for _HUMIDITY_UPDATED_ENDPOINT_ & _AWS_FINGERPRINT_ so that controller know where to send metrics to
    - Compile and flash the firwmare to controller
    - Turn on the controller, set _desired_humidity_level_ in DynamoDB table, and have fun!
