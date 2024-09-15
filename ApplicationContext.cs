using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrainStationApp
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<Train> Trains { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TrainDB;Trusted_Connection=True;");
        }

    }
}
