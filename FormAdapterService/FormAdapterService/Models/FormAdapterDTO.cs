using EventForm.bcfed9e1.Class2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormAdapterService.Models
{
    public class FormAdapterDTO
    {
        public int formID { get; set; }
        public int localID { get; set; }
        public List<FormField> forms { get; set; }
    }
}