using System;
using System.Collections.Generic;
using System.Text;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class PersonalReportMapper : MappingService<PersonalReportDTO, T_PersonalReport>
    {
        public override PersonalReportDTO GetDTO(T_PersonalReport e)
        {
            return new PersonalReportDTO()
            {
                global_rule_id = e.global_rule_id,
                id = e.id,
                name = e.name,
                aggregation_option = e.aggregation_option,
                aggregation_option2 = e.aggregation_option2,
                end_date = e.end_date,
                end_date2 = e.end_date2,
                global_rule2_id = e.global_rule2_id,
                modification_date = e.modification_date,
                report_type = e.report_type,
                start_date = e.start_date,
                start_date2 = e.start_date2
            };
        }

        public override T_PersonalReport GetEntity(PersonalReportDTO o, T_PersonalReport e)
        {
            e.global_rule_id = o.global_rule_id;
            e.name = o.name;
            e.aggregation_option = o.aggregation_option;
            e.aggregation_option2 = o.aggregation_option2;
            e.end_date = o.end_date;
            e.end_date2 = o.end_date2;
            e.global_rule2_id = o.global_rule2_id;
            e.modification_date = o.modification_date;
            e.report_type = o.report_type;
            e.start_date = o.start_date;
            e.start_date2 = o.start_date2;
            return e;
        }
    }
}
