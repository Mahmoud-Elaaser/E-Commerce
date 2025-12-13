using ECommerce.DTOs.Pagination;

namespace ECommerce.DTOs
{
    public class ResponseDto
    {
        public bool IsSucceeded { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Model { get; set; }
        public IEnumerable<object>? Models { get; set; }
        public object Metadata { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();

        public static ResponseDto Success(int status, string message, object model = null) =>
            new ResponseDto
            {
                IsSucceeded = true,
                Status = status,
                Message = message,
                Model = model
            };

        public static ResponseDto Failure(int status, string message) =>
            new ResponseDto
            {
                IsSucceeded = false,
                Status = status,
                Message = message
            };

        public static ResponseDto PaginatedSuccess<T>(
            IEnumerable<T> items,
            int totalCount,
            int currentPage,
            int pageSize,
            string message = "Data retrieved successfully")
        {
            var metadata = new PaginationMetadata(totalCount, currentPage, pageSize);

            return new ResponseDto
            {
                IsSucceeded = true,
                Status = 200,
                Message = message,
                Model = items,
                Metadata = new
                {
                    metadata.CurrentPage,
                    metadata.TotalPages,
                    metadata.PageSize,
                    metadata.TotalCount,
                    metadata.HasPrevious,
                    metadata.HasNext,
                    metadata.PreviousPage,
                    metadata.NextPage,
                    metadata.FirstItemIndex,
                    metadata.LastItemIndex
                }
            };
        }


        public static ResponseDto ValidationFailure(int status, string message, List<string> validationErrors) =>
            new ResponseDto
            {
                IsSucceeded = false,
                Status = status,
                Message = message,
                ValidationErrors = validationErrors
            };
    }
}