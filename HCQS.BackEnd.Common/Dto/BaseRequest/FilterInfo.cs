namespace HCQS.BackEnd.Common.Dto.BaseRequest
{
    public class FilterInfo
    {
        public string FieldName { get; set; }

        //public bool isValueFilter { get; set; }
        public double? Min { get; set; }

        public double? Max { get; set; }
        //public IList<SearchFieldDto>? values { get; set; }
    }
}