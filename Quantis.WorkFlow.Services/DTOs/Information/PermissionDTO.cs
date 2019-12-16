namespace Quantis.WorkFlow.Services.DTOs.Information
{
    public class PermissionDTO : BaseNameCodeDTO
    {
        public PermissionDTO(int id, string name, string code, string category, string type) : base(id, name, code)
        {
            this.category = category;
            permission_type = type;
        }

        public string category { get; set; }
        public string permission_type { get; set; }
    }
}