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
        IDistributedCache _distributedCache;
        IManagerService _technicalService;
        public ManagerController(IDistributedCache distributedCache, IManagerService technicalService)
        {
            _distributedCache = distributedCache;
            _technicalService = technicalService;
        }
        public async Task<ActionResult> Index()
        {
            var cacheData = await _distributedCache.GetStringAsync("manager");
            if (cacheData == null)
            {
                var data = await _technicalService.GetAll();
                if (data.Any())
                {
                    await _distributedCache.SetStringAsync("manager", JsonConvert.SerializeObject(data));
                }
                return View(data);
            }
            else
            {
                var cacheDate = await _distributedCache.GetStringAsync("manager");
                var data = JsonConvert.DeserializeObject<List<WorksDto>>(cacheDate);
                return View(data);
            }
        }
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _technicalService.Delete(id);
            await _distributedCache.RemoveAsync("manager");
            return RedirectToAction("Index", "Manager");
        }
        public async Task<ActionResult> ChangeStatus(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _technicalService.ChangeStatus(id);
            await _distributedCache.RemoveAsync("manager");
            return RedirectToAction("Index", "Manager");
        }
    }
}