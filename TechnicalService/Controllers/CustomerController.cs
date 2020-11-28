using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TechnicalService.Dto;
using TechnicalService.Models;
using TechnicalService.Services.Abstract;

namespace TechnicalService.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        IDistributedCache _distributedCache;
        ICustomerService _customerService;
        IMapper _mapper;
        public CustomerController(IDistributedCache distributedCache, ICustomerService customerService, IMapper mapper)
        {
            _distributedCache = distributedCache;
            _customerService = customerService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = JsonConvert.DeserializeObject<ClaimDto>(User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).SingleOrDefault());
            var cacheData = await _distributedCache.GetStringAsync("customer");
            if (cacheData == null)
            {
                var data = await _customerService.GetAllByUserId(user.Id);
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
            ViewBag.Categories = await _customerService.GetCategories();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Add(WorkDto worksDto)
        {
            ViewBag.Categories = await _customerService.GetCategories();
            if (!ModelState.IsValid)
            {
                return View(worksDto);

            }
            var user = JsonConvert.DeserializeObject<ClaimDto>(User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault());
            worksDto.UserId = user.Id;
            await _customerService.Add(worksDto);
            await _distributedCache.RemoveAsync("customer");
            return RedirectToAction("Index", "Customer");
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.Categories = await _customerService.GetCategories();
            var entity = _mapper.Map<WorkDto>(await _customerService.Get<Works>(x => x.Id == id));
            return View(entity);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(WorkDto worksDto)
        {
            if (!ModelState.IsValid)
            {
                return View(worksDto);
            }
            await _customerService.Update(worksDto);
            await _distributedCache.RemoveAsync("customer");
            return RedirectToAction("Index", "Customer");
        }
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _customerService.Delete(id);
            await _distributedCache.RemoveAsync("customer");
            return RedirectToAction("Index", "Customer");
        }

    }
}