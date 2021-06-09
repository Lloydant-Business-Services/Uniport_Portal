﻿using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ELearningViewModel
    {
        public ELearningViewModel()
        {
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectList = Utility.PopulateProgrammeSelectListItem();
            LevelList = Utility.GetAllLevels();
            UserSelectList = Utility.PopulateStaffSelectListItem();
            ContentTypeSelectList = Utility.PopulateEContentTypeSelectListItem();
        }

        public List<SelectListItem> ContentTypeSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> UserSelectList { get; set; }
        public List<Level> LevelList { get; set; }
        public List<ECourse> ECourseList { get; set; }
        public List<EAssignment> EAssignmentList { get; set; }
        public List<EAssignmentSubmission> EAssignmentSubmissionList { get; set; }
        public EAssignment eAssignment { get; set; }
        public EContentType eContentType { get; set; }
        public List<EContentType> EContentTypes { get; set; }
        public ECourse eCourse { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public List<Course> Courses { get; set; }
        public User User { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
        public List<CourseAllocation> CourseAllocationList { get; set; }
        public long cid { get; set; }
        public List<CourseAllocation> CourseAllocations { get; set; }
        public List<UploadedCourseFormat> UploadedCourses { get; set; }
        public Staff Staff { get; set; }
        public List<CourseRegistrationDetail> CourseRegistration { get; set; }
        public List<ELearningView> ELearningViews { get; set; } 
        public long CourseAllocationId { get; set; }
        public List<AttendanceClassList> AttendanceClassList { get; set; }
        public List<GeneralAttendance> GeneralAttendanceList { get; set; }
        public List<EAssignmentClassList> EAssignmentClassList { get; set; }
        public string TextSubmission { get; set; }
        public List<EChatResponse> EChatResponses { get; set; }
        public Model.Model.Student Student { get; set; }
        public List<EContentType> EContentTypeList { get; set; }
        public CourseRegistrationSemesters CourseRegistrationSemesters { get; set; }
        public TimeSpan endTime { get; set; }
        public TimeSpan startTime { get; set; }
        public EAssignmentSubmission EAssignmentSubmission { get; set; }
    }
    public class CourseRegistrationSemesters
    {
        public List<CourseRegistrationDetail> CourseRegistrationFirstSemester { get; set; }
        public List<CourseRegistrationDetail> CourseRegistrationSecondSemester { get; set; }
    }
    public class AttendanceClassList
    {
        public CourseRegistrationDetail CourseRegistrationDetail { get; set; }
        public bool IsPresent { get; set; }
    }
    public class TopicAttendance
    {

        public CourseRegistrationDetail CourseRegistrationDetail { get; set; }
        public bool IsPresent { get; set; }
    }
    public class GeneralAttendance
    {
        public string Topics { get; set; }
        public List<TopicAttendance> AttendanceClassLists { get; set; }
        public List<CourseRegistrationDetail> CourseRegistrationDetailList { get; set; }
    }
    public class EAssignmentClassList
    {
        public CourseRegistrationDetail CourseRegistrationDetail { get; set; }
        public EAssignmentSubmission EAssignmentSubmission { get; set; }
        public bool IsSubmission { get; set; }
    }
}