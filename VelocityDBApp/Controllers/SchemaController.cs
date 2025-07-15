using Microsoft.AspNetCore.Mvc;
using VelocityDBApp.Services;

namespace VelocityDBApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchemaController : ControllerBase
    {
        private readonly SchemaService _schemaService;
        
        public SchemaController()
        {
            _schemaService = new SchemaService("MyAppDatabase");
        }
        
        [HttpGet("database")]
        public IActionResult GetDatabaseSchema()
        {
            try
            {
                var schema = _schemaService.GetDatabaseSchema();
                return Ok(schema);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error retrieving database schema: {ex.Message}");
            }
        }
        
        [HttpGet("type/{typeName}")]
        public IActionResult GetTypeSchema(string typeName)
        {
            try
            {
                var schema = _schemaService.GetTypeSchema(typeName);
                if (schema == null)
                    return NotFound($"Type '{typeName}' not found");
                    
                return Ok(schema);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error retrieving type schema: {ex.Message}");
            }
        }
        
        [HttpGet("structure")]
        public IActionResult GetDataStructure()
        {
            try
            {
                var structure = _schemaService.GetDataStructure();
                return Ok(structure);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error retrieving data structure: {ex.Message}");
            }
        }
        
        [HttpGet("types")]
        public IActionResult GetAvailableTypes()
        {
            try
            {
                var types = new[]
                {
                    "User",
                    "WorkspaceData", 
                    "WorkspaceObject",
                    "Position",
                    "Relationship"
                };
                
                return Ok(types);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error retrieving available types: {ex.Message}");
            }
        }
    }
}