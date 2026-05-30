using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class LinkService : ILinkService
    {
        private readonly LocalCRMContext _context;

        public LinkService(LocalCRMContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TTarget>> GetLinkedEntitiesAsync<TLink, TTarget>(
            IQueryable<TLink> linkQuery,
            Func<TLink, TTarget> selector) where TLink : class where TTarget : class
        {
            var links = await linkQuery.ToListAsync();
            return links.Select(selector);
        }

        public async Task<bool> AddLinkAsync<TLink>(TLink link) where TLink : class
        {
            _context.Set<TLink>().Add(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveLinkAsync<TLink>(TLink link, string username) where TLink : class
        {
            if (link is BaseLink baseLink)
            {
                baseLink.IsDeleted = true;
                baseLink.DeletedAt = DateTime.UtcNow;
                // baseLink.UpdatedBy = username; // BaseLink doesn't have UpdatedBy in my current definition, let's check
            }
            else
            {
                _context.Set<TLink>().Remove(link);
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
