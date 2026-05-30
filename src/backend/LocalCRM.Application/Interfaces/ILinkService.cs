using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface ILinkService
    {
        Task<IEnumerable<TTarget>> GetLinkedEntitiesAsync<TLink, TTarget>(int sourceId, string sourceIdPropertyName) where TLink : class where TTarget : class;
        Task<bool> AddLinkAsync<TLink>(TLink link) where TLink : class;
        Task<bool> RemoveLinkAsync<TLink>(params object[] keyValues) where TLink : class;
    }
}
