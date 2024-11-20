using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Model
{
    public class Plan : INotifyPropertyChanged
    {
        public int Id {  get; set; }                                             
        public string Name { get; set; }
        public string PlanImage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public bool Type { get; set; }// Traveller, Non-Traveller
        public DateTime? DeletedDate { get; set; }

        public List<Activity>? Activities { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
