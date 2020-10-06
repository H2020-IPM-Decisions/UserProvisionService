using H2020.IPMDecisions.UPR.Data.Core.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class FieldCropPestDssRepository : IFieldCropPestDssRepository
    {
        private ApplicationDbContext context;

        public FieldCropPestDssRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
    }
}