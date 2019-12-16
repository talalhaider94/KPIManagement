using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.Framework
{
    public interface ISMTPService
    {
        bool SendEmail(string subject, string body, List<string> recipients);
    }
}