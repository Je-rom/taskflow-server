using System.Security.Claims;
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
using taskflow.Repositories.Interfaces;

namespace taskflow.Controllers
{
    [Route("api/workspaces")]
    [ApiController]
    public class WorkspacesController(
        IWorkspaceRepository workspaceRepository,
        TaskFlowDbContext taskFlowDbContext,
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
            var user = await taskFlowDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound(ApiResponse.NotFoundException($"{userEmail}"));
            
            //1.  Create a new instance of the model from the DTO
            var workspaceModel = new Workspace
            {
                Name = requestDto.Name,
                Description = requestDto.Description,
            };

            workspaceModel.User = user;
           
            
            //2. Save the model using the repository class
            var modelResponse = await workspaceRepository.CreateAsync(workspaceModel);
            
            // 3. Convert the model response back to response DTo
            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<WorkspaceResponseDto>(workspaceModel)));
        }
        
        
    }
}

