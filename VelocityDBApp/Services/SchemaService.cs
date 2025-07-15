using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VelocityDb.Session;
using VelocityDBApp.Models;

namespace VelocityDBApp.Services
{
    public class SchemaService
    {
        private readonly string _databasePath;
        
        public SchemaService(string databasePath)
        {
            _databasePath = databasePath;
        }
        
        public object GetDatabaseSchema()
        {
            using (var session = new SessionNoServer(_databasePath))
            {
                var schema = new
                {
                    DatabasePath = _databasePath,
                    RegisteredTypes = GetRegisteredTypes(session),
                    Statistics = GetDatabaseStatistics(session),
                    Indexes = GetIndexInformation(session)
                };
                
                return schema;
            }
        }
        
        public object GetTypeSchema(string typeName)
        {
            var type = GetTypeByName(typeName);
            if (type == null)
                return null;
                
            return new
            {
                TypeName = type.Name,
                FullName = type.FullName,
                Properties = GetTypeProperties(type),
                IsSerializable = type.IsSerializable,
                BaseType = type.BaseType?.Name
            };
        }
        
        public object GetDataStructure()
        {
            using (var session = new SessionNoServer(_databasePath))
            {
                var structure = new
                {
                    Users = GetObjectStructure<User>(session),
                    WorkspaceData = GetObjectStructure<WorkspaceData>(session),
                    WorkspaceObjects = GetObjectStructure<WorkspaceObject>(session),
                    Relationships = GetObjectStructure<Relationship>(session),
                    Positions = GetObjectStructure<Position>(session)
                };
                
                return structure;
            }
        }
        
        private List<object> GetRegisteredTypes(SessionNoServer session)
        {
            var types = new List<object>();
            
            // Get all registered types
            var registeredTypes = new[]
            {
                typeof(User),
                typeof(WorkspaceData),
                typeof(WorkspaceObject),
                typeof(Position),
                typeof(Relationship)
            };
            
            foreach (var type in registeredTypes)
            {
                types.Add(new
                {
                    TypeName = type.Name,
                    FullName = type.FullName,
                    Properties = GetTypeProperties(type)
                });
            }
            
            return types;
        }
        
        private object GetTypeProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .Select(p => new
                      {
                          Name = p.Name,
                          Type = p.PropertyType.Name,
                          IsGeneric = p.PropertyType.IsGenericType,
                          GenericArguments = p.PropertyType.IsGenericType ? 
                              p.PropertyType.GetGenericArguments().Select(g => g.Name).ToArray() : 
                              new string[0]
                      }).ToList();
        }
        
        private object GetDatabaseStatistics(SessionNoServer session)
        {
            return new
            {
                UserCount = session.AllObjects<User>().Count(),
                WorkspaceDataCount = session.AllObjects<WorkspaceData>().Count(),
                WorkspaceObjectCount = session.AllObjects<WorkspaceObject>().Count(),
                RelationshipCount = session.AllObjects<Relationship>().Count(),
                PositionCount = session.AllObjects<Position>().Count()
            };
        }
        
        private object GetIndexInformation(SessionNoServer session)
        {
            // VelocityDB index information - this is a simplified version
            return new
            {
                Message = "Index information would be retrieved from VelocityDB metadata",
                // You can expand this based on VelocityDB's index APIs
            };
        }
        
        private object GetObjectStructure<T>(SessionNoServer session) where T : class
        {
            var objects = session.AllObjects<T>().Take(5).ToList(); // Get first 5 for structure
            
            return new
            {
                TypeName = typeof(T).Name,
                Count = session.AllObjects<T>().Count(),
                SampleData = objects,
                Properties = GetTypeProperties(typeof(T))
            };
        }
        
        private Type GetTypeByName(string typeName)
        {
            var types = new[]
            {
                typeof(User),
                typeof(WorkspaceData),
                typeof(WorkspaceObject),
                typeof(Position),
                typeof(Relationship)
            };
            
            return types.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
        }
    }
}