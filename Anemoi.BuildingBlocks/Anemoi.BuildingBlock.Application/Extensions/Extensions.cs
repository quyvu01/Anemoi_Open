using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.ApplicationModels;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using DynamicExpresso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using OneOf;
using Polly;
using Polly.Retry;
using RandomStringCreator;

namespace Anemoi.BuildingBlock.Application.Extensions;

public static class Extensions
{
    public static string GetToken(this HttpContext context) => GetToken(context?.Request);

    public static string GetToken(this HttpRequest httpRequest)
    {
        var authorizationHeader = httpRequest?.Headers[HeaderNames.Authorization] ?? StringValues.Empty;
        return authorizationHeader == StringValues.Empty
            ? string.Empty
            : authorizationHeader.Single().Split(" ").Last();
    }

    public static string GetUserId(this HttpContext httpContext) =>
        GetClaimValue(httpContext, "id");

    public static List<string> GetUserRoles(this HttpContext httpContext)
    {
        var token = httpContext?.GetToken();
        return token is not { Length: > 0 }
            ? Array.Empty<string>().ToList()
            : GetClaimValues(httpContext, "role")?.ToList() ?? [];
    }

    public static ApplicationPolicy GetApplicationPolicy(this HttpContext httpContext)
    {
        const string prefix = "applicationPolicy";
        return httpContext?.User.Claims
            .Where(x => x.Type.Contains(prefix)).Select(x => new ApplicationPolicy(x.Type, x.Value)).FirstOrDefault();
    }


    public static string GetWorkspaceId(this HttpContext httpContext)
    {
        var token = httpContext?.GetToken();
        return token is not { Length: > 0 } ? null : GetClaimValue(httpContext, "workspaceId");
    }

    public static string GetUserFirstName(this HttpContext httpContext) =>
        GetClaimValue(httpContext, JwtRegisteredClaimNames.GivenName);

    public static string GetUserLastName(this HttpContext httpContext) =>
        GetClaimValue(httpContext, JwtRegisteredClaimNames.FamilyName);

    public static IEnumerable<string> GetClaimValuesFromToken(this string token, string claimType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenReader = tokenHandler.ReadJwtToken(token);
        return tokenReader.Claims.Where(x => x.Type == claimType).Select(x => x.Value);
    }

    public static string GetEmailFromToken(this string token) =>
        GetClaimValuesFromToken(token, "email").FirstOrDefault();

    public static string GetClaimValueFromToken(this string token, string claim) =>
        GetClaimValuesFromToken(token, claim).FirstOrDefault();

    public static string GetUserIdFromToken(this string token) => GetClaimValuesFromToken(token, "id").FirstOrDefault();

