using System.Threading.Tasks;
using ApiTemplate.Constants;
using IdentityServer.Abstract;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using StorageCore.Domain.Abstract;

namespace ApiTemplate.Controllers.V1
{
    public class AccountController : ApiBaseController
    {
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IAccountService accountService, IUnitOfWork unitOfWork)
        {
            _accountService = accountService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(PhoneNumberAuthenticateRequest request)
        {
            var resp = await _accountService.Authenticate(request);

            return StatusCode(ResultCodes.SuccessCreated, new ApiResponse
            {
                ResultCode = ResultCodes.Success,
                Result = resp
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateAccountRequest request)
        {
            using var tx = await _unitOfWork.CreateTransactionAsync();

            await _accountService.CreateAccountRequest(request, tx);

            tx.Commit();

            return Ok(new ApiResponse
            {
                ResultCode = ResultCodes.SuccessCreated,
            });
        }
    }
}