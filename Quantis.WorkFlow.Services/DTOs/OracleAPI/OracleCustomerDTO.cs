using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class OracleCustomerDTO
    {
        public int customer_id { get; set; }
        public string customer_name { get; set; }

        public List<CustomerSLA> slas { get; set; }
    }

    public class CustomerSLA
    {
        public int sla_id { get; set; }
        public string sla_name { get; set; }
    }
}