using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class GenericResponse
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public IActionResult RequestResult { get; set; }
    }

    public class GenericResponse<T> : GenericResponse
    {
        public T Result { get; set; }
    }

    public static class GenericResponseBuilder
    {
        public static GenericResponse<T> Success<T>(T result)
        {
            return new GenericResponse<T>()
            {
                IsSuccessful = true,
                Result = result
            };
        }

        public static GenericResponse Success()
        {
            return new GenericResponse()
            {
                IsSuccessful = true
            };
        }

        public static GenericResponse<T> NoSuccess<T>(T result, string errorMessage = "")
        {
            return new GenericResponse<T>()
            {
                IsSuccessful = false,
                Result = result,
                ErrorMessage = errorMessage,
                RequestResult = new BadRequestObjectResult(new { message = errorMessage })
            };
        }

        public static GenericResponse NoSuccess(string errorMessage = "")
        {
            return new GenericResponse()
            {
                IsSuccessful = false,
                ErrorMessage = errorMessage,
                RequestResult = new BadRequestObjectResult(new { message = errorMessage })
            };
        }

        public static GenericResponse<T> Unauthorized<T>()
        {
            return new GenericResponse<T>()
            {
                IsSuccessful = false,
                RequestResult = new UnauthorizedResult()
            };
        }

        public static GenericResponse<T> NotFound<T>()
        {
            return new GenericResponse<T>()
            {
                IsSuccessful = false,
                RequestResult = new NotFoundResult()
            };
        }

        public static GenericResponse<T> Duplicated<T>(string errorMessage = "")
        {
            return new GenericResponse<T>()
            {
                IsSuccessful = false,
                ErrorMessage = errorMessage,
                RequestResult = new CustomConflictResult(errorMessage)
            };
        }

        public static GenericResponse<T> Accepted<T>(T result)
        {
            return new GenericResponse<T>()
            {
                IsSuccessful = true,
                Result = result,
                RequestResult = new AcceptedResult()
            };
        }
    }
}