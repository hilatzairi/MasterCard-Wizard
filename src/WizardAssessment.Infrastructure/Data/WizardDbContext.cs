using Microsoft.EntityFrameworkCore;
using WizardAssessment.Domain.Models.DTOs;

namespace WizardAssessment.Infrastructure.Data;

public class WizardDbContext : DbContext
{
    public WizardDbContext(DbContextOptions<WizardDbContext> options) : base(options)
    {
    }

    public DbSet<OrganizationDto> Organizations { get; set; }
    public DbSet<EnvironmentDto> Environments { get; set; }
    public DbSet<QuestionDto> Questions { get; set; }
    public DbSet<QuestionOptionDto> QuestionOptions { get; set; }
    public DbSet<NavigationRuleDto> NavigationRules { get; set; }
    public DbSet<BucketConfigurationDto> BucketConfigurations { get; set; }
    public DbSet<WizardSessionDto> WizardSessions { get; set; }
    public DbSet<SessionAnswerDto> SessionAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrganizationDto>(entity =>
        {
            entity.ToTable("Organizations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<EnvironmentDto>(entity =>
        {
            entity.ToTable("Environments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<QuestionDto>(entity =>
        {
            entity.ToTable("Questions");
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<QuestionOptionDto>(entity =>
        {
            entity.ToTable("QuestionOptions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuestionCode).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<NavigationRuleDto>(entity =>
        {
            entity.ToTable("NavigationRules");
            entity.HasKey(e => e.RuleId);
            entity.Property(e => e.CurrentQuestionCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RecommendedBucket).HasMaxLength(50);
        });

        modelBuilder.Entity<BucketConfigurationDto>(entity =>
        {
            entity.ToTable("BucketConfigurations");
            entity.HasKey(e => e.BucketName);
            entity.Property(e => e.BucketName).HasMaxLength(50);
        });

        modelBuilder.Entity<WizardSessionDto>(entity =>
        {
            entity.ToTable("WizardSessions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentQuestionCode).HasMaxLength(50);
            entity.Property(e => e.RecommendedBucket).HasMaxLength(50);
        });

        modelBuilder.Entity<SessionAnswerDto>(entity =>
        {
            entity.ToTable("SessionAnswers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuestionCode).IsRequired().HasMaxLength(50);
        });
    }
}
