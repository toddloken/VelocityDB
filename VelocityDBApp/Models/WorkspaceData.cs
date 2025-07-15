using System;
using System.Collections.Generic;

namespace VelocityDBApp.Models
{
    [Serializable]
    public class WorkspaceData
    {
        public string MethodName { get; set; }
        public string Title { get; set; }
        public List<WorkspaceObject> Objects { get; set; } = new List<WorkspaceObject>();
        public List<Relationship> Relationships { get; set; } = new List<Relationship>();
        public DateTime Timestamp { get; set; }
        public DateTime SavedAt { get; set; }
        public string Version { get; set; }
        public string Workspace { get; set; }
    }
}