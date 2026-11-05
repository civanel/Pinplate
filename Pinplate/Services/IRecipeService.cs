using RecipesDB;

namespace Pinplate.Services;

public interface IRecipeService
{
    Task<IReadOnlyList<Recipes>> GetPendingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Recipes>> GetRejectedAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Recipes>> GetTopAsync(CancellationToken ct = default);
    Task<Recipes?> GetByIdAsync(int id, CancellationToken ct = default);
    Task ApproveAsync(int recipeId, CancellationToken ct = default);
    Task RejectAsync(int recipeId, string reason, CancellationToken ct = default);
    Task ReturnToReviewAsync(int recipeId, CancellationToken ct = default);
    Task AddToTopAsync(int recipeId, CancellationToken ct = default);
    Task DeleteAsync(int recipeId, string reason, CancellationToken ct = default);
}