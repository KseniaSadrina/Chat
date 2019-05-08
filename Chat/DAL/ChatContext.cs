using Chat.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.DAL
{
  public class ChatContext : IdentityDbContext<User, Role, int>
  {

    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // One to many
      modelBuilder.Entity<Training>()
       .HasOne(t => t.Scenario);

      modelBuilder.Entity<ChatSession>()
        .HasMany(e => e.Messages);

      // Map many to many users to sessions

      modelBuilder.Entity<SessionUser>().HasKey(sc => new { sc.SessionId, sc.UserId });

      modelBuilder.Entity<SessionUser>()
        .HasOne<ChatSession>(sc => sc.Session)
        .WithMany(s => s.Users)
        .HasForeignKey(sc => sc.SessionId);


      modelBuilder.Entity<SessionUser>()
          .HasOne<User>(sc => sc.User)
          .WithMany(s => s.Sessions)
          .HasForeignKey(sc => sc.UserId);

      Seed(modelBuilder);
    }

    private void Seed(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Scenario>().HasData(
         new Scenario
         {
           Id = 1,
           Name = "SQLInjection",
           Description = "SQL injection is a code injection technique that might destroy your database.SQL injection is one of the most common web hacking techniques.SQL injection is the placement of malicious code in SQL statements, via web page input."
         },
          new Scenario
          {
            Id = 2,
            Name = "Apache Shutdown",
            Description = "Slowloris is a type of denial of service attack tool invented by Robert 'RSnake' Hansen which allows a single machine to take down another machine's web server with minimal bandwidth and side effects on unrelated services and ports."
          },
           new Scenario
           {
             Id = 3,
             Name = "Trojan",
             Description = "Any malicious computer program which misleads users of its true intent. The term is derived from the Ancient Greek story of the deceptive wooden horse that led to the fall of the city of."
           }
     );
    }

    public DbSet<ChatSession> ChatSessions { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<Training> Trainings { get; set; }

    public DbSet<Scenario> Scenarios { get; set; }

    public DbSet<SessionUser> SessionsUsers { get; set; }

  }
}
