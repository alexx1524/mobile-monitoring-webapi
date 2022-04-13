using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
public class MonitoringDataRepositoryTest : IAsyncLifetime
{
    private readonly IConfiguration config;
    private readonly DapperContext context;
    private readonly Mock<ILogger<MonitoringDataRepository>> loggerMock;
    private readonly MonitoringDataRepository repository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MonitoringDataRepositoryTest()
    {
        config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        context = new DapperContext(config);
        loggerMock = new Mock<ILogger<MonitoringDataRepository>>();
        repository = new MonitoringDataRepository(context, loggerMock.Object);
    }

    /// <summary>
    /// Метод проверки возврата исключения при невалидных данных.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <returns>Задача.</returns>
    [Theory]
    [ClassData(typeof(WrongMonitoringData))]
    public async Task CreateAsync_IfDataIsInvalid_ThrowsPostgresException(MonitoringData monitoringData)
    {
        // Assert
        await Assert.ThrowsAsync<PostgresException>(async() => await repository.CreateAsync(monitoringData));
    }

    /// <summary>
    /// Проверка успешного создания мониторинговых данных.
    /// </summary>
    /// <returns>Задача.</returns>
    [Fact]
    public async Task CreateAsync_IfDataIsValid_ShouldSucessfullyCreate()
    {
        // Arrange
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
    public async Task UpdateAsync_IfDataIsValid_ShouldSuccessfullyUpdate()
    {
        // Arrange
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

    /// <summary>
    /// Тест проверяет что метод получения всех мониторинговых данных работает корректно.
    /// </summary>
    /// <param name="count">Количество данных данных.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(50)]
    public async Task GetAllAsync_IfDataExist_ShouldReturnAllMonitoringData(int count)
    {
        // Arrange
        for (var i = 0; i < count; i++)
        {
            var data = new MonitoringData
            {
                Id = Guid.NewGuid().ToString(),
                Version = "1.2.3",
                NodeName = $"Name {i}",
                OperatingSystem = "Android",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };

            await repository.CreateAsync(data);
        }

        // Act
        IEnumerable<MonitoringData>? result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count());
    }

    /// <summary>
    /// Тест проверяет, что если записи нет в базе данных, то метод GetById должен возвращать null значение.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Fact]
    public async Task GetByIdAsync_IfRecordDoesntExist_ShouldReturnNull()
    {
        // Act
        MonitoringData? expected = await repository.GetByIdAsync("non-existent identifier");

        // Assert
        Assert.Null(expected);
    }

    /// <summary>
    /// Тест проверяет, чтобы не добавлялись ивенты от ноды, которой нет. Проверка целостности базы данных
    /// и вторичного ключа.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Fact]
    public async Task AddEventAsync_IfNodeDoesntExist_ShouldThrowPostgresException()
    {
        var nodeEvent = new NodeEvent
        {
            NodeId = "non-existent identifier",
            Name = "eventName",
            Date = DateTime.UtcNow,
        };

        // Assert
        await Assert.ThrowsAsync<PostgresException>(async () => await repository.AddEventAsync(nodeEvent));
    }

    /// <summary>
    /// Тест проверяет, что метод GetEvents возвращает валидные данные для указанной ноды.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Fact]
    public async Task GetEventsAsync_IfEventsExist_ShouldReturnValidEvents()
    {
        var monitoringData = new MonitoringData
        {
            Id = Guid.NewGuid().ToString(),
            Version = "1.2.3",
            NodeName = "NodeName",
            OperatingSystem = "Android",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };

        var nodeEvent = new NodeEvent
        {
            NodeId = monitoringData.Id,
            Name = "eventName",
            Date = DateTime.UtcNow,
        };

        // Act
        await repository.CreateAsync(monitoringData);
        await repository.AddEventAsync(nodeEvent);

        IEnumerable<NodeEvent> data = await repository.GetEventsAsync(monitoringData.Id);

        // Assert
        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Single(data);
        Assert.Equal(nodeEvent.Name, data.First().Name);
        Assert.Equal(nodeEvent.Date?.ToString("u"), data.First().Date?.ToString("u"));
        Assert.Equal(nodeEvent.NodeId, data.First().NodeId);
    }

    /// <inheritdoc />
    public Task InitializeAsync() => Task.CompletedTask;

    /// <inheritdoc />
    public Task DisposeAsync() => ClearData();

    private async Task ClearData()
    {
        string? connectionString = config.GetConnectionString(DapperContext.ConnectionString);
        using IDbConnection connection = new NpgsqlConnection(connectionString);

        await connection.ExecuteAsync("DELETE FROM node_events;");
        await connection.ExecuteAsync("DELETE FROM monitoring_data;");
    }
}
