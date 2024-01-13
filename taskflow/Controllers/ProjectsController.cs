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
    public class ProjectController(IMapper mapper,
                                    IProjectRepository projectRepository,
                                    ILogger<WorkspacesController> logger,
                                     IUserRepository userRepository,
                                      IWorkspaceRepository workspaceRepository  
                                         ) :  ControllerBase
    {
          
       [HttpPost]
       [Authorize]
       [ValidateModel]
        public async Task<IActionResult> Create([FromRoute] Guid workspaceId,  [FromBody] CreateProjectRequestDto projectRequestDto)
        {
           
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                // Check if the workspace belongs to the user (you might have a different logic for this)
                var workspace = await workspaceRepository.ShowAsync(workspaceId);

                if (workspace == null ||workspace.User?.Email != userEmail)
                {
                    return NotFound(workspace);
                }

                // Create a new instance of the model from the Dto
                var projectModel = new Project
                {
                    Name = projectRequestDto.Name,
                    Description = projectRequestDto.Description,
                    Workspace = workspace
                };

                // Save the model using the repository class
                await projectRepository.CreateAsync(projectModel);

                // Convert the model response back to DTO
                //var projectDto = mapper.Map<ProjectResponseDto>(projectModel);

                return Ok(ApiResponse.SuccessMessageWithData(projectModel));
            
        }
        
    }
}