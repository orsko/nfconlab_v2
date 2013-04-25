using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nfconlab.Models
{
    public class QuestionItem
    {
        public int QuestionItemId { get; set; }
        public string Question { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string RightAnswer { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string Image { get; set; }
    }
}