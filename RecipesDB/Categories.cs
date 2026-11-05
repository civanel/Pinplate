using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Categories
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Recipe_Categories> RecipeCategories { get; set; }
        = new List<Recipe_Categories>();

        public Categories() { }

        public Categories(int categoryid,
                    string name)
        {
            CategoryId = categoryid;
            Name = name;
        }
    }
}
