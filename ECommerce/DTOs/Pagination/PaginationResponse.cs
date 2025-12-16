namespace ECommerce.DTOs.Pagination
{
    public class PaginationResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public PaginationMetadata Metadata { get; set; }

        public PaginationResponse(IEnumerable<T> data, PaginationMetadata metadata)
        {
            Data = data;
            Metadata = metadata;
        }
    }
}
