using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Abstarct;
using IdentityServer.Exceptions;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace IdentityServer
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ILogger<AccountService> _logger;

        private readonly AccountOptions _options;

        public AccountService(AccountOptions options, UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = options;
            _logger = logger;
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

            string token = this.generateJwtToken(user);

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

            string token = this.generateJwtToken(user);

            _logger.LogInformation("phone number auth request processed @request", request);

            return new AuthenticateResponse
            {
                JwtToken = token,
                RefreshToken = null
            };
        }

        public async Task CreateAccountRequest(CreateAccountRequest request)
        {
            if ((await _userManager.FindByEmailAsync(request.Email)) != null)
            {
                throw new IdentityException($"Email '{request.Email}' already exists.");
            }

            var user = new User
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new IdentityException(this.extarctErrorString(result));

            }

            await _userManager.AddToRoleAsync(user, request.Rolename);

            _logger.LogInformation("create account processed @request", request);
        }



        private string generateJwtToken(User account)
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


        private string extarctErrorString(IdentityResult result)
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