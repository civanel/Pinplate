using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class User_Favorites
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public DateTime AddedAt { get; set; }
        public virtual Users User { get; set; } = null!;
        public virtual Recipes Recipe { get; set; } = null!;

        public User_Favorites() { }

        public User_Favorites(int userid,
                            int recipeid,
                            DateTime addedat)
        {
            UserId = userid;
            RecipeId = recipeid;
            AddedAt = addedat;
        }
    }
}
