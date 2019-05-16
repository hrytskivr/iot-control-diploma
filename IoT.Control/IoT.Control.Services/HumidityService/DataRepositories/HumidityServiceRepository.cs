using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using IoT.Control.Services.HumidityService.DataRepositories.Interfaces;
using IoT.Control.Services.HumidityService.Models;

namespace IoT.Control.Services.HumidityService.DataRepositories
{
    public class HumidityServiceRepository : IHumidityServiceRepository
    {
        private readonly IAmazonDynamoDB _dynamoClient;

        public HumidityServiceRepository()
        {
            // TODO: IoC
            _dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUCentral1);
        }

        public async Task SetCurrentHumidityLevel(double currentHumidityLevel)
        {
            var request = new PutItemRequest
            {
                // TODO: do something?
                TableName = Environment.GetEnvironmentVariable("HUMIDITY_SERVICE_TABLE"),

                Item = new Dictionary<string, AttributeValue>
                {
                    {"key", new AttributeValue {S = "current_humidity_level"}},
                    {"value", new AttributeValue {N = currentHumidityLevel.ToString(CultureInfo.InvariantCulture)}}
                }
            };

            await _dynamoClient.PutItemAsync(request);
        }

        public async Task SetCurrentHumidifierState(HumidifierStates currentHumidifierState)
        {
            var request = new PutItemRequest
            {
                // TODO: do something?
                TableName = Environment.GetEnvironmentVariable("HUMIDITY_SERVICE_TABLE"),

                Item = new Dictionary<string, AttributeValue>
                {
                    {"key", new AttributeValue {S = "current_humidifier_state"}},
                    {"value", new AttributeValue {S = currentHumidifierState.ToString()}}
                }
            };

            await _dynamoClient.PutItemAsync(request);
        }

        public async Task<double> GetDesiredHumidityLevel()
        {
            var request = new GetItemRequest
            {
                // TODO: do something?
                TableName = Environment.GetEnvironmentVariable("HUMIDITY_SERVICE_TABLE"),
                
                Key = new Dictionary<string, AttributeValue>
                {
                    {"key", new AttributeValue {S = "desired_humidity_level"}}
                }
            };

            var response = await _dynamoClient.GetItemAsync(request);
            var desiredHumidityLevel = response.Item["value"].N;

            return double.Parse(desiredHumidityLevel, CultureInfo.InvariantCulture);
        }
    }
}
