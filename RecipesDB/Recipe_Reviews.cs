using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Recipe_Reviews
    { 
        public int ReviewId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public virtual Recipes Recipe { get; set; } = null!;
        public virtual Users User { get; set; } = null!;

        public Recipe_Reviews() { }

        public Recipe_Reviews(int reviewid,
                            int recipeid,
                            int userid,
                            int rating,
                            string comment,
                            DateTime createdat) 
        {
            ReviewId = reviewid;
            RecipeId = recipeid;
            UserId = userid;
            Rating = rating;
            Comment = comment;
            CreatedAt = createdat;
        }
    }
}
