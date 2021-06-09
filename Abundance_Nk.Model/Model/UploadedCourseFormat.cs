﻿namespace Abundance_Nk.Model.Model
{
    public class UploadedCourseFormat
    {
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string Department { get; set; }
        public string LecturerName { get; set; }
        public string Programme { get; set; }
        public string Level { get; set; }
        public int ProgrammeId { get; set; }
        public int DepartmentId { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        public int LevelId { get; set; }
        public long CourseId { get; set; }
    }
}