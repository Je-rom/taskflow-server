using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using taskflow.CustomActionFilters;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Request;
using taskflow.Models.DTO.Response;
using taskflow.Models.DTO.Response.Shared;
using taskflow.Repositories.Implementations;
using taskflow.Repositories.Interfaces;

namespace taskflow.Controllers
{
    [Route("api/projects/{projectId:guid}/tasks")]
    [ApiController]
    public class ProjectTaskController(
         IWorkspaceRepository workspaceRepository,
        IProjectTaskRepository projectTaskRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IMapper mapper, ILogger<ProjectController> logger
        )
        : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> CreateAsync(
           [FromRoute] Guid projectId, Guid workspaceId,
           [FromBody] ProjectTaskRequestDto projectTaskRequestDto
           )
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));

            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);

            if (workspace == null || workspace.User?.Email != userEmail)
                return NotFound(ApiResponse
                    .NotFoundException($"Workspace not found with the id: {workspaceId}, or does not belong to the user"));


            // check if each of the project exists
            var projectTask = await projectRepository.ShowAsync(workspace, projectId);
            if (projectTask == null)
                return NotFound(ApiResponse.NotFoundException($"Project not found with the " +
                    $"id:[{projectId}], or does not belong to the workspace."));


            // Create a new instance of the model from the Dto
            var projectModel = new ProjectTask
            {
                Name = projectTaskRequestDto.Name,
                Description = projectTaskRequestDto.Description,
                StartDate = (DateTime)projectTaskRequestDto.StartDate,
                EndDate = (DateTime)projectTaskRequestDto.EndDate,
               
            };

            // Save the model using the repository class
            await projectTaskRepository.CreateAsync(projectModel);

            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<ProjectTaskResponseDto>>(projectTask)));





        }

        [HttpGet]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Show([FromRoute] Guid projectId, [FromRoute] Guid id)
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));

            // check if the project exists
            var project = await projectRepository.FindById(projectId);
            if (project == null)
                return NotFound(ApiResponse.NotFoundException($"Project not found with the " +
                                                              $"id:[{projectId}]"));

            var workspace = await workspaceRepository.ShowAsync(project.Workspace.Id);

            // Check if the user is already added to the project or the owner of thew workspace.
           // var userExist = await projectMemberRepository.ShowAsync(project, Guid.Parse(user.Id));
           // if ((userExist == null) && workspace.User?.Email != userEmail)
            //    return Unauthorized(ApiResponse.AuthorizationException("The current user is not a member of the project"));


            // fetch the project task
            var task = await projectTaskRepository.ShowAsync(project, id);

            // Send the response back to user containing the created model.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectTaskResponseDto>(task)));
        }
    }

}

