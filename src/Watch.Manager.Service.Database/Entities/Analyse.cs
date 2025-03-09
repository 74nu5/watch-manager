namespace Watch.Manager.Service.Database.Entities;

using Newtonsoft.Json;

public class Analyse
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }

    public string[] Tags { get; set; }

    public string[] Authors { get; set; }

    public string Summary { get; set; }

    public Uri Url { get; set; }

    public DateTime AnalyzeDate { get; set; }
}
