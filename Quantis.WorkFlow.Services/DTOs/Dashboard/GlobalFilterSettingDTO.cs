namespace Quantis.WorkFlow.Services.DTOs.Dashboard
{
    public class GlobalFilterSettingDTO
    {
        public int Id { get; set; }
        public string FilterKey { get; set; }
        public string FilterValue { get; set; }
        public string GlobalFilterId { get; set; }
        public string GlobalFilterName { get; set; }
    }
}