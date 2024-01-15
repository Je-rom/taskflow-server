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
                return NotFound(ApiResponse.NotFoundException($"Workspace: {workspace.Id}"));

            // Check if the user is permitted to perform this operation
            if (workspace.User == null || workspace.User?.Id != user.Id)
                return Unauthorized(ApiResponse.AuthorizationException("Permission denied"));
            
            var convertUserIdStringToGuid = Guid.TryParse(requestDto.UserId, out Guid resultGuid);
            if (!convertUserIdStringToGuid)
                return BadRequest(ApiResponse.ConflictException($"Badly formatted UserId: {requestDto.UserId}"));
                
            // Fetch user to be Added
            var userToBeAdded = await userRepository.findById(resultGuid);
            if (userToBeAdded == null)
                return NotFound(ApiResponse.NotFoundException($"User not found with the Id: {requestDto.UserId}"));
            
            // Check if the user to added already exists as a member
            var checkIfUserIsAlreadyMember = await workspaceMemberRepository.FindByUserIdAsync(workspace, resultGuid);
            if (checkIfUserIsAlreadyMember != null)
                return BadRequest(ApiResponse.ConflictException($"User with the ID:{resultGuid} is already a member of the workspace"));
            
            //1.  Create a new instance of the model from the DTO
            var workspaceMemberModel = new WorkspaceMember();
            workspaceMemberModel.User = userToBeAdded;
            workspaceMemberModel.Workspace = workspace;

            //2. Save the model using the repository class
            await workspaceMemberRepository.CreateAsync(workspaceMemberModel);
            
            // 3. Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<WorkspaceMemberResponseDto>(workspaceMemberModel)));
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
                return NotFound(ApiResponse.NotFoundException($"Workspace: {workspace.Id}"));
            
            var dto = await workspaceMemberRepository.FindAllAsync(workspace);

            // 3. Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<WorkspaceMemberResponseDto>>(dto)));
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

