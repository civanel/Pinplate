using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Ingredients
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Recipe_Ingredients> RecipeIngredients { get; set; }
        = new List<Recipe_Ingredients>();

        public Ingredients() { }

        public Ingredients(int ingredientid,
                           string name)
        {
            IngredientId = ingredientid;
            Name = name;
        }
    }
}
