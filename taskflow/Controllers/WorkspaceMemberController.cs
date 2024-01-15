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

            var convertUserIdStringToGuid = Guid.TryParse(requestDto.UserId, out Guid resultGuid);
            if (!convertUserIdStringToGuid)
                return BadRequest(ApiResponse.ConflictException($"Badly formatted UserId: {requestDto.UserId}"));
                
            // Fetch user to be Added
            var userToBeAdded = await userRepository.findById(resultGuid);
            if (userToBeAdded == null)
                return NotFound(ApiResponse.NotFoundException($"User not found with the Id: {requestDto.UserId}"));
            
            // Check if the user to added already exists as a member
            var checkIfUserIsAlreadyMember = await workspaceMemberRepository.ShowAsync(workspace, resultGuid);
            if (checkIfUserIsAlreadyMember != null)
                return BadRequest(ApiResponse.ConflictException($"User with the ID:{resultGuid} is already a member of the workspace"));
            
            //1.  Create a new instance of the model from the DTO
            var workspaceMemberModel = new WorkspaceMember();
            workspaceMemberModel.User = userToBeAdded;
            workspaceMemberModel.Workspace = workspace;
            
            // Chekck if the user is already a member of the workspace
            

            //2. Save the model using the repository class
            await workspaceMemberRepository.CreateAsync(workspaceMemberModel);

            var dto = mapper.Map<WorkspaceMemberResponseDto>(workspaceMemberModel);

            // 3. Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(dto));
        }

        /*[HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));

            // Fetch the data with the repo
            var workspaces = user.Workspaces;
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<WorkspaceResponseDto>>(workspaces)));
        }*/

        /*[HttpGet]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Show([FromRoute] Guid id)
        {
            var workspaceMemberModel = await workspaceMemberRepository.ShowAsync(id);
            if (workspaceModel == null)
            {
                return NotFound(ApiResponse.NotFoundException($"Workspace with the id: {id} not found"));
            }
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<WorkspaceResponseDto>(workspaceModel)));
        }*/


        /*
        [HttpPut]
        [Authorize]
        [Route("{id:Guid}")]

        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateWorkspaceMemberRequestDTO requestDto)
        {
            var workspaceMemberDomain = new WorkspaceMember
            {
                Name = requestDto.Name,
                WorkspaceId = requestDto.WorkspaceId
            };
            var workspaceModel = await workspaceRepository.UpdateAsync(id, workspaceDomain);

            if (workspaceMemberModel == workspaceId)
                return NotFound(ApiResponse.NotFoundException($"Workspace with the id: {id} not found"));

            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<WorkspaceResponseDto>(workspaceModel)));

        }
        */

        /*[HttpDelete]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var workspaceMemberDomain = await workspaceMemberRepository.Delete(id);

            if (workspaceMemberDomain == null)
                return NotFound(ApiResponse.NotFoundException($"Workspace with the id: {id} not found"));

            return Ok(ApiResponse.SuccessMessage("Workspace deleted"));
        }*/

    }
}

