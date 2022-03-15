using Ich.Saas.Core.Code.Database;
using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Code.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class SaaSContext : DbContext
    {
        public SaaSContext()
        {

        }

        public SaaSContext(DbContextOptions<SaaSContext> options) : base(options) { }

        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<Error> Error { get; set; }
        public virtual DbSet<Instructor> Instructor { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Locale> Locale { get; set; }
        public virtual DbSet<Quiz> Quiz { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Tenant> Tenant { get; set; }
        public virtual DbSet<TimeZone> TimeZone { get; set; }
        public virtual DbSet<Translate> Translate { get; set; }
        public virtual DbSet<User> User { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var currentUser = ServiceLocator.Resolve<ICurrentUser>();

            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = entry.Entity.ChangedBy = currentUser.Id;
                        entry.Entity.CreatedOn = entry.Entity.ChangedOn = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ChangedBy = currentUser.Id;
                        entry.Entity.ChangedOn = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
