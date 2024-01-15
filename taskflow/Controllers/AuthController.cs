using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using taskflow.CustomActionFilters;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Models.DTO;
using taskflow.Models.DTO.Request;
using taskflow.Models.DTO.Response;
using taskflow.Models.DTO.Response.Shared;
using taskflow.Repositories;
using taskflow.Repositories.Interfaces;
using taskflow.Services.Interfaces;

namespace taskflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<User> userManager,
        TaskFlowDbContext dbContext,
        ITokenRepository repository,
        IWorkspaceRepository workspaceRepository,
        IEmailService emailService,
        IUserRepository userRepository,
        IMapper mapper
        ) : ControllerBase {
        
        [HttpPost]
        [Route("register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var user = new User
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username,
                FirstName = registerRequestDto.Firstname,
                LastName = registerRequestDto.Lastname,
                EmailConfirmed = false,
            };

            // check if user exists with the email
            var userExists = await userManager.FindByNameAsync(registerRequestDto.Username);
            if (userExists != null)
                return BadRequest(
                    ApiResponse.ConflictException(
                        $"User already exists with the email: {registerRequestDto.Username}"));
            
            
            var userResult = await userManager.CreateAsync(user, registerRequestDto.Password);

            if (userResult.Succeeded)
            {
                // Generate an email verification token
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

                emailService.SendEmailAsync(user.Email, "Confirm Email",  callbackUrl);
                
                return Ok(ApiResponse.SuccessMessage(callbackUrl));
            }

            return BadRequest(ApiResponse.UnknownException("Something went wrong, try again"));
        }

        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            // FInd user in the database
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest(ApiResponse.NotFoundException("User not found"));
            }

            if (await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(ApiResponse.ConflictException("Email already confirmed"));
            }
            
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<UserDto>(user)));   
            }

            return BadRequest(ApiResponse.GenericException(result.Errors));
        }
        
        [HttpPost]
        [Route("resend-confirmation-token")]
        [ValidateModel]
        public async Task<IActionResult> ResentVerificationToken([FromBody] ResendVerificationTokenRequestDto requestDto)
        {
            var user = await userManager.FindByNameAsync(requestDto.Username);
            if (user == null)
            {
                return BadRequest(ApiResponse.NotFoundException("User not found"));
            }

            if (await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(ApiResponse.ConflictException("Email already confirmed"));
            }
            
            // Generate an email verification token
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);
            
            // Resend email verification link
            emailService.SendEmailAsync(user.Email, "Confirm Email",  callbackUrl);
            
            return Ok(ApiResponse.SuccessMessage("Verification link generated, please check your email for verification link"));
        }
        
        // Login route
        [HttpPost]
        [Route("login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            // FInd the user by email
            var user = await userManager.FindByNameAsync(loginRequestDto.Username);

            // Verify user email or password
            if (user == null || !await userManager.CheckPasswordAsync(user, loginRequestDto.Password))
            {
                return Unauthorized(ApiResponse.AuthenticationException("Invalid email or password"));
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(ApiResponse.AuthorizationException("Email not confirmed"));
            }
            
            var loginResponseDto = new LoginResponseDto
            {
                JwtToken = repository.CreateJwtToken(user),
                User = mapper.Map<UserDto>(user)
            };
                        
            return Ok(ApiResponse.SuccessMessageWithData(loginResponseDto));
        }

        [HttpPost]
        [Route("forgot-password")]
        [ValidateModel]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            var user = await userManager.FindByNameAsync(forgotPasswordRequestDto.Username);
            if (user == null || !await userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal if the user does not exist or not confirmed
                return Ok(ApiResponse.SuccessMessage(
                    "Password reset email sent. If the email exists, you will receive notifications to reset your password"));
            }

            // Generate password reset token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Send email to the user.
            emailService.SendEmailAsync(user.Email, "Reset Password Link",  $"Reset Token: {token}");
            
            // Send password reset link
            return Ok(ApiResponse.SuccessMessage(
                "Password reset email sent. If the email exists, you will receive notifications to reset your password"));
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var user = await userManager.FindByNameAsync(resetPasswordRequestDto.Username);
            if (user == null)
            {
                return BadRequest(ApiResponse.NotFoundException("User not found"));
            }

            var result = await userManager.ResetPasswordAsync(user, resetPasswordRequestDto.Token,
                resetPasswordRequestDto.NewPassword);
            
            if (result.Succeeded)
            {
                return Ok(ApiResponse.SuccessMessage("Password reset successfully"));
            }

            return BadRequest(ApiResponse.GenericException(result.Errors));
        }


        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
             var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.findByEmailDetailed(userEmail);
            if (user == null)
                return Unauthorized(ApiResponse.NotFoundException($"Invalid user"));

            var userDto = mapper.Map<UserDto>(user);
            //userDto.Workspaces = user.Workspaces;
            
            // Return the user data as response.
            return Ok(ApiResponse.SuccessMessageWithData(userDto));
        }
        
        
    }
}

