using Infotecs.Mobile.Monitoring.Core.ClientsInforming;
using Infotecs.Mobile.Monitoring.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace Infotecs.Mobile.Monitoring.WebApi.Hubs;

/// <summary>
/// Класс для информирования клиентов об изменениях на сервере.
/// </summary>
public class ChangeNotifier : IChangeNotifier
{
    private readonly IHubContext<MonitoringDataHub> hubContext;
    private readonly ILogger<ChangeNotifier> logger;

    /// <summary>
    /// Создание нового экземпляра класса <see cref="ChangeNotifier"/> class.
    /// </summary>
    /// <param name="hubContext">Контекс хаба SignalR.</param>
    /// <param name="logger">Интерфейс логгирования.</param>
    public ChangeNotifier(IHubContext<MonitoringDataHub> hubContext, ILogger<ChangeNotifier> logger)
    {
        this.hubContext = hubContext;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task SendNewMonitoringData(MonitoringData monitoringData) {
        try
        {
            await hubContext.Clients.All.SendAsync("onNewMonitoringDataAdded", monitoringData);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка отправки данных клиенту {@MonitoringData}", monitoringData);
        }
    }
}
