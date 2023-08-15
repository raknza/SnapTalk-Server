using Microsoft.EntityFrameworkCore;
namespace android_backend.Models;

public class MyDbContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Contact> Contact { get; set; }
    public DbSet<Message> Message { get; set;}

    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }


}