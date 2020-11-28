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
    [Authorize]
    public class TechnicalServiceController : Controller
    {
        IDistributedCache _distributedCache;
        ITechnicalService _technicalService;
        IMapper _mapper;
        public TechnicalServiceController(IDistributedCache distributedCache, ITechnicalService technicalService, IMapper mapper)
        {
            _distributedCache = distributedCache;
            _technicalService = technicalService;
            _mapper = mapper;
        }


        [Authorize(Roles ="ServiceManager")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = JsonConvert.DeserializeObject<ClaimDto>(User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).SingleOrDefault());
            var cacheData = await _distributedCache.GetStringAsync("servicemanager");
            if (cacheData == null)
            {
                var data = await _technicalService.GetAll();
                if (data.Any())
                {
                    await _distributedCache.SetStringAsync("servicemanager", JsonConvert.SerializeObject(data));
                }
                return View(data);
            }
            else
            {
                var data = JsonConvert.DeserializeObject<List<WorksDto>>(await _distributedCache.GetStringAsync("servicemanager"));
                return View(data);
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<ActionResult> CustomerPanel()
        {
            var user = JsonConvert.DeserializeObject<ClaimDto>(User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).SingleOrDefault());
            var cacheData = await _distributedCache.GetStringAsync("customer");
            if (cacheData == null)
            {
                var data = await _technicalService.GetAllByUserId(user.Id);
                if (data.Any())
                {
                    await _distributedCache.SetStringAsync("customer", JsonConvert.SerializeObject(data));
                }
                return View(data);
            }
            else
            {
                var data = JsonConvert.DeserializeObject<List<WorksDto>>(await _distributedCache.GetStringAsync("customer"));
                return View(data);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Add()
        {
            ViewBag.Categories = await _technicalService.GetCategories();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Add(WorkDto worksDto)
        {
            ViewBag.Categories = await _technicalService.GetCategories();
            if (!ModelState.IsValid)
            {
                return View(worksDto);

            }
            var user = JsonConvert.DeserializeObject<ClaimDto>(User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault());
            worksDto.UserId = user.Id;
            await _technicalService.Add(worksDto);
            await _distributedCache.RemoveAsync("works");
            return RedirectToAction("Index", "TechnicalService");
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.Categories = await _technicalService.GetCategories();
            var entity = _mapper.Map<WorkDto>(await _technicalService.Get<Works>(x => x.Id == id));
            return View(entity);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(WorkDto worksDto)
        {

            if (!ModelState.IsValid)
            {
                return View(worksDto);
            }
            await _technicalService.Update(worksDto);
            await _distributedCache.RemoveAsync("works");
            return RedirectToAction("Index", "TechnicalService");
        }
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _technicalService.Delete(id);
            await _distributedCache.RemoveAsync("works");
            return RedirectToAction("Index", "TechnicalService");
        }
        public async Task<ActionResult> ChangeStatus(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _technicalService.ChangeStatus(id);
            await _distributedCache.RemoveAsync("works");
            return RedirectToAction("Index", "TechnicalService");
        }
    }
}