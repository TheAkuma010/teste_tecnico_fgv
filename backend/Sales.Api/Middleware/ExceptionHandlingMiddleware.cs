using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Sales.Application.Exceptions;
using Sales.Domain.Exceptions;

namespace Sales.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Erro não tratado durante a requisição.");

            await HandleExceptionAsync(
                context,
                exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException =>
                ((int)HttpStatusCode.NotFound, exception.Message),

            ConflictException =>
                ((int)HttpStatusCode.Conflict, exception.Message),

            ArgumentException =>
                ((int)HttpStatusCode.BadRequest, exception.Message),

            DomainException =>
                ((int)HttpStatusCode.BadRequest, exception.Message),

            SqlException sqlException when IsDuplicateCnpj(
                sqlException) =>
                ((int)HttpStatusCode.Conflict,
                    "Já existe um cliente cadastrado com este CNPJ."),

            _ =>
                (
                    (int)HttpStatusCode.InternalServerError,
                    "Ocorreu um erro interno ao processar a solicitação."
                )
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            statusCode,
            message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    private static bool IsDuplicateCnpj(SqlException exception)
    {
        return exception.Number is 2601 or 2627;
    }
}