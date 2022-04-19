using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Core.Services;
using Microsoft.Extensions.Logging;

namespace Infotecs.Mobile.Monitoring.Data.Services;

/// <summary>
/// Сервис работы с мониторинговыми данными.
/// </summary>
public class MonitoringService : IMonitoringService
{
    private readonly IMonitoringDataRepository monitoringDataRepository;
    private readonly ILogger<MonitoringService> logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="monitoringDataRepository">Репозиторий для работы с мониторингвыми данными.</param>
    /// <param name="logger">Интерфейс для логгирования.</param>
    public MonitoringService(IMonitoringDataRepository monitoringDataRepository, ILogger<MonitoringService> logger)
    {
        this.monitoringDataRepository = monitoringDataRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Создание или обновление данных мониторинга.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <param name="events">Список ивентов.</param>
    /// <exception cref="ArgumentNullException">Исключение вызывается, если мониторингавые данные или идентификатор устроства равны Null или пустой строке.</exception>
    /// <returns>Задача возвращает true, если был добавлена новая нода (устройство).</returns>
    public async Task<bool> AddOrUpdateAsync(MonitoringData monitoringData, IEnumerable<NodeEvent> events)
    {
        if (string.IsNullOrEmpty(monitoringData.Id))
        {
            throw new ArgumentNullException(nameof(monitoringData));
        }

        MonitoringData? existedMonitoringData = await monitoringDataRepository.GetByIdAsync(monitoringData.Id);

        if (existedMonitoringData is null)
        {
            monitoringData.CreatedDate =
                monitoringData.UpdatedDate =
                    DateTime.UtcNow;

            await monitoringDataRepository.CreateAsync(monitoringData);
        }
        else
        {
            existedMonitoringData.NodeName = monitoringData.NodeName;
            existedMonitoringData.OperatingSystem = monitoringData.OperatingSystem;
            existedMonitoringData.Version = monitoringData.Version;
            existedMonitoringData.UpdatedDate = DateTime.UtcNow;

            await monitoringDataRepository.UpdateAsync(existedMonitoringData);
        }

        foreach (NodeEvent nodeEvent in events)
        {
            await monitoringDataRepository.AddEventAsync(monitoringData.Id, nodeEvent);
        }

        return existedMonitoringData is null;
    }

    /// <summary>
    /// Получение данных мониторинга по идентификатору устройства.
    /// </summary>
    /// <param name="id">Идентификатор устройства.</param>
    /// <returns>Мониторинговые данные по указанному идентификатору.</returns>
    public async Task<MonitoringData?> GetByIdAsync(string id)
    {
        MonitoringData? result = await monitoringDataRepository.GetByIdAsync(id);

        return result;
    }

    /// <summary>
    /// Получение всех данных мониторинга.
    /// </summary>
    /// <returns>Все мониторинговые данные.</returns>
    public async Task<IEnumerable<MonitoringData>> GetListAsync() =>
        await monitoringDataRepository.GetAllAsync();

    /// <summary>
    /// Поиск мониторинговых данных по набору критериев.
    /// </summary>
    /// <param name="criteria">Набор критериев поиска.</param>
    /// <returns>Результаты поиска мониторинговых данных.</returns>
    public async Task<SearchResult<MonitoringData>> SearchAsync(MonitoringSearchCriteria criteria)
    {
        SearchResult<MonitoringData> result = await monitoringDataRepository.SearchAsync(criteria);

        return result;
    }

    /// <summary>
    /// Получение списка ивентов для указанной ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды (устройства).</param>
    /// <returns>Список ивентов.</returns>
    public Task<IEnumerable<NodeEvent>> GetNodeEvents(string nodeId) => monitoringDataRepository.GetEventsAsync(nodeId);

    /// <summary>
    /// Добавление ивентов от ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды.</param>
    /// <param name="events">Ивенты от ноды (устройства).</param>
    /// <returns>Задача.</returns>
    public async Task AddEvents(string nodeId, IEnumerable<NodeEvent> events)
    {
        foreach (NodeEvent nodeEvent in events)
        {
            try
            {
                logger.LogInformation("Ивент {@NodeEvent}", nodeEvent);

                await monitoringDataRepository.AddEventAsync(nodeId, nodeEvent);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Ошибка сохранения ивента {@NodeEvent}", nodeEvent);
            }
        }
    }
}
