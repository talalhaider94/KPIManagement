namespace Quantis.WorkFlow.Services.DTOs.BusinessLogic
{
    public class ChangeStatusDTO
    {
        public bool IsSDMStatusChanged { get; set; }
        public bool IsBSIStatusChanged { get; set; }
        public bool IsArchived { get; set; }
        public bool ShowArchivedMsg { get; set; }

        public ChangeStatusDTO()
        {
            IsSDMStatusChanged = false;
            IsBSIStatusChanged = false;
            IsArchived = false;
            ShowArchivedMsg = false;
        }
    }
}