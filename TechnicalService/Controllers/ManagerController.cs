using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TechnicalService.Dto;
using TechnicalService.Models;
using TechnicalService.Services.Abstract;

namespace TechnicalService.Controllers
{
    [Authorize(Roles = "ServiceManager")]
    public class ManagerController : Controller
    {
        IManagerService _managerService;
        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }
        public async Task<ActionResult> Index()
        {
            var response = await _managerService.GetAll();
            return View(response);
        }
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _managerService.Delete(id);
            return RedirectToAction("Index", "Manager");
        }
        public async Task<ActionResult> ChangeStatus(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _managerService.ChangeStatus(id);
            return RedirectToAction("Index", "Manager");
        }
    }
}