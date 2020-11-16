using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webApi.classes
{

    public class User
    {

    public string    FirstNameEnglish { get; set; }
        public string LastNameEnglish { get; set; }
        public string  FirstNameHebrew { get; set; }
        public string  LastNameHebrew { get; set; }
        public string  City { get; set; }
        public string  Street { get; set; }
        public string  NumberHome { get; set; }
        public string  NumberPhone1 { get; set; }
        public string  NumberPhone2 { get; set; }
        public string  selectedTitle { get; set; }
        public string selectedCountry { get; set; }
        public string  PostCode { get; set; }
        public string  Email { get; set; }
        public string Bio { get; set; }
        public bool Students { get; set; }
        public bool WithoutStudemt { get; set; }
        public bool  EAJS { get; set; }
        public bool  AJS { get; set; }
        public bool  Hebrew { get; set; }
        public bool  English { get; set; }
        public bool  Both { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Language { get; set; }
        public int MemberShip { get; set; }
        public DateTime DateMember { get; set; }
    }
}