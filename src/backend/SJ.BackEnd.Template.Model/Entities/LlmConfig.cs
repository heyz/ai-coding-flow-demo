using SqlSugar;

namespace SJ.BackEnd.Template.Model;

[SugarTable("llm_config")]
[Tenant("1")]
public class LlmConfig
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    [SugarColumn(Length = 50)]
    public string Provider { get; set; } = string.Empty;

    [SugarColumn(Length = 200)]
    public string ApiKey { get; set; } = string.Empty;

    [SugarColumn(Length = 100)]
    public string ChatModel { get; set; } = string.Empty;

    [SugarColumn(Length = 100)]
    public string EmbeddingModel { get; set; } = string.Empty;

    [SugarColumn(Length = 200)]
    public string BaseUrl { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    [SugarColumn(IsNullable = true)]
    public DateTime? CreatedAt { get; set; }
}