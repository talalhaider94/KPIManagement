namespace Quantis.WorkFlow.Services.DTOs.Dashboard
{
    public class WidgetDTO : BaseIdNameDTO
    {
        public string URL { get; set; }
        public string Help { get; set; }
        public int WigetCategoryId { get; set; }
        public string WidgetCategoryName { get; set; }
        public string IconURL { get; set; }
        public string UIIdentifier { get; set; }
    }
}