using RentFlow.Application.Services;
using RentFlow.Domain.Enums;

namespace RentFlow.Tests.Unit;

public class RentStatusCalculatorTests
{
    [Theory]
    [InlineData(5000, 0, RentPaymentStatus.Unpaid)]
    [InlineData(5000, 1000, RentPaymentStatus.Partial)]
    [InlineData(5000, 5000, RentPaymentStatus.Paid)]
    [InlineData(5000, 7000, RentPaymentStatus.Paid)]
    public void Calculate_Returns_Expected_Status(decimal expectedAmount, decimal paidAmount, RentPaymentStatus expectedStatus)
    {
        var status = RentStatusCalculator.Calculate(expectedAmount, paidAmount);
        Assert.Equal(expectedStatus, status);
    }

    [Fact]
    public void Calculate_Throws_When_PaidAmount_Is_Negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RentStatusCalculator.Calculate(5000, -1));
    }

    [Fact]
    public void Calculate_Throws_When_ExpectedAmount_Is_Invalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RentStatusCalculator.Calculate(0, 0));
    }
}
