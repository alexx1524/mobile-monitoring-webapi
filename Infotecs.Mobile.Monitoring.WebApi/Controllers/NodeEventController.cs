using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs.Mobile.Monitoring.WebApi.Controllers;

/// <summary>
/// Контроллер для работы с ивентами от нод (устройств).
/// </summary>
[ApiController]
[Route("event")]
public class NodeEventController : Controller
{

    private readonly IMonitoringService monitoringService;
    private readonly ILogger<NodeEventController> logger;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="monitoringService">Интерфейс сервиса мониторинга.</param>
    /// <param name="logger">Интерфейс логгирования.</param>
    public NodeEventController(IMonitoringService monitoringService, ILogger<NodeEventController> logger)
    {
        this.monitoringService = monitoringService;
        this.logger = logger;
    }

    /// <summary>
    /// Добавление ивентов ноды (устройства).
    /// </summary>
    /// <param name="events">Список ивентов от ноды.</param>
    /// <returns>Статус выполнения.</returns>
    [HttpPost]
    public async Task<ActionResult> AddEventAsync([FromBody] IEnumerable<NodeEvent> events)
    {
        foreach (NodeEvent nodeEvent in events)
        {
            try
            {
                logger.LogInformation("Ивент {@NodeEvent}", nodeEvent);

                await monitoringService.AddEvent(nodeEvent);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Ошибка сохранения ивента {@NodeEvent}", nodeEvent);
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Получение списка событий по идентификатору ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор узла (устройства).</param>
    /// <returns>Список ивентов.</returns>
    [HttpGet]
    [Route("bynode/{nodeId}")]
    public async Task<ActionResult<IEnumerable<NodeEvent>>> GetEventsByNodeIdAsync(string nodeId)
    {
        IEnumerable<NodeEvent> events = await monitoringService.GetNodeEvents(nodeId);

        return Ok(events);
    }
}
