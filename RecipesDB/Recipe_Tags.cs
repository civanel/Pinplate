using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Recipe_Tags
    {
        public int RecipeId { get; set; }
        public int TagId { get; set; }
        public virtual Recipes Recipe { get; set; } = null!;
        public virtual Tags Tag { get; set; } = null!;

        public Recipe_Tags() { }

        public Recipe_Tags(int recipeid,
                            int tagid) 
        {
            RecipeId = recipeid;
            TagId = tagid;
        }
    }
}
