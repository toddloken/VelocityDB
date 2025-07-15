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
        private readonly string _databaseName;
        private readonly string _basePath;

        public SchemaService(string databaseName)
        {
            _databaseName = databaseName;
            _basePath = System.IO.Path.Combine(Environment.CurrentDirectory, "VDB");
        }

        private SessionNoServer CreateReadSession()
        {
            SessionBase.BaseDatabasePath = _basePath;
            return new SessionNoServer(_databaseName);
        }

        public object GetDatabaseSchema()
        {
            using (var session = CreateReadSession())
            {
                session.BeginRead();

                return new
                {
                    DatabasePath = _basePath,
                    RegisteredTypes = GetRegisteredTypes(),
                    Statistics = GetDatabaseStatistics(session),
                    Indexes = GetIndexInformation()
                };
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
            using (var session = CreateReadSession())
            {
                session.BeginRead();

                return new
                {
                    Users = GetObjectStructure<User>(session),
                    WorkspaceData = GetObjectStructure<WorkspaceData>(session),
                    WorkspaceObjects = GetObjectStructure<WorkspaceObject>(session),
                    Relationships = GetObjectStructure<Relationship>(session),
                    Positions = GetObjectStructure<Position>(session)
                };
            }
        }

        private List<object> GetRegisteredTypes()
        {
            var types = new[]
            {
                typeof(User),
                typeof(WorkspaceData),
                typeof(WorkspaceObject),
                typeof(Position),
                typeof(Relationship)
            };

            return types.Select(type => new
            {
                TypeName = type.Name,
                FullName = type.FullName,
                Properties = GetTypeProperties(type)
            }).ToList<object>();
        }

        private object GetTypeProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .Select(p => new
                      {
                          Name = p.Name,
                          Type = p.PropertyType.Name,
                          IsGeneric = p.PropertyType.IsGenericType,
                          GenericArguments = p.PropertyType.IsGenericType
                              ? p.PropertyType.GetGenericArguments().Select(g => g.Name).ToArray()
                              : Array.Empty<string>()
                      }).ToList();
        }

        private object GetDatabaseStatistics(SessionNoServer session)
        {
            try
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
            catch (Exception ex)
            {
                return new
                {
                    Error = $"Could not retrieve statistics: {ex.Message}",
                    UserCount = 0,
                    WorkspaceDataCount = 0,
                    WorkspaceObjectCount = 0,
                    RelationshipCount = 0,
                    PositionCount = 0
                };
            }
        }

        private object GetIndexInformation()
        {
            return new
            {
                Message = "Index information would be retrieved from VelocityDB metadata"
            };
        }

        private object GetObjectStructure<T>(SessionNoServer session) where T : class
        {
            try
            {
                var objects = session.AllObjects<T>().Take(5).ToList();
                return new
                {
                    TypeName = typeof(T).Name,
                    Count = session.AllObjects<T>().Count(),
                    SampleData = objects,
                    Properties = GetTypeProperties(typeof(T))
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    TypeName = typeof(T).Name,
                    Count = 0,
                    SampleData = new List<T>(),
                    Properties = GetTypeProperties(typeof(T)),
                    Error = ex.Message
                };
            }
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
