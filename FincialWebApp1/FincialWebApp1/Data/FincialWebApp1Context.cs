using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinancialWebApp1.Models;
using FincialWebApp1.Models;

namespace FincialWebApp1.Data
{
    public class FincialWebApp1Context : DbContext
    {
        public FincialWebApp1Context (DbContextOptions<FincialWebApp1Context> options)
            : base(options)
        {
        }

        public DbSet<FinancialWebApp1.Models.Track> Trackers { get; set; }
        public virtual DbSet<FinancialWebApp1.Models.MainClass> MainClass { get; set; } = default!;

        public DbSet<FinancialWebApp1.Models.BankList> BankList { get; set; } = default!;

    }

    // Inside the import function, before adding entities
}
