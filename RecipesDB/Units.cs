using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RecipesDB
{
    public class Units
    {
        public int UnitId { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public virtual ICollection<Recipe_Ingredients> RecipeIngredients { get; set; }
        = new List<Recipe_Ingredients>();

        public Units() { }

        public Units(int unitid,
                     string name,
                     string shortname)
        {
            UnitId = unitid;
            Name = name;
            ShortName = shortname;
        }
    }
}
