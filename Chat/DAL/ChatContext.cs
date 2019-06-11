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

      modelBuilder.Entity<Scenario>()
        .HasMany(e => e.Goals);

      // Map many to many users to sessions

      modelBuilder.Entity<SessionUser>().HasKey(sc => new { sc.SessionId, sc.UserId });
      modelBuilder.Entity<TrainingGoal>().HasKey(sc => new { sc.TrainingId, sc.GoalId });
      modelBuilder.Entity<Salt>().HasKey(s => new { s.UserId });
      modelBuilder.Entity<SessionUser>()
        .HasOne(sc => sc.Session)
        .WithMany(s => s.Users)
        .HasForeignKey(sc => sc.SessionId);


      modelBuilder.Entity<SessionUser>()
          .HasOne(sc => sc.User)
          .WithMany(s => s.Sessions)
          .HasForeignKey(sc => sc.UserId);

      modelBuilder.Entity<TrainingGoal>()
        .HasOne(tg => tg.Goal)
        .WithMany(g => g.TrainingGoals)
        .HasForeignKey(sc => sc.GoalId);
    }

    public DbSet<ChatSession> ChatSessions { get; set; }

    public DbSet<Goal> Goals { get; set; }

    public DbSet<TrainingGoal> TrainingGoals { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<Training> Trainings { get; set; }

    public DbSet<Scenario> Scenarios { get; set; }

    public DbSet<SessionUser> SessionsUsers { get; set; }

    public DbSet<Salt> Salts { get; set; }

  }
}
