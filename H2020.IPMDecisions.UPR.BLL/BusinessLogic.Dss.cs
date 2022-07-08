using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse<IEnumerable<DssInformation>>> GetAllAvailableDssOnFarmLocation(DssListFilterDto dssListFilterDto)
        {
            throw new NotImplementedException();
        }
    }
}