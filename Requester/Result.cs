using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Requester
{
    public class Result
    {
        public string URL { get; set; }
        public bool Passed { get; set; }
        public string State { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }
        public Result(string url, bool passed, string state, string body, string description)
        {
            URL = url;
            Passed = passed;
            State = state;
            Body = body;
            Description = description;
        }
    }
}
