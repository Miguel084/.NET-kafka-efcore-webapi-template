using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain.Data.Models;

public class Users : ModelsBase
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Address? Address { get; set; }

    public Wallet? Wallet { get; set; }

    public string ValidateUser()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return "Name cannot be empty.";
        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            return "Email is invalid.";
        if (Email.Length > 255)
            return "Email cannot exceed 255 characters.";
        if (Address != null)
        {
            var addressValidation = Address.ValidateAddress();
            if (!string.IsNullOrEmpty(addressValidation))
                return addressValidation;
        }
        if (Name.Length > 100)
            return "Name cannot exceed 100 characters.";
        if (Email.Length > 255)
            return "Email cannot exceed 255 characters.";
        if (Name.Length < 2)
            return "Name must be at least 2 characters long.";
        if (Email.Length < 5)
            return "Email must be at least 5 characters long.";
        if (Email.Length > 320)
            return "Email cannot exceed 320 characters.";

        return string.Empty;
    }

    public decimal GetWalletBalance()
    {
        return Wallet?.Transactions.Sum(t => t.TransactionType == WalletTransaction.TransactionTypeEnum.Deposit ? t.Amount : -t.Amount) ?? 0;
    }

    public List<WalletTransaction> GetWalletTransactions()
    {
        return Wallet?.Transactions ?? new List<WalletTransaction>();
    }

}