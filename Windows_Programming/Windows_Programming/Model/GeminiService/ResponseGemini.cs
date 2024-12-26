using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Model.GeminiService
{
    public class ResponseGemini
    {
        public class GeminiResponse
        {
            public List<Candidate> Candidates { get; set; } = new List<Candidate>();
        }

        public class Candidate
        {
            public Content Content { get; set; } = new Content();
        }

        public class Content
        {
            public List<Part> Parts { get; set; } = new List<Part>();
        }

        public class Part
        {
            public string Text { get; set; } = string.Empty;
        }
    }
}
