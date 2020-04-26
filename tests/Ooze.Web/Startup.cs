using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ooze.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Ooze.Configuration;
using Ooze.AspNetCore.Filters;
using Ooze.Filters;
using Ooze.Sorters;
using System.Collections.Generic;
using System;
using Ooze.Paging;

namespace Ooze.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite("Data Source=./database.db;"), ServiceLifetime.Transient);
            services.AddOoze(typeof(Startup).Assembly, opts =>
            {
                opts.Operations.GreaterThan = ".";
                opts.UseSelections = true;
                opts.Paging.UsePaging = true;
                opts.Paging.DefaultPageSize = 88;
            });
            services.AddScoped(typeof(OozeFilter<>));
            services.AddScoped<IOozeProvider, CustomFilterProvider>();
            services.AddScoped<IOozeProvider, CustomSorterProvider>();

            services.Remove(services.First(descriptor => descriptor.ServiceType.Equals(typeof(IOozePagingHandler))));
            services.Add(new ServiceDescriptor(typeof(IOozePagingHandler), typeof(CustomPagingProvider), ServiceLifetime.Scoped));

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
                        Name = _.ToString(),
                        Comments = new[] { 
                            new Comment
                            {
                                Id = _,
                                Date = DateTime.Now.AddDays(_),
                                Text = $"Sample comment {_}",
                                User = new User
                                {
                                    Id = _,
                                    Email = $"sample_{_}@email.com"
                                }
                            }
                        }
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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var post = modelBuilder.Entity<Post>();
            post.HasKey(x => x.Id);
            post.Property(x => x.Id).ValueGeneratedOnAdd();
            post.HasMany(x => x.Comments)
                .WithOne();

            var comment = modelBuilder.Entity<Comment>();
            comment.HasKey(x => x.Id);
            comment.Property(x => x.Id).ValueGeneratedOnAdd();
            comment.HasOne(x => x.User)
                .WithOne(x => x.Comment)
                .HasForeignKey<Comment>(x => x.Id);

            var user = modelBuilder.Entity<User>();
            user.HasKey(x => x.Id);
            user.Property(x => x.Id);
        }
    }

    public class Post
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public Comment Comment { get; set; }
    }

    public class PostConfiguration : IOozeConfiguration
    {
        public void Configure(IOozeConfigurationBuilder builder)
        {
            builder.Entity<Post>()
                 .Sort(post => post.Enabled)
                 .Sort(post => post.Name)
                 .Sort("id2", post => post.Id)
                 .Filter(post => post.Id)
                 .Filter(post => post.Name)
                 .Filter("bool", post => post.Enabled);
        }
    }

    public class CustomFilterProvider : IOozeFilterProvider<Post>
    {
        public string Name => "custom";

        public IQueryable<Post> ApplyFilter(IQueryable<Post> query, FilterParserResult filter)
        {
            return query.Where(post => post.Enabled == false);
        }
    }

    public class CustomSorterProvider : IOozeSorterProvider<Post>
    {
        public string Name => "custom";

        public IQueryable<Post> ApplySorter(IQueryable<Post> query, bool ascending)
        {
            return query.OrderBy(x => x.Id);
        }

        public IQueryable<Post> ThenApplySorter(IOrderedQueryable<Post> query, bool ascending)
        {
            return query.ThenBy(x => x.Id);
        }
    }

    public class CustomPagingProvider : IOozePagingHandler
    {
        public IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, int? page, int? pageSize)
        {
            var toSkip = page.GetValueOrDefault(0) * pageSize.GetValueOrDefault(20);

            return query.Skip(toSkip)
                .Take(pageSize.GetValueOrDefault(20));
        }
    }
}
