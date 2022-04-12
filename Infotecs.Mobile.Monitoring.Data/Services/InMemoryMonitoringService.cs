using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Services;

namespace Infotecs.Mobile.Monitoring.Data.Services;

/// <summary>
/// Сервис для хранения мониторинговых данных в памяти.
/// Внимание - после перезапуска приложения данные не сохраняются.
/// </summary>
public class InMemoryMonitoringService : IMonitoringService
{
    private readonly Dictionary<string, MonitoringData> monitoringDataDictionary = new();

    /// <summary>
    /// Добавление/обновление записи.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <exception cref="ArgumentNullException">Переданы невалидные данные.</exception>
    /// <returns>Задача.</returns>
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

    /// <summary>
    /// Получение данных мониторинга по идентификатору устройства.
    /// </summary>
    /// <param name="id">Идентификатор устройства.</param>
    /// <returns>Мониторинговые данные.</returns>
    public Task<MonitoringData?> GetByIdAsync(string id)
    {
        monitoringDataDictionary.TryGetValue(id, out MonitoringData? result);

        return Task.FromResult(result);
    }

    /// <summary>
    /// Получение всех данных мониторинга.
    /// </summary>
    /// <returns>Все мониторинговые данные.</returns>
    public Task<IEnumerable<MonitoringData>> GetListAsync() =>
        Task.FromResult((IEnumerable<MonitoringData>)monitoringDataDictionary.Values.ToArray());

    /// <summary>
    /// Метод поиска мониторинговых данных по критериям.
    /// </summary>
    /// <param name="criteria">Критерии поиска.</param>
    /// <returns>Результат поиска.</returns>
    /// <exception cref="NotImplementedException">Метод не определен.</exception>
    public Task<SearchResult<MonitoringData>> SearchAsync(MonitoringSearchCriteria criteria) =>
        throw new NotImplementedException();

}
