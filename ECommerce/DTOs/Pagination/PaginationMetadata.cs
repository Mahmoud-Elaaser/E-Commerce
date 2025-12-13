namespace ECommerce.DTOs.Pagination
{
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public int? PreviousPage => HasPrevious ? CurrentPage - 1 : null;
        public int? NextPage => HasNext ? CurrentPage + 1 : null;
        public int FirstItemIndex => ((CurrentPage - 1) * PageSize) + 1;
        public int LastItemIndex => Math.Min(CurrentPage * PageSize, TotalCount);

        public PaginationMetadata(int totalCount, int currentPage, int pageSize)
        {
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}