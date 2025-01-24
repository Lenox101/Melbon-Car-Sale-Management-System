using Microsoft.EntityFrameworkCore; //Library that allows CRUD operations

namespace Melbon_Car_Sale_Management_System.Models //Defines the namespace where the UsersContext class resides.
{
    public class UsersContext : DbContext
    {
        public DbSet<Register> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarImage> CarImages { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options) //A constructor that accepts options for configuring the database context
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //Configuring further the schema of the database using the ModelBuilder.
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)"); //setting price to decimal point value
            });

            modelBuilder.Entity<CarImage>(entity => //Configures the CarImage entity's relationship with the Car entity
            {
                entity.HasOne(ci => ci.Car)
                      .WithMany(c => c.Images)
                      .HasForeignKey(ci => ci.CarId)
                      .OnDelete(DeleteBehavior.Cascade); 
                //on creating Models we also create an object of the car's images
            });
        }
    }
}
