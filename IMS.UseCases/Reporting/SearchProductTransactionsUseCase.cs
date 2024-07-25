using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Reporting
{
    public class SearchProductTransactionsUseCase : ISearchProductTransactionsUseCase
    {
        private readonly IProductTransactionRepository _productTransactionRepository;

        public SearchProductTransactionsUseCase(IProductTransactionRepository productTransactionRepository)
        {
            _productTransactionRepository = productTransactionRepository;
        }

        public async Task<IEnumerable<ProductTransaction>> ExecuteAsync(
            string productName,
            DateTime? dateFrom,
            DateTime? dateTo,
            ProductTransactionType? transactionType)

        {
            if (dateTo.HasValue)
                dateTo = dateTo.Value.Date;

            if (dateTo.HasValue)
                dateTo = dateTo.Value.Date.AddDays(1);

            return await this._productTransactionRepository.GetProductTransactionsAsync(
                productName,
                dateFrom,
                dateTo,
                transactionType);
        }
    }
}