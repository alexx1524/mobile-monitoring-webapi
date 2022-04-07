using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs.Mobile.Monitoring.WebApi.Controllers;

/// <summary>
/// Контроллер для работы с данными мониторинга мобильных устройств
/// </summary>
[ApiController]
[Route("monitoring")]
public class MonitoringController : Controller
{
    private readonly ILogger<MonitoringController> logger;
    private readonly IMonitoringService monitoringService;

    public MonitoringController(ILogger<MonitoringController> logger, IMonitoringService monitoringService)
    {
        this.logger = logger;
        this.monitoringService = monitoringService;
    }

    /// <summary>
    /// Создание или обновление мониторинговых данных для устройства.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddOrUpdateAsync(MonitoringData monitoringData)
    {
        try
        {
            logger.LogInformation($"Monitoring Id={monitoringData.Id}, " +
                $"NodeName={monitoringData.NodeName}, " +
                $"OS={monitoringData.OperatingSystem}, " +
                $"Version={monitoringData.Version}");

            await monitoringService.AddOrUpdateAsync(monitoringData);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка сохранения данных мониторинга");
        }

        return Ok();
    }

    /// <summary>
    /// Получение последних мониторинговых данных по идентификатору устройства
    /// </summary>
    /// <param name="id">Идентификатор устройства</param>
    /// <returns>Последние данные мониторинга или NULL, если нет данных по этому устройству</returns>
    [HttpGet]
    [Route("{id}")]
    public async Task<MonitoringData?> GetByIdAsync(string id)
    {
        MonitoringData? result = null;

        try
        {
            result = await monitoringService.GetByIdAsync(id);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Ошибка получения данных по идентификатору {id}");
        }

        return result;
    }

    /// <summary>
    /// Получение всех мониторинговых данных
    /// </summary>
    /// <returns>Коллекция всех мониторинговых данных</returns>
    [HttpGet]
    [Route("list")]
    public async Task<IEnumerable<MonitoringData>> GetListAsync()
    {
        IEnumerable<MonitoringData>? result = null;

        try
        {
            result = await monitoringService.GetListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка получения списка данных мониторинга");
        }

        return result ?? Array.Empty<MonitoringData>();
    }
}
