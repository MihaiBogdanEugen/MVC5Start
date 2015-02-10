using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC5Start.Models;

namespace MVC5Start.Infrastructure
{
    public class WebAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public WebAppDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static WebAppDbContext Create()
        {
            return new WebAppDbContext();
        }
    }
}