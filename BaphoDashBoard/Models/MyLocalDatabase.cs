using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.Models
{
    public class MyLocalDatabase : DbContext
    {
        public MyLocalDatabase(DbContextOptions<MyLocalDatabase> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<VictimDetail> VictimDetail {get;set;}
        public DbSet<Administrators> Administrators { get; set; }
        public DbSet<Ransomware> Ransomware { get; set; }

       // public DbSet<RsaKeys> RsaKeys { get; set; }
    }
}
