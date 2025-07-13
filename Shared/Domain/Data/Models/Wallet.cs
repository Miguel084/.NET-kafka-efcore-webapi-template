using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain.Data.Models;

public class Wallet
{
    [Key]
    public int WalletId { get; set; }

    public int UserId { get; set; }

    public required Users User { get; set; }

    public List<WalletTransaction>? Transactions { get; set; } = new();
}