namespace StockApi.Data
{
    public sealed record Stock
    {
        public string StockItem { get; init; }
        public float Price { get; init; }
        public float Fluctuation { get; init; }
        public float FluctuationRate { get; init; }
    }
}
