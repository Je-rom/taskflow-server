using System.Collections;
using System.Security.Claims;
using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using taskflow.CustomActionFilters;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Request;
using taskflow.Models.DTO.Response;
using taskflow.Models.DTO.Response.Shared;
using taskflow.Repositories.Implementations;
using taskflow.Repositories.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace taskflow.Controllers
{
    [Route("api/projects/{projectId:guid}/members")]
    [ApiController]
    public class ProjectMemberContoller(
        IWorkspaceRepository workspaceRepository,
        IProjectMemberRepository projectMemberRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IMapper mapper, ILogger<ProjectController> logger
        ) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> CreateAsync(
            [FromRoute] Guid projectId,
            [FromBody] ProjectMemberRequestDto projectMemberRequestDto
            ) {

            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));

            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(Guid.Parse(projectMemberRequestDto.WorkspaceId));
            if (workspace == null || workspace.User?.Email != userEmail)
                return NotFound(ApiResponse
                    .NotFoundException($"Workspace with id: {projectMemberRequestDto.WorkspaceId} is either" +
                                       $" not found or does not belong to the user"));


            // Check if the user is permitted to perform this operation
            if (workspace.User == null || workspace.User?.Id != user.Id)
                return Unauthorized(ApiResponse.AuthorizationException("Permission denied"));


            // check if each of the project exists
            var project = await projectRepository.ShowAsync(workspace, projectId);
            if (project == null)
                return NotFound(ApiResponse.NotFoundException($"Project not found with the " +
                    $"id:[{projectId}], or does not belong to the workspace."));


            // Check if the user ids is a member of the workspace.
            var userIds = projectMemberRequestDto.UserIds;
            var invalidUserIds = new List<string>();
            var invalidWorkspaceMemberIds = new List<string>();
            

            foreach (var userId in userIds)
            {
                // Validate the Guids
                var validId = Guid.TryParse(userId, out Guid guidUserId);
                if (!validId)
                {
                  invalidUserIds.Add(userId);   
                  continue;
                }
                
                // Only someone within the workspace can be added to a project
                var member = await workspaceMemberRepository.FindByUserIdAsync(workspace, guidUserId);
                if (member == null)
                    invalidWorkspaceMemberIds.Add(userId);
            }
            // Check is invalid Ids were found
            if (invalidUserIds.Count > 0)
                return BadRequest(ApiResponse
                    .ConflictException($"Invalid ids found in the payload :[{string.Join(", ", invalidUserIds)}]"));
            
            // Check if a non member of the workspace is found withing the payload
            if (invalidWorkspaceMemberIds.Count > 0)
                return BadRequest(ApiResponse
                    .ConflictException($"Non member of the workspace found in the payload :[{string.Join(", ", invalidWorkspaceMemberIds)}]"));
            
            // Check if the user is already added to the project
            var existingUserIds = new List<string>();
            foreach (var userId in userIds)
            {
                // Validate the Guids
                var validId = Guid.TryParse(userId, out Guid guidUserId);
                if (!validId)
                {
                    invalidUserIds.Add(userId);   
                    continue;
                }
                // Check if the user is already added to the project
                var userExist = await projectMemberRepository.ShowAsync(project, guidUserId);
                if (userExist != null)
                    existingUserIds.Add(userId);
            }
            
            if (existingUserIds.Count > 0)
                return BadRequest(ApiResponse
                    .ConflictException($"User with ids: [{string.Join(", ", existingUserIds)}] already added to the project"));
            
            foreach (var userId in userIds)
            {
                // Validate the Guids
                 Guid.TryParse(userId, out Guid guidUserId);
                // Fetch the user data
                var userToBeAdded = await userRepository.findById(guidUserId);
                
                // Crate a project member model fron request dto
                var projectMemberModel = new ProjectMember();
                projectMemberModel.Project = project;
                projectMemberModel.User = userToBeAdded;

                // Save the model to the db through the repository
                await projectMemberRepository.CreateAsync(projectMemberModel);

            }
            
            // Fetch the list of members of the projects
            var members = await projectMemberRepository.FindAllAsync(project);
            
            // Send the response back to user containing the created models.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<ProjectMemberResponseDto>>(members)));
        }
        
        
        // Fetch all project members
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
                                                              $"id:[{projectId}]"));
            
            var workspace = await workspaceRepository.ShowAsync(project.Workspace.Id);
            
            // Check if the user is already added to the project or the owner of thew workspace.
            var userExist = await projectMemberRepository.ShowAsync(project, Guid.Parse(user.Id));
            if ((userExist == null) && workspace.User?.Email != userEmail)
                return Unauthorized(ApiResponse.AuthorizationException("The current user is not a member of the project"));
            
            // fetch the project members
            var members = await projectMemberRepository.FindAllAsync(project);
            
            // Send the response back to user containing the data list.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<ProjectMemberResponseDto>>(members)));
        }
        
        
        // Show project members
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
            var userExist = await projectMemberRepository.ShowAsync(project, Guid.Parse(user.Id));
            if ((userExist == null) && workspace.User?.Email != userEmail)
                return Unauthorized(ApiResponse.AuthorizationException("The current user is not a member of the project"));

            
            // fetch the project members
            var member = await projectMemberRepository.ShowAsync(project, id);
            
            // Send the response back to user containing the created model.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectMemberResponseDto>(member)));
        }
        
        // Detach project member
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
                                                              $"id:[{projectId}]"));

            // Fetch the workspace from the DB
            var workspace = await workspaceRepository.ShowAsync(project.Workspace.Id);

            // Check if the current user is the admin of the workspace
            if (workspace.User?.Email != userEmail)
                return Unauthorized(
                    ApiResponse.AuthorizationException("Only and Admin of workspace can remove project"));

              // Check if the user is already added to the project
            var deletedMember = await projectMemberRepository.DeleteAsync(project, id);
            if (deletedMember == null)
                return NotFound(ApiResponse
                    .NotFoundException($"Project member(user) not found with the id: {id}"));

            return Ok(ApiResponse.SuccessMessage("Member detached."));
        }

    }

}
