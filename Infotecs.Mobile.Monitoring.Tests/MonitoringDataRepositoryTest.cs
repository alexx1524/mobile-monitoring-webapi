using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using Dapper;
using FluentAssertions;
using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Data.Migrations;
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
    private readonly MonitoringDataRepository repository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MonitoringDataRepositoryTest()
    {
        config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var loggerMock = new Mock<ILogger<MonitoringDataRepository>>();

        var dbConnectionFactory = new DbConnectionFactory(config);

        var dbContext = new UnitOfWork(dbConnectionFactory);

        repository = new MonitoringDataRepository(dbContext, loggerMock.Object);
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
        Func<Task> act = async () => { await repository.CreateAsync(monitoringData); };

        await act.Should().ThrowAsync<PostgresException>();
    }

    /// <summary>
    /// Проверка успешного создания мониторинговых данных.
    /// </summary>
    /// <param name="monitoringData">Мониторинговые данные ноды.</param>
    /// <returns>Задача.</returns>
    [Theory, AutoData]
    public async Task CreateAsync_IfDataIsValid_ShouldSucessfullyCreate(MonitoringData monitoringData)
    {
        // Act
        await repository.CreateAsync(monitoringData);

        MonitoringData? result = await repository.GetByIdAsync(monitoringData.Id ??
            throw new ArgumentException(nameof(monitoringData)));

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(monitoringData, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, new TimeSpan(1000)))
            .WhenTypeIs<DateTime>());
    }

    /// <summary>
    /// Проверка успешного обновления мониторинговых данных.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <returns>Задача.</returns>
    [Theory, AutoData]
    public async Task UpdateAsync_IfDataIsValid_ShouldSuccessfullyUpdate(MonitoringData monitoringData)
    {
        // Act
        await repository.CreateAsync(monitoringData);

        monitoringData.NodeName = "Node name";
        monitoringData.Version = "1.1.1";
        monitoringData.OperatingSystem = "Ios";
        monitoringData.CreatedDate = DateTime.UtcNow;
        monitoringData.UpdatedDate = DateTime.UtcNow;

        await repository.UpdateAsync(monitoringData);

        MonitoringData? result = await repository.GetByIdAsync(monitoringData.Id ??
            throw new ArgumentException(nameof(monitoringData)));

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(monitoringData, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, new TimeSpan(1000)))
            .WhenTypeIs<DateTime>());
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
        var fixture = new Fixture();

        IEnumerable<MonitoringData>? items =  fixture.CreateMany<MonitoringData>(count);

        foreach (MonitoringData monitoringData in items)
        {
            await repository.CreateAsync(monitoringData);
        }

        // Act
        IEnumerable<MonitoringData> result = await repository.GetAllAsync();

        // Assert
        result.Should()
            .NotBeNullOrEmpty()
            .And.HaveCount(count);
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
        expected.Should().BeNull();
    }

    /// <summary>
    /// Тест проверяет, чтобы не добавлялись ивенты от ноды, которой нет. Проверка целостности базы данных
    /// и вторичного ключа.
    /// </summary>
    /// <param name="nodeEvent">Ивент от ноды.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Theory, AutoData]
    public async Task AddEventAsync_IfNodeDoesntExist_ShouldThrowPostgresException(NodeEvent nodeEvent)
    {
        // Assert
        Func<Task> act = async () => { await repository.AddEventAsync("non-existent identifier", nodeEvent); };

        await act.Should().ThrowAsync<PostgresException>();
    }

    /// <summary>
    /// Тест проверяет, что метод GetEvents возвращает валидные данные для указанной ноды.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга ноды.</param>
    /// <param name="count">Количество ивентов.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Theory, AutoData]
    public async Task GetEventsAsync_IfEventsExist_ShouldReturnValidEvents(MonitoringData monitoringData, byte count)
    {
        // Act
        await repository.CreateAsync(monitoringData);

        var fixture = new Fixture();

        IEnumerable<NodeEvent> events = fixture.CreateMany<NodeEvent>(count).ToArray();

        foreach (NodeEvent nodeEvent in events)
        {
            await repository.AddEventAsync(monitoringData.Id ??
                throw new ArgumentException(nameof(monitoringData)), nodeEvent);
        }

        NodeEvent[] result = (await repository.GetEventsAsync(monitoringData.Id ??
            throw new ArgumentException(nameof(monitoringData)))).ToArray();

        // Assert
        result.Should()
            .NotBeNullOrEmpty()
            .And.HaveCount(count)
            .And.BeEquivalentTo(events, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, new TimeSpan(1000)))
                .WhenTypeIs<DateTime>());
    }

    /// <inheritdoc />
    public Task InitializeAsync() => Task.CompletedTask;

    /// <inheritdoc />
    public Task DisposeAsync() => ClearData();

    private async Task ClearData()
    {
        string? connectionString = config.GetConnectionString(MigrationManager.ConnectionString);
        using IDbConnection connection = new NpgsqlConnection(connectionString);

        await connection.ExecuteAsync("DELETE FROM node_events;");
        await connection.ExecuteAsync("DELETE FROM monitoring_data;");
    }
}
