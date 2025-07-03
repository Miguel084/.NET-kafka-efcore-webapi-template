namespace Shared.Domain.Data.Models;

public class ModelsBase
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}