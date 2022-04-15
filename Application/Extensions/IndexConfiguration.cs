using Nest;

namespace Application.Extensions;

public static class IndexConfiguration
{
    public static async Task CreateIndex<T>(this IElasticClient elasticClient, string indexName)
    {
        ArgumentNullException.ThrowIfNull(indexName);

        await elasticClient.Indices.CreateAsync(indexName, descriptor => descriptor
            .Settings(settingsDescriptor => settingsDescriptor
                .Analysis(analysisDescriptor => analysisDescriptor
                    .CharFilters(filtersDescriptor => filtersDescriptor
                        .PatternReplace("synonyms-replacement", patternReplace => patternReplace
                            .Pattern("(good)|(awesome)")
                            .Replacement("cool"))
                        .PatternReplace("extra-signs-replacement", patternReplace => patternReplace
                            .Pattern("[^a-zA-Z0-9 ]")
                            .Replacement(""))
                        .PatternReplace("extra-space-replacement", patternReplace => patternReplace
                            .Pattern("\\s+")
                            .Replacement(" ")))
                    .TokenFilters(filtersDescriptor => filtersDescriptor
                        .Synonym("synonyms-tokens", synonymsDescriptor => synonymsDescriptor
                            .Expand()
                            .Synonyms("cool, good, awesome")))
                    .Analyzers(analyzerDescriptor => analyzerDescriptor
                        .Custom("custom-analyzer", customAnalyzerDescriptor => customAnalyzerDescriptor
                            .Tokenizer("keyword")
                            .Filters("lowercase", "trim")
                            .CharFilters("synonyms-replacement", "extra-signs-replacement", "extra-space-replacement"))
                        .Custom("synonym-token-analyzer", customAnalyzerDescriptor => customAnalyzerDescriptor
                            .Tokenizer("standard")
                            .Filters("synonyms-tokens"))))));
    }

    public static async Task<bool> EnsureExist(this IElasticClient elasticClient, string indexName)
    {
        return (await elasticClient.Indices.ExistsAsync(indexName)).Exists;
    }
}