using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    //contex de la db basé sur IDENTITY pour l'authentification derive sur domain app user
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        // creation de la db ACTIVITIES
        public DbSet<Activity> Activities { get; set; }
        // creation de la db ActivityAttendee RELATION MANY MANY ENTRE USER ET ACTIVITY
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        // creation de la db ActivityAttendee RELATION MANY MANY ENTRE USER ET PHOTO
        public DbSet<Photo> Photos { get; set; }
        // MODELE DE LA CREATION ORM USER/ACTIVITIE MANY TO MANY JOINTURE
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId }));
            // LIEN TABLE USER VERS ACTIVITE
            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.Activities)
                .HasForeignKey(aa => aa.AppUserId);
            // LIEN ACTIVITE VERS USER
            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.Activity)
                .WithMany(a => a.Attendees)
                .HasForeignKey(aa => aa.ActivityId);
        }
    }
}
