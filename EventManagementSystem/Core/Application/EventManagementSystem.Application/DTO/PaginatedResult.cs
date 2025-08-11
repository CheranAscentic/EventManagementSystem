using EventManagementSystem.Domain.Interfaces;

namespace EventManagementSystem.Application.DTO
{
    /// <summary>
    /// Data Transfer Object for paginated results.
    /// Follows Single Responsibility Principle by encapsulating pagination metadata.
    /// Generic implementation supports reusability across different entity types.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated result.</typeparam>
    public class PaginatedResult<T> : HasDto
    {
        public PaginatedResult()
        {
        }

        public PaginatedResult(List<T> items, int totalCount, int pageNumber, int itemsPerPage)
        {
            this.Items = items;
            this.TotalCount = totalCount;
            this.PageNumber = pageNumber;
            this.ItemsPerPage = itemsPerPage;
        }

        public List<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int ItemsPerPage { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)this.TotalCount / this.ItemsPerPage);

        public bool HasPreviousPage => this.PageNumber > 1;

        public bool HasNextPage => this.PageNumber < this.TotalPages;

        public object ToDto()
        {
            if (typeof(HasDto).IsAssignableFrom(typeof(T)))
            {
                var convertedItems = this.Items
                    .Where(item => item != null)
                    .Select(item => ((HasDto)(object)item!).ToDto())
                    .ToList();

                return new PaginatedResultDto<object>
                {
                    Items = convertedItems,
                    TotalCount = this.TotalCount,
                    PageNumber = this.PageNumber,
                    ItemsPerPage = this.ItemsPerPage,
                };
            }
            else
            {
                return new PaginatedResultDto<T>
                {
                    Items = this.Items,
                    TotalCount = this.TotalCount,
                    PageNumber = this.PageNumber,
                    ItemsPerPage = this.ItemsPerPage,
                };
            }
        }

        public class PaginatedResultDto<T> : IsDto
        {
            public List<T> Items { get; set; } = new List<T>();

            public int TotalCount { get; set; }

            public int PageNumber { get; set; }

            public int ItemsPerPage { get; set; }

            public int TotalPages => (int)Math.Ceiling((double)this.TotalCount / this.ItemsPerPage);

            public bool HasPreviousPage => this.PageNumber > 1;

            public bool HasNextPage => this.PageNumber < this.TotalPages;
        }
    }
}