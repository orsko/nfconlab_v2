using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace nfconlab.Models
{
    public class PlayerDb : DbContext
    {
        public DbSet<Player> Players { get; set; }
    }
}