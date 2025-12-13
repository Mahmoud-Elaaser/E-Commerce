using ECommerce.Models;
using ECommerce.Specifications;
using System.Linq.Expressions;

namespace ECommerce.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification(
            string searchTerm = null,
            int? brandId = null,
            int? typeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool inStockOnly = false,
            bool includeBrand = false,
            bool includeType = false)
            : base(GetCriteria(searchTerm, brandId, typeId, minPrice, maxPrice, inStockOnly))
        {
            if (includeBrand)
            {
                AddInclude(p => p.ProductBrand);
            }

            if (includeType)
            {
                AddInclude(p => p.ProductType);
            }

            // Default ordering by name
            AddOrderBy(p => p.Name);
        }

        private static Expression<Func<Product, bool>> GetCriteria(
            string searchTerm,
            int? brandId,
            int? typeId,
            decimal? minPrice,
            decimal? maxPrice,
            bool inStockOnly)
        {
            return p =>
                (string.IsNullOrEmpty(searchTerm) ||
                 p.Name.Contains(searchTerm) ||
                 p.Description.Contains(searchTerm)) &&
                (!brandId.HasValue || p.ProductBrandId == brandId.Value) &&
                (!typeId.HasValue || p.ProductTypeId == typeId.Value) &&
                (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                (!inStockOnly || p.QuantityInStock > 0);
        }

        public void ApplyPagination(int pageNumber, int pageSize)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }

        public void ApplySorting(string sortBy, bool descending = false)
        {
            switch (sortBy?.ToLower())
            {
                case "price":
                    if (descending)
                        AddOrderByDescending(p => p.Price);
                    else
                        AddOrderBy(p => p.Price);
                    break;
                case "name":
                    if (descending)
                        AddOrderByDescending(p => p.Name);
                    else
                        AddOrderBy(p => p.Name);
                    break;
                case "stock":
                case "quantity":
                    if (descending)
                        AddOrderByDescending(p => p.QuantityInStock);
                    else
                        AddOrderBy(p => p.QuantityInStock);
                    break;
                case "brand":
                    if (descending)
                        AddOrderByDescending(p => p.ProductBrand.Name);
                    else
                        AddOrderBy(p => p.ProductBrand.Name);
                    break;
                case "type":
                    if (descending)
                        AddOrderByDescending(p => p.ProductType.Name);
                    else
                        AddOrderBy(p => p.ProductType.Name);
                    break;
                default:
                    // Default sorting already applied in constructor
                    break;
            }
        }

        // Special specification for getting a single product with includes
        public ProductSpecification(int productId, bool includeBrand = false, bool includeType = false)
            : base(p => p.Id == productId)
        {
            if (includeBrand)
            {
                AddInclude(p => p.ProductBrand);
            }

            if (includeType)
            {
                AddInclude(p => p.ProductType);
            }
        }
    }
}