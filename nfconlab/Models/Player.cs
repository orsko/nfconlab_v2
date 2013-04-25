using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nfconlab.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string User_ID { get; set; }
        public string Answered { get; set; }
        public int Points { get; set; }
    }
}