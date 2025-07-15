using System;
using System.Collections.Generic;
using System.IO;
using VelocityDb.Session;
using VelocityDBApp.Models;

namespace VelocityDBApp.Services
{
    public class DatabaseInitializer
    {
        private readonly string _databasePath;
        
        public DatabaseInitializer(string databasePath)
        {
            _databasePath = databasePath;
        }
        
        public void InitializeDatabase()
        {
            // Ensure database directory exists
            var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), _databasePath);
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
            
            using (var session = new SessionNoServer(_databasePath))
            {
                session.BeginUpdate();
                
                try
                {
                    // Register classes
                    session.RegisterClass(typeof(User));
                    session.RegisterClass(typeof(WorkspaceData));
                    session.RegisterClass(typeof(WorkspaceObject));
                    session.RegisterClass(typeof(Position));
                    session.RegisterClass(typeof(Relationship));
                    
                    // Create initial data
                    SeedInitialData(session);
                    
                    session.Commit();
                }
                catch (Exception)
                {
                    session.Abort();
                    throw;
                }
            }
        }
        
        private void SeedInitialData(SessionNoServer session)
        {
            // Create admin user
            var adminUser = new User { Id = 1, Name = "Admin", Role = "Administrator" };
            session.Persist(adminUser);
            
            // Create workspace data from your JSON
            var workspaceData = new WorkspaceData
            {
                MethodName = "Methods of run.json",
                Title = "Methods of run.json",
                Timestamp = DateTime.Parse("2025-07-09T13:16:20.329Z"),
                SavedAt = DateTime.Parse("2025-07-09T13:16:20.341Z"),
                Version = "1.0",
                Workspace = "antlr"
            };
            
            // Add objects
            workspaceData.Objects.AddRange(new[]
            {
                new WorkspaceObject
                {
                    Name = "Primitive",
                    Type = "primitive",
                    Position = new Position { X = 196, Y = 156 }
                },
                new WorkspaceObject
                {
                    Name = "External Method",
                    Type = "persistent",
                    Position = new Position { X = 27, Y = 93 }
                },
                new WorkspaceObject
                {
                    Name = "Local Method",
                    Type = "local",
                    Position = new Position { X = 187, Y = 94 }
                },
                new WorkspaceObject
                {
                    Name = "Evaluation",
                    Type = "evaluation",
                    Position = new Position { X = 195, Y = 286 }
                },
                new WorkspaceObject
                {
                    Name = "Persistent",
                    Type = "persistent",
                    Position = new Position { X = 310, Y = 356 }
                },
                new WorkspaceObject
                {
                    Name = "Match",
                    Type = "match",
                    Position = new Position { X = 51, Y = 280 }
                }
            });
            
            // Add relationships
            workspaceData.Relationships.AddRange(new[]
            {
                new Relationship { From = "Match", To = "External Method" },
                new Relationship { From = "Evaluation", To = "Primitive" },
                new Relationship { From = "Persistent", To = "Primitive" }
            });
            
            session.Persist(workspaceData);
        }
    }
}