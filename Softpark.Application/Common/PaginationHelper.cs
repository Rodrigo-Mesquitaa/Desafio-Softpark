namespace Softpark.Application.Common
{
    public static class PaginationHelper
    {
        public static int NormalizePage(int page)
        {
            return page <= 0 ? 1 : page;
        }

        public static int NormalizePageSize(int pageSize)
        {
            if (pageSize <= 0) return 10;
            if (pageSize > 100) return 100;
            return pageSize;
        }

        public static int CalculateTotalPages(int totalRecords, int pageSize)
        {
            if (pageSize <= 0) return 0;
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }
}
