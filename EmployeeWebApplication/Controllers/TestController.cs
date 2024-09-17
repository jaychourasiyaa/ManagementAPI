using ManagementAPI.Contract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestServices _myService1;
        private readonly ITestServices _myService2;
        private readonly IServiceScopeFactory _scopeFactory;

        public TestController(ITestServices myService1, ITestServices myService2 , IServiceScopeFactory scopefactory)
        {
            _myService1 = myService1;
            _myService2 = myService2;
            _scopeFactory = scopefactory;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Service1Guid = _myService1.GetGuid(),
                Service2Guid = _myService2.GetGuid()
            });
        }
        [HttpGet("Id")]
        public IActionResult GetBy()
        {
            // Create a new scope manually
            using (var scope = _scopeFactory.CreateScope())
            {
                // Resolve a new instance of ITestServices within the new scope
                var myService2 = scope.ServiceProvider.GetRequiredService<ITestServices>();

                return Ok(new
                {
                    Service1Guid = _myService1.GetGuid(), // From the existing scope
                    Service2Guid = myService2.GetGuid()   // From the new manually created scope
                });
            }
        }
    }
}

