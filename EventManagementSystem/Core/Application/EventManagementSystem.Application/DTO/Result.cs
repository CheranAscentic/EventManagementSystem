namespace EventManagementSystem.Application.DTO
{
    using EventManagementSystem.Domain.Interfaces;
    using Microsoft.AspNetCore.Http;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Result<T>
        where T : class
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public int Status { get; set; }

        public string? Error { get; set; }

        public T? Value { get; set; }

        public Result()
        {
        }

        public Result(bool success, string? message, int status, T? data, string? error = null)
        {
            this.IsSuccess = success;
            this.Message = message;
            this.Status = status;
            this.Value = data;
            this.Error = error;
        }

        public static Result<T> Success(string? message, T? data, int? status)
        {
            return new Result<T>(true, message ?? "Success", status ?? 200, data);
        }

        public static Result<T> Failure(string? message, T? data, int? status, string error)
        {
            return new Result<T>(false, message ?? "Failure", status ?? 500, data ?? null, error ?? "Internal Server Error");
        }

        public static Result<T> Ok(T? data, string? message = null)
        {
            return new Result<T>(true, message ?? "Success", 200, data);
        }

        public IResult ToApiResult()
        {
            // Handle cases where no transformation is needed
            if (this.Value == null || this.Value is IsDto || !this.IsSuccess)
            {
                return Results.Json(this, statusCode: this.Status);
            }

            // Transform HasDto to DTO while preserving the original Result structure
            if (this.Value is HasDto hasDto)
            {
                var transformedResult = new Result<object>
                {
                    IsSuccess = this.IsSuccess,
                    Message = this.Message,
                    Status = this.Status,
                    Error = this.Error,
                    Value = hasDto.ToDto(),
                };
                return Results.Json(transformedResult, statusCode: this.Status);
            }

            // Handle collections: convert each item to DTO if applicable
            if (this.Value is IEnumerable enumerable && !(this.Value is string))
            {
                var convertedCollection = ConvertCollectionToDtos(enumerable);
                var transformedResult = new Result<object>
                {
                    IsSuccess = this.IsSuccess,
                    Message = this.Message,
                    Status = this.Status,
                    Error = this.Error,
                    Value = convertedCollection,
                };
                return Results.Json(transformedResult, statusCode: this.Status);
            }

            // Default case - return as is
            return Results.Json(this, statusCode: this.Status);
        }

        private static List<object> ConvertCollectionToDtos(IEnumerable collection)
        {
            var resultList = new List<object>();
            foreach (var item in collection)
            {
                if (item is HasDto hasDto)
                {
                    resultList.Add(hasDto.ToDto());
                }
                else if (item is IsDto)
                {
                    resultList.Add(item);
                }
                else
                {
                    resultList.Add(item);
                }
            }
            return resultList;
        }
    }
}
