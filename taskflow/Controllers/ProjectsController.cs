using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using taskflow.CustomActionFilters;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Response;
using taskflow.Models.DTO.Request;
using taskflow.Models.DTO.Response.Shared;
using taskflow.Repositories.Interfaces;
using taskflow.Models.DTO;

namespace taskflow.Controllers
{
    [Route("api/workspaces/{workspaceId:guid}/projects")]
    [ApiController]
    public class ProjectController(
        IMapper mapper,
        IProjectRepository projectRepository,
        ILogger<WorkspacesController> logger,
        IUserRepository userRepository,
        IWorkspaceRepository workspaceRepository  
        ) :  ControllerBase {
          
       [HttpPost]
       [Authorize]
       [ValidateModel]
        public async Task<IActionResult> Create([FromRoute] Guid workspaceId, [FromBody] CreateProjectRequestDto projectRequestDto)
        {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await userRepository.findByEmail(userEmail);
                if (user == null)
                    return Unauthorized(ApiResponse.NotFoundException($"Invalid user"));

                // Check if the workspace belongs to the user (you might have a different logic for this)
                var workspace = await workspaceRepository.ShowAsync(workspaceId);

                if (workspace == null ||workspace.User?.Email != userEmail)
                {
                    return NotFound(ApiResponse.NotFoundException($"Workspace not found with the id: {workspaceId}, or does not belong to the user"));
                }

                // Create a new instance of the model from the Dto
                var projectModel = new Project
                {
                    Name = projectRequestDto.Name,
                    Description = projectRequestDto.Description,
                    StartDate = projectRequestDto.StartDate,
                    EndDate = projectRequestDto.EndDate,
                    Workspace = workspace
                };
                
                // Save the model using the repository class
                await projectRepository.CreateAsync(projectModel);
  
                // 3. Convert the model response back to response DTo
                return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectResponseDto>(projectModel)));
        }

        [HttpGet]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> Index([FromRoute] Guid workspaceId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return Unauthorized(ApiResponse.NotFoundException($"Invalid user"));

            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);

            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace not found with the id: {workspaceId}, or does not belong to the user"));

            var projects = workspace.Projects;
            
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<List<ProjectResponseDto>>(projects)));
        }


        [HttpGet]
        [Authorize]
        [ValidateModel]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Show([FromRoute] Guid workspaceId, [FromRoute] Guid id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return Unauthorized(ApiResponse.NotFoundException($"Invalid user"));

            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);

            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace not found with the id: {workspaceId}, or does not belong to the user"));

            var project = await projectRepository.ShowAsync(workspace, id);
            
            // Check if the project does not exist, or not exist within  the workspace
            if (project == null)
                return NotFound(ApiResponse.NotFoundException($"Project not found with thw Id: {id}, or does not belong to the context workspace"));
            
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectResponseDto>(project)));
        }

        [HttpPut]
        [Authorize]
        [ValidateModel]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid workspaceId, [FromRoute] Guid id, [FromBody] UpdateProjectRequestDto requestDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return Unauthorized(ApiResponse.NotFoundException($"Invalid user"));

            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);

            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace not found with the id: {workspaceId}, or does not belong to the user"));

            // Create a new instance of the model from the Dto
            var projectModel = new Project
            {
                Name = requestDto.Name,
                Description = requestDto.Description,
                StartDate = requestDto.StartDate,
                EndDate = requestDto.EndDate,
                Workspace = workspace
            };
            
            var project = await projectRepository.UpdateAsync(workspace, id, projectModel);

            // Check if the project does not exist, or not exist within  the workspace
            if (project == null)
                return NotFound(ApiResponse.NotFoundException($"Project not found with thw Id: {id}, or does not belong to the context workspace"));

            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ProjectResponseDto>(project)));
        }
        
        [HttpDelete]
        [Authorize]
        [ValidateModel]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid workspaceId, [FromRoute] Guid id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmail(userEmail);
            if (user == null)
                return Unauthorized(ApiResponse.NotFoundException($"Invalid user"));

            // Check if the workspace belongs to the user (you might have a different logic for this)
            var workspace = await workspaceRepository.ShowAsync(workspaceId);

            if (workspace == null ||workspace.User?.Email != userEmail)
                return NotFound(ApiResponse.NotFoundException($"Workspace   not found with the id: {workspaceId}, or does not belong to the user"));
            
            var project = await projectRepository.DeleteAsync(workspace, id);

            // Check if the project does not exist, or not exist within  the workspace
            if (project == null)
                return NotFound(ApiResponse.NotFoundException($"Project not found with thw Id: {id}, or does not belong to the context workspace"));

            return Ok(ApiResponse.SuccessMessage("Project deleted!"));
        }
    }
}