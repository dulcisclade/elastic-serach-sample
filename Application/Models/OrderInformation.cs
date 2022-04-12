using Nest;

namespace Application.Models;

[ElasticsearchType(IdProperty = nameof(OrderId))]
public class OrderInformation
{
    [Text(Name = "order-id", Analyzer = "custom-analyzer")]
    public string OrderId { get; set; }
    
    [Text(Name = "name", Analyzer = "custom-analyzer")]
    public string Name { get; set; }
    
    [Number(NumberType.Integer, Name = "count")]
    public int Count { get; set; }
    
    [Text(Name = "description", Analyzer = "synonym-token-analyzer")]
    public string Description { get; set; }
}