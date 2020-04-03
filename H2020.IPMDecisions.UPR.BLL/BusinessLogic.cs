using AutoMapper;
using H2020.IPMDecisions.UPR.Data.Core;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        
        public BusinessLogic(
            IMapper mapper,
            IDataService dataService)
        {
            this.mapper = mapper 
                ?? throw new System.ArgumentNullException(nameof(mapper));
            this.dataService = dataService 
                ?? throw new System.ArgumentNullException(nameof(dataService));
        }
    }
}