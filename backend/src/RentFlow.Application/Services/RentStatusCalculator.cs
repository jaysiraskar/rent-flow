using RentFlow.Domain.Enums;

namespace RentFlow.Application.Services;

public static class RentStatusCalculator
{
    public static RentPaymentStatus Calculate(decimal expectedAmount, decimal paidAmount)
    {
        if (expectedAmount <= 0) throw new ArgumentOutOfRangeException(nameof(expectedAmount), "Expected amount must be greater than 0.");
        if (paidAmount < 0) throw new ArgumentOutOfRangeException(nameof(paidAmount), "Paid amount cannot be negative.");

        if (paidAmount == 0) return RentPaymentStatus.Unpaid;
        if (paidAmount < expectedAmount) return RentPaymentStatus.Partial;
        return RentPaymentStatus.Paid;
    }
}
