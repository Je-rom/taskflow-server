using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskflow.CustomActionFilters;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Request;
using taskflow.Models.DTO.Response;
using taskflow.Models.DTO.Response.Shared;
using taskflow.Repositories.Implementations;
using taskflow.Repositories.Interfaces;

namespace taskflow.Controllers //
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspacesMemberController(
        IWorkspaceMemberRepository workspaceRepository,
        IUserRepository userRepository,
        IMapper mapper, ILogger<WorkspacesController> logger
        ) : ControllerBase
    {

        [HttpPost]
        [ValidateModel]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceRequestDto requestDto)
        {
            // Get the authenticated user's ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));

            //1.  Create a new instance of the model from the DTO
            var workspaceMemberModel = new WorkspaceMember
            {
                Name = requestDto.Name,
                WorkspaceId = requestDto.WorkspaceId,
            };
            workspaceMemberModel.User = user;

            //2. Save the model using the repository class
            await workspaceMemberRepository.CreateAsync(workspaceMemberModel);

            // 3. Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<WorkspaceMemberResponseDto>(workspaceMemberModel)));
        }

        [HttpGet]
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
        }

        [HttpGet]
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
        }


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

        [HttpDelete]
        [Authorize]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var workspaceMemberDomain = await workspaceMemberRepository.Delete(id);

            if (workspaceMemberDomain == null)
                return NotFound(ApiResponse.NotFoundException($"Workspace with the id: {id} not found"));

            return Ok(ApiResponse.SuccessMessage("Workspace deleted"));
        }

    }
}

