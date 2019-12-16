using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.Information
{
    public class MultipleRecordsDTO
    {
        public int Id { get; set; }
        public List<int> Ids { get; set; }
    }
}