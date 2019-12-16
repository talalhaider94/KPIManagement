namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class UserDTO
    {
        public int id { get; set; }
        public string ca_bsi_account { get; set; }
        public int? ca_bsi_user_id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string organization { get; set; }
        public string mail { get; set; }
        public string userid { get; set; }
        public string manager { get; set; }
        public string password { get; set; }
    }
}