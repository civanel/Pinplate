using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Tags
    {
        public int TagId { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Recipe_Tags> RecipeTags { get; set; }
        = new List<Recipe_Tags>();

        public Tags() { }

        public Tags(int tagid,
                    string name)
        { 
            TagId = tagid;  
            Name = name;
        }
    }
}
