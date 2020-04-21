using System;
using AutoMapper;
using H2020.IPMDecisions.UPR.Data.Core;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        private readonly IMapper mapper;
        private readonly IDataService dataService;
        private readonly IUrlHelper url;
        
        public BusinessLogic(
            IMapper mapper,
            IDataService dataService,
            IUrlHelper url)
        {
            this.mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
            this.url = url
                ?? throw new ArgumentNullException(nameof(url));
        }
    }
}