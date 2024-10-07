using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User_Management.Models
{
    public class PasswordDetail
    {
        [Key, ForeignKey("UserDetail")]
        //Foreign Key
        public int UserID { get; set; }  
        public string Password { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}