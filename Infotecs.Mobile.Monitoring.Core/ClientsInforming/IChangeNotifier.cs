using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.Core.ClientsInforming;

/// <summary>
/// Интерфейс для информирования об изменениях.
/// </summary>
public interface IChangeNotifier
{
    /// <summary>
    /// Метод отправки данных о ноде (устройстве).
    /// </summary>
    /// <param name="monitoringData">Мониторинговые данные.</param>
    /// <returns>Задача.</returns>
    public Task SendNewMonitoringDataAsync(MonitoringData monitoringData);
}
