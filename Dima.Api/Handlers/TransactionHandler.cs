using Dima.Api.Data;
using Dima.Core.Common.Extensions;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Response;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers
{
    public class TransactionHandler(AppDbContext context) : ITransactionHandler
    {
        public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
        {
            try
            {
                var transaction = new Transaction
                {
                    UserId = request.UserId,
                    Title = request.Title,
                    Amount = request.Amount,
                    PaidOrReceiveAt = request.PaidOrReceived,
                    CreatedAt = DateTime.Now,
                    Type = request.Type,
                    CategoryId = request.CategoryId,
                };

                await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction);
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possível criar a transação");
            }
        }

        public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
        {
            try
            {
                var transaction = await context
                    .Transactions
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "Transação não encontrada");

                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, message: "Transação excluída com sucesso!");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possível excluir a transação");
            }
        }

        public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
        {
            try
            {
                var transaction = await context
                    .Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                return transaction is null
                    ? new Response<Transaction?>(null, 404, "Transação não encontrada")
                    : new Response<Transaction?>(transaction);
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possível recuperar a transação");
            }
        }

        public async Task<PagedResponse<List<Transaction>?>> GetByPeriod(GetTransactionByPeriodRequest request)
        {
            try
            {
                try
                {
                    request.StartDate ??= DateTime.Now.GetFirstDay();
                    request.EndDate ??= DateTime.Now.GetLastDay();

                }
                catch
                {
                    return new PagedResponse<List<Transaction>?>(null, 500, "Não foi possível determinar a data de inicio ou fim");
                }

                var query = context.Transactions.AsNoTracking()
                                                .Where(x => x.CreatedAt >= request.StartDate &&
                                                            x.CreatedAt <= request.EndDate &&
                                                            x.UserId == request.UserId)
                                                .OrderBy(x => x.CreatedAt);

                var transactions = await query
                                       .Skip((request.PageNumber - 1) * request.PageSize)
                                       .Take(request.PageSize)
                                       .ToListAsync();

                var count = await query.CountAsync();

                return new PagedResponse<List<Transaction>?>(
                    transactions,
                    count,
                    request.PageNumber,
                    request.PageSize);
            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 500, "Não foi possível consultar as transações");
            }

        }


        public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
        {
            try
            {
                var transaction = await context
                    .Transactions
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "Transação não encontrada");

                transaction.Title = request.Title;
                transaction.Amount = request.Amount;
                transaction.PaidOrReceiveAt = request.PaidOrReceived;
                transaction.CategoryId = request.CategoryId;
                transaction.Type = request.Type;


                context.Transactions.Update(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, message: "Transãção atualizada com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possível alterar a transação");
            }
        }
    }
}
