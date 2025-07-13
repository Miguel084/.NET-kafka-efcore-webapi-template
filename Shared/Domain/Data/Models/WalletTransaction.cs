using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Domain.Data.Models;

public class WalletTransaction : ModelsBase
{
    [Key]
    public int WalletTransactionId { get; set; }

    public int WalletId { get; set; }

    [JsonIgnore]
    public Wallet Wallet { get; set; } = null!;

    public decimal Amount { get; set; }

    public TransactionTypeEnum TransactionType { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public string? Description { get; set; }

    public string ValidateTransaction()
    {
        if (Amount <= 0)
            return "Amount must be greater than zero.";
        if (Description != null && Description.Length > 500)
            return "Description cannot exceed 500 characters.";
        if (TransactionType < TransactionTypeEnum.Deposit || TransactionType > TransactionTypeEnum.Transfer)
            return "Invalid transaction type.";

        return string.Empty;
    }

    public enum TransactionTypeEnum
    {
        Deposit = 0,
        Withdrawal = 1,
        Transfer = 2
    }
}