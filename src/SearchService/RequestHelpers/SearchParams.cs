namespace SearchService.RequestHelpers
{
    public class SearchParams
    {
        public string Query { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public string Seller { get; set; }

        public string Winner { get; set; }

        public string OrderBy { get; set; }

        public string OrderDirection { get; set; }

        public string FilterBy { get; set; }
    }
}
