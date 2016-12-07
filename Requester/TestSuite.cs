using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Requester
{
    public class TestSuite
    {
        private Collection<Test> _tests;
        public int TotalCount { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public Collection<Result> FailedInfo { get; set; }
        public TestSuite(Collection<Test> tests)
        {
            _tests = tests;
            TotalCount = _tests.Count;
            PassedCount = 0;
            FailedCount = 0;
        }
        public Collection<Result> Start(string oauth_token)
        {
            Result result;
            Collection<Result> results = new Collection<Result>();
            foreach (Test test in _tests)
            {
                result = test.Start(oauth_token);
                if (!result.Passed)
                {
                    FailedCount++;
                    results.Add(result);
                }
                else
                    PassedCount++;
            }
            if (results.Count <= 0)
                return null;
            else
                return results;
        }
    }
}
