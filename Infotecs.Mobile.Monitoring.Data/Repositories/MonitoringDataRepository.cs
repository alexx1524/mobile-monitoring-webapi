using Dapper;
using FluentMigrator.Runner;
using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Models.Sorting;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Data.Models;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <summary>
/// Репозиторий мониторинговых данных от устройств.
/// </summary>
public class MonitoringDataRepository : DbRepositoryBase, IMonitoringDataRepository
{
    private readonly ILogger<MonitoringDataRepository> logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="dbContext">Контекст подключений к базе данных.</param>
    /// <param name="logger">Интерфейс логгирования.</param>
    public MonitoringDataRepository(IDbContext dbContext, ILogger<MonitoringDataRepository> logger) : base(dbContext)
        => this.logger = logger;

    /// <summary>
    /// Создание новой записи мониторингавых данных.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <returns>Задача.</returns>
    public async Task CreateAsync(MonitoringData monitoringData)
    {
        const string Query = "INSERT INTO monitoring_data (id, nodename, operatingsystem, version, createddate, updateddate) " +
            "VALUES (@Id, @NodeName, @OperatingSystem, @Version, @CreatedDate, @UpdatedDate)";

        await Connection.ExecuteAsync(Query, new[]{ new
        {
            monitoringData.Id,
            monitoringData.NodeName,
            monitoringData.OperatingSystem,
            monitoringData.Version,
            monitoringData.CreatedDate,
            monitoringData.UpdatedDate,
        }}, Transaction);
    }


    /// <summary>
    /// Получение всех записей.
    /// </summary>
    /// <returns>Все мониторинговые данные.</returns>
    public async Task<IEnumerable<MonitoringData>> GetAllAsync()
    {
        const string Query = "SELECT * FROM monitoring_data";

        IEnumerable<MonitoringDataEntity>? entities = await Connection
            .QueryAsync<MonitoringDataEntity>(Query, transaction: Transaction);

        return entities.Adapt<IEnumerable<MonitoringData>>();
    }


    /// <summary>
    /// Получение записи мониторинговых данных по идентификатору устройства.
    /// </summary>
    /// <param name="id">Идентификатор устройства.</param>
    /// <returns>Мониторинговые данные.</returns>
    public async Task<MonitoringData?> GetByIdAsync(string id)
    {
        const string Query = "SELECT * FROM monitoring_data WHERE id = @id";

        var entity = await Connection.QueryFirstOrDefaultAsync<MonitoringDataEntity>(Query, new { id }, Transaction);

        return entity.Adapt<MonitoringData>();
    }


    /// <summary>
    /// Обновить мониторингвые данные.
    /// </summary>
    /// <param name="monitoringData">Мониторинговые данные.</param>
    /// <returns>Задача.</returns>
    public async Task UpdateAsync(MonitoringData monitoringData)
    {
        const string Query = "UPDATE monitoring_data SET nodename=@NodeName, operatingsystem=@OperatingSystem, " +
            "version=@Version, createddate=@CreatedDate, updateddate=@UpdatedDate WHERE id = @Id;";

        await Connection.ExecuteAsync(Query, new[]{ new
        {
            monitoringData.Id,
            monitoringData.NodeName,
            monitoringData.OperatingSystem,
            monitoringData.Version,
            monitoringData.CreatedDate,
            monitoringData.UpdatedDate,
        }}, Transaction);
    }


    /// <summary>
    /// Поиск мониторинговых данных по набору критериев.
    /// </summary>
    /// <param name="criteria">Критерии поиска мониторинговых данных.</param>
    /// <returns>Результат поиска мониторингвых данных.</returns>
    public async Task<SearchResult<MonitoringData>> SearchAsync(MonitoringSearchCriteria criteria)
    {
        if (criteria.PageNumber is < 1)
        {
            throw new ArgumentException(nameof(criteria.PageNumber));
        }

        int count = criteria.PageSize ?? 10;
        int skip = ((criteria.PageNumber ?? 1) - 1) * count;

        var queryBuilder = new SqlBuilder();

        SqlBuilder.Template? selector = queryBuilder
            .AddTemplate($"SELECT * FROM monitoring_data /**orderby**/ OFFSET {skip} ROWS FETCH NEXT {count} ROWS ONLY");

        if (!string.IsNullOrEmpty(criteria.Sorting?.FieldName) && criteria.Sorting.Direction.HasValue)
        {
            string direction = criteria.Sorting.Direction == SortOrder.Descending ? "DESC" : "ASC";

            queryBuilder.OrderBy($"{criteria.Sorting.FieldName} {direction}");
        }

        logger.LogSql(selector.RawSql);

        var totalCount = await Connection
            .ExecuteScalarAsync<int>("SELECT COUNT(*) FROM monitoring_data", transaction: Transaction);

        IEnumerable<MonitoringDataEntity>? entities = await Connection
            .QueryAsync<MonitoringDataEntity>(selector.RawSql, transaction: Transaction);

        return new SearchResult<MonitoringData>
        {
            TotalCount = totalCount,
            Items = entities.Adapt<IEnumerable<MonitoringData>>()
        };
    }

    /// <summary>
    /// Получение списка ивентов для указанной ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды (устройства).</param>
    /// <returns>Список ивентов.</returns>
    public async Task<IEnumerable<NodeEvent>> GetEventsAsync(string nodeId)
    {
        const string Query = "SELECT * FROM public.node_events WHERE nodeid = @nodeId;";

        IEnumerable<NodeEventEntity>? entity = await Connection
            .QueryAsync<NodeEventEntity>(Query, new { nodeId }, Transaction);

        return entity.Adapt<IEnumerable<NodeEvent>>();
    }

    /// <summary>
    /// Добавление ивентd от ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды.</param>
    /// <param name="nodeEvent">Ивент.</param>
    /// <returns>Задача.</returns>
    public async Task AddEventAsync(string nodeId, NodeEvent nodeEvent)
    {
        const string Query = "INSERT INTO public.node_events(name, date, nodeid) " +
            "VALUES (@Name, @Date, @NodeId);";

        await Connection.ExecuteAsync(Query, new
        {
            nodeEvent.Name,
            nodeEvent.Date,
            nodeId,
        }, Transaction);
    }
}
