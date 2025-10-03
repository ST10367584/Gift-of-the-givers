using Microsoft.EntityFrameworkCore;
using Gift_of_the_givers.Data;

namespace Gift_of_the_givers.Services
{
    public static class DatabaseSchemaUpdater
    {
        public static async Task UpdateSchema(AppDbContext context)
        {
            try
            {
                // Check if IsApproved column exists in Volunteers table
                var columnExists = await context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                  WHERE TABLE_NAME = 'Volunteers' AND COLUMN_NAME = 'IsApproved')
                    BEGIN
                        ALTER TABLE Volunteers ADD IsApproved BIT NOT NULL DEFAULT 0
                        PRINT 'Added IsApproved column to Volunteers table'
                    END
                    ELSE
                    BEGIN
                        PRINT 'IsApproved column already exists in Volunteers table'
                    END
                ");

                Console.WriteLine("✅ Database schema check completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database schema update failed: {ex.Message}");
            }
        }
    }
}