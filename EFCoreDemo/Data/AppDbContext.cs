using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EnterpriseBusinessActivity>()
                   .HasKey(x => new { x.EnterpriseId, x.ActivityId });

            builder.Entity<TaxPayer>()
                .Property(tp => tp.TaxPayerId)
                .ValueGeneratedNever();

            // Disable auto-increment for EnterpriseId in Enterprise
            builder.Entity<Enterprise>()
                .Property(e => e.EnterpriseId)
                .ValueGeneratedNever();

            // Disable auto-increment for ActivityId in Activity
            builder.Entity<Activity>()
                .Property(a => a.ActivityId)
                .ValueGeneratedNever();
        }

        public DbSet<TaxPayer> TaxPayers { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<EnterpriseBusinessActivity> EnterpriseBusinessActivities { get; set; }
        public DbSet<Activity> Activities { get; set; }
    }

    public partial class TaxPayer
    {
        [Key]
        public long TaxPayerId { get; set; }

        [Required]
        [MaxLength(255)]
        public string? TaxPayerName { get; set; }

        public  ICollection<Enterprise>? Enterprises { get; set; }
    }

    public partial class Enterprise
    {
        [Key]
        public long EnterpriseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string? EnterpriseName { get; set; }

        public long TaxPayerId { get; set; }

        [ForeignKey(nameof(TaxPayerId))]
        public  TaxPayer? TaxPayer { get; set; }

        public  ICollection<EnterpriseBusinessActivity>? EnterpriseBusinessActivities { get; set; }
    }

    public class EnterpriseBusinessActivity
    {
        public byte ActivityId { get; set; }

        public long EnterpriseId { get; set; }

        [Required]
        [StringLength(1)]
        public string? MainActivityFlag { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public Activity? Activity { get; set; }

        [ForeignKey(nameof(EnterpriseId))]
        public Enterprise? Enterprise { get; set; }
    }

    public partial class Activity
    {
        [Key]
        public byte ActivityId { get; set; }
            
        [Required]
        [MaxLength(190)]
        public string? ActivityDescription { get; set; }

        public ICollection<EnterpriseBusinessActivity>? EnterpriseBusinessActivities { get; set; }
    }



    public class TaxPayerDto
    {
        public long TaxPayerId { get; set; }
        public string? TaxPayerName { get; set; }
        public ICollection<EnterpriseDto>? Enterprises { get; set; }
    }

    public class EnterpriseDto
    {
        public long EnterpriseId { get; set; }
        public string? EnterpriseName { get; set; }
        public long TaxPayerId { get; set; }
        public ICollection<EnterpriseBusinessActivityDto>? EnterpriseBusinessActivities { get; set; }
    }

    public class EnterpriseBusinessActivityDto
    {
        public byte ActivityId { get; set; }
        public long EnterpriseId { get; set; }
        public string? MainActivityFlag { get; set; }
        //public ActivityDto? Activity { get; set; }
        //public EnterpriseDto? Enterprise { get; set; }
    }

    public class ActivityDto
    {
        public byte ActivityId { get; set; }
        public string? ActivityDescription { get; set; }
        public ICollection<EnterpriseBusinessActivityDto>? EnterpriseBusinessActivities { get; set; }
    }

}
