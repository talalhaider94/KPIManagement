using EventForm.bcfed9e1.Class2;
using FormAdapterService.Models;
using System.Web.Http;

namespace FormAdapterService.Controllers
{
    public class FormAdapterController : ApiController
    {
        [HttpGet]
        public int RunAdapterSample()
        {
            //SET Parameters
            long formID = 3281;
            int LocaleID = 1000;
            FormRow formrow = new FormRow();
            formrow.AddItem(new FormField("Text_Box_1", "resource1-LF", "string"));
            formrow.AddItem(new FormField("Checkbox_1", "1", "integer"));
            formrow.AddItem(new FormField("Date_Picker_1", "09/02/2019 05:00:00", "time"));
            formrow.AddItem(new FormField("Text_Area_1", "10 - questa è 'con apici semplici' la prima riga&#xD;&#xA; questa è la &quot;seconda&quot; riga con apici", "string"));
            formrow.AddItem(new FormField("Text_Box_2", "9999.83", "real"));
            MyFormCommitResult formcommitresult = new MyFormCommitResult();
            Class2 formadapter = new Class2(formID, LocaleID);
            formcommitresult = formadapter.AddFormData(formrow);

            return (int)formcommitresult;
        }

        public int RunAdapter([FromBody]FormAdapterDTO form)
        {
            long formID = form.formID;
            int LocaleID = form.localID;

            FormRow formrow = new FormRow();
            foreach (var f in form.forms)
            {
                formrow.AddItem(f);
            }
            MyFormCommitResult formcommitresult = new MyFormCommitResult();
            Class2 formadapter = new Class2(formID, LocaleID);
            formcommitresult = formadapter.AddFormData(formrow);

            return (int)formcommitresult;
        }
    }
}
