using Microsoft.AspNetCore.Http;
using Quantis.WorkFlow.Services.DTOs.BusinessLogic;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.API
{
    public interface IServiceDeskManagerService
    {
        List<SDMTicketLVDTO> GetAllTickets();

        List<SDMTicketLVDTO> GetTicketsRicercaByUser(HttpContext context, string period);

        List<SDMTicketLVDTO> GetTicketsVerificationByUser(HttpContext context, string period);

        List<SDMTicketLVDTO> GetTicketsAdministratorByPeriod(string period);

        SDMTicketLVDTO CreateTicket(CreateTicketDTO dto);

        SDMTicketLVDTO CreateTicketByKPIID(int Id);

        SDMTicketLVDTO GetTicketByID(int Id);

        ChangeStatusDTO TransferTicketByID(int id, string status, string description, HttpContext context);

        ChangeStatusDTO EscalateTicketbyID(int id, string status, string description, HttpContext context);

        string UploadAttachmentToTicket(SDMUploadAttachmentDTO dto, string userName);

        void UpdateTicketValue(HttpContext context, TicketValueDTO dto);

        byte[] DownloadAttachment(string attachmentHandle);

        List<SDMAttachmentDTO> GetAttachmentsByTicket(int ticketId);

        List<SDMTicketLogDTO> GetTicketHistory(int ticketId);
    }
}