using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    [Authorize(Roles = "Administrator")]
    public class LeaveTypesController : Controller
    {
        private ILeaveTypeRepository _repo;
        private IMapper _mapper;

        public LeaveTypesController(ILeaveTypeRepository repo, IMapper mapper)
        {
            this._repo = repo;
            this._mapper = mapper;
        }
        // GET: LeaveTypes
        public ActionResult Index()
        {
            var leaveTypes = this._repo.FindAll().ToList();
            var model = this._mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes);
            return this.View(model);
        }

        // GET: LeaveTypes/Details/5
        public ActionResult Details(int id)
        {
            if (!this._repo.isExists(id))
            {
                return this.NotFound();
            }
            var leaveType = this._repo.FindById(id);
            var model = this._mapper.Map<LeaveTypeViewModel>(leaveType);
            return this.View(model);
        }

        // GET: LeaveTypes/Create
        public ActionResult Create()
        {
            return this.View();
        }

        // POST: LeaveTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeaveTypeViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if (!this.ModelState.IsValid)
                {
                    return this.View(model);
                }

                var leaveType = this._mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;
                var isSuccess = this._repo.Create(leaveType);

                if (!isSuccess)
                {
                    this.ModelState.AddModelError("", "Something went wrong...");
                    return this.View(model);
                }

                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong..."); 
                return this.View();
            }
        }

        // GET: LeaveTypes/Edit/5
        public ActionResult Edit(int id)
        {
            if (!this._repo.isExists(id))
            {
                return this.NotFound();
            }

            var leaveType = this._repo.FindById(id);
            var model = this._mapper.Map<LeaveTypeViewModel>(leaveType);

            return this.View(model);
        }

        // POST: LeaveTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeaveTypeViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (!this.ModelState.IsValid)
                {
                    return this.View(model);
                }

                var leaveType = this._mapper.Map<LeaveType>(model);
                var isSuccess = this._repo.Update(leaveType);
                if (!isSuccess)
                {
                    this.ModelState.AddModelError("", "Something went wrong...");
                    return this.View(model);
                }
                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                this.ModelState.AddModelError("", "Something went wrong...");
                return this.View();
            }
        }

        // GET: LeaveTypes/Delete/5
        public ActionResult Delete(int id)
        {
            var leaveType = this._repo.FindById(id);
            if (leaveType == null)
            {
                return this.NotFound();
            }

            var isSuccess = this._repo.Delete(leaveType);
            if (!isSuccess)
            {
                return this.BadRequest();
            }
            
            return this.RedirectToAction(nameof(this.Index));
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, LeaveTypeViewModel model)
        {
            try
            {
                // TODO: Add delete logic here
                var leaveType = this._repo.FindById(id);


                if(leaveType == null)
                {
                    return this.NotFound();
                }
                var isSuccess = this._repo.Delete(leaveType);
                if (!isSuccess)
                {
                    return this.View(model);
                }

                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                return this.View(model);
            }
        }
    }
}