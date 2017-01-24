using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace FredOyen.Models
{
    public class FredOyenData : DbContext
    {
        public FredOyenData()
                : base("name=FredOyen")
        {
            Database.SetInitializer<FredOyenData>(null);
        }

        public DbSet<Talks> Talks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}