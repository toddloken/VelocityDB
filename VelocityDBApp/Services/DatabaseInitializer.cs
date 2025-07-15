using System;
using System.IO;
using VelocityDb;
using VelocityDb.Session;
using VelocityDBApp.Models;

namespace VelocityDBApp.Services
{
    public class DatabaseInitializer
    {
        private readonly string _databaseName;
        private readonly string _basePath;

        public DatabaseInitializer(string databaseName)
        {
            _databaseName = databaseName;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "VDB");
        }

        public void InitializeDatabase(bool recreate = false)
        {
            try
            {
                if (recreate && Directory.Exists(_basePath))
                {
                    Directory.Delete(_basePath, true);
                    System.Threading.Thread.Sleep(500); // Allow time for file locks to release
                }

                Directory.CreateDirectory(_basePath);
                TestMinimalSession();
            }
            catch (Exception ex)
            {
                throw new Exception($"Database initialization failed: {ex.Message}");
            }
        }

        private void TestMinimalSession()
        {
            SessionBase.BaseDatabasePath = _basePath;

            using (var session = new SessionNoServer(_databaseName))
            {
                session.BeginUpdate();

                try
                {
                    // Register all data model types
                    session.RegisterClass(typeof(User));
                    session.RegisterClass(typeof(WorkspaceData));
                    session.RegisterClass(typeof(WorkspaceObject));
                    session.RegisterClass(typeof(Position));
                    session.RegisterClass(typeof(Relationship));

                    // Add a test User to confirm persistence
                    var testUser = new User
                    {
                        Id = 1,
                        Name = "TestUser",
                        Role = "TestRole"
                    };

                    session.Persist(testUser);
                    session.Commit();
                }
                catch
                {
                    session.Abort();
                    throw;
                }
            }
        }

        public string GetDatabasePath() => _basePath;

        public void RecreateDatabase() => InitializeDatabase(recreate: true);

        public static void SetupSession(string basePath)
        {
            SessionBase.BaseDatabasePath = basePath;
        }
    }
}
