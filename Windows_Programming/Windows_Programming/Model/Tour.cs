﻿using System.Collections.Generic;

namespace Windows_Programming.Model
{
    public class Tour
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Places { get; set; }
        public string Description { get; set; }
        public string Schedule { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        public int Rating { get; set; }
        public string Link { get; set; }
        public Dictionary<string, string> Activities { get; set; }
        public Dictionary<string, Dictionary<string, string>> Transport { get; set; }
    }
}
