using System.Runtime.InteropServices.JavaScript;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using taskflow.CustomActionFilters;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Request;
using taskflow.Models.DTO.Response;
using taskflow.Models.DTO.Response.Shared;
using taskflow.Repositories.Interfaces;

namespace taskflow.Controllers
{
    [Route("api/projects/{projectId:guid}/tasks")]
    [ApiController]
    public class ProjectTaskController(
        IWorkspaceRepository workspaceRepository,
        IProjectTaskRepository projectTaskRepository,
        IProjectMemberRepository projectMemberRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IMapper mapper, ILogger<ProjectController> logger
        ) : ControllerBase {

        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> CreateAsync(
           [FromRoute] Guid projectId,
           [FromBody] CreateProjectTaskRequestDto createProjectTaskRequestDto
           ) {
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
            
            // Check if the logged in user owns the workspace
            if (project.Workspace == null || project.Workspace.User?.Email != userEmail)
                return Unauthorized(ApiResponse
                    .AuthorizationException($"Invalid Operation!"));
            
            // Find the user to be assigned to the project.
            var assignee = await projectMemberRepository.ShowAsync(project, createProjectTaskRequestDto.UserId);
            if (assignee == null)
                return NotFound(ApiResponse
                    .NotFoundException(
                        $"User does not exist with the id: {createProjectTaskRequestDto.UserId} or has not yet been addes to the project"));
            
            // Check if the start date is in the future.
            if (createProjectTaskRequestDto.StartDate < DateTime.UtcNow)
                return BadRequest(ApiResponse.ConflictException("StartDate can only be present or Future"));
            
            // Check if the end date is in the future.
            if (createProjectTaskRequestDto.EndDate < DateTime.UtcNow)
                return BadRequest(ApiResponse.ConflictException("EndDate can only be present or Future"));

            // Check if the start date is less thab EndData.
            if (createProjectTaskRequestDto.EndDate < createProjectTaskRequestDto.StartDate)
                return BadRequest(ApiResponse.ConflictException("StartDate cannot be greater that future date"));
            
            // Create a new instance of the model from the Dto
            var projectModel = new ProjectTask
            { 
                Name = createProjectTaskRequestDto.Name,
                Description = createProjectTaskRequestDto.Description,
                StartDate = createProjectTaskRequestDto.StartDate,
                EndDate = createProjectTaskRequestDto.EndDate,
                Stage = createProjectTaskRequestDto.Stage,
                ProjectMember = assignee,
                Project = project
            };

            // Save the model using the repository class
            await projectTaskRepository.CreateAsync(projectModel);

            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectTaskResponseDto>(projectModel)));
        }
        
        // Fetch all projects tasks 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromRoute] Guid projectId)
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
            
            // Find if the user is a member of the project
            var assignee = await projectMemberRepository.ShowAsync(project, Guid.Parse(user.Id));
            if (assignee == null && project.Workspace.User.Email != userEmail)
                return Unauthorized(ApiResponse
                    .AuthorizationException(
                        $"User is not a member of the project to the project"));


            var tasks = await projectTaskRepository.FindAllAsync(project);
            
            // Send the response back to user containing the data list.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<ProjectTaskResponseDto>>(tasks)));
        }


        // Show project task
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
                                                              $"id:[{projectId}], or does not belong to the workspace."));
            
            // Find if the user is a member of the project
            var assignee = await projectMemberRepository.ShowAsync(project, Guid.Parse(user.Id));
            if (assignee == null && project.Workspace.User.Email != userEmail)
                return Unauthorized(ApiResponse
                    .AuthorizationException(
                        $"User is not a member of the project to the project"));

            // fetch the project members
            var task = await projectTaskRepository.ShowAsync(project, id);
            if (task == null)
                return NotFound(ApiResponse.NotFoundException($"Task not found with the id: {id}, " +
                                                              $"or does not belong to the project"));
            
            // Send the response back to user containing the created model.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectTaskResponseDto>(task)));
        }
        
        // Update project task
        [HttpPut]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid projectId, [FromRoute] Guid id, UpdateProjectTaskRequestDto requestDto)
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
            
            // Check if the logged in user owns the workspace
            if (project.Workspace == null || project.Workspace.User?.Email != userEmail)
                return Unauthorized(ApiResponse
                    .AuthorizationException($"Invalid Operation!"));
            
            // Find the user to be assigned to the project.
            var assignee = await projectMemberRepository.ShowAsync(project, requestDto.UserId);
            if (assignee == null)
                return NotFound(ApiResponse
                    .NotFoundException(
                        $"User does not exist with the id: {requestDto.UserId} or has not yet been addes to the project"));
            
            // Check if the start date is in the future.
            if (requestDto.StartDate < DateTime.UtcNow)
                return BadRequest(ApiResponse.ConflictException("StartDate can only be present or Future"));
            
            // Check if the end date is in the future.
            if (requestDto.EndDate < DateTime.UtcNow)
                return BadRequest(ApiResponse.ConflictException("EndDate can only be present or Future"));

            // Check if the start date is less thab EndData.
            if (requestDto.EndDate < requestDto.StartDate)
                return BadRequest(ApiResponse.ConflictException("StartDate cannot be greater that future date"));
            
            // Create a new instance of the model from the Dto
            var projectModel = new ProjectTask
            { 
                Name = requestDto.Name,
                Description = requestDto.Description,
                StartDate = requestDto.StartDate,
                EndDate = requestDto.EndDate,
                ProjectMember = assignee,
                Stage = requestDto.Stage,
                Project = project
            };

            
            var task = await projectTaskRepository.UpdateAsync(project, id, projectModel);
            if (task == null)
                return NotFound(ApiResponse.NotFoundException($"Task not found with the id: {id}, " +
                                                              $"or does not belong to the project"));
            
            // Send the response back to user containing the created model.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectTaskResponseDto>(task)));
        }

        
        // delete project task
        [HttpDelete]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid projectId, [FromRoute] Guid id)
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
            
            // Check if the logged in user owns the workspace
            if (project.Workspace == null || project.Workspace.User?.Email != userEmail)
                return Unauthorized(ApiResponse
                    .AuthorizationException($"Invalid Operation!"));
            
            var task = await projectTaskRepository.DeleteAsync(project, id);
            if (task == null)
                return NotFound(ApiResponse.NotFoundException($"Task not found with the id: {id}, " +
                                                              $"or does not belong to the project"));

            return Ok(ApiResponse.SuccessMessage("Task deleted successfully"));
        }

    }

}

