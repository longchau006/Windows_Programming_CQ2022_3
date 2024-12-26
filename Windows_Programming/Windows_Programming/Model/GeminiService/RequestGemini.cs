using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Model.GeminiService
{
    public class RequestGemini
    {
        public class FunctionDefinition
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public Dictionary<string, ParameterDefinition> Parameters { get; set; }
        }

        public class ParameterDefinition
        {
            public string Type { get; set; }
            public string Description { get; set; }
        }
    }
}
