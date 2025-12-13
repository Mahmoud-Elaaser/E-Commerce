namespace ECommerce.DTOs.Pagination
{
    public class ProductPaginationParams : PaginationParams
    {
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool InStockOnly { get; set; } = false;
        public bool IncludeBrand { get; set; } = false;
        public bool IncludeType { get; set; } = false;
    }

    public class PaginationParams
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 10;
        private const int DefaultPage = 1;

        private int _pageSize = DefaultPageSize;
        private int _pageNumber = DefaultPage;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? DefaultPage : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? DefaultPageSize : value);
        }

        public string? SearchTerm { get; set; }
        public string SortBy { get; set; } = "name";
        public bool SortDescending { get; set; } = false;
    }
}