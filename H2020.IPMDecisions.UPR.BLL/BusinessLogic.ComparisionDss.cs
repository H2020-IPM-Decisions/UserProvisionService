using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IEnumerable<FieldDssResultDetailedDto>>> CompareDssByIds(List<Guid> ids, Guid userId, int daysDataToReturn)
        {
            try
            {
                var first5Items = ids.Take(5);
                var listOfDss = await this.dataService.FieldCropPestDsses.FindAllAsync(
                    d => first5Items.Contains(d.Id)
                    & d.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId == userId);
                if (listOfDss == null || listOfDss.Count() == 0) return GenericResponseBuilder.NotFound<IEnumerable<FieldDssResultDetailedDto>>();

                var dataToReturn = new List<FieldDssResultDetailedDto>();
                foreach (var dss in listOfDss)
                {
                    dataToReturn.Add(await CreateDetailedResultToReturn(dss, daysDataToReturn));
                }
                return GenericResponseBuilder.Success<IEnumerable<FieldDssResultDetailedDto>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CompareDssByIds. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<FieldDssResultDetailedDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}