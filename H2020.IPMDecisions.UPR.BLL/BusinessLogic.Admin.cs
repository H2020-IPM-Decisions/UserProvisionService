using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IEnumerable<AdminVariableDto>>> GetAllAdminVariables()
        {
            try
            {
                var adminVariables = await this.dataService.AdminVariables.FindAllAsync();
                var variablesToReturn = this.mapper.Map<IEnumerable<AdminVariableDto>>(adminVariables);
                return GenericResponseBuilder.Success<IEnumerable<AdminVariableDto>>(variablesToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAdminVariables. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<AdminVariableDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> UpdateAdminVariableById(AdminValuesEnum id, AdminVariableForManipulationDto adminVariableForManipulationDto)
        {
            try
            {
                var adminVariable = await this.dataService.AdminVariables.FindByIdAsync(id);
                if (adminVariable == null)
                {
                    return GenericResponseBuilder.NotFound<AdministrationVariable>();
                }

                this.mapper.Map(adminVariableForManipulationDto, adminVariable);
                this.dataService.AdminVariables.Update(adminVariable);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateAdminVariable. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IEnumerable<DisabledDssDto>>> GetAllDisabledDss()
        {
            try
            {
                var listOfDssFromMicroservice = await this.internalCommunicationProvider.GetAllListOfDssFromDssMicroservice();
                List<DssInformationJoined> linkDssOnLocation = new List<DssInformationJoined>();
                var listOfDssAndModels = listOfDssFromMicroservice
                    .SelectMany(d => d.DssModelInformation, (dss, model) => new DssInformationJoined { DssInformation = dss, DssModelInformation = model });

                var dataToReturn = this.mapper.Map<IEnumerable<DisabledDssDto>>(listOfDssAndModels);
                var listOfDisableDss = await this.dataService.DisabledDss.GetAllAsync();

                if (listOfDisableDss.Count > 0)
                {
                    foreach (var disabledDss in listOfDisableDss)
                    {
                        var selectDss = dataToReturn.FirstOrDefault(d =>
                            d.DssId == disabledDss.DssId
                            & d.DssVersion == disabledDss.DssVersion
                            & d.DssModelId == disabledDss.DssModelId
                            & d.DssModelVersion == disabledDss.DssModelVersion);
                        if (selectDss != null)
                        {
                            selectDss.IsDisabled = true;
                            selectDss.Id = disabledDss.Id;
                        };
                    }
                }
                return GenericResponseBuilder.Success<IEnumerable<DisabledDssDto>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllDisabledDss. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<DisabledDssDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> RemoveDisabledDssFromListAsync(List<Guid> ids)
        {
            try
            {
                await this.dataService.DisabledDss.Delete(ids);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddDisabledDssFromListAsync. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IEnumerable<DisabledDssDto>>> AddDisabledDssFromListAsync(IEnumerable<DisabledDssForCreationDto> listOfDisabledDssDto)
        {
            try
            {
                var dataToInsert = this.mapper.Map<List<DisabledDss>>(listOfDisabledDssDto);
                await this.dataService.DisabledDss.Create(dataToInsert);
                await this.dataService.CompleteAsync();

                var dataToReturn = await GetAllDisabledDss();
                return GenericResponseBuilder.Success<IEnumerable<DisabledDssDto>>(dataToReturn.Result);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddDisabledDssFromListAsync. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<DisabledDssDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}