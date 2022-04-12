using Nest;
using Application.Models;

namespace Application.Services;

public interface ISearchService
{
    Task<IReadOnlyCollection<OrderInformation>> Search(string name);
}

public class SearchService : ISearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly string _index;

    public SearchService(IElasticClient elasticClient, string index)
    {
        _elasticClient = elasticClient;
        _index = index;
    }

    public async Task<IReadOnlyCollection<OrderInformation>> Search(string name)
    {
        var queries = new List<Func<QueryContainerDescriptor<OrderInformation>, QueryContainer>>();

        queries.Add(query => query
            .Match(descriptor => descriptor
                .Field(field => field.Name)
                .Query(name)));

        queries.Add(query => query
            .MatchPhrase(descriptor => descriptor
                .Field(field => field.Description)
                .Query(name)));

        var results = await _elasticClient.SearchAsync<OrderInformation>(selector => selector
            .Index(_index)
            .From(0)
            .Size(100)
            .Query(query => query
                .Bool(@bool => @bool
                    .Should(queries)
                    .MinimumShouldMatch(1))));

        return results.Documents;
    }
}