namespace GeoLocApi.Utils
{
    /// <summary>
    /// Used to store income pagination quarry from web browsers
    /// </summary>
    public class PaginationFilter
    {
        public const int PageSizeLimit = 15;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = PageSizeLimit;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > PageSizeLimit ? PageSizeLimit : pageSize;
        }
    }
}