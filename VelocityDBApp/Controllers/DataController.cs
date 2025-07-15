using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
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
        private readonly string _databasePath = "MyAppDatabase";
        
        public DataController(DatabaseInitializer dbInitializer)
        {
            _dbInitializer = dbInitializer;
        }
        
        [HttpPost("initialize")]
        public IActionResult InitializeDatabase()
        {
            try
            {
                // Create database directory if it doesn't exist
                var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), _databasePath);
                if (!Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory);
                }
                
                _dbInitializer.InitializeDatabase();
                return Ok("Database initialized successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Database initialization failed: {ex.Message}");
            }
        }
        
        [HttpGet("workspace")]
        public IActionResult GetWorkspaceData()
        {
            try
            {
                // Check if database exists
                var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), _databasePath);
                if (!Directory.Exists(dbDirectory))
                {
                    return BadRequest("Database not initialized. Please initialize the database first.");
                }
                
                using (var session = new SessionNoServer(_databasePath))
                {
                    session.BeginRead(); // Start read transaction
                    
                    var workspaceData = session.AllObjects<WorkspaceData>().FirstOrDefault();
                    
                    if (workspaceData == null)
                    {
                        return Ok(new { message = "No workspace data found. Database may not be properly seeded." });
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
                // Check if database exists
                var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), _databasePath);
                if (!Directory.Exists(dbDirectory))
                {
                    return BadRequest("Database not initialized. Please initialize the database first.");
                }
                
                using (var session = new SessionNoServer(_databasePath))
                {
                    session.BeginRead(); // Start read transaction
                    
                    var users = session.AllObjects<User>().ToList();
                    return Ok(users);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving users: {ex.Message}");
            }
        }
        
        [HttpPost("workspace")]
        public IActionResult SaveWorkspaceData([FromBody] WorkspaceData workspaceData)
        {
            try
            {
                // Check if database exists
                var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), _databasePath);
                if (!Directory.Exists(dbDirectory))
                {
                    return BadRequest("Database not initialized. Please initialize the database first.");
                }
                
                using (var session = new SessionNoServer(_databasePath))
                {
                    session.BeginUpdate();
                    session.Persist(workspaceData);
                    session.Commit();
                    return Ok("Workspace data saved successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving workspace data: {ex.Message}");
            }
        }
        
        [HttpGet("status")]
        public IActionResult GetDatabaseStatus()
        {
            try
            {
                var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), _databasePath);
                var exists = Directory.Exists(dbDirectory);
                
                if (!exists)
                {
                    return Ok(new { initialized = false, message = "Database not initialized" });
                }
                
                // Check if database files exist
                var dbFiles = Directory.GetFiles(dbDirectory, "*.odb");
                if (dbFiles.Length == 0)
                {
                    return Ok(new { initialized = false, message = "Database directory exists but no database files found" });
                }
                
                using (var session = new SessionNoServer(_databasePath))
                {
                    session.BeginRead(); // Start read transaction
                    
                    var userCount = session.AllObjects<User>().Count();
                    var workspaceCount = session.AllObjects<WorkspaceData>().Count();
                    
                    return Ok(new 
                    { 
                        initialized = true, 
                        userCount = userCount,
                        workspaceCount = workspaceCount,
                        databasePath = dbDirectory,
                        databaseFiles = dbFiles.Length
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking database status: {ex.Message}");
            }
        }
    }
}