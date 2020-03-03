using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Controllers
{
    using leave_management.Data;
    using leave_management.Models;
    using Microsoft.AspNetCore.Identity;

    [Authorize(Roles ="Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveTypeRepository _leaveRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(ILeaveTypeRepository leaveRepo, ILeaveAllocationRepository allocationRepo, IMapper mapper, UserManager<Employee> userManager)
        {
            this._leaveAllocationRepo = allocationRepo;
            this._leaveRepo = leaveRepo;
            this._mapper = mapper;
            this._userManager = userManager;
        }
        // GET: LeaveAllocation
        public ActionResult Index()
        {
            var leaveTypes = this._leaveRepo.FindAll().ToList();
            var mappedLeaveTypes = this._mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes);
            var model = new CreateLeaveAllocationViewModel
                            {
                                LeaveTypes = mappedLeaveTypes,
                                NumberUpdated =  0
                            };
            return this.View(model);
        }

        public ActionResult SetLeave(int id)
        {
            var leaveType = _leaveRepo.FindById(id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var emp in employees)
            {
                if (_leaveAllocationRepo.CheckAllocation(id, emp.Id))
                    continue;

                var allocation = new LeaveAllocationViewModel
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leaveType.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                _leaveAllocationRepo.Create(leaveAllocation);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployees()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeViewModel>>(employees);
            return View(model);
        }

        // GET: LeaveAllocation/Details/5
        public ActionResult Details(string id)
        {
            var employee = _mapper.Map<EmployeeViewModel>(_userManager.FindByIdAsync(id).Result);
            var allocations = _mapper.Map<List<LeaveAllocationViewModel>>(_leaveAllocationRepo.GetLeaveAllocationsByEmployee(id));
            var model = new ViewAllocationViewModel
            {
                Employee = employee,
                LeaveAllocations = allocations
            };
            return View(model);
        }

        // GET: LeaveAllocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Edit/5
        public ActionResult Edit(int id)
        {
            var leaveAllocation = _leaveAllocationRepo.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationViewModel>(leaveAllocation);
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var record = _leaveAllocationRepo.FindById(model.Id);
                record.NumberOfDays = model.NumberOfDays;

                var isSuccess = _leaveAllocationRepo.Update(record);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Error while saving");
                    return View(model);
                }

                return RedirectToAction(nameof(Details), new {id = model.EmployeeId});
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}