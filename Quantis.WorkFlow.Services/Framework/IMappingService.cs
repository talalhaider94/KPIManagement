using System.Collections.Generic;

namespace Quantis.WorkFlow.Services.Framework
{
    public interface IMappingService<DTO, Entity>
    {
        DTO GetDTO(Entity e);

        Entity GetEntity(DTO o, Entity e);

        List<DTO> GetDTOs(List<Entity> e);

        PagedList<DTO> GetPagedDTOs(PagedList<Entity> source);

        string SortMap(string col);
    }
}