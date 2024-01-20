using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using taskflow.CustomActionFilters;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Request;
using taskflow.Models.DTO.Response;
using taskflow.Models.DTO.Response.Shared;
using taskflow.Repositories.Implementations;
using taskflow.Repositories.Interfaces;

namespace taskflow.Controllers //
{
    [Route("api/workspaces/{workspaceId:guid}/members")]
    [ApiController]
    public class WorkspacesMemberController(
        IWorkspaceMemberRepository workspaceMemberRepository,
        IWorkspaceRepository workspaceRepository,
        IUserRepository userRepository,
        IMapper mapper, ILogger<WorkspacesController> logger
        ) : ControllerBase
    {

        [HttpPost]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> Create([FromRoute] Guid workspaceId, [FromBody] CreateWorksPaceMemberRequestDto requestDto)
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));
            
            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);
            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse
                    .NotFoundException($"Workspace with id: {workspaceId} is either not found or does not belong to the user"));

            // Check if the user is permitted to perform this operation
            if (workspace.User == null || workspace.User?.Id != user.Id)
                return Unauthorized(ApiResponse.AuthorizationException("Permission denied"));
            
            // loop through all the emails, and try adding them to the workspace.
            var memberEmails = requestDto.UserEmails;
            foreach (var memberEmail in memberEmails)
            {
                // validate the memberEmail
                var potentialMemberUser = await userRepository.findByEmail(memberEmail);
                if (potentialMemberUser != null)
                {
                    // Check if the user to added already exists as a member
                    var checkIfUserIsAlreadyMember = await workspaceMemberRepository.FindByUserIdAsync(workspace, Guid.Parse(potentialMemberUser.Id));
                    
                    if (potentialMemberUser.Email != user.Email && checkIfUserIsAlreadyMember == null)
                    {
                        var potentialWorkspaceMember = new WorkspaceMember();
                        potentialWorkspaceMember.User = potentialMemberUser;
                        potentialWorkspaceMember.Workspace = workspace;   
                    
                        // Save the workspace member data.
                        await workspaceMemberRepository.CreateAsync(potentialWorkspaceMember);
                    }
                }
                
            }
            
            var model = await workspaceMemberRepository.FindAllAsync(workspace);
            // 3. Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<WorkspaceMemberResponseDto>>(model)));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromRoute] Guid workspaceId)
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));
            
            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);
            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace: {workspace?.Id}"));
            
            var model = await workspaceMemberRepository.FindAllAsync(workspace);

            // 3. Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<WorkspaceMemberResponseDto>>(model)));
        }

        [HttpGet]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Show([FromRoute] Guid workspaceId, [FromRoute] Guid id)
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));
            
            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);
            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace: {workspace.Id}"));
            
            // Check if the user to added already exists as a member
            var workspaceMember = await workspaceMemberRepository.FindByUserIdAsync(workspace, id);
            if (workspaceMember == null)
                return BadRequest(ApiResponse.ConflictException($"User with the ID:{id} is not a member of the workspace"));
            
            //  Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<WorkspaceMemberResponseDto>(workspaceMember)));
        }

        [HttpDelete]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid workspaceId, [FromRoute] Guid id)
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));
            
            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);
            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace: {workspace.Id}"));
            
            // Check if the user is permitted to perform this operation
            if (workspace.User == null || workspace.User.Id != user.Id)
                return Unauthorized(ApiResponse.AuthorizationException("Permission denied"));
            
            // Check if the user to added already exists as a member
            var workspaceMember = await workspaceMemberRepository.FindByUserIdAsync(workspace, id);
            if (workspaceMember == null)
                return BadRequest(ApiResponse.ConflictException($"User with the ID:{id} is not a member of the workspace"));
            
            // check if the workspace member is the admin of the workspace
            if (workspaceMember.User?.Id == user.Id)
                return BadRequest(ApiResponse.ConflictException("And admin of a workspace cannot be detached"));
            
            var deletedWorkspaceMember = await workspaceMemberRepository.DeleteAsync(workspace, id);
            if (deletedWorkspaceMember == null)
                return BadRequest(ApiResponse.ConflictException($"User with the ID:{id} is not a member of the workspace"));

            return Ok(ApiResponse.SuccessMessage("User detached from the workspace."));
        }

    }
}

