using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EFCoreDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {


        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ENT_BUS_ACT>().HasKey(x => new { x.ENTERPRISE_NO, x.ENT_ACTIVITY_NO });
        }

        public DbSet<TAX_PAYER> TAX_PAYER { get; set; }

        public DbSet<ENTERPRISE> ENTERPRISE { get; set; }

        public DbSet<ENT_BUS_ACT> ENT_BUS_ACT { get; set; }

        

        
    }


    public partial class TAX_PAYER
    {
        [Key]
        public long TAX_PAYER_NO { get; set; } 

        [Required]
        [MaxLength(255)]
        public string TAX_PAYER_NAME { get; set; } 

        public virtual ICollection<ENTERPRISE> ENTERPRISE { get; set; } /*= new List<ENTERPRISE>();*/
    }

    public partial class ENTERPRISE
    {
        [Key]
        public long ENTERPRISE_NO { get; set; } 

        [Required]
        [MaxLength(255)]
        public string ENTERPRISE_NAME { get; set; } 

        public long TAX_PAYER_NO { get; set; } 
        [ForeignKey(nameof(TAX_PAYER_NO))]
        public virtual TAX_PAYER TAX_PAYER { get; set; }

        public virtual ICollection<ENT_BUS_ACT> ENT_BUS_ACT { get; set; } /*= new List<BUSINESS_ACTIVITY>();*/
    }

    public class ENT_BUS_ACT
    {
        public byte ENT_ACTIVITY_NO { get; set; }

        public long ENTERPRISE_NO { get; set; }

        [Required]
        [StringLength(1)]
        public string MAIN_ACTIVITY_FL { get; set; }

        [ForeignKey(nameof(ENT_ACTIVITY_NO))]
        public ENT_ACTIVITY? ENT_ACTIVITY { get; set; }

        [ForeignKey(nameof(ENTERPRISE_NO))]
        public ENTERPRISE? ENTERPRISE { get; set; }
    }

    public partial class ENT_ACTIVITY
    {
        [Key]
        public byte ENT_ACTIVITY_NO { get; set; }

        [Required]
        [StringLength(190)]
        public string? ENT_ACTIVITY_DESC { get; set; }

        public ICollection<ENT_BUS_ACT>? ENT_BUS_ACT { get; set; }
    }

}
