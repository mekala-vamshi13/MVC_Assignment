using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace User_Management.Models
{
    public class MvcDbContext : DbContext
    {
        public MvcDbContext() : base("DefaultConnection")
        {

        }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<PasswordDetail> PasswordDetails { get; set; }
    }
}
