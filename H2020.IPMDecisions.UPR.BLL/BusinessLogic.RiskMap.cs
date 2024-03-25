using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<List<RiskMapBaseDto>>> GetRiskMapDataSources()
        {
            // ToDo
            // Connect to microservice and get list
            // Map models to dto
            // Return list
            List<RiskMapBaseDto> riskMapList = new List<RiskMapBaseDto>
            {
                new RiskMapBaseDto { RiskMapId = "1", Name = "Risk Map 1", Url = "http://example.com/map1" },
                new RiskMapBaseDto { RiskMapId = "2", Name = "Risk Map 2", Url = "http://example.com/map2" },
                new RiskMapBaseDto { RiskMapId = "3", Name = "Risk Map 3", Url = "http://example.com/map3" }
            };

            return GenericResponseBuilder.Success(riskMapList);
        }


        public async Task<GenericResponse<RiskMapFullDetailDto>> GetRiskMapDetailedInformation(string id)
        {
            // Connect to microservice and get one datasource
            // Map model to dto
            // Return model
            RiskMapFullDetailDto riskMap = new RiskMapFullDetailDto
            {
                RiskMapId = "123456",
                Name = "Sample Risk Map",
                Url = "http://example.com/riskmap",
                Title = "The Title",
                Description = "All of EU risk"
            };
            return GenericResponseBuilder.Success(riskMap);
        }
    }
}