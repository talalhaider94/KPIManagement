namespace Quantis.WorkFlow.Services.DTOs.BusinessLogic
{
    public class SDMUploadAttachmentDTO
    {
        public int TicketId { get; set; }
        public string AttachmentName { get; set; }
        public byte[] AttachmentContent { get; set; }
    }
}