using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IEnumerable<DssInformation>>> GetAllAvailableDssOnFarmLocation(DssListFilterDto dssListFilterDto)
        {
            try
            {
                if (string.IsNullOrEmpty(dssListFilterDto.CropCodes))
                {
                    return GenericResponseBuilder.NoSuccess<IEnumerable<DssInformation>>(null, "Please select at least one crop");
                }

                var dssList = internalCommunicationProvider.GetAllListOfDssFilteredByCropsFromDssMicroservice(dssListFilterDto.CropCodes);
                var weatherParameters = internalCommunicationProvider.GetWeatherParametersAvailableByLocation(dssListFilterDto.LocationLongitude, dssListFilterDto.LocationLatitude);
                var eppoCodes = GetAllEppoCodes();

                var dssListResult = await dssList;
                var weatherParametersResult = await weatherParameters;
                var eppoCodesResult = await eppoCodes;
                //JOIN AND PROFIT...
                // Add message
                return GenericResponseBuilder.Success<IEnumerable<DssInformation>>(null);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllAvailableDssOnFarmLocation. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<DssInformation>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}