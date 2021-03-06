﻿using System;
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
        ICustomerService _customerService;
        IMapper _mapper;
        public CustomerController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = JsonConvert.DeserializeObject<ClaimDto>(User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).SingleOrDefault());
            var response = await _customerService.GetByUserId(user.Id);
            return View(response.Where(x => x.UserId == user.Id).ToList());
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
            return RedirectToAction("Index", "Customer");
        }
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _customerService.Delete(id);
            return RedirectToAction("Index", "Customer");
        }

    }
}