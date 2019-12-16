using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class SubmitFormDTO
    {
        public int form_id { get; set; }
        public int user_id { get; set; }
        public int locale_id { get; set; }
        public List<FormFieldDTO> inputs { get; set; }
        public bool empty_form { get; set; }
        public string period { get; set; }
        public int year { get; set; }
    }
}