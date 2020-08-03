using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using System;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse> RequestDataShare(Guid userId, DataShareRequestDto dataShareRequestDto, string mediaType)
        {
            throw new NotImplementedException();
        }

        #region Helpers
        
        #endregion
    }
}