//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace jTunes
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserSong
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public Nullable<int> Rating { get; set; }
        public System.DateTime PurchaseDate { get; set; }
    
        public virtual Song Song { get; set; }
        public virtual User User { get; set; }
    }
}
