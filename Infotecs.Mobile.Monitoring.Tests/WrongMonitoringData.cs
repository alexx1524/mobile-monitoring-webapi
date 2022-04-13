using System;
using System.Collections;
using System.Collections.Generic;
using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.Tests;

/// <summary>
/// Класс, который используется для проверки пполучения исключения при попытке сохранения невалидных данных.
/// </summary>
public class WrongMonitoringData : IEnumerable<object[]>
{
    /// <inheritdoc />
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new MonitoringData() };
        yield return new object[] { new MonitoringData
        {
            Id = null,
            Version = "3.2.1",
            NodeName = "Name",
            OperatingSystem = "Ios",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        }};
        yield return new object[] { new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = null,
            NodeName = "Name",
            OperatingSystem = "Ios",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        }};
        yield return new object[] { new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = "3.2.1",
            NodeName = null,
            OperatingSystem = "Ios",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        }};
        yield return new object[] { new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = "3.2.1",
            NodeName = "Name",
            OperatingSystem = null,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        }};
        yield return new object[] { new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = "3.2.1",
            NodeName = "Name",
            OperatingSystem = "Ios",
            CreatedDate = null,
            UpdatedDate = DateTime.UtcNow,
        }};
        yield return new object[] { new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = "3.2.1",
            NodeName = "Name",
            OperatingSystem = "Ios",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = null,
        }};
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
