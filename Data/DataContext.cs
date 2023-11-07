using Microsoft.EntityFrameworkCore;

namespace APICacheWithRedis.Entities
{
    public class DataContext:DbContext
    {

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)  { }

            public DbSet <Book>  Books { get; set; }

    }
}