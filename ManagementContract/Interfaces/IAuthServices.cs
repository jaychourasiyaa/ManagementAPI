using ManagementAPI.Contract.Dtos.AuthorizationDtos;
using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Interfaces
{
    public interface IAuthServices
    {
      
        public Task<LoginResponseDto?> LoginUser(CredentialsDto dto);
        



    }
}
