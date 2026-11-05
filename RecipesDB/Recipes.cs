using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecipesDB
{
    public class Recipes
    {
        public int RecipeId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Portion { get; set; }
        public int CookTimeMinutes { get; set; }
        public string Instruction { get; set; } = null!;
        public int? AuthorId { get; set; }
        public string Status { get; set; } = "Pending";
        public string? RejectReason { get; set; }
        public DateTime? RejectedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsTop { get; set; } = false;

        public virtual Users? Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Recipe_Ingredients> RecipeIngredients { get; set; }
        = new List<Recipe_Ingredients>();
        public virtual ICollection<Recipe_Images> RecipeImages { get; set; }
        = new List<Recipe_Images>();
        public virtual ICollection<Recipe_Categories> RecipeCategories { get; set; }
        = new List<Recipe_Categories>();
        public virtual ICollection<Recipe_Tags> RecipeTags { get; set; }
        = new List<Recipe_Tags>();
        public virtual ICollection<User_Favorites> UserFavorites { get; set; }
        = new List<User_Favorites>();
        public virtual ICollection<Recipe_Reviews> RecipeReviews { get; set; }
        = new List<Recipe_Reviews>();

        public Recipes() { }

        public Recipes(int recipeId,
                       string name,
                       string description,
                       int portion,
                       int cooktimeminutes,
                       string instruction,
                       DateTime createdat)
        {
            RecipeId = recipeId;
            Name = name;
            Description = description;
            Portion = portion;
            CookTimeMinutes = cooktimeminutes;
            Instruction = instruction;
            CreatedAt = createdat;
        }
    } 
}