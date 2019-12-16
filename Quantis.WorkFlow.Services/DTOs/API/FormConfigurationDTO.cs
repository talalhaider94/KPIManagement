using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.API
{
    public class FormConfigurationDTO
    {
        public FormConfigurationDTO()
        {
            Extras = new Dictionary<string, string>();
        }

        public string a_type { get; set; }
        public string a_dataType { get; set; }

        public Dictionary<string, string> Extras { get; set; }
        public string defaultValue { get; set; }
        public string a_isMandatory { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        //public string a_id { get; set; }
        //public string a_name { get; set; }
        //public string a_top { get; set; }
        //public string a_left { get; set; }
        //public string a_width { get; set; }
        //public string a_height { get; set; }
        //useless public string a_fontColor { get; set; }
        //useless public string a_fontFamily { get; set; }
        //useless public string a_fontWeight { get; set; }
        //useless public string a_fontItalic { get; set; }
        //useless public string a_textDecoration { get; set; }
        //useless public string a_fontSize { get; set; }
        //useless public string a_backgrounColor { get; set; }
        //useless public string a_isDefaultFontColor { get; set; }
        //useless public string a_isDefaultBGColor { get; set; }
        //useless public string a_text { get; set; }

        //if (a_type == "DLFLabel")
        //{
        //public string a_isMandatoryLabel { get; set; }
        ////}
        ////if (a_type == "DLFTextBox")
        ////{
        //public string a_controllerDataType { get; set; }

        //public string a_maxLength { get; set; }

        //public string a_labelId { get; set; }
        //}
        //if (a_type == "DLFDatePicker")
        //{
        // public string a_defaultValue { get; set; }
        // public string a_showLegend { get; set; }
        // public string a_isMandatory { get; set; }
        // public string a_labelId { get; set; }
        //}
        //if (a_type == "DLFCheckBox")
        //{
        // public string a_text { get; set; }
        // public string a_controllerDataType { get; set; }
        //public string a_checkedStatus { get; set; }
        //public string a_checkedValue { get; set; }
        //public string a_unCheckedValue { get; set; }
        //}
    }
}