﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;

namespace OpenLawOffice.WebClient.Controllers
{
    public class TaskTimeController : BaseController
    {
        [SecurityFilter(SecurityAreaName = "Tasks", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Create)]
        public ActionResult SelectContactToAssign()
        {
            List<ViewModels.Contacts.SelectableContactViewModel> modelList = new List<ViewModels.Contacts.SelectableContactViewModel>();
            List<Common.Models.Contacts.Contact> contactList = OpenLawOffice.Data.Contacts.Contact.List();

            contactList.ForEach(x =>
            {
                modelList.Add(Mapper.Map<ViewModels.Contacts.SelectableContactViewModel>(x));
            });

            return View(modelList);
        }

        [SecurityFilter(SecurityAreaName = "Tasks", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Create)]
        public ActionResult Create()
        {
            ViewModels.Tasks.TaskTimeViewModel viewModel = null;

            // Every TaskTime must be created from a task, so we should always know the TaskId
            long taskId = long.Parse(Request["TaskId"]);
            int contactId = int.Parse(Request["ContactId"]);

            // Load task & contact
            Common.Models.Tasks.Task task = Data.Tasks.Task.Get(taskId);
            Common.Models.Contacts.Contact contact = Data.Contacts.Contact.Get(contactId);

            viewModel = new ViewModels.Tasks.TaskTimeViewModel()
            {
                Task = Mapper.Map<ViewModels.Tasks.TaskViewModel>(task),
                Time = new ViewModels.Timing.TimeViewModel()
                {
                    Worker = Mapper.Map<ViewModels.Contacts.ContactViewModel>(contact),
                    Start = DateTime.Now
                }
            };

            return View(viewModel);
        }

        [SecurityFilter(SecurityAreaName = "Tasks", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Create)]
        [HttpPost]
        public ActionResult Create(ViewModels.Tasks.TaskTimeViewModel viewModel)
        {
            try
            {
                Common.Models.Security.User currentUser = UserCache.Instance.Lookup(Request);
                Common.Models.Tasks.TaskTime taskTime = Mapper.Map<Common.Models.Tasks.TaskTime>(viewModel);
                taskTime.Time = Mapper.Map<Common.Models.Timing.Time>(viewModel.Time);

                taskTime.Time = Data.Timing.Time.Create(taskTime.Time, currentUser);
                taskTime = Data.Tasks.TaskTime.Create(taskTime, currentUser);

                return RedirectToAction("Details", new { Id = taskTime.Id });
            }
            catch
            {
                return View(viewModel);
            }
        }

        [SecurityFilter(SecurityAreaName = "Tasks", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Create)]
        public ActionResult Details(Guid id)
        {
            Common.Models.Tasks.TaskTime taskTime = Data.Tasks.TaskTime.Get(id);
            taskTime.Task = Data.Tasks.Task.Get(taskTime.Task.Id.Value);
            taskTime.Time = Data.Timing.Time.Get(taskTime.Time.Id.Value);
            taskTime.Time.Worker = Data.Contacts.Contact.Get(taskTime.Time.Worker.Id.Value);

            ViewModels.Tasks.TaskTimeViewModel viewModel = Mapper.Map<ViewModels.Tasks.TaskTimeViewModel>(taskTime);
            viewModel.Task = Mapper.Map<ViewModels.Tasks.TaskViewModel>(taskTime.Task);
            viewModel.Time = Mapper.Map<ViewModels.Timing.TimeViewModel>(taskTime.Time);
            viewModel.Time.Worker = Mapper.Map<ViewModels.Contacts.ContactViewModel>(taskTime.Time.Worker);

            return View(viewModel);
        }
    }
}