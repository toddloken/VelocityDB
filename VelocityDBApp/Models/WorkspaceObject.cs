using System;

namespace VelocityDBApp.Models
{
    [Serializable]
    public class WorkspaceObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Position Position { get; set; }
    }
}