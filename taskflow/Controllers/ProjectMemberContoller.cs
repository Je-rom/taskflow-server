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
        public async Task<IActionResult> CreateAsync([FromRoute] Guid projectId, [FromBody] ProjectMemberRequestDto projectMemberrequestDto)
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));

            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(projectMemberrequestDto.WorkspaceId);
            if (workspace == null || workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace: {workspace.Id}"));

            // Check if the user is permitted to perform this operation
            if (workspace.User == null || workspace.User?.Id != user.Id)
                return Unauthorized(ApiResponse.AuthorizationException("Permission denied"));


            // check if each of the project exists
            var project = await projectRepository.ShowAsync(workspace, projectId);
            if (project == null)
                return NotFound(ApiResponse.NotFoundException($"Project not found with the " +
                    $"id:[{projectId}], or does not belong to the workspace."));


            // Check if workspace member exists
            var workspaceMembersUserIds = projectMemberrequestDto.UserId;
            var invalidWorkspaceMemberUserIds = new List<Guid>();

            foreach (var memberUserId in workspaceMembersUserIds)
            {

                // Check if workspace member belongs to the project's workspace
                var member = await workspaceMemberRepository.FindByUserIdAsync(workspace, memberUserId);
                if (member != null)
                {
                    invalidWorkspaceMemberUserIds.Add(memberUserId);
                    continue;
                }
            }

            if (invalidWorkspaceMemberUserIds.Count > 0)
                return NotFound(ApiResponse.NotFoundException($"Projects found with the " +
                    $"ids:[{invalidWorkspaceMemberUserIds}] within the workspace."));


            // Crate a project member model fron request dto
            var projectMemberModel = new ProjectMember();
            projectMemberModel.Project = project;
            projectMemberModel.User = user;

            // Save the model to the db through the repository
            await projectMemberRepository.CreateAsync(projectMemberModel);

            // Send the response back to user containing the created model.
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectMemberResponseDto>(projectMemberModel)));
        }


    }

}
