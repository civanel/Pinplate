using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB
{
    public class Users
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; } = false;
        public DateTime? BannedUntil { get; set; }
        public string? BanReason { get; set; }

        public virtual ICollection<Recipes> Recipes { get; set; } = new List<Recipes>();
        public virtual ICollection<User_Favorites> UserFavorites { get; set; }
        = new List<User_Favorites>();
        public virtual ICollection<Recipe_Reviews> RecipeReviews { get; set; }
        = new List<Recipe_Reviews>();

        public Users() { }

        public Users(int userid,
                     string name,
                     string email,
                     string passwordhash,
                     string role,
                     DateTime createdat)
        {
            UserId = userid;
            Name = name;
            Email = email;
            PasswordHash = passwordhash;
            Role = role;
            CreatedAt = createdat;
        }
    }
}
