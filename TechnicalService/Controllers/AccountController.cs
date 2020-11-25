using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name,user.Type,user.UserName)
                };
                var identity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(claimsPrincipal);
                return RedirectToAction("Index", "TechnicalService");
            }
            return View(entity);
        }
    }
}