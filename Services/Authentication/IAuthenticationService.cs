using BackEnd.Data.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Authentication
{
    public interface IAuthenticationService
    {
        JwtToken LoginAsync(string UserName, string Password);
    }
}