using Quantis.WorkFlow.Services.API;

namespace Quantis.WorkFlow.APIBase.API
{
    public class SampleAPI : ISampleAPI
    {
        public string GetSampleString()
        {
            return "Hello from Sample API";
        }
    }
}