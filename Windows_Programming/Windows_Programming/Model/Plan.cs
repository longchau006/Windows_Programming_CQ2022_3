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
        public bool IsSelected { get; set; } = false;
        public int RemainingDays
        {
            get
            {
                if (DeletedDate.HasValue)
                {
                    int daysLeft = (int)(30 - (DateTime.Now - DeletedDate.Value).TotalDays);
                    return Math.Max(0, daysLeft);
                }
                return 0;
                
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
