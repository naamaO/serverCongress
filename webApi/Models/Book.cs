//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public Nullable<double> Price { get; set; }
        public string Image { get; set; }
        public Nullable<double> SallePrice { get; set; }
    }
}
