using System;
using System.Collections.Generic;
using System.Text;
using dotnetWebApiRest.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetWebApiRest.Data
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Produto> Produtos {get; set;}

        public DbSet<Usuario> usuarios {get; set;}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }
    }
}