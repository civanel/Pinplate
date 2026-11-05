using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Recipe_Ingredients
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public int UnitId { get; set; }
        public decimal Count { get; set; }
        public virtual Recipes Recipe { get; set; } = null!;
        public virtual Ingredients Ingredient { get; set; } = null!;
        public virtual Units Unit { get; set; } = null!;

        public Recipe_Ingredients() { }

        public Recipe_Ingredients(int recipeid,
                                  int ingredientid,
                                  int unitid,
                                  decimal count,
                                  Recipes recipe,
                                  Ingredients ingredient,
                                  Units unit) 
        { 
            RecipeId = recipeid;
            IngredientId = ingredientid;
            UnitId = unitid;
            Count = count;
            Recipe = recipe;
            Ingredient = ingredient;
            Unit = unit;
        }
    }
}
