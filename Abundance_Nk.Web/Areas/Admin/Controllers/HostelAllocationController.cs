using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System.Transactions;
using System.Web.Script.Serialization;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using MailerApp.Business;
using Microsoft.Ajax.Utilities;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class HostelAllocationController : BaseController
    {
        private HostelViewModel viewModel;
        // GET: Admin/HostelAllocation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EditHostelRooms()
        {
            try
            {
                viewModel = new HostelViewModel();
                PopulateDropDownList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! ", Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditHostelRooms(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();

                    List<RoomSetting> roomSettings = new List<RoomSetting>();
                    List<string> corners = new List<string>();

                    viewModel.HostelRoomList = hostelRoomLogic.GetModelsBy(hr => hr.Hostel_Id == viewModel.HostelRoom.Hostel.Id && hr.Series_Id == viewModel.HostelRoom.Series.Id);
                    if (viewModel.HostelRoomList.Count <= 0)
                    {
                        SetMessage("Rooms has not been created for this Hostel-Series combination", Message.Category.Error);
                        RetainDropDownList(viewModel);
                        return View(viewModel);
                    }
                    for (int i = 0; i < viewModel.HostelRoomList.Count; i++)
                    {
                        long roomId = viewModel.HostelRoomList[i].Id;
                        List<HostelRoomCorner> hostelRoomCornerList = hostelRoomCornerLogic.GetModelsBy(hrc => hrc.Room_Id == roomId);
                        if (hostelRoomCornerList != null)
                        {
                            for (int j = 0; j < hostelRoomCornerList.Count; j++)
                            {
                                if (hostelRoomCornerList[j].Name!=null && !corners.Contains(hostelRoomCornerList[j].Name.Trim()))
                                {
                                    corners.Add(hostelRoomCornerList[j].Name.Trim());
                                }
                            }

                            RoomSetting roomSetting = new RoomSetting();
                            roomSetting.HostelRoom = viewModel.HostelRoomList[i];
                            roomSetting.HostelRoomCorners = hostelRoomCornerList;

                            roomSettings.Add(roomSetting);
                        }
                    }

                    viewModel.Corners = corners;
                    viewModel.RoomSettings = roomSettings;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }
        public ActionResult SaveEditedRooms(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelRoom != null && viewModel.RoomSettings.Count > 0)
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();

                    using (TransactionScope scope = new TransactionScope())
                    {
                        for (int i = 0; i < viewModel.RoomSettings.Count; i++)
                        {
                            long roomId = viewModel.RoomSettings[i].HostelRoom.Id;

                            HostelRoom hostelRoom = hostelRoomLogic.GetModelBy(hr => hr.Room_Id == roomId);
                            hostelRoom.Number = viewModel.RoomSettings[i].HostelRoom.Number;
                            hostelRoom.Reserved = viewModel.RoomSettings[i].HostelRoom.Reserved;
                            hostelRoom.Activated = viewModel.RoomSettings[i].HostelRoom.Activated;
                            hostelRoom.Series = viewModel.HostelRoom.Series;
                            hostelRoom.Hostel = viewModel.HostelRoom.Hostel;

                            hostelRoomLogic.Modify(hostelRoom);

                            for (int j = 0; j < viewModel.RoomSettings[i].HostelRoomCorners.Count; j++)
                            {
                                long cornerId = viewModel.RoomSettings[i].HostelRoomCorners[j].Id;

                                HostelRoomCorner hostelRoomCorner = hostelRoomCornerLogic.GetModelBy(hrc => hrc.Corner_Id == cornerId);
                                hostelRoomCorner.Name = viewModel.RoomSettings[i].HostelRoomCorners[j].Name;
                                hostelRoomCorner.Activated = viewModel.RoomSettings[i].HostelRoomCorners[j].Activated;
                                hostelRoomCorner.Room = viewModel.RoomSettings[i].HostelRoom;

                                hostelRoomCornerLogic.Modify(hostelRoomCorner);
                            }

                        }

                        SetMessage("Operation Successful! ", Message.Category.Information);

                        scope.Complete();
                    }
                }
                else
                {
                    SetMessage("Incomplete Input Elements Required To Perform This Operation", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("EditHostelRooms");
        }
        private void PopulateDropDownList()
        {
            try
            {
                ViewBag.HostelId = viewModel.HostelSelectListItem;
                ViewBag.HostelSeriesId = new SelectList(new List<HostelSeries>(), "Id", "Name");
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), "Id", "Name");
                ViewBag.LevelId = viewModel.LevelSelectListItem;
                ViewBag.CornerId = new MultiSelectList(new List<HostelRoomCorner>(), "Id", "Name");
                ViewBag.RoomId = new SelectList(new List<HostelRoom>(), "Id", "Name");
                ViewBag.SessionId = viewModel.SessionSelectListItem;

            }
            catch (Exception)
            {
                throw;
            }
        }
        private void RetainDropDownList(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    if (viewModel.HostelRoom != null)
                    {
                        if (viewModel.HostelRoom.Hostel != null)
                        {
                            ViewBag.HostelId = new SelectList(viewModel.HostelSelectListItem, "Value", "Text", viewModel.HostelRoom.Hostel.Id);
                        }
                        else
                        {
                            ViewBag.HostelId = viewModel.HostelSelectListItem;
                        }

                        if (viewModel.HostelRoom.Series != null && viewModel.HostelRoom.Hostel != null)
                        {
                            ViewBag.HostelSeriesId = new SelectList(Utility.PopulateHostelSeries(viewModel.HostelRoom.Hostel), "Value", "Text", viewModel.HostelRoom.Series.Id);
                        }
                        else
                        {
                            ViewBag.HostelSeriesId = new SelectList(new List<HostelSeries>(), "Id", "Name");
                        }
                    }

                    if (viewModel.HostelAllocationCriteria != null)
                    {
                        if (viewModel.HostelAllocationCriteria.Level != null)
                        {
                            ViewBag.LevelId = new SelectList(viewModel.LevelSelectListItem, "Value", "Text", viewModel.HostelAllocationCriteria.Level.Id);
                        }
                        else
                        {
                            ViewBag.LevelId = viewModel.LevelSelectListItem;
                        }

                        if (viewModel.HostelAllocationCriteria.Hostel != null)
                        {
                            ViewBag.HostelId = new SelectList(viewModel.HostelSelectListItem, "Value", "Text", viewModel.HostelAllocationCriteria.Hostel.Id);
                        }
                        else
                        {
                            ViewBag.HostelId = viewModel.HostelSelectListItem;
                        }

                        if (viewModel.HostelAllocationCriteria.Series != null && viewModel.HostelAllocationCriteria.Hostel != null)
                        {
                            ViewBag.HostelSeriesId = new SelectList(Utility.PopulateHostelSeries(viewModel.HostelAllocationCriteria.Hostel), "Value", "Text", viewModel.HostelAllocationCriteria.Series.Id);
                        }
                        else
                        {
                            ViewBag.HostelSeriesId = new SelectList(new List<HostelSeries>(), "Id", "Name");
                        }
                        if (viewModel.HostelAllocationCriteria.Corner != null && viewModel.HostelAllocationCriteria.Corner != null)
                        {
                            ViewBag.CornerId = new SelectList(Utility.PopulateHostelRoomCorners(viewModel.HostelAllocationCriteria.Room), "Value", "Text", viewModel.HostelAllocationCriteria.Corner.Id);
                        }
                        else
                        {
                            ViewBag.CornerId = new MultiSelectList(new List<HostelRoomCorner>(), "Id", "Name");
                        }
                        if (viewModel.HostelAllocationCriteria.Hostel != null && viewModel.HostelAllocationCriteria.Corner != null)
                        {
                            ViewBag.RoomId = new SelectList(Utility.PopulateHostelRooms(viewModel.HostelAllocationCriteria.Series), "Value", "Text", viewModel.HostelAllocationCriteria.Room.Id);
                        }
                        else
                        {

                            ViewBag.RoomId = new SelectList(new List<HostelRoom>(), "Id", "Name");
                        }
                    }

                    if (viewModel.HostelAllocation != null)
                    {

                        if (viewModel.HostelAllocation.Series != null && viewModel.HostelAllocation.Hostel != null)
                        {
                            ViewBag.HostelSeriesId = new SelectList(Utility.PopulateHostelSeries(viewModel.HostelAllocation.Hostel), "Value", "Text", viewModel.HostelAllocation.Series.Id);
                        }
                        else
                        {
                            ViewBag.HostelSeriesId = new SelectList(new List<HostelSeries>(), "Id", "Name");
                        }
                        if (viewModel.HostelAllocation.Corner != null && viewModel.HostelAllocation.Corner != null)
                        {
                            ViewBag.CornerId = new SelectList(Utility.PopulateHostelRoomCorners(viewModel.HostelAllocation.Room), "Value", "Text", viewModel.HostelAllocation.Corner.Id);
                        }
                        else
                        {
                            ViewBag.CornerId = new MultiSelectList(new List<HostelRoomCorner>(), "Id", "Name");
                        }
                        if (viewModel.HostelAllocation.Hostel != null && viewModel.HostelAllocation.Corner != null)
                        {
                            ViewBag.RoomId = new SelectList(Utility.PopulateHostelRooms(viewModel.HostelAllocation.Series), "Value", "Text", viewModel.HostelAllocation.Room.Id);
                        }
                        else
                        {

                            ViewBag.RoomId = new SelectList(new List<HostelRoom>(), "Id", "Name");
                        }
                        if (viewModel.HostelAllocation.Hostel != null)
                        {
                            ViewBag.HostelId = new SelectList(viewModel.HostelSelectListItem, "Value", "Text", viewModel.HostelAllocation.Hostel.Id);
                        }
                        else
                        {
                            ViewBag.HostelId = viewModel.HostelSelectListItem;
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult CreateHostelRooms()
        {
            try
            {
                viewModel = new HostelViewModel();
                PopulateDropDownList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! ", Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult CreateHostelRooms(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                    HostelLogic hostelLogic = new HostelLogic();

                    HostelSeries hostelSeries = hostelSeriesLogic.GetModelBy(hs => hs.Series_Id == viewModel.HostelRoom.Series.Id);
                    Hostel hostel = hostelLogic.GetModelBy(h => h.Hostel_Id == viewModel.HostelRoom.Hostel.Id);

                    if (hostelSeries == null)
                    {
                        SetMessage("Select the hostel Series", Message.Category.Error);
                        RetainDropDownList(viewModel);
                        return View(viewModel);
                    }

                    int roomCapacity = Convert.ToInt32(viewModel.HostelRoom.RoomCapacity);
                    int coners = Convert.ToInt32(viewModel.HostelRoom.Corners);

                    List<RoomSetting> allRoomSettings = new List<RoomSetting>();

                    for (int i = 0; i < roomCapacity; i++)
                    {
                        HostelRoom hostelRoom = new HostelRoom();
                        hostelRoom.Hostel = hostel;
                        hostelRoom.Series = hostelSeries;
                        hostelRoom.Activated = true;
                        hostelRoom.Reserved = false;
                        hostelRoom.Number = "Room " + (i + 1).ToString();

                        //if (hostelSeries.Id == 1)
                        //{
                        //    hostelRoom.Number = (100 + i).ToString();
                        //}
                        //if (hostelSeries.Id == 2)
                        //{
                        //    hostelRoom.Number = (200 + i).ToString();
                        //}
                        //if (hostelSeries.Id == 3)
                        //{
                        //    hostelRoom.Number = (300 + i).ToString();
                        //}
                        //if (hostelSeries.Id == 4)
                        //{
                        //    hostelRoom.Number = (400 + i).ToString();
                        //}

                        List<HostelRoomCorner> hostelRoomCorners = new List<HostelRoomCorner>();
                        for (int j = 0; j < coners; j++)
                        {
                            HostelRoomCorner hostelRoomCorner = new HostelRoomCorner();
                            hostelRoomCorner.Activated = true;
                            hostelRoomCorner.Name = GetCornerName(j);

                            hostelRoomCorners.Add(hostelRoomCorner);
                        }

                        allRoomSettings.Add(new RoomSetting() { HostelRoom = hostelRoom, HostelRoomCorners = hostelRoomCorners });
                    }

                    viewModel.RoomSettings = allRoomSettings;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! ", Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }
        public ActionResult SaveRooms(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelRoom != null && viewModel.RoomSettings.Count > 0)
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();

                    List<HostelRoom> hostelRooms = hostelRoomLogic.GetModelsBy(hr => hr.Hostel_Id == viewModel.HostelRoom.Hostel.Id && hr.Series_Id == viewModel.HostelRoom.Series.Id);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        for (int i = 0; i < viewModel.RoomSettings.Count; i++)
                        {
                            HostelRoom hostelRoomCheck = hostelRooms.Where(hr => hr.Number == viewModel.RoomSettings[i].HostelRoom.Number).FirstOrDefault();
                            if (hostelRoomCheck == null)
                            {
                                HostelRoom hostelRoom = new HostelRoom();
                                hostelRoom.Number = viewModel.RoomSettings[i].HostelRoom.Number;
                                hostelRoom.Reserved = viewModel.RoomSettings[i].HostelRoom.Reserved;
                                hostelRoom.Activated = viewModel.RoomSettings[i].HostelRoom.Activated;
                                hostelRoom.Series = viewModel.HostelRoom.Series;
                                hostelRoom.Hostel = viewModel.HostelRoom.Hostel;

                                HostelRoom newHostelRoom = hostelRoomLogic.Create(hostelRoom);

                                for (int j = 0; j < viewModel.RoomSettings[i].HostelRoomCorners.Count; j++)
                                {
                                    HostelRoomCorner hostelRoomCorner = new HostelRoomCorner();
                                    hostelRoomCorner.Name = viewModel.RoomSettings[i].HostelRoomCorners[j].Name;
                                    hostelRoomCorner.Activated = viewModel.RoomSettings[i].HostelRoomCorners[j].Activated;
                                    hostelRoomCorner.Room = newHostelRoom;

                                    hostelRoomCornerLogic.Create(hostelRoomCorner);
                                }
                            }
                        }

                        SetMessage("Operation Successful! ", Message.Category.Information);

                        scope.Complete();
                    }
                }
                else
                {
                    SetMessage("Incomplete Input Elements Required To Perform This Operation", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("CreateHostelRooms");
        }
        public ActionResult CreateHostelAllocationCriteria()
        {
            try
            {
                viewModel = new HostelViewModel();
                PopulateDropDownList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! ", Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult CreateHostelAllocationCriteria(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelAllocationCriteria.Series != null && viewModel.SelectedCorners.Any())
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    HostelAllocationCriteriaLogic hostelAllocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                    HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();
                    HostelAllocationCriteria hostelAllocationCriteria = new HostelAllocationCriteria();

                    hostelAllocationCriteria.Hostel = viewModel.HostelAllocationCriteria.Hostel;
                    hostelAllocationCriteria.Level = viewModel.HostelAllocationCriteria.Level;
                    hostelAllocationCriteria.Series = viewModel.HostelAllocationCriteria.Series;

                    List<HostelRoom> hostelRooms = hostelRoomLogic.GetModelsBy(hr => hr.Series_Id == viewModel.HostelAllocationCriteria.Series.Id && hr.Activated);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        for (int i = 0; i < hostelRooms.Count; i++)
                        {
                            hostelAllocationCriteria.Room = hostelRooms[i];
                            for (int j = 0; j < viewModel.SelectedCorners.Count(); j++)
                            {
                                long roomId = hostelRooms[i].Id;
                                string cornerName = viewModel.SelectedCorners[j];
                                HostelRoomCorner hostelRoomCorner = hostelRoomCornerLogic.GetModelBy(hrc => hrc.Room_Id == roomId && hrc.Corner_Name == cornerName && hrc.Activated);
                                if (hostelRoomCorner != null)
                                {
                                    hostelAllocationCriteria.Corner = hostelRoomCorner;

                                    HostelAllocationCriteria existingCriteria = hostelAllocationCriteriaLogic.GetModelBy(hac => hac.Corner_Id == hostelRoomCorner.Id && hac.Hostel_Id == viewModel.HostelAllocationCriteria.Hostel.Id && hac.Level_Id == viewModel.HostelAllocationCriteria.Level.Id && hac.Room_Id == roomId && hac.Series_Id == viewModel.HostelAllocationCriteria.Series.Id);
                                    if (existingCriteria == null)
                                    {
                                        hostelAllocationCriteriaLogic.Create(hostelAllocationCriteria);
                                    }
                                }

                            }
                        }

                        scope.Complete();
                        SetMessage("Operation Successful!", Message.Category.Information);
                        return RedirectToAction("CreateHostelAllocationCriteria");
                    }
                }
                else
                {
                    SetMessage("Inadequate parameters required to service operation!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! ", Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }

        public ActionResult ViewHostelAllocationCriteria()
        {
            try
            {
                viewModel = new HostelViewModel();
                PopulateDropDownList();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewHostelAllocationCriteria(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelAllocationCriteriaLogic criteriaLogic = new HostelAllocationCriteriaLogic();
                    List<HostelAllocationCriteria> allocationCriterias = new List<HostelAllocationCriteria>();
                    allocationCriterias = criteriaLogic.GetModelsBy(h => h.Level_Id == viewModel.HostelAllocationCriteria.Level.Id);

                    viewModel.HostelAllocationCriterias = allocationCriterias;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }

        public ActionResult EditHostelAllocationCriteria(int hId)
        {
            try
            {
                viewModel = new HostelViewModel();
                HostelAllocationCriteriaLogic hostelAllocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                HostelAllocationCriteria criteria = hostelAllocationCriteriaLogic.GetModelBy(x => x.Id == hId);
                if (criteria != null)
                {
                    viewModel.HostelAllocationCriteria = criteria;
                    RetainDropDownList(viewModel);
                    //TempData["HostelAllocationViewModel"] = viewModel;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditHostelAllocationCriteria(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelAllocationCriteria != null)
                {
                    HostelAllocationCriteriaLogic hostelAllocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                    HostelAllocationCriteria existingCriteria = hostelAllocationCriteriaLogic.GetModelBy(hac => hac.Corner_Id == viewModel.HostelAllocationCriteria.Corner.Id && hac.Hostel_Id == viewModel.HostelAllocationCriteria.Hostel.Id && hac.Level_Id == viewModel.HostelAllocationCriteria.Level.Id && hac.Room_Id == viewModel.HostelAllocationCriteria.Room.Id && hac.Series_Id == viewModel.HostelAllocationCriteria.Series.Id);

                    if (viewModel.HostelAllocationCriteria.EditAll)
                    {
                        List<HostelAllocationCriteria> existingCriteriaList = hostelAllocationCriteriaLogic.GetModelsBy(h => h.Hostel_Id == viewModel.HostelAllocationCriteria.Hostel.Id && h.Series_Id == viewModel.HostelAllocationCriteria.Series.Id && h.Level_Id == viewModel.HostelAllocationCriteria.Level.Id);
                        for (int i = 0; i < existingCriteriaList.Count; i++)
                        {
                            HostelAllocationCriteria currentCriteria = existingCriteriaList[i];
                            currentCriteria.Level = viewModel.Level;

                            hostelAllocationCriteriaLogic.Modify(currentCriteria);

                        }

                        SetMessage("Operation Successful ", Message.Category.Information);
                        return RedirectToAction("ViewHostelAllocationCriteria");
                    }
                    else if (!viewModel.HostelAllocationCriteria.EditAll)
                    {
                        if (existingCriteria == null)
                        {
                            viewModel.HostelAllocationCriteria.Level = viewModel.Level;
                            hostelAllocationCriteriaLogic.Modify(viewModel.HostelAllocationCriteria);

                            SetMessage("Operation Successful ", Message.Category.Information);
                            return RedirectToAction("ViewHostelAllocationCriteria");
                        }
                        else
                        {
                            SetMessage("Error! Criteria exists/NO Changes Made.", Message.Category.Information);

                            RetainDropDownList(viewModel);
                            return View();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return RedirectToAction("ViewHostelAllocationCriteria");
        }

        public ActionResult ConfirmDeleteHostelAllocationCriteria(int hid)
        {
            try
            {
                viewModel = new HostelViewModel();
                if (hid > 0)
                {
                    HostelAllocationCriteriaLogic hostelAllocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                    viewModel.HostelAllocationCriteria = hostelAllocationCriteriaLogic.GetModelBy(x => x.Id == hid);

                    RetainDropDownList(viewModel);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteHostelAllocationCriteria(HostelViewModel viewModel)
        {
            try
            {
                HostelAllocationCriteriaLogic hostelAllocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                hostelAllocationCriteriaLogic.Delete(x => x.Id == viewModel.HostelAllocationCriteria.Id);

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewHostelAllocationCriteria");

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View("ConfirmDeleteHostelAllocationCriteria", viewModel);
        }
        public ActionResult EditStudentHostelAllocation()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Sessions = viewModel.SessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditStudentHostelAllocation(HostelViewModel viewModel)
        {
            try
            {
                Model.Model.Student student = new Model.Model.Student();
                StudentLevel studentLevel = new StudentLevel();
                HostelAllocation hostelAllocation = new HostelAllocation();

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                StudentLogic studentLogic = new StudentLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                List<StudentLevel> studentLevels = new List<StudentLevel>();

                List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                if (students.Count != 1)
                {
                    SetMessage("Student with this Matriculation Number does not exist Or Matric Number is Duplicate!", Message.Category.Error);
                    ViewBag.Sessions = viewModel.SessionSelectListItem;
                    return View(viewModel);
                }

                student = students.FirstOrDefault();
                studentLevels = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == student.Id);
                if (studentLevels.Count == 0)
                {
                    SetMessage("No StudentLevel Record!", Message.Category.Error);
                    ViewBag.Sessions = viewModel.SessionSelectListItem;
                    return View(viewModel);
                }

                PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.ONLINE_PAYMENT.PAYMENT.Session_Id == viewModel.Session.Id && p.ONLINE_PAYMENT.PAYMENT.Person_Id == student.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees).LastOrDefault();
                if (paymentEtranzact == null)
                {
                    SetMessage("Student has not paid fees for this session!", Message.Category.Error);
                    ViewBag.Sessions = viewModel.SessionSelectListItem;
                    return View(viewModel);
                }

                studentLevel = studentLevels.LastOrDefault();
                viewModel.StudentLevel = studentLevel;
                viewModel.PaymentEtranzact = paymentEtranzact;

                hostelAllocation = hostelAllocationLogic.GetModelBy(x => x.Student_Id == student.Id && x.Session_Id == viewModel.Session.Id);
                if (hostelAllocation != null)
                {
                    viewModel.HostelAllocation = hostelAllocation;
                    ViewBag.Sessions = viewModel.SessionSelectListItem;
                    RetainDropDownList(viewModel);
                    return View(viewModel);
                }
                else
                {
                    SetMessage("No Hostel Allocation for this Student in the selected session", Message.Category.Error);
                    ViewBag.Sessions = viewModel.SessionSelectListItem;
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Sessions = viewModel.SessionSelectListItem;
            return View(viewModel);
        }

        public ActionResult SaveEditedStudentHostelAllocation(HostelViewModel viewModel)
        {
            try
            {
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();
                HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();

                HostelAllocation hostelAllocation = new HostelAllocation();
                HostelRoomCorner corner = new HostelRoomCorner();
                HostelRoom hostelRoom = new HostelRoom();

                if (viewModel.HostelAllocation.Series != null && viewModel.HostelAllocation.Room != null && viewModel.HostelAllocation.Hostel != null && viewModel.HostelAllocation.Corner != null)
                {
                    HostelAllocation existingAllocation = hostelAllocationLogic.GetModelBy(
                                                            x =>
                                                            x.Hostel_Id == viewModel.HostelAllocation.Hostel.Id &&
                                                            x.Series_Id == viewModel.HostelAllocation.Series.Id
                                                            && x.Room_Id == viewModel.HostelAllocation.Room.Id &&
                                                            x.Corner_Id == viewModel.HostelAllocation.Corner.Id &&
                                                            x.Session_Id == viewModel.HostelAllocation.Session.Id);
                    if (existingAllocation != null)
                    {
                        SetMessage("The Room and corner you are trying to allocate is Occupied! ", Message.Category.Error);
                        
                        return RedirectToAction("EditStudentHostelAllocation");
                    }

                    hostelRoom = hostelRoomLogic.GetModelBy(h => h.Room_Id == viewModel.HostelAllocation.Room.Id);
                    corner = hostelRoomCornerLogic.GetModelBy(h => h.Corner_Id == viewModel.HostelAllocation.Corner.Id);
                    if (!hostelRoom.Activated || !corner.Activated)
                    {
                        SetMessage("The room/corner you are trying to allocate is not activated! ", Message.Category.Error);
                        
                        return RedirectToAction("EditStudentHostelAllocation");
                    }

                    hostelAllocation.Id = viewModel.HostelAllocation.Id;
                    hostelAllocation.Hostel = viewModel.HostelAllocation.Hostel;
                    hostelAllocation.Series = viewModel.HostelAllocation.Series;
                    hostelAllocation.Room = viewModel.HostelAllocation.Room;
                    hostelAllocation.Corner = viewModel.HostelAllocation.Corner;
                }

                bool IsModified = hostelAllocationLogic.Modify(hostelAllocation);
                if (IsModified)
                {
                    SetMessage("Operation Successful!", Message.Category.Information);
                }
                else
                {
                    SetMessage("NO Changes Made!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("EditStudentHostelAllocation");
        }

        public ActionResult ChangeRoomName()
        {
            try
            {
                viewModel = new HostelViewModel();
                PopulateDropDownList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ChangeRoomName(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    viewModel.HostelRoom = hostelRoomLogic.GetModelBy(h => h.Hostel_Id == viewModel.HostelAllocation.Hostel.Id && h.Series_Id == viewModel.HostelAllocation.Series.Id && h.Room_Id == viewModel.HostelAllocation.Room.Id);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }
        public ActionResult SaveChangedRoomName(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelRoom.Number != null)
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();

                    HostelRoom hostelRoom = hostelRoomLogic.GetModelBy(h => h.Room_Id == viewModel.HostelRoom.Id);
                    hostelRoom.Number = viewModel.HostelRoom.Number;
                    hostelRoomLogic.Modify(hostelRoom);

                    SetMessage("Operation! Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ChangeRoomName");
        }

        public ActionResult ViewUnoccupiedAllocations()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Session = viewModel.SessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewUnoccupiedAllocations(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.Session != null)
                {
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                    viewModel.HostelAllocations = hostelAllocationLogic.GetModelsBy(h => h.Session_Id == viewModel.Session.Id && h.Occupied == false);
                    StudentLogic studentLogic = new StudentLogic();
                    for (int i = 0; i < viewModel.HostelAllocations.Count; i++)
                    {
                        HostelAllocation currentAllocation = viewModel.HostelAllocations[i];
                        if (viewModel.HostelAllocations[i].Person != null)
                        {
                            viewModel.HostelAllocations[i].Student = studentLogic.GetModelsBy(s => s.Person_Id == currentAllocation.Person.Id).LastOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = viewModel.SessionSelectListItem;
            return View(viewModel);
        }
        public ActionResult ViewAllAllocations()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Session = viewModel.SessionSelectListItem;
                ViewBag.Hostel = viewModel.HostelSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewAllAllocations(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.Session != null && viewModel.Hostel != null)
                {
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                    viewModel.HostelAllocations = hostelAllocationLogic.GetModelsBy(h => h.Session_Id == viewModel.Session.Id && h.Hostel_Id == viewModel.Hostel.Id);
                    StudentLogic studentLogic = new StudentLogic();
                    if (viewModel.HostelAllocations != null)
                    {
                        for (int i = 0; i < viewModel.HostelAllocations.Count; i++)
                        {
                            HostelAllocation currentAllocation = viewModel.HostelAllocations[i];
                            if (viewModel.HostelAllocations[i].Person != null)
                            {
                                viewModel.HostelAllocations[i].Student = studentLogic.GetModelsBy(s => s.Person_Id == currentAllocation.Person.Id).LastOrDefault();
                            }
                        }
                    }
                    if (viewModel.HostelAllocations == null || viewModel.HostelAllocations.Count == 0)
                    {
                        SetMessage("No allocations yet.", Message.Category.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = viewModel.SessionSelectListItem;
            ViewBag.Hostel = viewModel.HostelSelectListItem;
            return View(viewModel);
        }
        public ActionResult DeleteUnoccupiedAllocations(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelAllocations.Count > 0)
                {
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                    HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();

                    StudentLevel studentLevel = new StudentLevel();

                    List<HostelAllocation> allocations = viewModel.HostelAllocations.Where(h => h.Occupied).ToList();

                    for (int i = 0; i < allocations.Count; i++)
                    {
                        HostelAllocation currentAllocation = allocations[i];
                        currentAllocation = hostelAllocationLogic.GetModelBy(h => h.Id == currentAllocation.Id);

                        PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == currentAllocation.Payment.Id);
                        if (paymentEtranzact != null)
                        {
                            currentAllocation.Occupied = true;
                            hostelAllocationLogic.Modify(currentAllocation);

                            continue;
                        }

                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            hostelAllocationLogic.Delete(h => h.Id == currentAllocation.Id);

                            onlinePaymentLogic.Delete(o => o.Payment_Id == currentAllocation.Payment.Id);

                            studentPaymentLogic.Delete(s => s.Payment_Id == currentAllocation.Payment.Id);

                            paymentLogic.Delete(p => p.Payment_Id == currentAllocation.Payment.Id);

                            HostelRoom hostelRoom = hostelRoomLogic.GetModelBy(h => h.Room_Id == currentAllocation.Room.Id);

                            if (hostelRoom != null && hostelRoom.Reserved)
                            {
                                studentLevel = studentLevelLogic.GetModelBy(s => s.Session_Id == currentAllocation.Session.Id && s.Person_Id == currentAllocation.Person.Id);
                                if (studentLevel != null)
                                {
                                    HostelAllocationCount hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Level_Id == studentLevel.Level.Id && h.Sex_Id == currentAllocation.Person.Sex.Id);
                                    if (hostelAllocationCount != null)
                                    {
                                        hostelAllocationCount.TotalCount += 1;
                                        hostelAllocationCount.Reserved += 1;

                                        hostelAllocationCountLogic.Modify(hostelAllocationCount);
                                    }
                                }
                                else
                                {
                                    HostelAllocationCount hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Level_Id == 1 && h.Sex_Id == currentAllocation.Person.Sex.Id);
                                    if (hostelAllocationCount != null)
                                    {
                                        hostelAllocationCount.TotalCount += 1;
                                        hostelAllocationCount.Reserved += 1;

                                        hostelAllocationCountLogic.Modify(hostelAllocationCount);
                                    }
                                }
                            }
                            else if (hostelRoom != null && !hostelRoom.Reserved)
                            {
                                studentLevel = studentLevelLogic.GetModelBy(s => s.Session_Id == currentAllocation.Session.Id && s.Person_Id == currentAllocation.Person.Id);
                                if (studentLevel != null)
                                {
                                    HostelAllocationCount hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Level_Id == studentLevel.Level.Id && h.Sex_Id == currentAllocation.Person.Sex.Id);
                                    if (hostelAllocationCount != null)
                                    {
                                        hostelAllocationCount.TotalCount += 1;
                                        hostelAllocationCount.Free += 1;

                                        hostelAllocationCountLogic.Modify(hostelAllocationCount);
                                    }
                                }
                                else
                                {
                                    HostelAllocationCount hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Level_Id == 1 && h.Sex_Id == currentAllocation.Person.Sex.Id);
                                    if (hostelAllocationCount != null)
                                    {
                                        hostelAllocationCount.TotalCount += 1;
                                        hostelAllocationCount.Free += 1;

                                        hostelAllocationCountLogic.Modify(hostelAllocationCount);
                                    }
                                }
                            }

                            transactionScope.Complete();
                        }
                    }

                    SetMessage("Operation! Successful! ", Message.Category.Information);
                    return RedirectToAction("ViewUnoccupiedAllocations");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = viewModel.SessionSelectListItem;
            return View("ViewUnoccupiedAllocations");
        }

        public ActionResult ViewReservedRooms()
        {
            try
            {
                viewModel = new HostelViewModel();
                PopulateDropDownList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);

        }
        [HttpPost]
        public ActionResult ViewReservedRooms(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    viewModel.HostelRoomList = hostelRoomLogic.GetModelsBy(h => h.Hostel_Id == viewModel.HostelAllocation.Hostel.Id && h.Series_Id == viewModel.HostelAllocation.Series.Id && h.Reserved);
                    if (viewModel.HostelRoomList == null || viewModel.HostelRoomList.Count <= 0)
                    {
                        SetMessage("No reserved room! ", Message.Category.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);

        }
        public ActionResult ReleaseReservedRooms(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelRoomList.Count > 0)
                {
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    List<HostelRoom> roomsToRelease = viewModel.HostelRoomList.Where(r => r.Reserved == false).ToList();

                    for (int i = 0; i < roomsToRelease.Count; i++)
                    {
                        HostelRoom currentHostelRoom = roomsToRelease[i];
                        HostelRoom hostelRoom = hostelRoomLogic.GetModelBy(h => h.Room_Id == currentHostelRoom.Id);
                        hostelRoom.Reserved = currentHostelRoom.Reserved;
                        hostelRoomLogic.Modify(hostelRoom);

                        SetMessage("Operation! Successful! ", Message.Category.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewReservedRooms");
        }
        public ActionResult AllocateReservedRoom(int rid)
        {
            try
            {
                if (rid > 0)
                {
                    HostelViewModel viewModel = new HostelViewModel();
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();

                    viewModel.HostelRoom = hostelRoomLogic.GetModelBy(h => h.Room_Id == rid);
                    viewModel.HostelRoomCorners = hostelRoomCornerLogic.GetModelsBy(c => c.Room_Id == rid);

                    ViewBag.CornerId = new SelectList(Utility.PopulateHostelRoomCorners(viewModel.HostelRoom), "Value", "Text");
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }


            return RedirectToAction("ViewReservedRooms");
        }
        [HttpPost]
        public ActionResult AllocateReservedRoom(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    SessionLogic sessionLogic = new SessionLogic();
                    Session session=sessionLogic.GetModelsBy(f => f.Active_Hostel_Application).LastOrDefault();
                    HostelAllocation hostelAllocation = new HostelAllocation();
                    Payment craetedPayment = new Payment();

                    Person person = new Person();
                    Department department = new Department();
                    Programme programme = new Programme();
                    Level level = new Level();
                    StudentLevel studentLevel = new StudentLevel();

                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.Confirmation_No == viewModel.Student.MatricNumber && 
                                                        (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees) && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id).LastOrDefault();
                    if (paymentEtranzact == null)
                    {
                        SetMessage("Confirmation order number is not for current session's school fee!", Message.Category.Error);
                        ViewBag.CornerId = new SelectList(Utility.PopulateHostelRoomCorners(viewModel.HostelRoom), "Value", "Text");
                        return View(viewModel);
                    }

                    person = paymentEtranzact.Payment.Payment.Person;

                    StudentPayment studentPayment = studentPaymentLogic.GetModelBy(s => s.Person_Id == person.Id && s.Session_Id == session.Id && s.Payment_Id == paymentEtranzact.Payment.Payment.Id);

                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);
                    if (appliedCourse != null)
                    {
                        AdmissionList admissionList = admissionListLogic.GetModelBy(s => s.Application_Form_Id == appliedCourse.ApplicationForm.Id);
                        if (studentPayment == null && appliedCourse.ApplicationForm.Setting.Session.Id == session.Id)
                        {
                            level = new Level() { Id = 1 };
                            programme = admissionList.Programme;
                            department = admissionList.Deprtment;
                        }
                        else if (studentPayment != null)
                        {
                            level = studentPayment.Level;
                        }
                        else
                        {
                            SetMessage("Student Level not set, contact school ICS!", Message.Category.Error);
                            return View(viewModel);
                        }

                    }

                    Model.Model.Student student = studentLogic.GetModelBy(s => s.Person_Id == person.Id);
                    if (student != null)
                    {
                        studentLevel = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == student.Id).LastOrDefault();
                        if (studentPayment != null && studentLevel != null)
                        {
                            programme = studentLevel.Programme;
                            department = studentLevel.Department;
                            level = studentPayment.Level;
                        }
                    }

                    viewModel.Person = person;
                    viewModel.StudentLevel = new StudentLevel();
                    viewModel.StudentLevel.Programme = programme;
                    viewModel.StudentLevel.Department = department;
                    viewModel.StudentLevel.Level = level;
                    viewModel.Session = session;

                    Campus studentCampus = GetStudentCampus(viewModel.StudentLevel) ?? new Campus { Id = (int)Campuses.Uturu };

                    HostelAllocation existingHostelAllocation = hostelAllocationLogic.GetModelBy(ha => ha.Session_Id == session.Id && ha.Student_Id == student.Id);
                    if (existingHostelAllocation != null)
                    {
                        if (existingHostelAllocation.Occupied)
                        {
                            return RedirectToAction("HostelReceipt", "Hostel", new { Area = "Student", spmid = existingHostelAllocation.Payment.Id });
                        }
                        else
                        {
                            if (level.Id != 1)
                            {
                                return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(existingHostelAllocation.Payment.Id.ToString()), });
                            }
                            else
                            {
                                return RedirectToAction("HostelReceipt", "Hostel", new { Area ="Student" , spmid = existingHostelAllocation.Payment.Id });
                            }
                        }
                    }

                    if (student.Sex == null)
                    {
                        SetMessage("Error! Ensure that your student profile(Sex) is completely filled", Message.Category.Error);
                        ViewBag.CornerId = new SelectList(Utility.PopulateHostelRoomCorners(viewModel.HostelRoom), "Value", "Text");
                        return View(viewModel);
                    }

                    HostelAllocationCount hostelAllocationCount = new HostelAllocationCount();
                    if (level.Id == 1)
                    {
                        hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Sex_Id == person.Sex.Id && h.Level_Id == level.Id && h.Campus_Id == studentCampus.Id);
                    }
                    else
                    {
                        hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Sex_Id == person.Sex.Id && h.Level_Id != 1 && h.Campus_Id == studentCampus.Id);
                    }
                    if (hostelAllocationCount != null && hostelAllocationCount.Reserved == 0)
                    {
                        SetMessage("Error! The Set Number for reserved Bed Spaces has been exausted!", Message.Category.Error);
                        ViewBag.CornerId = new SelectList(Utility.PopulateHostelRoomCorners(viewModel.HostelRoom), "Value", "Text");
                        return View(viewModel);
                    }

                    HostelAllocation allocationCheck = hostelAllocationLogic.GetModelBy(h => h.Corner_Id == viewModel.HostelRoomCorner.Id && h.Hostel_Id == viewModel.HostelRoom.Hostel.Id && 
                                                        h.Room_Id == viewModel.HostelRoom.Id && h.Series_Id == viewModel.HostelRoom.Series.Id && h.Session_Id == session.Id);
                    if (allocationCheck != null)
                    {
                        SetMessage("Error! Bed Space has already been allocated!", Message.Category.Error);
                        ViewBag.CornerId = new SelectList(Utility.PopulateHostelRoomCorners(viewModel.HostelRoom), "Value", "Text");
                        return View(viewModel);
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        hostelAllocation.Corner = viewModel.HostelRoomCorner;
                        hostelAllocation.Hostel = viewModel.HostelRoom.Hostel;
                        if (level.Id == 1)
                        {
                            hostelAllocation.Occupied = true;
                        }
                        else
                        {
                            hostelAllocation.Occupied = false;
                        }

                        hostelAllocation.Room = viewModel.HostelRoom;
                        hostelAllocation.Series = viewModel.HostelRoom.Series;
                        hostelAllocation.Session = session;
                        hostelAllocation.Student = student;
                        hostelAllocation.Person = person;

                        //Person person = personLogic.GetModelBy(p => p.Person_Id == currentStudent.Id);
                        viewModel.Person = person;

                        craetedPayment = CreatePayment(viewModel);

                        StudentPayment currentStudentPayment = new StudentPayment();
                        currentStudentPayment.Level = level;
                        currentStudentPayment.Amount = craetedPayment.FeeDetails.Sum(f => f.Fee.Amount);
                        if (level.Id == 1)
                        {
                            currentStudentPayment.Status = true;
                        }
                        else
                        {
                            currentStudentPayment.Status = false;
                        }

                        currentStudentPayment.Student = student;
                        currentStudentPayment.Person = person;
                        currentStudentPayment.Session = viewModel.Session;
                        currentStudentPayment.Id = craetedPayment.Id;

                        StudentPayment existingStudentPayment = studentPaymentLogic.GetModelBy(p => p.Payment_Id == craetedPayment.Id);

                        if (existingStudentPayment == null)
                        {
                            studentPaymentLogic.Create(currentStudentPayment);
                        }

                        hostelAllocation.Payment = craetedPayment;

                        HostelAllocation newHostelAllocation = hostelAllocationLogic.Create(hostelAllocation);

                        hostelAllocationCount.Reserved -= 1;
                        hostelAllocationCount.TotalCount -= 1;
                        hostelAllocationCount.LastModified = DateTime.Now;
                        hostelAllocationCountLogic.Modify(hostelAllocationCount);

                        scope.Complete();
                    }

                    if (level.Id == 1)
                    {
                        return RedirectToAction("HostelReceipt", "Hostel", new { Area = "Student", spmid = craetedPayment.Id });
                    }

                    viewModel.Student = student;
                    viewModel.StudentLevel = studentLevel;

                    return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(craetedPayment.Id.ToString()), });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewReservedRooms");
        }
        private Campus GetStudentCampus(StudentLevel studentLevel)
        {
            Campus campus = null;
            try
            {
                if (studentLevel != null && studentLevel.Department.Id == 50 && studentLevel.Level.Id >= 4)
                {
                    campus = new Campus { Id = (int)Campuses.Aba };
                }
                else if (studentLevel != null && (studentLevel.Department.Id == 42 || (studentLevel.Department.Faculty.Id == 10 && studentLevel.Level.Id >= 2)))
                {
                    campus = new Campus { Id = (int)Campuses.Umuahia };
                }
                else
                {
                    campus = new Campus { Id = (int)Campuses.Uturu };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return campus;
        }
        public ActionResult ViewAllocationRequest()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Level = viewModel.LevelSelectListItem;
                ViewBag.Sessions = viewModel.SessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewAllocationRequest(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.Level != null)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    viewModel.HostelRequests = hostelRequestLogic.GetModelsBy(h => h.Level_Id == viewModel.Level.Id && h.Session_Id == viewModel.Session.Id && !h.Approved);
                    StudentLogic studentLogic = new StudentLogic();
                    for (int i = 0; i < viewModel.HostelRequests.Count; i++)
                    {
                        HostelRequest currentRequest = viewModel.HostelRequests[i];
                        if (viewModel.HostelRequests[i].Person != null)
                        {
                            viewModel.HostelRequests[i].Student = studentLogic.GetModelsBy(s => s.Person_Id == currentRequest.Person.Id).LastOrDefault();
                        }
                    }

                    if (viewModel.HostelRequests.Count == 0)
                    {
                        SetMessage("No requests! ", Message.Category.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Level = viewModel.LevelSelectListItem;
            ViewBag.Sessions = viewModel.SessionSelectListItem;

            TempData["viewModel"] = viewModel;

            return View(viewModel);
        }
        public ActionResult ApproveHostelRequest(HostelViewModel viewModel)
        {
            try
            {
                int male = 1;
                int female = 2;
                HostelViewModel existingViewModel = (HostelViewModel)TempData["viewModel"];
                if (viewModel.HostelRequests.Count > 0)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    HostelRequestCountLogic requestCountLogic = new HostelRequestCountLogic();

                    HostelRequestCount maleRequestCount = new HostelRequestCount();
                    HostelRequestCount femaleRequestCount = new HostelRequestCount();

                    if (existingViewModel.Level.Id == 1)
                    {
                        maleRequestCount = requestCountLogic.GetModelBy(a => a.Level_Id == existingViewModel.Level.Id && a.Sex_Id == male && a.Approved);
                        femaleRequestCount = requestCountLogic.GetModelBy(a => a.Level_Id == existingViewModel.Level.Id && a.Sex_Id == female && a.Approved);
                    }
                    else
                    {
                        maleRequestCount = requestCountLogic.GetModelBy(a => a.Level_Id != 1 && a.Sex_Id == male && a.Approved);
                        femaleRequestCount = requestCountLogic.GetModelBy(a => a.Level_Id != 1 && a.Sex_Id == female && a.Approved);
                    }

                    int approvalCountMale = 0;
                    int approvalCountFemale = 0;

                    bool maleReachedCount = false;
                    bool femaleReachedCount = false;

                    List<HostelRequest> requestsToApprove = viewModel.HostelRequests.Where(r => r.Approved).ToList();

                    for (int i = 0; i < requestsToApprove.Count; i++)
                    {
                        HostelRequest currentHostelRequest = requestsToApprove[i];
                        HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Hostel_Request_Id == currentHostelRequest.Id);

                        //List<HostelRequest> approvedHostelRequests = hostelRequestLogic.GetModelsBy(h => h.Level_Id == existingViewModel.Level.Id && h.Session_Id == existingViewModel.Session.Id && h.PERSON.Sex_Id == hostelRequest.Person.Sex.Id);

                        if (hostelRequest.Person.Sex == null)
                        {
                            continue;
                        }

                        if (hostelRequest.Person.Sex.Id == male)
                        {
                            if (maleRequestCount.TotalCount > 0)
                            {
                                hostelRequest.Approved = currentHostelRequest.Approved;
                                hostelRequestLogic.Modify(hostelRequest);

                                maleRequestCount.TotalCount -= 1;
                                requestCountLogic.Modify(maleRequestCount);

                                if (existingViewModel.Level.Id == 1)
                                {
                                    SendSms(hostelRequest.Person.FirstName, hostelRequest.Person.MobilePhone, 1);
                                }
                                else
                                {
                                    SendSms(hostelRequest.Person.FirstName, hostelRequest.Person.MobilePhone, 2);
                                }

                                approvalCountMale += 1;
                            }
                            else
                            {
                                maleReachedCount = true;
                            }
                        }
                        else if (hostelRequest.Person.Sex.Id == female)
                        {
                            if (femaleRequestCount.TotalCount > 0)
                            {
                                hostelRequest.Approved = currentHostelRequest.Approved;
                                hostelRequestLogic.Modify(hostelRequest);

                                femaleRequestCount.TotalCount -= 1;
                                requestCountLogic.Modify(femaleRequestCount);

                                if (existingViewModel.Level.Id == 1)
                                {
                                    SendSms(hostelRequest.Person.FirstName, hostelRequest.Person.MobilePhone, 1);
                                }
                                else
                                {
                                    SendSms(hostelRequest.Person.FirstName, hostelRequest.Person.MobilePhone, 2);
                                }

                                approvalCountFemale += 1;
                            }
                            else
                            {
                                femaleReachedCount = true;
                            }
                        }
                    }

                    string message = "";

                    if (femaleReachedCount)
                    {
                        message += approvalCountFemale + " female hostel requests were approved, other female hostel requests were not approved because the set allocation count has been reached.";
                    }
                    else
                    {
                        message += approvalCountFemale + " female hostel requests were approved.";
                    }

                    if (maleReachedCount)
                    {
                        message += approvalCountMale + " male hostel requests were approved, other male hostel requests were not approved because the set allocation count has been reached.";
                    }
                    else
                    {
                        message += approvalCountMale + " male hostel requests were approved";
                    }

                    SetMessage(message, Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewAllocationRequest");
        }

        private void SendSms(string name, string mobilePhone, int type)
        {
            try
            {
                string message = "";
                string phone = mobilePhone.Substring(1);
                string number = "234" + phone;
                if (type == 1)
                {
                    message = "Hello " + name + ", your hostel request has been approved, proceed to print your hostel slip by following the 'Hostel Allocation Status' link on the school portal.";
                }
                else
                {
                    message = "Hello " + name + ", your hostel request has been approved, proceed to generate invoice by following the 'Hostel Allocation Status' link on the school portal.";
                }

                var smsClient = new InfoBipSMS();
                var smsMessage = new InfoSMS();
                smsMessage.from = "ABSU";
                smsMessage.to = number;
                smsMessage.text = message;
                smsClient.SendSMS(smsMessage);
            }
            catch (Exception)
            {
                //do nothing
            }
        }
        public ActionResult SetAllocationCount()
        {
            viewModel = new HostelViewModel();
            try
            {
                HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();
                viewModel.HostelAllocationCounts = hostelAllocationCountLogic.GetAll();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Campus = viewModel.CampusSelectList;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult SetAllocationCount(HostelViewModel viewModel)
        {
            HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();
            try
            {
                if (viewModel.HostelAllocationCounts != null)
                {
                    for (int i = 0; i < viewModel.HostelAllocationCounts.Count; i++)
                    {
                        HostelAllocationCount currentHostelAllocationCount = viewModel.HostelAllocationCounts[i];
                        if (Convert.ToInt32(currentHostelAllocationCount.Free) + Convert.ToInt32(currentHostelAllocationCount.Reserved) != Convert.ToInt32(currentHostelAllocationCount.TotalCount))
                        {
                            continue;
                        }
                        HostelAllocationCount hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Hostel_Allocation_Count_Id == currentHostelAllocationCount.Id);

                        hostelAllocationCount.Free = currentHostelAllocationCount.Free;
                        hostelAllocationCount.LastModified = DateTime.Now;
                        hostelAllocationCount.Reserved = currentHostelAllocationCount.Reserved;
                        hostelAllocationCount.TotalCount = currentHostelAllocationCount.TotalCount;

                        hostelAllocationCountLogic.Modify(hostelAllocationCount);


                    }
                    SetMessage("Operation! Successful! ", Message.Category.Information);
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            viewModel = new HostelViewModel();
            viewModel.HostelAllocationCounts = hostelAllocationCountLogic.GetAll();
            return View(viewModel);
        }
        private Payment CreatePayment(HostelViewModel viewModel)
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();

                Payment newPayment = new Payment();

                PaymentMode paymentMode = new PaymentMode() { Id = 1 };
                PaymentType paymentType = new PaymentType() { Id = 2 };
                PersonType personType = viewModel.Person.Type;
                FeeType feeType = new FeeType() { Id = (int)FeeTypes.HostelFee };

                Payment payment = new Payment();
                payment.PaymentMode = paymentMode;
                payment.PaymentType = paymentType;
                payment.PersonType = personType;
                payment.FeeType = feeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                Payment checkPayment = paymentLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id && p.Fee_Type_Id == feeType.Id && p.Session_Id == viewModel.Session.Id).LastOrDefault();
                if (checkPayment != null)
                {
                    newPayment = checkPayment;
                }
                else
                {
                    newPayment = paymentLogic.Create(payment);
                }

                newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id, paymentMode.Id, viewModel.StudentLevel.Department.Id,
                    viewModel.Session.Id);

                OnlinePayment newOnlinePayment = null;

                if (newPayment != null)
                {
                    OnlinePayment onlinePaymentCheck = onlinePaymentLogic.GetModelsBy(op => op.Payment_Id == newPayment.Id).LastOrDefault();
                    if (onlinePaymentCheck == null)
                    {
                        PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                        OnlinePayment onlinePayment = new OnlinePayment();
                        onlinePayment.Channel = channel;
                        onlinePayment.Payment = newPayment;
                        newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                    }

                }

                //HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                //HostelFee hostelFee = new HostelFee();

                //hostelFee.Hostel = hostel;
                //hostelFee.Payment = newPayment;
                //hostelFee.Amount = GetHostelFee(hostel);

                //hostelFeeLogic.Create(hostelFee);

                //newPayment.Amount = GetHostelFee(hostel).ToString();

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private double GetHostelFee(Hostel hostel)
        {
            double amount = 0;
            try
            {
                string[] firstHostelGroup = { "KINGS PALACE", "KINGS ANNEX(A)", "KINGS ANNEX(B)", "ALUTA BASE", "ALUTA BASE(ANNEX)", "QUEENS PALACE(ANNEX)" };
                string[] secondHostelGroup = { "QUEENS PALACE I", "QUEENS PALACE II", "QUEENS PALACE III" };

                if (firstHostelGroup.Contains(hostel.Name))
                {
                    amount = 13000;
                }
                if (secondHostelGroup.Contains(hostel.Name))
                {
                    amount = 11500;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return amount;
        }
        private string GetCornerName(int j)
        {
            try
            {
                string strAlpha = "";

                if (j == 0)
                {
                    j = 65;
                }
                else
                {
                    j += 65;
                }

                for (int i = j; i <= j; i++)
                {

                    strAlpha += ((char)i).ToString() + " ";
                    return strAlpha;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return "";
        }
        public ActionResult ViewAllAllocationRequest()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Level = viewModel.LevelSelectListItem;
                ViewBag.Sessions = viewModel.SessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewAllAllocationRequest(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.Level != null)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    viewModel.HostelRequests = hostelRequestLogic.GetModelsBy(h => h.Level_Id == viewModel.Level.Id && h.Session_Id == viewModel.Session.Id);
                    StudentLogic studentLogic = new StudentLogic();
                    for (int i = 0; i < viewModel.HostelRequests.Count; i++)
                    {
                        HostelRequest currentRequest = viewModel.HostelRequests[i];
                        if (viewModel.HostelRequests[i].Person != null)
                        {
                            viewModel.HostelRequests[i].Student = studentLogic.GetModelsBy(s => s.Person_Id == currentRequest.Person.Id).LastOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Level = viewModel.LevelSelectListItem;
            ViewBag.Sessions = viewModel.SessionSelectListItem;
            return View(viewModel);
        }

        public ActionResult EditAllocationRequest(int rid)
        {
            viewModel = new HostelViewModel();
            try
            {
                if (rid >= 0)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Hostel_Request_Id == rid);

                    viewModel.HostelRequest = hostelRequest;
                    ViewBag.Level = new SelectList(viewModel.LevelSelectListItem, "Value", "Text", hostelRequest.Level.Id);
                    ViewBag.Session = new SelectList(viewModel.SessionSelectListItem, "Value", "Text", hostelRequest.Session.Id);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditAllocationRequest(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelRequest != null)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Hostel_Request_Id == viewModel.HostelRequest.Id);
                    hostelRequest.Approved = viewModel.HostelRequest.Approved;
                    hostelRequest.Session = viewModel.HostelRequest.Session;
                    hostelRequest.Level = viewModel.HostelRequest.Level;

                    hostelRequestLogic.Modify(hostelRequest);

                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewAllAllocationRequest");
        }
        public ActionResult RemoveHostelRequest(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.HostelRequests.Count > 0)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    List<HostelRequest> requestsToRemove = viewModel.HostelRequests.Where(r => r.Remove).ToList();

                    for (int i = 0; i < requestsToRemove.Count; i++)
                    {
                        HostelRequest currentHostelRequest = requestsToRemove[i];
                        HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Hostel_Request_Id == currentHostelRequest.Id);

                        hostelRequestLogic.Delete(h => h.Hostel_Request_Id == hostelRequest.Id);

                        SetMessage("Operation! Successful! ", Message.Category.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewAllAllocationRequest");
        }
        public ActionResult CompareAllocationCritereiaCount()
        {
            viewModel = new HostelViewModel();
            try
            {
                HostelAllocationCriteriaLogic allocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                LevelLogic levelLogic = new LevelLogic();
                HostelAllocationCountLogic allocationCountLogic = new HostelAllocationCountLogic();
                HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                HostelLogic hostelLogic = new HostelLogic();
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();

                List<HostelType> hostelTypes = hostelTypeLogic.GetAll();
                List<Level> levels = levelLogic.GetAll();

                List<DistinctAllocation> distinctAllocationList = new List<DistinctAllocation>();

                for (int i = 0; i < levels.Count; i++)
                {
                    Level currentLevel = levels[i];
                    List<HostelAllocationCriteria> allocationCriteriaList = new List<HostelAllocationCriteria>();
                    List<HostelAllocationCriteria> allocationCriteriaList1 = new List<HostelAllocationCriteria>();
                    allocationCriteriaList1 = allocationCriteriaLogic.GetModelsBy(a => a.Level_Id == currentLevel.Id);
                    List<int> distinctHostels = allocationCriteriaList1.Select(a => a.Hostel.Id).Distinct().ToList();
                    //allocationCriteriaList = allocationCriteriaList1.GroupBy(a => a.Hostel.Id).Last().Distinct().ToList();

                    for (int j = 0; j < distinctHostels.Count; j++)
                    {
                        int currentHostelId = distinctHostels[j];
                        Hostel hostel = hostelLogic.GetModelBy(h => h.Hostel_Id == currentHostelId);
                        List<HostelAllocationCriteria> allocationCriteriaList2 = new List<HostelAllocationCriteria>();

                        HostelType currentHostelType = hostel.HostelType;

                        List<HostelSeries> hostelSeries = hostelSeriesLogic.GetModelsBy(h => h.Hostel_Id == hostel.Id);
                        for (int m = 0; m < hostelSeries.Count; m++)
                        {
                            string RoomCorners = "";
                            string corners = "";
                            string series = "";
                            int usedCriteriaCount = 0;
                            int unusedCriteriaCount = 0;

                            HostelSeries currentSeries = hostelSeries[m];
                            allocationCriteriaList2 = allocationCriteriaLogic.GetModelsBy(a => a.Level_Id == currentLevel.Id && a.Hostel_Id == hostel.Id && a.HOSTEL.Hostel_Type_Id == currentHostelType.Hostel_Type_Id && a.Series_Id == currentSeries.Id);

                            for (int l = 0; l < allocationCriteriaList2.Count; l++)
                            {
                                HostelAllocationCriteria thisCriteria = allocationCriteriaList2[l];
                                HostelAllocation hostelAllocation = hostelAllocationLogic.GetModelsBy(h => h.Corner_Id == thisCriteria.Corner.Id && h.Room_Id == thisCriteria.Room.Id && h.Hostel_Id == hostel.Id && h.Series_Id == currentSeries.Id && h.Session_Id == 7).LastOrDefault();
                                if (hostelAllocation != null)
                                {
                                    usedCriteriaCount += 1;
                                }
                                else
                                {
                                    unusedCriteriaCount += 1;
                                }

                                RoomCorners += " |" + allocationCriteriaList2[l].Room.Number + ": BedSpace " + allocationCriteriaList2[l].Corner.Name;
                            }

                            DistinctAllocation distinctAllocation = new DistinctAllocation();
                            HostelAllocationCount allocationCount = allocationCountLogic.GetModelsBy(a => a.Level_Id == currentLevel.Id && a.Sex_Id == currentHostelType.Hostel_Type_Id).LastOrDefault();
                            distinctAllocation.FreeAllocationCount = allocationCount.Free;
                            distinctAllocation.ReservedAllocationAccount = allocationCount.Reserved;
                            distinctAllocation.Level = currentLevel.Name;
                            distinctAllocation.Hostel = hostel.Name;
                            distinctAllocation.HostelType = currentHostelType.Hostel_Type_Name;
                            distinctAllocation.RoomCorner = RoomCorners;
                            distinctAllocation.Series = currentSeries.Name;
                            distinctAllocation.UnusedCriteriaCount = unusedCriteriaCount;
                            distinctAllocation.UsedCriteriaCount = usedCriteriaCount;
                            distinctAllocation.CriteriaCount = allocationCriteriaLogic.GetModelsBy(a => a.Level_Id == currentLevel.Id && a.Hostel_Id == hostel.Id && a.HOSTEL.Hostel_Type_Id == currentHostelType.Hostel_Type_Id && a.Series_Id == currentSeries.Id).Count;

                            distinctAllocationList.Add(distinctAllocation);
                        }
                    }
                }

                viewModel.DistinctAllocation = distinctAllocationList.OrderBy(a => a.Level).ThenBy(a => a.HostelType).ThenBy(a => a.Hostel).ThenBy(a => a.Series).ToList();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult UpdateHostelRequest()
        {
            try
            {
                HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                List<HostelRequest> hostelRequests = hostelRequestLogic.GetAll();
                for (int i = 0; i < hostelRequests.Count; i++)
                {
                    if (hostelRequests[i].Student.MatricNumber.Contains("/12/") || hostelRequests[i].Student.MatricNumber.Contains("/13/") || hostelRequests[i].Student.MatricNumber.Contains("/14/") || hostelRequests[i].Student.MatricNumber.Contains("/15/"))
                    {
                        if (hostelRequests[i].Level.Id == 1)
                        {
                            hostelRequests[i].Level = new Level() { Id = 2 };
                        }
                        if (hostelRequests[i].Level.Id == 3)
                        {
                            hostelRequests[i].Level = new Level() { Id = 4 };
                        }
                    }
                    hostelRequestLogic.Modify(hostelRequests[i]);
                }
                SetMessage("Operation! Successful! ", Message.Category.Information);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewAllAllocationRequest");
        }
        public JsonResult GetHostelSeries(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                int hostelId = Convert.ToInt32(id);
                HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                List<HostelSeries> hostelSeries = hostelSeriesLogic.GetModelsBy(hs => hs.Hostel_Id == hostelId);

                return Json(new SelectList(hostelSeries, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetCorners(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                int seriesId = Convert.ToInt32(id);
                List<HostelRoomCorner> hostelRoomCorners = new List<HostelRoomCorner>();
                HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();

                List<string> corners = hostelRoomCornerLogic.GetEntitiesBy(c => c.HOSTEL_ROOM.Series_Id == seriesId).Select(c => c.Corner_Name).Distinct().ToList();
                foreach (var item in corners)
                {
                    hostelRoomCorners.Add(hostelRoomCornerLogic.GetModelsBy(hrc => hrc.Corner_Name == item).FirstOrDefault());
                }

                return Json(new MultiSelectList(hostelRoomCorners, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetCornersByRoom(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                int roomId = Convert.ToInt32(id);
                List<HostelRoomCorner> hostelRoomCorners = new List<HostelRoomCorner>();
                HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();

                hostelRoomCorners = hostelRoomCornerLogic.GetModelsBy(c => c.Room_Id == roomId).ToList();

                return Json(new SelectList(hostelRoomCorners, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetRooms(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                int seriesId = Convert.ToInt32(id);
                List<HostelRoom> hostelRooms = new List<HostelRoom>();
                HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();

                hostelRooms = hostelRoomLogic.GetModelsBy(c => c.Series_Id == seriesId).ToList();

                return Json(new SelectList(hostelRooms, "Id", "Number"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetDepartments(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };

                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, "Id", "Name"), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult HostelRequestCount()
        {
            try
            {
                viewModel = new HostelViewModel();
                HostelRequestCountLogic hostelRequestCountLogic = new HostelRequestCountLogic();
                viewModel.HostelRequestCounts = hostelRequestCountLogic.GetAll();
            }
            catch (Exception ex)
            {

                SetMessage("Error Occured" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult HostelRequestCount(HostelViewModel viewModel)
        {
            HostelRequestCountLogic hostelRequestCountLogic = new HostelRequestCountLogic();
            try
            {

                if (viewModel.HostelRequestCounts != null)
                {
                    for (int i = 0; i < viewModel.HostelRequestCounts.Count; i++)
                    {
                        HostelRequestCount currentHostelRequestCount = viewModel.HostelRequestCounts[i];
                        //if (Convert.ToInt32(currentHostelAllocationCount.Free) + Convert.ToInt32(currentHostelAllocationCount.Reserved) != Convert.ToInt32(currentHostelAllocationCount.TotalCount))
                        //{
                        //    continue;
                        //}
                        HostelRequestCount hostelAllocationCount = hostelRequestCountLogic.GetModelBy(h => h.Hostel_Request_Count_Id == currentHostelRequestCount.Id);


                        hostelAllocationCount.LastModified = DateTime.Now;
                        hostelAllocationCount.TotalCount = currentHostelRequestCount.TotalCount;

                        hostelRequestCountLogic.Modify(hostelAllocationCount);


                    }
                    SetMessage("Operation! Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {

                SetMessage("Error Occured" + ex.Message, Message.Category.Error);
            }

            viewModel = new HostelViewModel();
            viewModel.HostelRequestCounts = hostelRequestCountLogic.GetAll();
            return View(viewModel);
        }
        public ActionResult ViewVacantBedSpaces()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Hostel = viewModel.HostelSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewVacantBedSpaces(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.Hostel != null)
                {
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                    viewModel.HostelAllocations = hostelAllocationLogic.GetVacantBedSpaces(viewModel.Hostel);
                    
                    if (viewModel.HostelAllocations == null || viewModel.HostelAllocations.Count <= 0)
                    {
                        SetMessage("No vacant bedspace.", Message.Category.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Hostel = viewModel.HostelSelectListItem;
            return View(viewModel);
        }

        public ActionResult UploadHostelAllocation()
        {
            try
            {
                viewModel = new HostelViewModel();
                ViewBag.Session = viewModel.SessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult UploadHostelAllocation(HostelViewModel viewModel)
        {
            try
            {
                viewModel.HostelAllocations = new List<HostelAllocation>();

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    DataSet allocationSet = ReadExcel(savedFileName);

                    if (allocationSet != null && allocationSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < allocationSet.Tables[0].Rows.Count; i++)
                        {
                            var allocation = new HostelAllocation();
                            allocation.HostelName = allocationSet.Tables[0].Rows[i][0].ToString().Trim();
                            allocation.SeriesName = allocationSet.Tables[0].Rows[i][1].ToString().Trim();
                            allocation.RoomName = allocationSet.Tables[0].Rows[i][2].ToString().Trim();
                            allocation.Student = new Model.Model.Student();
                            allocation.FullName= allocationSet.Tables[0].Rows[i][4].ToString().Trim();
                            allocation.Student.MatricNumber = allocationSet.Tables[0].Rows[i][5].ToString().Trim();

                            if (!string.IsNullOrEmpty(allocation.Student.MatricNumber))
                            {
                                viewModel.HostelAllocations.Add(allocation);
                            }
                        }

                        List<HostelAllocation> hostelAllocations = viewModel.HostelAllocations.OrderBy(p => p.HostelName).ThenBy(p => p.SeriesName).ThenBy(p => p.RoomName).ThenBy(p => p.Student.MatricNumber).ToList();
                        viewModel.HostelAllocations = hostelAllocations;

                        TempData["HostelAllocations"] = viewModel.HostelAllocations;
                        TempData["Session"] = viewModel.Session;
                        TempData["ViewModel"] = viewModel;
                    }
                }
                
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = viewModel.SessionSelectListItem;
            return View(viewModel);
        }
        public ActionResult SaveHostelAllocation()
        {
            try
            {
                List<HostelAllocation> allocations = (List<HostelAllocation>) TempData["HostelAllocations"];
                Session session = (Session) TempData["Session"];
                viewModel = (HostelViewModel) TempData["ViewModel"];

                if (allocations != null && allocations.Count > 0 && session != null && session.Id > 0)
                {
                    List<HostelAllocation> successfulAllocations = new List<HostelAllocation>();
                    List<HostelAllocation> failedAllocations = new List<HostelAllocation>();

                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    HostelLogic hostelLogic = new HostelLogic();
                    HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
                    HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
                    HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();

                    for (int i = 0; i < allocations.Count; i++)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            HostelAllocation allocation = allocations[i];

                            Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number.Trim() == allocation.Student.MatricNumber).LastOrDefault();

                            if (student == null)
                            {
                                allocation.Reason = "Wrong Matric Number/Student does not exist.";
                                failedAllocations.Add(allocation);
                                continue;
                            }

                            allocation.Student.Id = student.Id;

                            CheckAndCreateHostelRequest(allocation, session);
                            Payment payment = CheckAndCreateHostelPayment(allocation, session);

                            if (payment == null)
                            {
                                allocation.Reason = "Wrong hostel payment.";
                                failedAllocations.Add(allocation);
                                continue;
                            }

                            Hostel hostel = hostelLogic.GetModelsBy(h => h.Hostel_Name == allocation.HostelName && h.Activated).LastOrDefault();
                            HostelSeries hostelSeries = new HostelSeries();
                            HostelRoom hostelRoom = new HostelRoom();
                            if (hostel != null)
                            {
                                hostelSeries = hostelSeriesLogic.GetModelsBy(h => h.Series_Name == allocation.SeriesName && h.Activated && h.Hostel_Id == hostel.Id).LastOrDefault();

                                if (hostelSeries != null && hostelSeries.Id > 0)
                                {
                                    hostelRoom = hostelRoomLogic.GetModelsBy(h => h.Room_Number == allocation.RoomName && h.Activated && h.Hostel_Id == hostel.Id && h.Series_Id == hostelSeries.Id).LastOrDefault();
                                }
                            }
                            
                            List<HostelRoomCorner> hostelRoomCorners = new List<HostelRoomCorner>();
                            if (hostelRoom != null && hostelRoom.Id > 0)
                            {
                                hostelRoomCorners = hostelRoomCornerLogic.GetModelsBy(h => h.Room_Id == hostelRoom.Id && h.Activated);
                            }

                            if (hostel != null && hostelRoom != null && hostelSeries != null && hostelRoomCorners.Count > 0)
                            {
                                bool allocated = false;
                                for (int j = 0; j < hostelRoomCorners.Count; j++)
                                {
                                    HostelRoomCorner hostelRoomCorner = hostelRoomCorners[j];
                                    HostelAllocation existingHostelAllocation = hostelAllocationLogic.GetModelsBy(ha => ha.Session_Id == session.Id && ha.Hostel_Id == hostel.Id && ha.Series_Id == hostelSeries.Id && ha.Room_Id == hostelRoom.Id && ha.Corner_Id == hostelRoomCorner.Id).LastOrDefault();
                                    if (existingHostelAllocation != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        HostelAllocation studentHasAllocation = hostelAllocationLogic.GetModelsBy(h => h.Session_Id == session.Id && h.Student_Id == student.Id).LastOrDefault();
                                        if (studentHasAllocation == null)
                                        {
                                            existingHostelAllocation = new HostelAllocation();
                                            existingHostelAllocation.Student = student;
                                            existingHostelAllocation.Room = hostelRoom;
                                            existingHostelAllocation.Series = hostelSeries;
                                            existingHostelAllocation.Corner = hostelRoomCorner;
                                            existingHostelAllocation.Hostel = hostel;
                                            existingHostelAllocation.Occupied = true;
                                            existingHostelAllocation.Payment = payment;
                                            existingHostelAllocation.Person = new Person() { Id = student.Id };
                                            existingHostelAllocation.Session = session;

                                            hostelAllocationLogic.Create(existingHostelAllocation);
                                            allocated = true;

                                            break;
                                        }
                                    }
                                }

                                if (allocated)
                                {
                                    successfulAllocations.Add(allocation);
                                }
                                else
                                {
                                    allocation.Reason = "Allocation exists.";
                                    failedAllocations.Add(allocation);
                                }
                            }
                            else
                            {
                                allocation.Reason = "Hostel, room, series does not exist/ not active or no active bedspace for room.";
                                failedAllocations.Add(allocation);
                            }

                            scope.Complete();
                        }
                       
                    }

                    viewModel.SuccessfulAllocations = successfulAllocations;
                    viewModel.FailedAllocations = failedAllocations;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = viewModel.SessionSelectListItem;
            return View("UploadHostelAllocation", viewModel);
        }

        private Payment CheckAndCreateHostelPayment(HostelAllocation allocation, Session session)
        {
            Payment payment = null;
            try
            {
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == allocation.Student.Id).LastOrDefault();
                if (studentLevel != null)
                {
                    HostelViewModel viewModel = new HostelViewModel();
                    viewModel.Person = new Person(){ Id = allocation.Student.Id, Type = new PersonType(){ Id = 3 }};
                    viewModel.StudentLevel = studentLevel;
                    viewModel.Session = session;

                    return CreatePayment(viewModel);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CheckAndCreateHostelRequest(HostelAllocation allocation, Session session)
        {
            try
            {
                HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                HostelRequest hostelRequest = hostelRequestLogic.GetModelsBy(h => h.Person_Id == allocation.Student.Id && h.Session_Id == session.Id).LastOrDefault();

                if (hostelRequest == null)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                    Programme programme = new Programme();
                    Department department = new Department();
                    Level level = new Level();

                    StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == allocation.Student.Id).LastOrDefault();
                    if (studentLevel != null)
                    {
                        programme = studentLevel.Programme;
                        department = studentLevel.Department;
                        level = studentLevel.Level;
                    }
                    else
                    {
                        return;
                    }

                    hostelRequest = new HostelRequest();
                    hostelRequest.Student = allocation.Student;
                    hostelRequest.Approved = true;
                    hostelRequest.Department = department;
                    hostelRequest.Level = level;
                    hostelRequest.Person = new Person(){ Id = allocation.Student.Id };
                    hostelRequest.Programme = programme;
                    hostelRequest.RequestDate = DateTime.Now;
                    hostelRequest.Session = session;

                    hostelRequestLogic.Create(hostelRequest);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +
                                  "Extended Properties=Excel 8.0;";
                var connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow dataRow in sheet.Rows)
                {
                    string sheetName = dataRow[2].ToString().Replace("'", "");
                    var command = new OleDbCommand("Select * FROM [" + sheetName + "]", connection);
                    // Create DbDataReader to Data Worksheet

                    var MyData = new OleDbDataAdapter();
                    MyData.SelectCommand = command;
                    var ds = new DataSet();
                    ds.Clear();
                    MyData.Fill(ds);
                    connection.Close();

                    Result = ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public JsonResult GetHostelAllocationCount(int campusId)
        {
            HostelCountResult result = new HostelCountResult();
            result.AllocationCountModels = new List<AllocationCountModel>();
            try
            {
                if (campusId > 0)
                {
                    HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();
                    hostelAllocationCountLogic.GetModelsBy(c => c.Campus_Id == campusId).ForEach(h =>
                    {
                        result.AllocationCountModels.Add(new AllocationCountModel
                        {
                           HostelAllocationCountId = h.Id,
                           Level = h.Level.Name,
                           Sex = h.Sex.Name,
                           Free = h.Free,
                           Reserved = h.Reserved,
                           TotalCount = h.TotalCount,
                           LastModified = h.LastModified.ToLongDateString()
                        });
                    });

                    result.IsError = false;
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Invalid Parameter";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveHostelAllocationCount(string allocationCounts)
        {
            HostelCountResult result = new HostelCountResult();
            try
            {
                if (!string.IsNullOrEmpty(allocationCounts))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    List<AllocationCountModel> allocationCountModels = serializer.Deserialize<List<AllocationCountModel>>(allocationCounts);

                    HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();

                    for (int i = 0; i < allocationCountModels.Count; i++)
                    {
                        int allocationCountId = allocationCountModels[i].HostelAllocationCountId;

                        HostelAllocationCount hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Hostel_Allocation_Count_Id == allocationCountId);
                        if (hostelAllocationCount != null)
                        {
                            hostelAllocationCount.Free = allocationCountModels[i].Free;
                            hostelAllocationCount.Reserved = allocationCountModels[i].Reserved;
                            hostelAllocationCount.TotalCount = allocationCountModels[i].TotalCount;
                            hostelAllocationCount.LastModified = DateTime.UtcNow;

                            hostelAllocationCountLogic.Modify(hostelAllocationCount);
                        }
                    }

                    result.IsError = false;
                    result.Message = "Operation Successful!";
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Invalid Parameter";
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HostelAllocationReport()
        {
            try
            {
                viewModel = new HostelViewModel();
                PopulateDropDownList();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult HostelAllocationReport(HostelViewModel hostelViewModel)
        {
            try
            {
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                StudentLogic studentLogic = new StudentLogic();
                hostelViewModel.HostelAllocations =
                    hostelAllocationLogic.GetModelsBy(
                        s =>
                            s.Hostel_Id == hostelViewModel.HostelAllocation.Hostel.Id &&
                            s.Session_Id == hostelViewModel.HostelAllocation.Session.Id);

                foreach (HostelAllocation allocation in hostelViewModel.HostelAllocations)
                {
                    long personId = allocation.Person.Id;
                    allocation.Student = studentLogic.GetBy(personId);
                }

                ViewBag.HostelId = new SelectList(hostelViewModel.HostelSelectListItem, Utility.VALUE, Utility.TEXT,
                    hostelViewModel.HostelAllocation.Hostel.Id);

                ViewBag.SessionId = new SelectList(hostelViewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT,
                    hostelViewModel.HostelAllocation.Session.Id);
            }
            catch (Exception ex)
            {
                    
                throw;
            }
            return View(hostelViewModel);
        }
       
    }
}