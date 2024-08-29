using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Provider.Services
{
    public class CustomHttpContextAccessor : IHttpContextAccessor
    {
        // AsyncLocal is used to store the HttpContext for the current async context.
        private readonly AsyncLocal<HttpContextHolder> _httpContextCurrent = new();

        public HttpContext? HttpContext
        {
            get => _httpContextCurrent.Value?.Context;
            set
            {
                var holder = _httpContextCurrent.Value;
                if (holder != null)
                {
                    // Clear the current HttpContext trapped in the AsyncLocal.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use a new HttpContextHolder object to store the HttpContext in the AsyncLocal.
                    _httpContextCurrent.Value = new HttpContextHolder { Context = value };
                }
            }
        }

        // Helper class to store the HttpContext in AsyncLocal.
        private class HttpContextHolder
        {
            public HttpContext? Context { get; set; }
        }
    }

}

