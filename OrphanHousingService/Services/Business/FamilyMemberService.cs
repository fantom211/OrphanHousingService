using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class FamilyMemberService : CrudService<FamilyMember>
    {
        public FamilyMemberService(OrphanHousingDbContext context) : base(context) { }

        public async Task<List<FamilyMember>> GetAllAsync()
        {
            return await _context.FamilyMembers
                .Include(x => x.Contract)
                    .ThenInclude(c => c.Person)
                .ToListAsync();
        }

        public async Task CreateAsync(FamilyMember member)
        {
            member.Id = Guid.NewGuid();

            await base.AddAsync(member);
        }

        public async Task RemoveAsync(Guid id)
        {
            var member = await GetByIdAsync(id);
            if (member == null)
                throw new Exception("Член семьи не найден");

            await base.RemoveAsync(member);
        }
    }
}
