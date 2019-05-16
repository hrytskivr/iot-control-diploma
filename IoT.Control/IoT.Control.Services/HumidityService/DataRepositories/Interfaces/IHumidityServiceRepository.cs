using System.Threading.Tasks;
using IoT.Control.Services.HumidityService.Models;

namespace IoT.Control.Services.HumidityService.DataRepositories.Interfaces
{
    public interface IHumidityServiceRepository
    {
        Task SetCurrentHumidityLevel(double currentHumidityLevel);
        Task SetCurrentHumidifierState(HumidifierStates currentHumidifierState);
        Task<double> GetDesiredHumidityLevel();
    }
}
