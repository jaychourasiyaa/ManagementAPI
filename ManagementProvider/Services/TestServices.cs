using ManagementAPI.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Provider.Services
{
    public class TestServices : ITestServices
    {
        private readonly string _guid;

        public TestServices()
        {
            _guid = Guid.NewGuid().ToString();  // Generate a unique GUID when the instance is created
        }

        public string GetGuid()
        {
            return _guid;  // Return the same GUID for this instance
        }
    }
}
