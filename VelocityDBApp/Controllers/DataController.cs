using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VelocityDb;
using VelocityDb.Session;
using VelocityDBApp.Models;
using VelocityDBApp.Services;

namespace VelocityDBApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly DatabaseInitializer _dbInitializer;
        private readonly string _databaseName = "MyAppDatabase";

        public DataController(DatabaseInitializer dbInitializer)
        {
            _dbInitializer = dbInitializer;
        }

        [HttpPost("initialize")]
        public IActionResult InitializeDatabase()
        {
            try
            {
                _dbInitializer.InitializeDatabase(recreate: false);
                return Ok("Database initialized successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Database initialization failed: {ex.Message}");
            }
        }

        [HttpPost("reset")]
        public IActionResult ResetDatabase()
        {
            try
            {
                _dbInitializer.RecreateDatabase();
                return Ok("Database reset and recreated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Database reset failed: {ex.Message}");
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteDatabase()
        {
            try
            {
                var dbPath = _dbInitializer.GetDatabasePath();
                if (Directory.Exists(dbPath))
                {
                    Directory.Delete(dbPath, true);
                    return Ok("Database deleted successfully");
                }
                return Ok("Database directory does not exist");
            }
            catch (Exception ex)
            {
                return BadRequest($"Database deletion failed: {ex.Message}");
            }
        }

        [HttpGet("workspace")]
        public IActionResult GetWorkspaceData()
        {
            try
            {
                var basePath = _dbInitializer.GetDatabasePath();
                SessionBase.BaseDatabasePath = basePath;

                using (var session = new SessionNoServer(_databaseName))
                {
                    session.BeginRead();
                    var workspaceData = session.AllObjects<WorkspaceData>().FirstOrDefault();

                    if (workspaceData == null)
                    {
                        return Ok(new { message = "No workspace data found" });
                    }

                    return Ok(workspaceData);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving workspace data: {ex.Message}");
            }
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            try
            {
                var basePath = _dbInitializer.GetDatabasePath();
                SessionBase.BaseDatabasePath = basePath;

                using (var session = new SessionNoServer(_databaseName))
                {
                    session.BeginRead();
                    var users = session.AllObjects<User>().ToList();
                    return Ok(users);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving users: {ex.Message}");
            }
        }

        [HttpGet("status")]
        public IActionResult GetDatabaseStatus()
        {
            try
            {
                var dbPath = _dbInitializer.GetDatabasePath();

                if (!Directory.Exists(dbPath))
                {
                    return Ok(new
                    {
                        initialized = false,
                        message = "Database directory does not exist",
                        path = dbPath
                    });
                }

                var myAppDbDir = Path.Combine(dbPath, _databaseName);
                var files = Directory.Exists(myAppDbDir) ? Directory.GetFiles(myAppDbDir) : Array.Empty<string>();


                if (files.Length == 0)
                {
                    return Ok(new
                    {
                        initialized = false,
                        message = "Database directory is empty",
                        path = dbPath
                    });
                }

                try
                {
                    SessionBase.BaseDatabasePath = dbPath;

                    using (var session = new SessionNoServer(_databaseName))
                    {
                        session.BeginRead();
                        var userCount = session.AllObjects<User>().Count();

                        return Ok(new
                        {
                            initialized = true,
                            userCount = userCount,
                            path = dbPath,
                            fileCount = files.Length,
                            files = files.Select(Path.GetFileName).ToArray()
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        initialized = false,
                        message = $"Database exists but cannot read: {ex.Message}",
                        path = dbPath,
                        fileCount = files.Length
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking status: {ex.Message}");
            }
        }

        [HttpGet("debug")]
        public IActionResult GetDebugInfo()
        {
            try
            {
                var dbPath = _dbInitializer.GetDatabasePath();

                return Ok(new
                {
                    databasePath = dbPath,
                    databasePathExists = Directory.Exists(dbPath),
                    currentBasePath = SessionBase.BaseDatabasePath,
                    databaseName = _databaseName,
                    currentDirectory = Directory.GetCurrentDirectory()
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Debug error: {ex.Message}");
            }
        }
    }
}
