using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Services;

namespace Infotecs.Mobile.Monitoring.Data.Services;

public class MonitoringService : IMonitoringService
{
    private readonly Dictionary<string, MonitoringData> monitoringDataDictionary = new();

    public Task AddOrUpdateAsync(MonitoringData monitoringData)
    {
        if (string.IsNullOrEmpty(monitoringData?.Id))
        {
            throw new ArgumentNullException(nameof(monitoringData));
        }

        if (monitoringDataDictionary.ContainsKey(monitoringData.Id))
        {
            monitoringData.UpdatedDate = DateTime.UtcNow;
        }
        else
        {
            monitoringData.CreatedDate = DateTime.UtcNow;
        }

        monitoringDataDictionary[monitoringData.Id] = monitoringData;

        return Task.CompletedTask;
    }

    public Task<MonitoringData?> GetByIdAsync(string id)
    {
        MonitoringData? result = null;

        if (monitoringDataDictionary.ContainsKey(id))
        {
            result = monitoringDataDictionary[id];
        }

        return Task.FromResult(result);
    }

    public Task<IEnumerable<MonitoringData>> GetListAsync() =>
        Task.FromResult((IEnumerable<MonitoringData>)monitoringDataDictionary.Values.ToArray());
}
