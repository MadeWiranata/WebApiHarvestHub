namespace WebApiHarvestHub.Models
{

    public class Status
    {
        public int Code { get; set; }
        public int PageCount { get; set; }
        public string Description { get; set; }
    }

    public static class StTrans
    {
        public static Status SetSt(int a, int b, string c)
        {
            Status st = new Status
            {
                Code = a,
                PageCount = b,
                Description = c
            };
            return st;
        }
    }
}