﻿using Abundance_Nk.Model.Model;
using System.Collections.Generic;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class CourseRegistrationViewModel
    {
        //private CourseLogic courseLogic;

        public CourseRegistrationViewModel()
        {
            ScratchCard = new ScratchCard();
            Student = new Model.Model.Student();

            StudentLevel = new StudentLevel();
            StudentLevel.Programme = new Programme();
            StudentLevel.Department = new Department();
            StudentLevel.DepartmentOption = new DepartmentOption();
            StudentLevel.Level = new Level();

            CarryOverCourses = new List<CourseRegistrationDetail>();
            FirstSemesterCarryOverCourses = new List<CourseRegistrationDetail>();
            SecondSemesterCarryOverCourses = new List<CourseRegistrationDetail>();
            FirstSemesterCourses = new List<CourseRegistrationDetail>();
            SecondSemesterCourses = new List<CourseRegistrationDetail>();

            RegisteredCourse = new CourseRegistration();
        }

        public string MatricNumber { get; set; }
        public ScratchCard ScratchCard { get; set; }
        public Model.Model.Student Student { get; set; }
        public CurrentSessionSemester CurrentSessionSemester { get; set; }
        public int SumOfFirstSemesterSelectedCourseUnit { get; set; }
        public int SumOfSecondSemesterSelectedCourseUnit { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public int FirstSemesterMinimumUnit { get; set; }
        public int FirstSemesterMaximumUnit { get; set; }
        public int SecondSemesterMinimumUnit { get; set; }
        public int SecondSemesterMaximumUnit { get; set; }
        public int TotalFirstSemesterCarryOverCourseUnit { get; set; }
        public int TotalSecondSemesterCarryOverCourseUnit { get; set; }
        public bool CarryOverExist { get; set; }
        public bool CourseAlreadyRegistered { get; set; }
        public CourseRegistration RegisteredCourse { get; set; }
        public List<CourseRegistrationDetail> CarryOverCourses { get; set; }
        public List<CourseRegistrationDetail> FirstSemesterCarryOverCourses { get; set; }
        public List<CourseRegistrationDetail> SecondSemesterCarryOverCourses { get; set; }
        public List<CourseRegistrationDetail> FirstSemesterCourses { get; set; }
        public List<CourseRegistrationDetail> SecondSemesterCourses { get; set; }
        public string invoiceNumber { get; set; }
        public Semester Semester { get; set; }
    }
}