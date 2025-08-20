using System.Collections.Generic;

namespace Common.Infrastructure
{
    public class PagedResults<T>
    {
        public PagedResults()
        {
            Data = new List<T>();
        }
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
    }
}
