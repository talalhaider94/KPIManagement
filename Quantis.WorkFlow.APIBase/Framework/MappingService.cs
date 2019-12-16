using Quantis.WorkFlow.Services.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Quantis.WorkFlow.APIBase.Framework
{
    public abstract class MappingService<DTO, Entity> : IMappingService<DTO, Entity>
    {
        private Dictionary<string, string> sortingparams;

        public MappingService()
        {
            sortingparams = new Dictionary<string, string>();
        }

        public abstract DTO GetDTO(Entity e);

        public abstract Entity GetEntity(DTO o, Entity e);

        public List<DTO> GetDTOs(List<Entity> e)
        {
            return e.Select(p => GetDTO(p)).ToList();
        }

        public void AddSortParams<IProp, OProp>(Expression<Func<DTO, OProp>> Oproperty, Expression<Func<Entity, IProp>> Iproperty)
        {
            sortingparams.Add(Oproperty.Body.ToString().Remove(0, 2), Iproperty.Body.ToString().Remove(0, 2));
        }

        public PagedList<DTO> GetPagedDTOs(PagedList<Entity> source)
        {
            return new PagedList<DTO>(this.GetDTOs(source.Source.ToList()), source.PageIndex, source.PageSize, source.TotalRows);
        }

        public string SortMap(string col)
        {
            if (string.IsNullOrEmpty(col))
            {
                return col;
            }
            if (sortingparams.ContainsKey(col))
            {
                return sortingparams[col];
            }
            else
            {
                return col;
            }
        }
    }
}