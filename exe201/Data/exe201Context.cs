using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Data
{
    public class exe201Context : DbContext
    {
        public exe201Context (DbContextOptions<exe201Context> options)
            : base(options)
        {
        }

        public DbSet<exe201.Models.User> User { get; set; } = default!;
    }
}
