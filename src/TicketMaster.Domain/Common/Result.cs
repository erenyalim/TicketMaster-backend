using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Common
{
    public class Result
    {
        public bool IsSuccessful { get; }
        public string? Message { get; }
        public List<string>? ValidationErrors { get; }

        protected Result(bool isSuccessful, string? message, List<string>? validationErrors = null)
        {
            IsSuccessful = isSuccessful;
            Message = message;
            ValidationErrors = validationErrors;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string errorMessage, List<string>? validationErrors = null) => new(false, errorMessage, validationErrors);
    }
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool isSuccessful, T? data, string? message, List<string>? validationErrors = null)
        : base(isSuccessful, message, validationErrors)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string? message = null) => new(true, data, message);
    public static new Result<T> Failure(string errorMessage, List<string>? validationErrors = null) => new(false, default, errorMessage, validationErrors);
}
