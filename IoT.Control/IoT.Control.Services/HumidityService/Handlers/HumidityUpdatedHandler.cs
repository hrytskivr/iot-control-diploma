using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using IoT.Control.Services.HumidityService.DataRepositories;
using IoT.Control.Services.HumidityService.DataRepositories.Interfaces;
using IoT.Control.Services.HumidityService.Models;
using Newtonsoft.Json;

namespace IoT.Control.Services.HumidityService.Handlers
{
    public class HumidityUpdatedHandler
    {
        private readonly IHumidityServiceRepository _repository;

        public HumidityUpdatedHandler()
        {
            // TODO: IoC
            _repository = new HumidityServiceRepository();
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log("Parsing request...");
            // parse sensor request
            var requestData = JsonConvert.DeserializeObject<HumidityUpdatedRequest>(request.Body);
            
            context.Logger.Log("Storing fresh sensor data in DynamoDB...");
            // put fresh sensor data into DynamoDB
            await _repository.SetCurrentHumidityLevel(requestData.CurrentHumidityLevel);
            await _repository.SetCurrentHumidifierState(requestData.CurrentHumidifierState);
            
            context.Logger.Log("Checking if humidifier toggle needed...");
            // check if humidifier toggle needed, send 202 if so
            var desiredHumidityLevel = await _repository.GetDesiredHumidityLevel();
            if (desiredHumidityLevel > requestData.CurrentHumidityLevel)
            {
                context.Logger.Log("Humidifier toggle needed, sending appropriate response...");
                if (requestData.CurrentHumidifierState == HumidifierStates.Disabled)
                    return new APIGatewayProxyResponse {StatusCode = 202};
            }
            else
            {
                context.Logger.Log("Humidifier toggle needed, sending appropriate response...");
                if (requestData.CurrentHumidifierState == HumidifierStates.Enabled)
                    return new APIGatewayProxyResponse {StatusCode = 202};
            }
            
            context.Logger.Log("Humidifier toggle not needed...");
            return new APIGatewayProxyResponse {StatusCode = 200};
        }
    }
}
