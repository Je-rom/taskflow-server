using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using taskflow.CustomActionFilters;
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
        ITokenRepository repository,
        IEmailService emailService,
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

            var userResult = await userManager.CreateAsync(user, registerRequestDto.Password);

            if (userResult.Succeeded)
            {
                // Generate an email verification token
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

                emailService.SendEmailAsync(user.Email, "Confirm Email",  callbackUrl);
                
                return Ok(ApiResponse.SuccessMessage("Account created, please check your email for verification link"));
            }

            return BadRequest(ApiResponse.UnknownException("Something went wrong, try again"));
        }

        [HttpPost]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromRoute] string userId, [FromRoute] string token)
        {
            // FInd user in the database
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest(ApiResponse.NotFoundException("User not found"));
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ApplicationUserDto>(user)));   
            }

            return BadRequest(ApiResponse.GenericException(result.Errors));
        }
        
        [HttpPost]
        [Route("resend-verification-token")]
        public async Task<IActionResult> ResentVerificationToken([FromBody] ResendVerificationTokenRequestDto requestDto)
        {
            var user = await userManager.FindByEmailAsync(requestDto.Email);
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

            /*if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(ApiResponse.AuthorizationException("Email not confirmed"));
            }*/
            
            var loginResponseDto = new LoginResponseDto
            {
                JwtToken = repository.CreateJwtToken(user),
                User = mapper.Map<ApplicationUserDto>(user)
            };
                        
            return Ok(ApiResponse.SuccessMessageWithData(loginResponseDto));
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);
            if (user == null || !await userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal if the user does not exist or not confirmed
                return Ok(ApiResponse.SuccessMessage(
                    "Password reset email sent. If the email exists, you will receive notifications to reset your password"));
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Auth", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

            // Send password reset link
            return Ok(ApiResponse.SuccessMessage(
                "Password reset email sent. If the email exists, you will receive notifications to reset your password"));
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var user = await userManager.FindByIdAsync(resetPasswordRequestDto.UserId);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest(ApiResponse.AuthenticationException("Unauthenticated user"));
            }
            
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(ApiResponse.NotFoundException("User not found"));
            }

            return Ok(ApiResponse.SuccessMessageWithData(mapper.Map<ApplicationUserDto>(user)));
        }
        
        
    }
}

