using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using System;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Tests.Data
{
    internal static class ItemCheckinCheckoutDataTests
    {
        internal static ItemCheckinCheckoutRequestViewModel GetItemRequest()
            => new ItemCheckinCheckoutRequestViewModel
            {
                WorkshopCheckDate = DateTime.Now,
                Document = "123.456.789-09"
            };
    }
}
