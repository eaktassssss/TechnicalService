using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TechnicalService.Dto;
using TechnicalService.Models;

namespace TechnicalService.Controllers
{
    public class AccountController : Controller
    {
        TechnicalServiceContext _databaseContext;
        public AccountController(TechnicalServiceContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserDto entity)
        {
            if (!ModelState.IsValid)
            {
                return View(entity);
            }
            var user = _databaseContext.Users.FirstOrDefault(x => x.UserName.Trim().ToLower() == entity.UserName.Trim().ToLower() && x.Password.Trim().ToLower() == entity.Password.Trim().ToLower());
            if (user != null)
            {
                var model = new ClaimDto { UserName = user.UserName, Type = user.Type, Id = user.Id };
                var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Role,user.Type),
                     new Claim(ClaimTypes.Name,JsonConvert.SerializeObject(model))
                };
                var identity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(claimsPrincipal);
                switch (user.Type)
                {
                    case "Customer":
                        return RedirectToAction("Index", "Customer");
                    case "ServiceManager":
                        return RedirectToAction("Index", "Manager");
                    default:
                        throw new Exception("Not Found User");
                }
            }
            return View(entity);
        }
        public async Task<ActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult AccessDenied()
        {

            return View();
        }

    }
}