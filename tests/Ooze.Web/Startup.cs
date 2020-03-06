using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ooze.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Ooze.Configuration;

namespace Ooze.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite("Data Source=./database.db;"), ServiceLifetime.Transient);
            services.AddOoze(typeof(Startup).Assembly);
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext db)
        {
            db.Database.EnsureCreated();

            if (!db.Posts.Any())
            {
                var posts = Enumerable.Range(1, 100)
                    .Select(_ => new Post
                    {
                        Id = _,
                        Enabled = _ % 2 == 0,
                        Name = _.ToString()
                    });

                db.Posts.AddRange(posts);
                db.SaveChanges();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Post> Posts { get; set; }
    }

    public class Post
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }

    public class PostConfiguration : IOozeConfiguration
    {
        public void Configure(OozeConfigurationBuilder builder)
        {
            builder.Entity<Post>()
                 .Sort(post => post.Enabled)
                 .Sort(post => post.Name)
                 .Sort("id2", post => post.Id)
                 .Filter(post => post.Id);
        }
    }
}
