using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nfconlab.Models
{
    public class PlayerItem
    {
        public int PlayerItemId { get; set; }
        public long User_ID { get; set; }
        public string Answered { get; set; }
        public int Points { get; set; }
        public virtual ICollection<QuestionItem> QuestionItems { get; set; }
    }
}