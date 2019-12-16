namespace Quantis.WorkFlow.Services.DTOs.Information
{
    public class UserKPIDTO
    {
        public string Rule_Name { get; set; }
        public int Global_Rule_Id { get; set; }
        public int Sla_Id { get; set; }
        public string Sla_Name { get; set; }
        public string Customer_name { get; set; }
        public int Customer_Id { get; set; }
        public int User_Id { get; set; }
    }
}