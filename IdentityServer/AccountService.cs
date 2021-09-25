using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Abstract;
using IdentityServer.Exceptions;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace IdentityServer
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<AccountService> _logger;

        private readonly AccountOptions _options;

        public AccountService(IOptions<AccountOptions> options, 
            UserManager<Account> userManager, 
            SignInManager<Account> signInManager, 
            ILogger<AccountService> logger,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentException(nameof(signInManager));
            _options = options?.Value ?? throw new ArgumentException(nameof(options));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
        }

        public async Task<AuthenticateResponse> Authenticate(EmailAuthenticateRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                throw new AuthenticateException("PhoneNumber or password incorrect");
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new AuthenticateException("PhoneNumber or password incorrect");
            }

            string token = this.GenerateJwtToken(user);

            _logger.LogInformation("email auth request processed @request", request);

            return new AuthenticateResponse
            {
                JwtToken = token,
                RefreshToken = null
            };
        }

        public async Task<AuthenticateResponse> Authenticate(PhoneNumberAuthenticateRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);

            if (user is null)
            {
                throw new AuthenticateException("PhoneNumber or password incorrect");
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new AuthenticateException("PhoneNumber or password incorrect");
            }

            string token = this.GenerateJwtToken(user);

            _logger.LogInformation("phone number auth request processed @request", request);

            return new AuthenticateResponse
            {
                JwtToken = token,
                RefreshToken = null
            };
        }

        public async Task CreateAccountRequest(CreateAccountRequest request, DbTransaction transaction)
        {
            if ((await _userManager.FindByEmailAsync(request.Email)) != null)
            {
                throw new IdentityException($"Email '{request.Email}' already exists.");
            }

            if ((await _userManager.FindByNameAsync(request.PhoneNumber)) != null)
            {
                throw new IdentityException($"PhoneNumber '{request.PhoneNumber}' already exists.");
            }

            var user = new Account
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpperInvariant(),
                PhoneNumber = request.PhoneNumber,
            };

            foreach (var validator in _userManager.UserValidators)
            {
                var validationRes = await validator.ValidateAsync(_userManager, user);

                if(!validationRes.Succeeded)
                {
                    throw new IdentityException(this.ExtractErrorString(validationRes));
                }
            }

            foreach (var validator in _userManager.PasswordValidators)
            {
                var validationRes = await validator.ValidateAsync(_userManager, user, request.Password);

                if (!validationRes.Succeeded)
                {
                    throw new IdentityException(this.ExtractErrorString(validationRes));
                }
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.Password);

            await _unitOfWork.UserRepository.CreateAsync(user, transaction);

            var role = await _unitOfWork.RoleRepository.GetAsync(request.Rolename.ToUpperInvariant());

            await _unitOfWork.UserRoleRepository.CreateAsync(new AccountRole
            {
                Role = role,
                User = user
            }, transaction);

            _logger.LogInformation("create account processed @request", request);
        }


        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_options.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                new[] {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new Claim(ClaimTypes.Name, account.PhoneNumber),
                    new Claim(ClaimTypes.Email, account.Email),
                }),
                Expires = DateTime.UtcNow.AddDays(15), //Token expires after 15 days
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        private string ExtractErrorString(IdentityResult result)
        {
            var builder = new StringBuilder();

            foreach (var error in result?.Errors ?? new List<IdentityError>())
            {
                builder.Append(error?.Description ?? "Can not create account" + ".");

            }
            return builder.ToString();
        }
    }
}