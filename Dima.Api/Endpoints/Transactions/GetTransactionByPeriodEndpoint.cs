using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Response;
using Dima.Core;
using Microsoft.AspNetCore.Mvc;
using Dima.Core.Requests.Transactions;

namespace Dima.Api.Endpoints.Transactions
{
    public class GetTransactionByPeriodEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapGet("/", HandleAsync)
                .WithName("Transactions: Get By Period")
                .WithSummary("Recupera as transações por um periodo de tempo")
                .WithDescription("Recupera as transações por um periodo de tempo")
                .WithOrder(5)
                .Produces<PagedResponse<List<Transaction?>>>();

        private static async Task<IResult> HandleAsync(ITransactionHandler handler,
                [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null,
                [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
                [FromQuery] int pageSize = Configuration.DefaultPageSize)
        {
            var request = new GetTransactionByPeriodRequest
            {
                UserId = "test@balta.io",
                PageNumber = pageNumber,
                PageSize = pageSize,
                StartDate = startDate,
                EndDate = endDate,
            };

            var result = await handler.GetByPeriod(request);

            return result.IsSuccess
                        ? TypedResults.Ok(result)
                        : TypedResults.BadRequest(result);
        }
    }
}
