namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormFieldDTO
    {
        public FormFieldDTO(string fn, string fv, string ft)
        {
            FieldName = fn;
            FieldValue = fv;
            FieldType = ft;
        }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldType { get; set; }
    }
}