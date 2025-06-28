namespace BooksApi.Data
{
using Microsoft.EntityFrameworkCore;
using BooksApi.Models;
using System.Collections.Generic;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
