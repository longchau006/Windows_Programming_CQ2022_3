using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Model
{
    public class Activity
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public virtual string Venue { get; set; }
        public virtual string Address { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }
    public class Transport : Activity
    {
        public string Vehicle {  get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public override string Venue => Vehicle;
        public override string Address => StartLocation;
    }
    public class Lodging : Activity
    {
        public string RoomInfo { get; set; }
        public override string Venue => RoomInfo;
    }
    public class Extend : Activity
    {
        public string NameMore { get; set; }
    }
}
