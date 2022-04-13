using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Data.Context;
using Infotecs.Mobile.Monitoring.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using Xunit;

namespace Infotecs.Mobile.Monitoring.Tests;

/// <summary>
/// Тест для репозитория мониторинговых данных.
/// </summary>
public class MonitoringDataRepositoryTest
{
    private readonly IConfiguration config;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MonitoringDataRepositoryTest()
    {
        this.config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }

    /// <summary>
    /// Метод проверки возврата исключения при невалидных данных.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <returns>Задача.</returns>
    [Theory]
    [ClassData(typeof(WrongMonitoringData))]
    public async Task MonitoringData_Incorrect_data_throws_exception(MonitoringData monitoringData)
    {
        await ClearData();

        // Arrange
        var context = new DapperContext(config);
        var loggerMock = new Mock<ILogger<MonitoringDataRepository>>();

        var repository = new MonitoringDataRepository(context, loggerMock.Object);

        // Assert
        await Assert.ThrowsAsync<PostgresException>(async() => await repository.CreateAsync(monitoringData));
    }

    /// <summary>
    /// Проверка успешного создания мониторинговых данных.
    /// </summary>
    /// <returns>Задача.</returns>
    [Fact]
    public async Task MonitoringData_Success_adding()
    {
        await ClearData();

        // Arrange
        var context = new DapperContext(config);
        var loggerMock = new Mock<ILogger<MonitoringDataRepository>>();

        var repository = new MonitoringDataRepository(context, loggerMock.Object);

        var data = new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = "1.2.3",
            NodeName = "Name",
            OperatingSystem = "Android",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };

        // Act
        await repository.CreateAsync(data);

        MonitoringData? result = await repository.GetByIdAsync(data.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result?.Id);
        Assert.NotNull(result?.NodeName);
        Assert.NotNull(result?.Version);
        Assert.NotNull(result?.OperatingSystem);
        Assert.NotNull(result?.CreatedDate);
        Assert.NotNull(result?.UpdatedDate);
        Assert.Equal(data.Id, result?.Id);
        Assert.Equal(data.Version, result?.Version);
        Assert.Equal(data.NodeName, result?.NodeName);
        Assert.Equal(data.OperatingSystem, result?.OperatingSystem);
        Assert.Equal(data.CreatedDate?.ToString("u"), result?.CreatedDate?.ToString("u"));
        Assert.Equal(data.UpdatedDate?.ToString("u"), result?.UpdatedDate?.ToString("u"));
    }

    /// <summary>
    /// Проверка успешного обновления мониторинговых данных.
    /// </summary>
    /// <returns>Задача.</returns>
    [Fact]
    public async Task MonitoringData_Success_updating()
    {
        await ClearData();

        // Arrange
        var context = new DapperContext(config);
        var loggerMock = new Mock<ILogger<MonitoringDataRepository>>();

        var repository = new MonitoringDataRepository(context, loggerMock.Object);

        var data = new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = "1.2.3",
            NodeName = "Name",
            OperatingSystem = "Android",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };

        // Act
        await repository.CreateAsync(data);

        data.NodeName = "Node name";
        data.Version = "1.1.1";
        data.OperatingSystem = "Ios";
        data.CreatedDate = DateTime.UtcNow;
        data.UpdatedDate = DateTime.UtcNow;

        await repository.UpdateAsync(data);

        MonitoringData? result = await repository.GetByIdAsync(data.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result?.Id);
        Assert.NotNull(result?.NodeName);
        Assert.NotNull(result?.Version);
        Assert.NotNull(result?.OperatingSystem);
        Assert.NotNull(result?.CreatedDate);
        Assert.NotNull(result?.UpdatedDate);
        Assert.Equal(data.Id, result?.Id);
        Assert.Equal(data.NodeName, result?.NodeName);
        Assert.Equal(data.Version, result?.Version);
        Assert.Equal(data.OperatingSystem, result?.OperatingSystem);
        Assert.Equal(data.CreatedDate?.ToString("u"), result?.CreatedDate?.ToString("u"));
        Assert.Equal(data.UpdatedDate?.ToString("u"), result?.UpdatedDate?.ToString("u"));
    }

    private async Task ClearData()
    {
        string? connectionString = config.GetConnectionString(DapperContext.ConnectionString);
        using IDbConnection connection = new NpgsqlConnection(connectionString);

        await connection.ExecuteAsync("DELETE FROM monitoring_data;");
        await connection.ExecuteAsync("DELETE FROM node_events;");
    }
}
