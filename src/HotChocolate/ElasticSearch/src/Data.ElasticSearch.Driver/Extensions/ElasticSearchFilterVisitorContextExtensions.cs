using System.Diagnostics.CodeAnalysis;
using HotChocolate.Data.ElasticSearch.Filters;
using HotChocolate.Data.Filters;

namespace HotChocolate.Data.ElasticSearch;

public static class ElasticSearchFilterVisitorContextExtensions
{
    /// <summary>
    /// Reads the current scope from the context
    /// </summary>
    /// <param name="context">The context</param>
    /// <returns>The current scope</returns>
    public static ElasticSearchFilterScope GetElasticSearchFilterScope(
        this ElasticSearchFilterVisitorContext context) =>
        (ElasticSearchFilterScope )context.GetScope();

    /// <summary>
    /// Tries to build the query based on the items that are stored on the scope
    /// </summary>
    /// <param name="context">the context</param>
    /// <param name="query">The query that was build</param>
    /// <returns>True in case the query has been build successfully, otherwise false</returns>
    public static bool TryCreateQuery(
        this ElasticSearchFilterVisitorContext context,
        [NotNullWhen(true)] out QueryDefinition? query)
    {
        return context.GetElasticSearchFilterScope().TryCreateQuery(out query);
    }
}