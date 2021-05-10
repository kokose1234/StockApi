namespace StockApi.Data
{
    public sealed record News
    { 
        public string Title { get; init; }
        public string Url { get; init; }
        public string ThumbnailUrl { get; init; }
    }
}