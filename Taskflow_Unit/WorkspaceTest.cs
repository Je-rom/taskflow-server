using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using taskflow.Controllers;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Response;
using taskflow.Repositories.Interfaces;


namespace Taskflow_Unit
{
    public class WorkspaceTest
    {
        [Fact]
    
        public async Task Show_Returns_NotFoundResult_With_Invalid_Id()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedWorkspaceModel = new Workspace(); // Replace with the actual model type
            var expectedDto = new WorkspaceResponseDto(); // Replace with the actual DTO type

            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            workspaceRepositoryMock.Setup(repo => repo.ShowAsync(id)).ReturnsAsync((Workspace)null);

            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();

            var userRepositoryMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<WorkspacesController>>();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<WorkspaceResponseDto>(expectedWorkspaceModel)).Returns(expectedDto);

            var controller = new WorkspacesController(
                                                         workspaceRepositoryMock.Object,
                                                         userRepositoryMock.Object,
                                                         workspaceMemberRepositoryMock.Object,
                                                         mapperMock.Object,
                                                         loggerMock.Object
                                                     );

            // Act
            var result = await controller.Show(id);
            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);


        }
        [Fact]
        public async Task Show_Returns_OkResult_With_Valid_Id()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedWorkspaceModel = new Workspace();
            var expectedDto = new WorkspaceResponseDto();

            var workspaceRepositoryMock = new Mock<IWorkspaceRepository>();
            workspaceRepositoryMock.Setup(repo => repo.ShowAsync(id)).ReturnsAsync(expectedWorkspaceModel);

            var workspaceMemberRepositoryMock = new Mock<IWorkspaceMemberRepository>();
            // Assu

            var userRepositoryMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<WorkspacesController>>();

            var mapperMock = new Mock<IMapper>(); // Replace with the actual mapper service
            mapperMock.Setup(mapper => mapper.Map<WorkspaceResponseDto>(expectedWorkspaceModel)).Returns(expectedDto);

            var controller = new WorkspacesController(
                                    workspaceRepositoryMock.Object,
                                    userRepositoryMock.Object,
                                    workspaceMemberRepositoryMock.Object,
                                    mapperMock.Object,
                                    loggerMock.Object
                                );

            // Act
            var result = await controller.Show(id);




            // if result.status == success
            // result.data?.Id == id 


            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);


            // var responseData = Assert.IsType<ApiResponse>(okObjectResult.Value);

            Assert.NotNull(okObjectResult);


        }


    }
}
