using System;

namespace VelocityDBApp.Models
{
    [Serializable]
    public class Relationship
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}