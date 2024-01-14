namespace HCQS.BackEnd.Common.Dto.BaseRequest
{
    public class FilterInfo
    {
        public string fieldName { get; set; }

        //public bool isValueFilter { get; set; }
        public double? min { get; set; }

        public double? max { get; set; }
        //public IList<SearchFieldDto>? values { get; set; }
    }
}