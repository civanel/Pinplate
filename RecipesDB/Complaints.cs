using System;

namespace RecipesDB;

public class Complaints
{
    public int ComplaintId { get; set; }
    public int ReporterId { get; set; }
    public int RecipeId { get; set; }
    public string Reason { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Открыта";
    public DateTime CreatedAt { get; set; }

    // Навигационные свойства
    public Users Reporter { get; set; } = null!;
    public Recipes Recipe { get; set; } = null!;
}