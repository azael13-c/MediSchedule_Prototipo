using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MediSchedule_Prototipo.Data
{
    
    /// Esta clase le dice a EF Core cómo crear el DbContext para migraciones y comandos CLI.
    
    public class MediScheduleContextFactory : IDesignTimeDbContextFactory<MediScheduleContext>
    {
        public MediScheduleContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MediScheduleContext>();
            optionsBuilder.UseSqlite("Data Source=medischedule.db"); // Base de datos física SQLite

            return new MediScheduleContext(optionsBuilder.Options);
        }
    }
}
