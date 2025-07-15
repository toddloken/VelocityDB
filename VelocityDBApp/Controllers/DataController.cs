using Microsoft.AspNetCore.Mvc;
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
                _dbInitializer.InitializeDatabase();
                return Ok("Database initialized successfully");
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Database initialization failed: {ex.Message}");
            }
        }
        
        [HttpGet("workspace")]
        public IActionResult GetWorkspaceData()
        {
            try
            {
                using (var session = new SessionNoServer(_databasePath))
                {
                    var workspaceData = session.AllObjects<WorkspaceData>().FirstOrDefault();
                    return Ok(workspaceData);
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error retrieving workspace data: {ex.Message}");
            }
        }
        
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            try
            {
                using (var session = new SessionNoServer(_databasePath))
                {
                    var users = session.AllObjects<User>().ToList();
                    return Ok(users);
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error retrieving users: {ex.Message}");
            }
        }
        
        [HttpPost("workspace")]
        public IActionResult SaveWorkspaceData([FromBody] WorkspaceData workspaceData)
        {
            try
            {
                using (var session = new SessionNoServer(_databasePath))
                {
                    session.BeginUpdate();
                    session.Persist(workspaceData);
                    session.Commit();
                    return Ok();
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error saving workspace data: {ex.Message}");
            }
        }
    }
}