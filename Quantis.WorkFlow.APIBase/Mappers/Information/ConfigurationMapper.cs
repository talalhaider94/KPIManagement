using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Models.Information;
using Quantis.WorkFlow.Services.DTOs.Information;

namespace Quantis.WorkFlow.APIBase.Mappers.Information
{
    public class ConfigurationMapper : MappingService<ConfigurationDTO, T_Configuration>
    {
        public override ConfigurationDTO GetDTO(T_Configuration e)
        {
            return new ConfigurationDTO()
            {
                Key = e.key,
                Owner = e.owner,
                Description = e.description,
                IsEnable = e.enable,
                IsEditable = e.iseditable,
                Value = e.value
            };
        }

        public override T_Configuration GetEntity(ConfigurationDTO o, T_Configuration e)
        {
            e.key = o.Key;
            e.owner = o.Owner;
            e.value = o.Value;
            e.enable = o.IsEnable;
            e.description = o.Description;
            e.isvisible = true;
            e.iseditable = true;
            return e;
        }
    }
}