﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QlThuVien.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class QLTVEntities : DbContext
    {
        public QLTVEntities()
            : base("name=QLTVEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<TboAdmin> TboAdmins { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<GetAllBorrow_Result> GetAllBorrow()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAllBorrow_Result>("GetAllBorrow");
        }
    
        public virtual ObjectResult<GetAllRequest_Result> GetAllRequest()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAllRequest_Result>("GetAllRequest");
        }
    
        public virtual ObjectResult<getAllReturn_Result> getAllReturn()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getAllReturn_Result>("getAllReturn");
        }
    }
}
