using System.Collections.Generic;

namespace StockApi.Data
{
    public sealed record StockInfoDto
    {
        public string UpdateTime { get; init; }
        public List<Stock> Stocks { get; init; } 
    }
}