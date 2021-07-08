using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class AdminVariableRepository : IAdminVariableRepository
    {
        private ApplicationDbContext context;
        public AdminVariableRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<AdministrationVariable>> FindAllAsync()
        {
            return await this
                .context
                .AdministrationVariable
                .ToListAsync<AdministrationVariable>();
        }

        public async Task<AdministrationVariable> FindById(AdminValuesEnum id)
        {
            return await this
                .context
                .AdministrationVariable
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public void Update(AdministrationVariable entity)
        {
            this.context.AdministrationVariable.Update(entity);
        }
    }
}