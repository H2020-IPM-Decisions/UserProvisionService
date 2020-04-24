// using System;
// using System.Collections.Generic;
// using AutoMapper;
// using H2020.IPMDecisions.IDP.UPR.Core;
// using H2020.IPMDecisions.UPR.BLL;
// using H2020.IPMDecisions.UPR.Core.Dtos;
// using H2020.IPMDecisions.UPR.Core.Entities;
// using H2020.IPMDecisions.UPR.Core.Profiles;
// using H2020.IPMDecisions.UPR.Core.Services;
// using H2020.IPMDecisions.UPR.Data.Core;
// using H2020.IPMDecisions.UPR.Data.Core.Repositories;
// using H2020.IPMDecisions.UPR.Data.Persistence;
// using H2020.IPMDecisions.UPR.Data.Persistence.Repositories;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Moq;
// using Xunit;

// namespace H2020.IPMDecisions.UPR.Tests.UnitTests
// {
//     public class UserProfilesTests
//     {

//         private ApplicationDbContext testDbContext;

//         [Fact]
//         public async void AddNewUserProfile_WrongMediaType_ShouldReturnGenericResponseNoSuccess()
//         {
//             // Arrange
//             Mock<IMapper> mockMapper = new Mock<IMapper>();
//             Mock<IDataService> mockDataService = new Mock<IDataService>();        
//             Mock<IUrlHelper> mockUrl = new Mock<IUrlHelper>();
//             Mock<IPropertyCheckerService> mockPropertyCheckerService = new Mock<IPropertyCheckerService>();          

//             var bll = new BusinessLogic(mockMapper.Object, mockDataService.Object, mockUrl.Object, mockPropertyCheckerService.Object);

//             UserProfileForCreationDto user = new UserProfileForCreationDto();
//             // Act
//             var response = await bll.AddNewUserProfile(Guid.NewGuid(), user, "application");

//             // Assert
//             Assert.Equal(response.IsSuccessful, false);
//             Assert.Equal(response.ErrorMessage, "Wrong media type.");
//         }

//         [Fact]
//         public async void AddNewUserProfile_ExistingUser_ShouldReturnGenericResponseNoSuccess()
//         {
//             // Arrange
//             var userId = Guid.NewGuid();
//             UserProfile existingUser = new UserProfile()
//             {
//                 Id = userId,
//                 FirstName = "1"
//             };

//             var mockMapper = new Mock<IMapper>();

//             var myProfile = new MainProfile();
//             var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
//             var mapper = new Mapper(configuration);


//             var mockUrl = new Mock<IUrlHelper>();
//             var mockPropertyCheckerService = new Mock<IPropertyCheckerService>();
//             var mockPropertyMappingService = new Mock<IPropertyMappingService>();
//             var mockDataService = new Mock<IDataService>();
//             var mockContext = new Mock<IApplicationDbContext>(); 

//             var dat = new DataService(testDbContext, mockPropertyMappingService.Object);

//             var bll = new BusinessLogic(mapper, dat, mockUrl.Object, mockPropertyCheckerService.Object);

//             var user = new UserProfileForCreationDto(){
//                 FirstName = "1"
//             };

//             var response = await bll.AddNewUserProfile(userId, user, "application/json");



//             //     var mockContext = new Mock<IApplicationDbContext>();

//             //     await mockContext.Object.UserProfile.AddAsync(existingUser);
//             //     var _mockRepository = new Mock<IUserProfileRepository>();
//             //     var _users = new List<UserProfile>();

//             //     var x = _mockRepository.Setup(m => m.Create(existingUser));

//             //    var mockDataService = new Mock<IDataService>();
//             //     mockDataService.SetupGet(u => u.UserProfiles).Returns(_mockRepository.Object);


//             // _mockRepository.Object.
//             // var response = await bll.AddNewUserProfile(userId, user, "application/json");
//             // var xr = new UserProfileRepository(mockContext.Object, mockPropertyMappingService.Object);

//             // var mockUoW = new Mock<IDataService>();
//             // mockUoW.SetupGet(u => u.UserProfiles).Returns(_mockRepository.Object);

//             // var 

//             // 
//             // Mock<IDataService> mockDataService = new Mock<IDataService>();
//             // Mock<IUserProfileRepository> mockRepository = new Mock<IUserProfileRepository>();
//             // mockDataService.SetupGet(u => u.UserProfiles).Returns(mockRepository.Object);






//             // // Act
//             // 
//             // UserProfile existingUser = new UserProfile(){
//             //     Id = userId,
//             //     FirstName = "1"
//             // };
//             // mockRepository.Setup(u => u.Create(existingUser))/;
//             // await mockDataService.Object.CompleteAsync();

//             // UserProfileForCreationDto user = new UserProfileForCreationDto();          
//             // var response = await bll.AddNewUserProfile(userId, user, "application/json");

//             // // Assert
//             // Assert.Equal(response.IsSuccessful, false);
//             // Assert.Contains(response.ErrorMessage, "User profile aready exits.");
//         }
//     }
// }