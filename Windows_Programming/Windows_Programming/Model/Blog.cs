using System;
using System.ComponentModel;

namespace Windows_Programming.Model
{
    public class Blog
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Author { get; set; }
        public DateTime PublishDate { get; set; }
        public string Image { get; set; }  // save image path on client storage

        public PropertyChangedEventHandler PropertyChanged;

    }
}
