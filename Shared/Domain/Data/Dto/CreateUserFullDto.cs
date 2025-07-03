namespace Shared.Domain.Data.Dto;

public class CreateUserFullDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public AddressDto? Address { get; set; }

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
        if (Email.Length > 320)
            return "Email cannot exceed 320 characters.";
        if (Name.Length < 2)
            return "Name must be at least 2 characters long.";
        if (Email.Length < 5)
            return "Email must be at least 5 characters long.";

        return string.Empty;
    }
}