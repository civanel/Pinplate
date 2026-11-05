using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Recipe_Categories
    {
        public int RecipeId { get; set; }
        public int CategoryId { get; set; }
        public virtual Recipes Recipe { get; set; } = null!;
        public virtual Categories Category { get; set; } = null!;

        public Recipe_Categories() { }

        public Recipe_Categories(int recipeid,
                                 int categoryid)
        {
            RecipeId = recipeid;
            CategoryId = categoryid;
        }
    }
}
