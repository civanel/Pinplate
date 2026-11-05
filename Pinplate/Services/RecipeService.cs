using Microsoft.EntityFrameworkCore;
using RecipesDB;

namespace Pinplate.Services;

public class RecipeService : IRecipeService
{
    private readonly DBContext _db;

    public RecipeService(DBContext db) => _db = db;

    public async Task<IReadOnlyList<Recipes>> GetAllAsync(CancellationToken ct = default)
        => await _db.Recipes.AsNoTracking().OrderByDescending(r => r.CreatedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Recipes>> GetPendingAsync(CancellationToken ct = default)
        => await _db.Recipes.AsNoTracking().Where(r => r.Status == "Pending").OrderBy(r => r.CreatedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Recipes>> GetRejectedAsync(CancellationToken ct = default)
        => await _db.Recipes.AsNoTracking().Where(r => r.Status == "Rejected").OrderByDescending(r => r.RejectedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Recipes>> GetTopAsync(CancellationToken ct = default)
        => await _db.Recipes.AsNoTracking().Where(r => r.Status == "Approved" && r.IsTop).OrderByDescending(r => r.CreatedAt).ToListAsync(ct);

    public async Task<Recipes?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Recipes.AsNoTracking()
            .Include(r => r.RecipeImages)
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
            .FirstOrDefaultAsync(r => r.RecipeId == id, ct);

    public async Task ApproveAsync(int recipeId, CancellationToken ct = default)
    {
        var r = await _db.Recipes.FirstOrDefaultAsync(x => x.RecipeId == recipeId, ct)
                ?? throw new InvalidOperationException("Рецепт не найден");
        r.Status = "Approved";
        r.ApprovedAt = DateTime.UtcNow;
        r.RejectReason = null;
        r.RejectedAt = null;
        await _db.SaveChangesAsync(ct);
    }

    public async Task RejectAsync(int recipeId, string reason, CancellationToken ct = default)
    {
        var r = await _db.Recipes.FirstOrDefaultAsync(x => x.RecipeId == recipeId, ct)
                ?? throw new InvalidOperationException("Рецепт не найден");
        r.Status = "Rejected";
        r.RejectReason = reason;
        r.RejectedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task ReturnToReviewAsync(int recipeId, CancellationToken ct = default)
    {
        var r = await _db.Recipes.FirstOrDefaultAsync(x => x.RecipeId == recipeId, ct)
                ?? throw new InvalidOperationException("Рецепт не найден");
        r.Status = "Pending";
        r.RejectReason = null;
        r.RejectedAt = null;
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddToTopAsync(int recipeId, CancellationToken ct = default)
    {
        var r = await _db.Recipes.FirstOrDefaultAsync(x => x.RecipeId == recipeId, ct)
                ?? throw new InvalidOperationException("Рецепт не найден");
        r.IsTop = true;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int recipeId, string reason, CancellationToken ct = default)
    {
        var r = await _db.Recipes.FirstOrDefaultAsync(x => x.RecipeId == recipeId, ct);
        if (r is null) return;
        _db.Recipes.Remove(r);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Recipes> CreateAsync(string name, string description, int portion, int cookMinutes, string instruction, CancellationToken ct = default)
    {
        var r = new Recipes
        {
            Name = name,
            Description = description,
            Portion = portion,
            CookTimeMinutes = cookMinutes,
            Instruction = instruction,
            CreatedAt = DateTime.UtcNow
        };
        _db.Recipes.Add(r);
        await _db.SaveChangesAsync(ct);
        return r;
    }
}