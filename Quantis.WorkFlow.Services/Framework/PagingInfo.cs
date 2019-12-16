namespace Quantis.WorkFlow.Services.Framework
{
    public class PagingInfo
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public string OrderBy { get; set; }
        public OrderDirection OrderDirection { get; set; }
    }

    public enum OrderDirection
    {
        Asc = 0,
        Desc = 1
    }
}