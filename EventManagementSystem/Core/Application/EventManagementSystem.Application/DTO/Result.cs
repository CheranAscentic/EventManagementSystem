using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.DTO
{
    public class Result<T> where T : class
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
            return Results.Json(this, statusCode: this.Status);
        }
    }
}
