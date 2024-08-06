using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class TokenEmployeeDto
    {
        public string Token {  get; set; }
        public int EntriesCount     { get; set; }
        public Employee Employe { get; set; }
    }
}
