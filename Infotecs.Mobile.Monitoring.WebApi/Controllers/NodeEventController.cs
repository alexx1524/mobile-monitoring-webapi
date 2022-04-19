using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Repositories;
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
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="monitoringService">Интерфейс сервиса мониторинга.</param>
    /// <param name="logger">Интерфейс логгирования.</param>
    /// <param name="unitOfWork">Интерфейс управления транзакицями.</param>
    public NodeEventController(IMonitoringService monitoringService,
        ILogger<NodeEventController> logger,
        IUnitOfWork unitOfWork)
    {
        this.monitoringService = monitoringService;
        this.logger = logger;
        this.unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Добавление ивентов ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды.</param>
    /// <param name="events">Список ивентов от ноды.</param>
    /// <returns>Статус выполнения.</returns>
    [HttpPost]
    [Route("{nodeId}")]
    public async Task<ActionResult> AddEventAsync([FromRoute] string nodeId, [FromBody] IEnumerable<NodeEvent> events)
    {

        try
        {
            await monitoringService.AddEvents(nodeId, events);

            unitOfWork.Commit();
        }
        catch (Exception)
        {
            unitOfWork.Rollback();

            throw;
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
