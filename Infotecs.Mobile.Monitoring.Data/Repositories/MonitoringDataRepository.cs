using System.Data;
using Dapper;
using FluentMigrator.Runner;
using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Models.Sorting;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Data.Context;
using Infotecs.Mobile.Monitoring.Data.Models;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Infotecs.Mobile.Monitoring.Data.Repositories;

/// <summary>
/// Репозиторий мониторинговых данных от устройств
/// </summary>
public class MonitoringDataRepository : IMonitoringDataRepository
{
    private readonly DapperContext context;
    private readonly ILogger<MonitoringDataRepository> logger;

    public MonitoringDataRepository(DapperContext context, ILogger<MonitoringDataRepository> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Создание новой записи мониторингавых данных
    /// </summary>
    /// <param name="monitoringData"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Create(MonitoringData monitoringData)
    {
        const string Query = "INSERT INTO monitoring_data (id, nodename, operatingsystem, version, createddate, updateddate) " +
            "VALUES (@Id, @NodeName, @OperatingSystem, @Version, @CreatedDate, @UpdatedDate)";

        using (IDbConnection connection = context.CreateConnection())
        {
            await connection.ExecuteAsync(Query, new[]{ new
            {
                monitoringData.Id,
                monitoringData.NodeName,
                monitoringData.OperatingSystem,
                monitoringData.Version,
                monitoringData.CreatedDate,
                monitoringData.UpdatedDate
            }});
        }
    }


    /// <summary>
    /// Получение всех записей
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<MonitoringData>> GetAll()
    {
        const string Query = "SELECT * FROM monitoring_data";

        using (IDbConnection connection = context.CreateConnection())
        {
            IEnumerable<MonitoringDataEntity>? entities = await connection.QueryAsync<MonitoringDataEntity>(Query);

            return entities.Adapt<IEnumerable<MonitoringData>>();
        }
    }


    /// <summary>
    /// Получение записи мониторинговых данных по идентификатору устройства
    /// </summary>
    /// <param name="id">Идентификатор устройства</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<MonitoringData?> GetById(string id)
    {
        const string Query = "SELECT * FROM monitoring_data WHERE id = @id";

        using (IDbConnection connection = context.CreateConnection())
        {
            var entity = await connection.QueryFirstOrDefaultAsync<MonitoringDataEntity>(Query, new { id });

            return entity.Adapt<MonitoringData>();
        }
    }


    /// <summary>
    /// Обновить мониторингвые данные
    /// </summary>
    /// <param name="monitoringData"></param>
    /// <returns></returns>
    public async Task Update(MonitoringData monitoringData)
    {
        const string Query = "UPDATE monitoring_data SET nodename=@NodeName, operatingsystem=@OperatingSystem, " +
            "version=@Version, createddate=@CreatedDate, updateddate=@UpdatedDate WHERE id = @Id;";

        using (IDbConnection connection = context.CreateConnection())
        {
            await connection.ExecuteAsync(Query, new[]{ new
            {
                monitoringData.Id,
                monitoringData.NodeName,
                monitoringData.OperatingSystem,
                monitoringData.Version,
                monitoringData.CreatedDate,
                monitoringData.UpdatedDate
            }});
        }
    }


    /// <summary>
    /// Поиск мониторинговых данных по набору критериев
    /// </summary>
    /// <param name="criteria">Критерии поиска мониторинговых данных</param>
    /// <returns></returns>
    public async Task<SearchResult<MonitoringData>> Search(MonitoringSearchCriteria criteria)
    {
        if (criteria.PageNumber is < 1)
        {
            throw new ArgumentException(nameof(criteria.PageNumber));
        }

        int count = criteria.PageSize ?? 10;
        int skip = ((criteria.PageNumber ?? 1) - 1) * count;

        using (IDbConnection connection = context.CreateConnection())
        {
            var queryBuilder = new SqlBuilder();

            SqlBuilder.Template? selector = queryBuilder
                .AddTemplate($"SELECT * FROM monitoring_data /**orderby**/ OFFSET {skip} ROWS FETCH NEXT {count} ROWS ONLY");

            if (!string.IsNullOrEmpty(criteria.Sorting?.FieldName) && criteria.Sorting.Direction.HasValue)
            {
                string direction = criteria.Sorting.Direction == SortOrder.Descending ? "DESC" : "ASC";

                queryBuilder.OrderBy($"{criteria.Sorting.FieldName} {direction}");
            }

            logger.LogSql(selector.RawSql);

            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM monitoring_data");

            IEnumerable<MonitoringDataEntity>? entities = await connection.QueryAsync<MonitoringDataEntity>(selector.RawSql);

            return new SearchResult<MonitoringData>
            {
                TotalCount = totalCount,
                Items = entities.Adapt<IEnumerable<MonitoringData>>()
            };
        }
    }
}
