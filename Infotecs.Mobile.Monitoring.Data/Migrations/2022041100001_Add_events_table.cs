using System.ComponentModel.DataAnnotations.Schema;
using FluentMigrator;

namespace Infotecs.Mobile.Monitoring.Data.Migrations;

/// <summary>
/// Создание таблицы для событий от узлов.
/// </summary>
[Migration(2022041100001)]
public class Add_events_table_2022041100001 : Migration
{
    /// <summary>
    /// Применение миграции.
    /// </summary>
    public override void Up()
    {
        Create.Table("node_events")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("description").AsString(256).NotNullable()
            .WithColumn("date").AsDateTimeOffset().NotNullable()
            .WithColumn("nodeid").AsString(128).NotNullable().ForeignKey("monitoring_data", "id");
    }

    /// <summary>
    /// Откат миграции.
    /// </summary>
    public override void Down()
    {
        Delete.Table("node_events");
    }
}
