using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webApi.classes
{
    public class Drafts
    {
        public int IdProposal { get; set; }
        public string UserName { get; set; }
        public string Division { get; set; }
        public string SubDivision { get; set; }
        public string TitleEnglish { get; set; }
        public string TitleHebrew { get; set; }
        public string Proposal { get; set; }
        public string Language { get; set; }
        public string Keywords { get; set; }
        public string SessionName { get; set; }
        public string Chairman { get; set; }
        public string FirstNameEnglish { get; set; }
        public string LastNameEnglish { get; set; }
    }
}