using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ooze.Expressions;
using Ooze.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                .Sort("id2", post => post.Id);
        }
    }
}
