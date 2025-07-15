using System;
using System.IO;
using VelocityDb.Session;
using VelocityDb;
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
            // Use current directory for simplicity
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "VDB");
        }
        
        public void InitializeDatabase(bool recreate = false)
        {
            try
            {
                // Clean directory if recreating
                if (recreate && Directory.Exists(_basePath))
                {
                    Directory.Delete(_basePath, true);
                    System.Threading.Thread.Sleep(500);
                }
                
                Directory.CreateDirectory(_basePath);
                
                // Test basic VelocityDB operation
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
                    session.RegisterClass(typeof(User));
                    
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
        
        public string GetDatabasePath()
        {
            return _basePath;
        }
        
        public void RecreateDatabase()
        {
            InitializeDatabase(recreate: true);
        }
        
        public static void SetupSession(string basePath)
        {
            SessionBase.BaseDatabasePath = basePath;
        }
    }
}