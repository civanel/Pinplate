using Microsoft.EntityFrameworkCore;

namespace RecipesDB
{
    public class DBContext : DbContext
    {
        public DbSet<Recipes> Recipes { get; set; } = null!;
        public DbSet<Ingredients> Ingredients { get; set; } = null!;
        public DbSet<Units> Units { get; set; } = null!;
        public DbSet<Categories> Categories { get; set; } = null!;
        public DbSet<Tags> Tags { get; set; } = null!;
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Recipe_Ingredients> RecipeIngredients { get; set; } = null!;
        public DbSet<Recipe_Categories> RecipeCategories { get; set; } = null!;
        public DbSet<Recipe_Tags> RecipeTags { get; set; } = null!;
        public DbSet<User_Favorites> UserFavorites { get; set; } = null!;
        public DbSet<Recipe_Images> RecipeImages { get; set; } = null!;
        public DbSet<Recipe_Reviews> RecipeReviews { get; set; } = null!;
        public DbSet<App_Settings> AppSettings { get; set; } = null!;
        public DbSet<Complaints> Complaints { get; set; } = null!;
        public DbSet<Log_Entries> Log_Entries { get; set; } = null!;
        public DbSet<Replies> Replies { get; set; } = null!;

        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=RecipesDB;Username=postgres;Password=1234");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ПЕРВИЧНЫЕ КЛЮЧИ
            modelBuilder.Entity<App_Settings>().HasKey(a => a.SettingId);
            modelBuilder.Entity<Categories>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Ingredients>().HasKey(i => i.IngredientId);
            modelBuilder.Entity<Recipes>().HasKey(r => r.RecipeId);
            modelBuilder.Entity<Tags>().HasKey(t => t.TagId);
            modelBuilder.Entity<Units>().HasKey(u => u.UnitId);
            modelBuilder.Entity<Users>().HasKey(u => u.UserId);
            modelBuilder.Entity<Recipe_Images>().HasKey(ri => ri.ImageId);
            modelBuilder.Entity<Recipe_Reviews>().HasKey(rr => rr.ReviewId);
            modelBuilder.Entity<Complaints>().HasKey(c => c.ComplaintId);
            modelBuilder.Entity<Log_Entries>().HasKey(l => l.LogId);
            modelBuilder.Entity<Replies>().HasKey(r => r.ReplyId);

            // СОСТАВНЫЕ КЛЮЧИ
            modelBuilder.Entity<Recipe_Ingredients>().HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            modelBuilder.Entity<Recipe_Categories>().HasKey(rc => new { rc.RecipeId, rc.CategoryId });
            modelBuilder.Entity<Recipe_Tags>().HasKey(rt => new { rt.RecipeId, rt.TagId });
            modelBuilder.Entity<User_Favorites>().HasKey(uf => new { uf.UserId, uf.RecipeId });

            // ИМЕНА ТАБЛИЦ (чтобы EF Core не путался с подчеркиваниями)
            modelBuilder.Entity<User_Favorites>().ToTable("UserFavorites");
            modelBuilder.Entity<Recipe_Categories>().ToTable("RecipeCategories");
            modelBuilder.Entity<Recipe_Images>().ToTable("RecipeImages");
            modelBuilder.Entity<Recipe_Tags>().ToTable("RecipeTags");

