using System;
using AutoMapper;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IUrlHelper url;
        private readonly IPropertyCheckerService propertyCheckerService;
        private readonly IPropertyMappingService propertyMappingService;

        public BusinessLogic(
            IMapper mapper,
            IDataService dataService,
            IUrlHelper url,
            IPropertyCheckerService propertyCheckerService,
            IPropertyMappingService propertyMappingService)
        {
            this.mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
            this.url = url
                ?? throw new ArgumentNullException(nameof(url));
            this.propertyCheckerService = propertyCheckerService
                ?? throw new ArgumentNullException(nameof(propertyCheckerService));
            this.propertyMappingService = propertyMappingService 
                ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }
    }
}