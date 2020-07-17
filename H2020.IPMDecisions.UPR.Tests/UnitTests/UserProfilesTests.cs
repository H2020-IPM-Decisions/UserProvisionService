using System;
using AutoMapper;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.UnitTests
{
    public class UserProfilesTests
    {
        [Fact]
        public async void AddNewUserProfile_WrongMediaType_ShouldReturnGenericResponseNoSuccess()
        {
            // Arrange
            Mock<IMapper> mockMapper = new Mock<IMapper>();
            Mock<IDataService> mockDataService = new Mock<IDataService>();        
            Mock<IUrlHelper> mockUrl = new Mock<IUrlHelper>();
            Mock<IPropertyCheckerService> mockPropertyCheckerService = new Mock<IPropertyCheckerService>();
            Mock<IPropertyMappingService> mockPropertyMappingService = new Mock<IPropertyMappingService>();

            var bll = new BusinessLogic(
                mockMapper.Object, 
                mockDataService.Object, 
                mockUrl.Object, 
                mockPropertyCheckerService.Object,
                mockPropertyMappingService.Object);

            UserProfileForCreationDto user = new UserProfileForCreationDto();
            // Act
            var response = await bll.AddNewUserProfile(Guid.NewGuid(), user, "application");

            // Assert
            Assert.False(response.IsSuccessful);
            Assert.Equal("Wrong media type.", response.ErrorMessage);
        }
    }
}