using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse<IEnumerable<RiskMapBaseDto>>> GetRiskMapDataSources()
        {
            // ToDo
            // Connect to microservice and get list
            // Map models to dto
            // Return list
            throw new NotImplementedException();
        }

        public Task<GenericResponse<RiskMapFullDetailDto>> GetRiskMapDetailedInformation(string id)
        {
            // Connect to microservice and get one datasource
            // Map model to dto
            // Return model
            throw new NotImplementedException();
        }
    }
}