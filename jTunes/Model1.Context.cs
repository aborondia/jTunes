﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class jTunesDBEntities : DbContext
    {
        public jTunesDBEntities()
            : base("name=jTunesDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Song> Songs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSong> UserSongs { get; set; }
    }
}
