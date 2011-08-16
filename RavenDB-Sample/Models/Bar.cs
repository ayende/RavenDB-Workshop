using System;

namespace RavenDB_Sample.Models
{
    public class Bar
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Name { get; set; }
        public string[] Songs { get; set; }
        public TimeSpan OpensAt { get; set; }
        public TimeSpan ClosesAt { get; set; }
    }
}