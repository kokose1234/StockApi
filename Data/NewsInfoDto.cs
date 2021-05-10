using System.Collections.Generic;

namespace StockApi.Data
{
    public sealed record NewsInfoDto
    {
        public string UpdateDate { get; init; }
        public IList<News> NewsList { get; init; }
    }
}