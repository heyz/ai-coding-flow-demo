namespace SJ.BackEnd.Template.Common;

public class SerilogConfig
{
    public MinimumLevelConfig MinimumLevel { get; set; } = new();
    public string[] Enrich { get; set; } = Array.Empty<string>();
    public SeqConfig Seq { get; set; } = new();
    public ElasticsearchConfig Elasticsearch { get; set; } = new();
}

public class MinimumLevelConfig
{
    public string Default { get; set; } = "Information";
    public Dictionary<string, string> Override { get; set; } = new();
}

public class SeqConfig
{
    public bool Enabled { get; set; }
    public string ServerUrl { get; set; } = "http://localhost:5341";
    public string ApiKey { get; set; } = "";
    public string MinimumLevel { get; set; } = "Information";
}

public class ElasticsearchConfig
{
    public bool Enabled { get; set; }
    public string ServerUrl { get; set; } = "http://localhost:9200";
    public string IndexFormat { get; set; } = "logs-{0:yyyy.MM.dd}";
    public bool AutoRegisterTemplate { get; set; } = true;
    public string MinimumLevel { get; set; } = "Information";
}