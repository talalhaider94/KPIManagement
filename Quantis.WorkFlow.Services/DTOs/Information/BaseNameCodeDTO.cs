namespace Quantis.WorkFlow.Services.DTOs.Information
{
    public class BaseNameCodeDTO
    {
        public BaseNameCodeDTO(int id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}