    public static string GetClaimValue(this HttpContext httpContext, string type)
    {
        var endpoint = httpContext?.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is { } &&
            string.IsNullOrEmpty(httpContext.GetToken())) return default;
        var jwtSecureToken = GetJwtSecureToken(httpContext);
        var claim = jwtSecureToken?.Claims?.FirstOrDefault(x => x.Type == type);
        return claim?.Value;
    }

    private static IEnumerable<string> GetClaimValues(this HttpContext httpContext, string type)
    {
        var endpoint = httpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is { }) return default;
        var jwtSecureToken = GetJwtSecureToken(httpContext);
        var claims = jwtSecureToken.Claims.Where(x => x.Type == type);
        return claims.Select(a => a.Value);
    }

    public static string GenerateKey(this string phrase)
    {
        if (phrase is null) return null;
        var str = ConvertToUnSign(phrase).ToLower();
        // invalid chars           
        str = Regex.Replace(str, @"\s+", " ").Trim();
        str = Regex.Replace(str, " ", ""); // hyphens   
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        return str;
    }

    public static string GenerateSearchHint(this string phrase)
    {
        if (phrase is null) return null;
        var str = ConvertToUnSign(phrase).ToLower();
        // invalid chars           
        str = Regex.Replace(str, @"\s+", " ").Trim();
        str = Regex.Replace(str, @"\s", "-"); // hyphens   
        return str;
    }

    public static string GenerateSlug(this string phrase)
    {
        if (phrase is null) return null;
        var hint = GenerateSearchHint(phrase);
        hint = Regex.Replace(hint, @"[^a-z0-9\s-]", "");
        var newRandom = new StringCreator().Get(13);
        return hint.Length > 60 ? $"{hint[..60]}-{newRandom}" : $"{hint}-{newRandom}";
    }


    private static string ConvertToUnSign(string src)
    {
        var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        var temp = src.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
    }

    public static AsyncRetryPolicy ExponentialRetryPolicy<TException>(int retryCount,
        Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan> onRetry) where TException : Exception =>
        Policy.Handle<TException>().WaitAndRetryAsync(retryCount, sleepDurationProvider, onRetry);

    public static AsyncRetryPolicy ExponentialRetryPolicy<TException>(Func<int, TimeSpan> sleepDurationProvider,
        Action<Exception, TimeSpan> onRetry) where TException : Exception =>
        Policy.Handle<TException>().WaitAndRetryForeverAsync(sleepDurationProvider, onRetry);

    public static Expression<Func<T, TProp>> And<T, TProp>(this Expression<Func<T, TProp>> srcExp,
        Expression<Func<T, TProp>> extExp)
        => ExpressionHelper.AndAlso(srcExp, extExp);

    public static Expression<Func<T, TProp>> Or<T, TProp>(this Expression<Func<T, TProp>> srcExp,
        Expression<Func<T, TProp>> extExp)
        => ExpressionHelper.OrElse(srcExp, extExp);

    private static JwtSecurityToken GetJwtSecureToken(HttpContext httpContext)
    {
        var token = httpContext?.GetToken();
        if (string.IsNullOrEmpty(token)) return default;
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ReadJwtToken(token);
    }

    public static bool HasAny<T>(this List<T> src) => src is { Count: > 0 };

    public static bool HasAny<T>(this IEnumerable<T> src, Func<T, bool> predict) => src?.Any(predict) == true;

    public static async Task<OneOf<T0, T1>> ToTaskOneOf<T0, T1>(this OneOf<Task<T0>, Task<T1>> src) =>
        src switch
        {
            { IsT0: true } => await src.AsT0,
            { IsT1: true } => await src.AsT1,
            _ => throw new UnreachableException()
        };

    public static IQueryable<T> Offset<T>(this IQueryable<T> src, int? skip) =>
        skip is null ? src : src.Skip(skip.Value);

    public static IQueryable<T> Limit<T>(this IQueryable<T> src, int? take) =>
        take is null ? src : src.Take(take.Value);

    public static IOrderedQueryable<T> OrderByWithDynamic<T>(this IQueryable<T> src, string fieldName,
        Expression<Func<T, object>> defaultOrError,
        SortedDirection sortedDirection)
    {
        Func<Expression<Func<T, object>>, SortedDirection?, IOrderedQueryable<T>> getOrdered =
            (expression, direction) => direction is SortedDirection.Ascending
                ? src.OrderBy(expression)
                : src.OrderByDescending(expression);
        if (string.IsNullOrEmpty(fieldName)) return getOrdered.Invoke(defaultOrError, sortedDirection);
        try
        {
            var interpreter = new Interpreter(InterpreterOptions.DefaultCaseInsensitive);
            var expressionStr = $"x.{fieldName}";
            var expressionFilter = interpreter.ParseAsExpression<Func<T, object>>(expressionStr, "x");
            return getOrdered.Invoke(expressionFilter, sortedDirection);
        }
        catch (Exception)
        {
            return getOrdered.Invoke(defaultOrError, sortedDirection);
        }
    }

    public static IFindFluent<T, T> OrderByWithDynamic<T>(this IFindFluent<T, T> src, string fieldName,
        Expression<Func<T, object>> defaultOrError,
        SortedDirection sortedDirection)
    {
        Func<Expression<Func<T, object>>, SortedDirection?, IFindFluent<T, T>> getOrdered =
            (expression, direction) => direction is null or SortedDirection.Ascending
                ? src.SortBy(expression)
                : src.SortByDescending(expression);
        if (string.IsNullOrEmpty(fieldName)) return getOrdered.Invoke(defaultOrError, sortedDirection);
        try
        {
            var interpreter = new Interpreter();
            var expressionStr = $"x.{fieldName}";
            var expressionFilter = interpreter.ParseAsExpression<Func<T, object>>(expressionStr, "x");
            return getOrdered.Invoke(expressionFilter, sortedDirection);
        }
        catch (Exception)
        {
            return getOrdered.Invoke(defaultOrError, sortedDirection);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> src, Action<T> action)
    {
        if (src is null) return;
        foreach (var item in src) action?.Invoke(item);
    }

    public static void ForEach<T>(this IEnumerable<T> src, Action<T, int> action)
    {
        if (src is null) return;
        foreach (var item in src.Select((value, index) => (value, index))) action?.Invoke(item.value, item.index);
    }

    public static void IteratorVoid<T>(this IEnumerable<T> src) => src.ForEach(_ => { });

    public static string GetName<T>(this T value) where T : struct => Enum.GetName(typeof(T), value);

    public static DateTime EndOfDay(this DateTime date) =>
        new(date.Year, date.Month, date.Day, 23, 59, 59, 999);

    public static DateTime StartOfDay(this DateTime date) =>
        new(date.Year, date.Month, date.Day, 0, 0, 0, 0);

    public static DateTime ToUtc(this DateTime dateTime) => TimeZoneInfo.ConvertTimeToUtc(dateTime);

    public static ErrorDetailResponse ToErrorDetailResponse(this ErrorDetail errorDetail) => new()
    {
        Messages = errorDetail.Messages, Code = errorDetail.Code
    };

    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled)
            .Trim()
            .ToLower();
    }

    public static void Forget(this Task task)
    {
        // note: this code is inspired by a tweet from Ben Adams: https://twitter.com/ben_a_adams/status/1045060828700037125
        // Only care about tasks that may fault (not completed) or are faulted,
        // so fast-path for SuccessfullyCompleted and Canceled tasks.
        if (!task.IsCompleted || task.IsFaulted)
        {
            // use "_" (Discard operation) to remove the warning IDE0058: Because this call is not awaited, execution of the current method continues before the call is completed
            // https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/discards?WT.mc_id=DT-MVP-5003978#a-standalone-discard
            _ = ForgetAwaited(task);
        }

        return;

        // Allocate the async/await state machine only when needed for performance reasons.
        // More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/?WT.mc_id=DT-MVP-5003978
        static async Task ForgetAwaited(Task task)
        {
            try
            {
                // No need to resume on the original SynchronizationContext, so use ConfigureAwait(false)
                await task.ConfigureAwait(false);
            }
            catch
            {
                // Nothing to do here
            }
        }
    }

    public static string CreatePassword(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var res = new StringBuilder();
        var rnd = new Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }

        return res.ToString();
    }

    public static string CleanSpace(this string content)
    {
        var strings = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var contentFinal = string.Join(" ", strings);
        return contentFinal;
    }
}