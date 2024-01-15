using Microsoft.AspNetCore.Mvc;

namespace taskflow.Models.DTO.Response.Shared
{
    public class ApiResponse
    {
        public static object SuccessMessage(string message)
        {
            return new
            {
                Status = Status.SUCCESS,
                Message = message
            };
        }
        
        public static object SuccessMessageWithData(object data)
        {
            return new
            {
                Status = Status.SUCCESS,
                Data = data
            };
        }
        
        /*public static object Collection(List<object> data)
        {
            return new
            {
                Status = Status.SUCCESS,
                Result = data.Length,
                Data = data
            };
        }*/
        
        public static object Failure(string status, object message)
        {
            return new
            {
                Status = status,
                Message = message
            };
        }
        
        public static object UnknownException(object message)
        {
            return new
            {
                Status = Status.UNKNOWN_ERROR,
                Message = message
            };
        }
        
        public static object NotFoundException(object message)
        {
            return new
            {
                Status = Status.NOT_FOUND_ERROR,
                Message = message
            };
        }
        
        public static object GenericException(object message)
        {
            return new
            {
                Status = Status.ERROR,
                Message = message
            };
        }
        
        public static object ConflictException(object message)
        {
            return new
            {
                Status = Status.CONFLICT_ERROR,
                Message = message
            };
        }
        
        public static object AuthorizationException(object message)
        {
            return new
            {
                Status = Status.AUTHORIZATION_ERROR,
                Message = message
            };
        }
        
        public static object NewAuthorizationException(object message)
        {
            return new ContentResult
            {
                StatusCode = 403,
                Content = "Permission Denied: You do not have permission to access this resource.",
                ContentType = "text/plain",
            };
        }
        
        public static object AuthenticationException(object message)
        {
            return new
            {
                Status = Status.AUTHENTICATION_ERROR,
                Message = message
            };
        }
    }
}

