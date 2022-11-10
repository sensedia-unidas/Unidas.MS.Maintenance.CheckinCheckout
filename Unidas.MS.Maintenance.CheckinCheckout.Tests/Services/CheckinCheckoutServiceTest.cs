using FluentAssertions;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Tests.Data;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Tests.Services
{
    public class CheckinCheckoutServiceTest
    {
        private Mock<ICheckinCheckoutService> MockService()
        {
            var mock = new Mock<ICheckinCheckoutService>();
            //mock.Setup(x => x.Integrate(It.IsAny<ItemCheckinCheckoutRequestViewModel>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            return mock;
        }
        [Fact]
        public async void ShouldIntegrateRequest()
        {
            var service = MockService().Object;

            var result = await service.Integrate(ItemCheckinCheckoutDataTests.GetItemRequest());

            Assert.NotNull(result);
            result.IsValid.Should().BeTrue();
        }
    }
}
