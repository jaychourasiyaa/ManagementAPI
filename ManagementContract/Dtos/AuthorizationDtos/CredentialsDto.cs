using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.AuthorizationDtos
{
    public class CredentialsDto
    {
        [Required(ErrorMessage = "username is required field")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "username length must be greater than 1")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required field")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "password length must be greater than 6")]
        public string Password { get; set; }
    }
}
