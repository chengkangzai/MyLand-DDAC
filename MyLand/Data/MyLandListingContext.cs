using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyLand.Models;

namespace MyLand.Data
{
    public class MyLandListingContext : DbContext
    {
        public MyLandListingContext (DbContextOptions<MyLandListingContext> options)
            : base(options)
        {
        }

        public DbSet<MyLand.Models.Listing> Listing { get; set; }
    }
}
