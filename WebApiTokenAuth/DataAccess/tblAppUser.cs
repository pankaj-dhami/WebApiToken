//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApiTokenAuth.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAppUser
    {
        public int UserID { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string AboutMe { get; set; }
        public string MyStatus { get; set; }
        public string ProfilePicUrl { get; set; }
        public byte[] ProfilePicData { get; set; }
    }
}
