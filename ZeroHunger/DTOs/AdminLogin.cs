using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZeroHunger.DTOs
{
    public class AdminLogin
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}