using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models;
using Quantis.WorkFlow.Services.DTOs.API;
using System;

namespace Quantis.WorkFlow.APIBase.Mappers
{
    public class FormAttachmentMapper : MappingService<FormAttachmentDTO, T_FormAttachment>
    {
        public override FormAttachmentDTO GetDTO(T_FormAttachment e)
        {
            return new FormAttachmentDTO()
            {
                checksum = e.checksum,
                content = e.content,
                doc_name = e.doc_name,
                form_id = e.form_id,
                period = e.period,
                year = e.year,
                form_attachment_id = e.t_form_attachments_id,
                create_date = e.create_date
            };
        }

        public override T_FormAttachment GetEntity(FormAttachmentDTO o, T_FormAttachment e)
        {
            e.checksum = o.checksum;
            e.content = o.content;
            e.doc_name = o.doc_name;
            e.form_id = o.form_id;
            e.period = o.period;
            e.year = o.year;
            e.create_date = DateTime.Now;
            return e;
        }
    }
}