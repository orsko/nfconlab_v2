using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace nfconlab.Models
{
    public class QuestionDb: DbContext
    {
        public DbSet<QuestionItem> Questions { get; set; }
        public DbSet<PlayerItem> Players { get; set; }
    }
}