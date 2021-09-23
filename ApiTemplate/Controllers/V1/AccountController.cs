using System.Threading.Tasks;
using IdentityServer.Abstarct;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using StorageCore.Domain.Abstract;

namespace ApiTemplate.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IAccountService accountService, IUnitOfWork unitOfWork)
        {
            _accountService = accountService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Authenticate(PhoneNumberAuthenticateRequest request)
        {
            var resp = await _accountService.Authenticate(request);

            return Ok(resp);
        }
    }
}