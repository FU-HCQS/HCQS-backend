namespace HCQS.BackEnd.Common.Dto.BaseRequest
{
    public class BaseFilterRequest
    {
        public string? keyword { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public IList<SortInfo>? sortInfoList { get; set; }
        public IList<FilterInfo>? filterInfoList { get; set; }
    }
}