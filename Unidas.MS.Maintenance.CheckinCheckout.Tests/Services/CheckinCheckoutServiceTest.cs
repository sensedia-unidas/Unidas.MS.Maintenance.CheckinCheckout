using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services.UseCases;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Tests.Data;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Xunit;

namespace Unidas.MS.Maintenance.CheckinCheckout.Application.Tests.Services
{
    public class CheckinCheckoutServiceTest
    {
        private Mock<ISendToAxUseCase> MockSendToAxUseCase(bool isExecuted)
        {
            var mock = new Mock<ISendToAxUseCase>();
            mock.Setup(x => x.Execute(It.IsAny<ItemCheckinCheckoutRequestViewModel>())).ReturnsAsync(isExecuted);

            return mock;
        }

        private Mock<ILogger<CheckinCheckoutService>> MockLogger()
        {
            return new Mock<ILogger<CheckinCheckoutService>>();
        }

        [Fact]
        public async Task ShouldIntegrateRequest()
        {
            var service = new CheckinCheckoutService(MockSendToAxUseCase(true).Object, MockLogger().Object);

            var result = await service.Integrate(ItemCheckinCheckoutDataTests.GetItemRequest());

            Assert.NotNull(result);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldntIntegrateRequest()
        {
            var service = new CheckinCheckoutService(MockSendToAxUseCase(false).Object, MockLogger().Object);

            var result = await service.Integrate(ItemCheckinCheckoutDataTests.GetItemRequest());

            Assert.NotNull(result);
            result.IsValid.Should().BeFalse();
        }
    }
}
