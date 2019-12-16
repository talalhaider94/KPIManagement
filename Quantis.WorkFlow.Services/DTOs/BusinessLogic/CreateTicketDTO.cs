namespace Quantis.WorkFlow.Services.DTOs.BusinessLogic
{
    public class CreateTicketDTO
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public int? SecondaryContractParty { get; set; }

        public string Customer { get; set; }
        public string Status { get; set; }
        public int GroupCategoryId { get; set; }
        public string Group { get; set; }
        public string ID_KPI { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Reference3 { get; set; }
        public string Period { get; set; }
        public string zz1_contractParties { get; set; }
        public string zz2_calcValue { get; set; }
        public string zz3_KpiIds { get; set; }
    }
}