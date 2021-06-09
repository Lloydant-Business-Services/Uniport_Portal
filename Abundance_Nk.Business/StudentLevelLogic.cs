using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class StudentLevelLogic : BusinessBaseLogic<StudentLevel, STUDENT_LEVEL>
    {
        public StudentLevelLogic()
        {
            translator = new StudentLevelTranslator();
        }
        
        public StudentLevel GetBy(long studentId)
        {
            try
            {
                var sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Person_Id == studentId;
                List<StudentLevel> studentLevels = base.GetModelsBy(selector);
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel = studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 =
                        sl => sl.Person_Id == studentId && sl.Level_Id == maxLevel && sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = base.GetModelBy(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 =
                            sl => sl.Person_Id == studentId && sl.Level_Id == minLevel;
                        StudentLevel CurrentLevelAlt = base.GetModelBy(selector3);
                        CurrentLevel = CurrentLevelAlt;
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<StudentLevel> GetByAsync(long studentId)
        {
            try
            {
                var sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Person_Id == studentId;
                List<StudentLevel> studentLevels = await base.GetModelsByAsync(selector);
                Session session = await sessionLogic.GetModelByAsync(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel = studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 =
                        sl => sl.Person_Id == studentId && sl.Level_Id == maxLevel && sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = await base.GetModelByAsync(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 = sl => sl.Person_Id == studentId && sl.Level_Id == minLevel;
                        StudentLevel CurrentLevelAlt = await base.GetModelByAsync(selector3);
                        CurrentLevel = CurrentLevelAlt;
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public StudentLevel GetExtraYearBy(long studentId)
        {
            try
            {
                var sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Person_Id == studentId;
                List<StudentLevel> studentLevels = base.GetModelsBy(selector);
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel = studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 =
                        sl => sl.Person_Id == studentId && sl.Level_Id == maxLevel && sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = base.GetModelBy(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 =
                            sl => sl.Person_Id == studentId && sl.Level_Id == minLevel && sl.Session_Id == session.Id;
                        StudentLevel CurrentLevelAlt = base.GetModelBy(selector3);
                        CurrentLevel = CurrentLevelAlt;
                    }
                    if (CurrentLevel == null)
                    {
                        int maxLevel2 = studentLevels.Max(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector4 =
                            sl => sl.Person_Id == studentId && sl.Level_Id == maxLevel;
                        StudentLevel CurrentLevel2 = base.GetModelBy(selector4);
                        CurrentLevel = CurrentLevel2;
                        if (CurrentLevel2 == null)
                        {
                            int minLevel = studentLevels.Min(p => p.Level.Id);
                            Expression<Func<STUDENT_LEVEL, bool>> selector5 =
                                sl => sl.Person_Id == studentId && sl.Level_Id == minLevel;
                            StudentLevel CurrentLevelAlt = base.GetModelBy(selector5);
                            CurrentLevel = CurrentLevelAlt;
                        }
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public StudentLevel GetBy(string MatricNumber)
        {
            try
            {
                var sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.STUDENT.Matric_Number == MatricNumber;
                List<StudentLevel> studentLevels = base.GetModelsBy(selector);
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel = studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 =
                        sl =>
                            sl.STUDENT.Matric_Number == MatricNumber && sl.Level_Id == maxLevel &&
                            sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = base.GetModelBy(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 =
                            sl => sl.STUDENT.Matric_Number == MatricNumber && sl.Level_Id == minLevel;
                        StudentLevel CurrentLevelAlt = base.GetModelBy(selector3);
                        CurrentLevel = CurrentLevelAlt;
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<StudentLevel> GetByAsync(string MatricNumber)
        {
            try
            {
                var sessionLogic = new SessionLogic();
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.STUDENT.Matric_Number == MatricNumber;
                List<StudentLevel> studentLevels = base.GetModelsBy(selector);
                Session session = await sessionLogic.GetModelByAsync(p => p.Activated == true);
                if (studentLevels != null && studentLevels.Count > 0)
                {
                    int maxLevel = studentLevels.Max(p => p.Level.Id);
                    Expression<Func<STUDENT_LEVEL, bool>> selector2 =
                        sl =>
                            sl.STUDENT.Matric_Number == MatricNumber && sl.Level_Id == maxLevel &&
                            sl.Session_Id == session.Id;
                    StudentLevel CurrentLevel = await base.GetModelByAsync(selector2);
                    if (CurrentLevel == null)
                    {
                        int minLevel = studentLevels.Min(p => p.Level.Id);
                        Expression<Func<STUDENT_LEVEL, bool>> selector3 =
                            sl => sl.STUDENT.Matric_Number == MatricNumber && sl.Level_Id == minLevel;
                        StudentLevel CurrentLevelAlt =await base.GetModelByAsync(selector3);
                        CurrentLevel = CurrentLevelAlt;
                    }
                    return CurrentLevel;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public StudentLevel GetBy(Student student, Session session)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector =
                    sl => sl.Person_Id == student.Id && sl.Session_Id == session.Id;
                return base.GetModelsBy(selector).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentLevel> GetBy(Level level, Session session)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector =
                    sl => sl.Level_Id == level.Id && sl.Session_Id == session.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<StudentLevel> GetBy(Level level, Programme programme, Department department, Session session)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector =
                    sl =>
                        sl.Level_Id == level.Id && sl.Programme_Id == programme.Id && sl.Department_Id == department.Id &&
                        sl.Session_Id == session.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(StudentLevel student)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Student_Level_Id == student.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);

               
                if (student.Level != null && student.Level.Id > 0)
                {
                    entity.Level_Id = student.Level.Id;
                }
                if (student.Session != null && student.Session.Id > 0)
                {
                    entity.Session_Id = student.Session.Id;
                }

                int modifiedRecordCount = Save();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ModifyIncludingDepartmentAndProgrammme(StudentLevel student)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Student_Level_Id == student.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);


                if (student.Level != null && student.Level.Id > 0)
                {
                    entity.Level_Id = student.Level.Id;
                }
                if (student.Session != null && student.Session.Id > 0)
                {
                    entity.Session_Id = student.Session.Id;
                }
                if (student.Department != null && student.Department.Id > 0)
                {
                    entity.Department_Id = student.Department.Id;
                }
                if (student.Programme != null && student.Programme.Id > 0)
                {
                    entity.Programme_Id = student.Programme.Id;
                }

                int modifiedRecordCount = Save();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ModifyAsync(StudentLevel student)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector = sl => sl.Student_Level_Id == student.Id;
                STUDENT_LEVEL entity = await GetEntityByAsync(selector);


                if (student.Level != null && student.Level.Id > 0)
                {
                    entity.Level_Id = student.Level.Id;
                }
                if (student.Session != null && student.Session.Id > 0)
                {
                    entity.Session_Id = student.Session.Id;
                }

                int modifiedRecordCount = Save();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(StudentLevel student, Person person)
        {
            try
            {
                StudentLevel model = GetBy(person.Id);
                Expression<Func<STUDENT_LEVEL, bool>> selector =
                    sl =>
                        sl.Level_Id == model.Level.Id && sl.Person_Id == model.Student.Id &&
                        sl.Session_Id == student.Session.Id;
                STUDENT_LEVEL entity = GetEntityBy(selector);

                entity.Level_Id = student.Level.Id;
                entity.Department_Id = student.Department.Id;
                entity.Programme_Id = student.Programme.Id;


                int modifiedRecordCount = Save();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<StudentLevel> GetBy(Programme programme, Department department, Session session)
        {
            try
            {
                Expression<Func<STUDENT_LEVEL, bool>> selector =
                    sl =>
                        sl.Programme_Id == programme.Id && sl.Department_Id == department.Id &&
                        sl.Session_Id == session.Id;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}