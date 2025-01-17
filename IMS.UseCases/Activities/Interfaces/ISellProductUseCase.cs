﻿using IMS.CoreBusiness;

namespace IMS.UseCases.Activities
{
    public interface ISellProductUseCase
    {
        Task ExecuteAsync(string salesOrderNumber, Product product, int quantity, decimal unitPrice, string doneBy);
    }
}