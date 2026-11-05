using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Recipe_Images
    {
        public int ImageId { get; set; }
        public int RecipeId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
        public string Caption { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public virtual Recipes Recipe { get; set; } = null!;

        public Recipe_Images() { }

        public Recipe_Images(int imageid,
                            int recipeid,
                            string imageurl,
                            bool isprimary,
                            int sortorder,
                            string caption,
                            DateTime createdat) 
        { 
            ImageId = imageid;
            RecipeId = recipeid;
            ImageUrl = imageurl;
            IsPrimary = isprimary;
            SortOrder = sortorder;
            Caption = caption;
            CreatedAt = createdat;
        }
    }
}
