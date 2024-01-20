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
           [FromRoute] Guid projectId,
           [FromBody] ProjectTaskRequestDto projectTaskRequestDto
           )
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
                    $"id:[{projectId}], or does not belong to the workspace."));
              
            // Create a new instance of the model from the Dto
            var projectModel = new ProjectTask
            {
                Name = projectTaskRequestDto.Name,
                Description = projectTaskRequestDto.Description,
               
            };

            // Save the model using the repository class
            await projectTaskRepository.CreateAsync(projectModel);

            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<ProjectTaskResponseDto>>(projectModel)));



        }

        
    }

}

