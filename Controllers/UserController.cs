using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ttt.Models;
using ttt.Models.DTOs;
using ttt.Models.Repositories;

namespace ttt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        IRepository repo;

        readonly string conString;
        public UserController(IConfiguration config)
        {
            repo = new SqliteTestRepository(config);
            conString = config.GetConnectionString("sqlite");
        }



        ////// 7. If user not authorized, the request is redirected to /api/user/login
        ////// 8. If user is authorized but isn't in role Admin he receives the 403 status (look in Startup.cs)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                using (DbConnection db = new SQLiteConnection(conString))
                {
                    IEnumerable<User> users = await db.QueryAsync<User>(@"SELECT
                                                                  u.Id
                                                                  ,u.DomainName
                                                                  ,u.RoleId
                                                                  ,r.RoleName
                                                                  ,u.Email
                                                                  ,u.FIO
                                                                FROM Users u
                                                                JOIN UserRoles r ON u.RoleId=r.Id");
                    return Ok(users);
                }
            }
            catch (System.Exception e) { return BadRequest(e.Message); }
        }



        /////// 1. Request hits this method either within first [Authorized] method call or if it's actually forbidden
        [HttpGet("ValidateAsync")]
        public async Task<IActionResult> ValidateAsync(string ReturnUrl)  ///// request URI contains ReturnUrl
        {
            ////// 2. If user has already got the Role claim, return forbidden status
            if (HttpContext.User.FindFirst(ClaimTypes.Role) != null)
                return StatusCode(StatusCodes.Status405MethodNotAllowed, "You are not allowed to use this resource");

            ////// 3. No Role claim exists. As user is authenticated, retrieve Domain name
            //string domainName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ////? "IvanovNA01"
            //? "DolgiyGR"
            //: HttpContext.User.Identity.Name.Split("\\")[1];
            string domainName = HttpContext.User.Identity.Name.Split("\\")[1];

            try
            {
                string roleName = null;

                ////// 4. Here we either got the role or threw an exception [sequence containes no element..]
                await using (DbConnection db = new SQLiteConnection(conString))
                    roleName = await db.QuerySingleAsync<string>(@"SELECT RoleName
                                                                      FROM UserRoles
                                                                      JOIN Users ON UserRoles.Id=Users.RoleId
                                                                      WHERE Upper(Users.DomainName)=:DomainName", new { DomainName = domainName.ToUpperInvariant() });

                ////// 5. We get the role. Fill new claims
                Claim[] claims = new[] {
          new Claim(ClaimsIdentity.DefaultNameClaimType, $"MECHEL\\{domainName}"),
          new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName)
        };

                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal newUser = new ClaimsPrincipal(identity);

                ////// 6. Replace Negotiate authentication scheme, sign in and redirect to the request's initial url
                await HttpContext.SignInAsync(newUser);
                return Redirect(ReturnUrl);
            }
            catch (System.Exception e) { return BadRequest(e.Message); }
            ///// 5. If we threw an exception, return Unauthorized status
        }




        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync();
                return Ok();
            }
            catch (System.Exception e) { return BadRequest(e.Message); }
        }



        [HttpGet("[action]")]
        public List<Test> GetTests()
        {
            try { return repo.GetList(); }
            catch (System.Exception) { return null; }
        }


        [HttpGet("[action]")]
        public void InsertTest(string name, int age)
        {
            Test toInsert = new Test()
            {
                Age = age,
                Name = name,
            };

            try { repo.Create(toInsert); }
            catch (System.Exception) {  }
        }
    }
}
