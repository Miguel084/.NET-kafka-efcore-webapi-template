using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Data.Models;

public class Address
{

    [Key]
    public int Id { get; set; }

    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    public ICollection<Users> Users { get; set; } = new List<Users>();

    public string? ValidateAddress()
    {
        if (string.IsNullOrWhiteSpace(Street))
            return "Street cannot be empty.";
        if (string.IsNullOrWhiteSpace(City))
            return "City cannot be empty.";
        if (string.IsNullOrWhiteSpace(State))
            return "State cannot be empty.";
        if (string.IsNullOrWhiteSpace(ZipCode))
            return "ZipCode cannot be empty.";
        if (Street.Length > 200)
            return "Street cannot exceed 200 characters.";
        if (City.Length > 100)
            return "City cannot exceed 100 characters.";
        if (State.Length > 100)
            return "State cannot exceed 100 characters.";
        if (ZipCode.Length > 20)
            return "ZipCode cannot exceed 20 characters.";

        return null;
    }
}