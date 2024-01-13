namespace HCQS.BackEnd.Common.Model
{
    public class AppActionResult
    {
        public Result Result { get; set; } = new();

        public bool IsSuccess { get; set; } = true;
        public List<string?> Messages { get; set; } = new List<string?>();
    }

    public class Result
    {
        public object Data { get; set; }
        public int TotalPage { get; set; } = 0;
    }
}