            // ВНЕШНИЕ КЛЮЧИ
            modelBuilder.Entity<Recipe_Ingredients>().HasOne(ri => ri.Recipe).WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Ingredients>().HasOne(ri => ri.Ingredient).WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Ingredients>().HasOne(ri => ri.Unit).WithMany(u => u.RecipeIngredients)
                .HasForeignKey(ri => ri.UnitId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Recipe_Images>().HasOne(ri => ri.Recipe).WithMany(r => r.RecipeImages)
                .HasForeignKey(ri => ri.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Categories>().HasOne(rc => rc.Recipe).WithMany(r => r.RecipeCategories)
                .HasForeignKey(rc => rc.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Categories>().HasOne(rc => rc.Category).WithMany(c => c.RecipeCategories)
                .HasForeignKey(rc => rc.CategoryId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Tags>().HasOne(rt => rt.Recipe).WithMany(r => r.RecipeTags)
                .HasForeignKey(rt => rt.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Tags>().HasOne(rt => rt.Tag).WithMany(t => t.RecipeTags)
                .HasForeignKey(rt => rt.TagId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User_Favorites>().HasOne(uf => uf.User).WithMany(u => u.UserFavorites)
                .HasForeignKey(uf => uf.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User_Favorites>().HasOne(uf => uf.Recipe).WithMany(r => r.UserFavorites)
                .HasForeignKey(uf => uf.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Reviews>().HasOne(rr => rr.Recipe).WithMany(r => r.RecipeReviews)
                .HasForeignKey(rr => rr.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe_Reviews>().HasOne(rr => rr.User).WithMany(u => u.RecipeReviews)
                .HasForeignKey(rr => rr.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Complaints>().HasOne(c => c.Reporter).WithMany()
                .HasForeignKey(c => c.ReporterId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Complaints>().HasOne(c => c.Recipe).WithMany()
                .HasForeignKey(c => c.RecipeId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Replies>().HasOne(r => r.Complaint).WithMany()
                .HasForeignKey(r => r.ComplaintId).OnDelete(DeleteBehavior.Cascade);

            // Связь Рецепта и Автора
            modelBuilder.Entity<Recipes>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            // ОГРАНИЧЕНИЯ И ИНДЕКСЫ
            modelBuilder.Entity<Ingredients>().HasIndex(i => i.Name).IsUnique();
            modelBuilder.Entity<Units>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<Tags>().HasIndex(t => t.Name).IsUnique();
            modelBuilder.Entity<Categories>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Users>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<App_Settings>().HasIndex(ap => ap.SettingKey).IsUnique();

            modelBuilder.Entity<Recipes>().ToTable("Recipes", r =>
            {
                r.HasCheckConstraint("CK_Recipes_PortionPositive", "\"Portion\" > 0");
                r.HasCheckConstraint("CK_Recipes_CookTimePositive", "\"CookTimeMinutes\" > 0");
            });

            modelBuilder.Entity<Recipe_Ingredients>().ToTable("Recipe_Ingredients", r =>
            {
                r.HasCheckConstraint("CK_Recipe_Ingredients_CountPositive", "\"Count\" > 0");
            });

            modelBuilder.Entity<Recipe_Reviews>().ToTable("Recipe_Reviews", r =>
            {
                r.HasCheckConstraint("CK_RecipeReviews_RatingRange", "\"Rating\" BETWEEN 1 AND 5");
            });

            modelBuilder.Entity<Users>().ToTable("Users", t =>
            {
                t.HasCheckConstraint("CK_Users_RoleAllowed", "\"Role\" IN ('User', 'Moderator', 'Expert')");
            });

            modelBuilder.Entity<Complaints>().ToTable("Complaints", t =>
            {
                t.HasCheckConstraint("CK_Complaints_StatusAllowed", "\"Status\" IN ('Открыта', 'В работе', 'Решено', 'Скрыто')");
                t.HasCheckConstraint("CK_Complaints_ReasonAllowed", "\"Reason\" IN ('Спам', 'Мат', 'Плагиат', 'Другое')");
            });

            modelBuilder.Entity<Log_Entries>().ToTable("Log_Entries", t =>
            {
                t.HasCheckConstraint("CK_Logs_StatusAllowed", "\"Status\" IN ('Открыта', 'В работе', 'Решено')");
            });

            // ДЛИНЫ СТРОК
            modelBuilder.Entity<Recipes>().Property(r => r.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Recipes>().Property(r => r.Description).HasMaxLength(500).IsRequired();
            modelBuilder.Entity<Recipes>().Property(r => r.Instruction).HasColumnType("text").IsRequired();

            modelBuilder.Entity<Ingredients>().Property(i => i.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Units>().Property(u => u.Name).HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Units>().Property(u => u.ShortName).HasMaxLength(10).IsRequired();
            modelBuilder.Entity<Recipe_Ingredients>().Property(ri => ri.Count).HasPrecision(10, 2);
            modelBuilder.Entity<Recipe_Images>().Property(ri => ri.ImageUrl).HasMaxLength(500).IsRequired();
            modelBuilder.Entity<Recipe_Images>().Property(ri => ri.Caption).HasMaxLength(200);
            modelBuilder.Entity<Recipe_Images>().Property(ri => ri.IsPrimary).HasDefaultValue(false);
            modelBuilder.Entity<Recipe_Images>().Property(ri => ri.SortOrder).HasDefaultValue(0);
            modelBuilder.Entity<Tags>().Property(t => t.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Categories>().Property(c => c.Name).HasMaxLength(100).IsRequired();

            modelBuilder.Entity<Users>().Property(u => u.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Users>().Property(u => u.Email).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Users>().Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Users>().Property(u => u.Role).HasMaxLength(50).IsRequired().HasDefaultValue("User");

            modelBuilder.Entity<User_Favorites>().Property(uf => uf.AddedAt).HasDefaultValueSql("NOW()");
            modelBuilder.Entity<Recipe_Reviews>().Property(rr => rr.Comment).HasMaxLength(1000);

            modelBuilder.Entity<App_Settings>().Property(a => a.SettingKey).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<App_Settings>().Property(a => a.SettingValue).HasMaxLength(500).IsRequired();
            modelBuilder.Entity<App_Settings>().Property(a => a.Description).HasMaxLength(255);

            modelBuilder.Entity<Complaints>().Property(c => c.Reason).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Complaints>().Property(c => c.Description).HasMaxLength(1000);
            modelBuilder.Entity<Complaints>().Property(c => c.Status).HasMaxLength(50).IsRequired().HasDefaultValue("Открыта");
            modelBuilder.Entity<Complaints>().Property(c => c.CreatedAt).HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Log_Entries>().Property(l => l.UserName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Log_Entries>().Property(l => l.Action).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Log_Entries>().Property(l => l.Status).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Log_Entries>().Property(l => l.TargetTitle).HasMaxLength(200);
            modelBuilder.Entity<Log_Entries>().Property(l => l.CreatedAt).HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Replies>().Property(r => r.RecipientName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Replies>().Property(r => r.ComplaintTitle).HasMaxLength(200).IsRequired();
            modelBuilder.Entity<Replies>().Property(r => r.Message).HasColumnType("text").IsRequired();
            modelBuilder.Entity<Replies>().Property(r => r.AuthorName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Replies>().Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");

            // ДЕФОЛТНЫЕ ДАТЫ
            modelBuilder.Entity<Recipes>().Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");
            modelBuilder.Entity<Users>().Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
            modelBuilder.Entity<Recipe_Images>().Property(ri => ri.CreatedAt).HasDefaultValueSql("NOW()");
            modelBuilder.Entity<Recipe_Reviews>().Property(rr => rr.CreatedAt).HasDefaultValueSql("NOW()");
        }
    }
}