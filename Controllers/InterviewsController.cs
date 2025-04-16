using klitatOved.DTO;
using klitatOved.Entities;
using klitatOved.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace klitatOved.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewsController : ControllerBase
    {
        public Context context;

        public InterviewsController(Context context)
        {
            this.context = context;
        }

        //יצירת ריאיון חדש
        [HttpPost("AddInterview")]
        public IActionResult Add_Interview([FromBody] InterviewsEO interview)
        {
            //בדיקה שנושא הראיון תקין
            if (string.IsNullOrWhiteSpace(interview.InterviewName) || !Regex.IsMatch(interview.InterviewName, @"^[א-ת\s]+$"))
                return BadRequest("הזן נושא ריאיון בעברית בלבד");

            //המרה אם השם עברי
            var interviewType = InterviewHelper.TranslateToInterviewType(interview.InterviewType);

            //בדיקה שסוג הריאיון תקין
            if (!Enum.IsDefined(typeof(InterviewType), interviewType))
            {
                return BadRequest("הזן סוג ריאיון תקין - פרונטלי, זום או טלפוני");
            }

            var dateOnly = interview.InterviewDate.Date;
            //בדיקת תאריך הריאיון
            if (dateOnly < DateTime.Now.Date)
                return BadRequest("תאריך הריאיון אינו יכול להיות בעבר");

            var nextWeek = DateTime.Now.AddDays(7).Date;
            if (interview.InterviewDate < nextWeek)
                return BadRequest(" הריאיון צריך להיות לפחות בשבוע הבא");

            var twoWeeksFromNow = DateTime.Now.AddDays(14).Date;
            if (dateOnly > twoWeeksFromNow)
                return BadRequest("ריאיון לא יכול להיות יותר משבועיים קדימה");

            //בדיקת תקינות השעה
            if (TimeSpan.TryParse(interview.InterviewTime, out var interviewTime))
            {
                // אם השעה פחות מ-10:00 או יותר מ-18:00
                if (interviewTime < TimeSpan.FromHours(10) || interviewTime > TimeSpan.FromHours(18))
                    return BadRequest("הזן שעת ריאיון בין 10:00 - 18:00");
            }
            else
            {
                return BadRequest("הזן שעת ריאיון תקנית");
            }


            //בדיקה אם המועמד קיים
            var candidate = context.db_candidates.FirstOrDefault(c => c.CandidateId == interview.CandidateId);
            if (candidate == null)
                return NotFound("המועמד לא נמצא במערכת");

            //בדיקה אם המשרה קיימת
            var job = context.db_jobs.FirstOrDefault(j => j.JobId == interview.JobId);
            if (job == null)
                return BadRequest("משרה לא קיימת");
            //בדיקה אם יש כבר ריאיון בתאריך ובשעה זו
            var existingInterview = context.db_interviews
                 .FirstOrDefault(i => i.CandidateId == interview.CandidateId && i.InterviewDate == interview.InterviewDate && i.InterviewTime == interview.InterviewTime);
            if (existingInterview != null)
                return BadRequest("מועמד כבר קבע ריאיון בתאריך ובשעה זו");

            //בדיקות תקינות מיקום עבור ריאיון פרונטלי
            if(Enum.TryParse<InterviewType>(interview.InterviewType, out var parsedType))
            {
                if (parsedType == InterviewType.InPerson && string.IsNullOrWhiteSpace(interview.InterviewLoc))
                    return BadRequest("הזן מיקום לריאיון פרונטלי");

                return BadRequest("סוג ריאיון לא תקני");

               
            }
            interview.Interview_status = "מתואם";
            //הוספת - קביעת ריאיון
            context.db_interviews.Add(interview);
            context.SaveChanges();

            return Ok( " ריאיון נקבע בהצלחה ל " + candidate.FirstName + " " + candidate.LastName);
        }


        //מתודה לעדכון פרטי ריאיון
        [HttpPut("UpdateInterview/{id}")]
        public IActionResult Update_Interview(int id, [FromBody] InterviewsEO updatedInterview)
        {
            var existingInterview = context.db_interviews.FirstOrDefault(i => i.InterviewId == id);

            if (existingInterview == null)
            {
                return NotFound("הריאיון לא נמצא.");
            }

            // עדכון פרטי הריאיון (ניתן לעדכן שדות רלוונטיים בלבד)
            existingInterview.InterviewDate = updatedInterview.InterviewDate;
            existingInterview.InterviewTime = updatedInterview.InterviewTime;
            existingInterview.InterviewType = updatedInterview.InterviewType;
            existingInterview.InterviewName = updatedInterview.InterviewName;
            existingInterview.Interview_status = updatedInterview.Interview_status;
            existingInterview.InterviewLoc = updatedInterview.InterviewLoc;

            context.SaveChanges();

            return Ok("הריאיון עודכן בהצלחה.");
        }

        //מתודה לביטול ריאיון
        [HttpDelete("DeleteInterview/{id}")]
        public IActionResult Delete_Interview(int id)
        {
            var interview = context.db_interviews.FirstOrDefault(i => i.InterviewId == id);

            if (interview == null)
            {
                return NotFound("ריאיון לא נמצא.");
            }

            context.db_interviews.Remove(interview);
            context.SaveChanges();

            return Ok("הריאיון בוטל בהצלחה.");
        }

        //מתודה לשליפת כל הריאיונות
        [HttpGet("GetInterviews")]
        public IActionResult Get_Interviews()
        {
            var interviews = context.db_interviews.ToList();
            if (interviews == null || !interviews.Any())
                return NotFound("אין ריאיונות במערכת");

            return Ok(interviews);
        }

        //שליפת ריאיון למזהה ספציפי
        [HttpGet("GetInterview/{id}")]
        public IActionResult Get_Interview(int id)
        {
            var interview = context.db_interviews.
                Where(i=>i.InterviewId == id)
                .Select(i => new
                {
                    i.InterviewId,
                    candidateName = i.Candidate.FirstName + " " + i.Candidate.LastName,
                    jobTitle = i.Job.JobTitle,
                    i.InterviewDate,
                    i.InterviewName,
                    i.InterviewType,
                    i.Interview_status,
                    i.InterviewTime,
                    i.InterviewLoc
                })

                .FirstOrDefault();
            if (interview == null)
                return BadRequest("הריאיון לא נמצא");

            return Ok(interview);
        }

        //שליפת ריאיונות לפי מועמד
        [HttpGet("GetInterviewByCandidate/{candidateId}")]
        public IActionResult Get_Interview_By_Candidate(int candidateId)
        {
            var interviews = context.db_interviews
        .Where(i => i.CandidateId == candidateId)
        .Select(i => new
        {
            i.InterviewId,
            CandidateName = i.Candidate.FirstName + " " + i.Candidate.LastName, // שם מלא של המועמד
            JobTitle = i.Job.JobTitle, // שם המשרה
            i.InterviewDate,
            i.InterviewType,
            i.InterviewName,
            i.Interview_status,
            i.InterviewTime,
            i.InterviewLoc
        })
        .ToList();

            if (!interviews.Any())
                return NotFound("לא נמצאו ריאיונות למועמד זה");

            return Ok(interviews);
        }
    

        //שליפת ריאיונות לפי משרה
        [HttpGet("GetInterviewsByJob/{jobId}")]
        public IActionResult Get_Interviews_By_Job(int jobId)
        {
            var interviews = context.db_interviews
        .Where(i => i.JobId == jobId)
        .Select(i => new
        {
            i.InterviewId,
            CandidateName = i.Candidate.FirstName + " " + i.Candidate.LastName, // שם מלא של המועמד
            JobTitle = i.Job.JobTitle, // שם המשרה
            i.InterviewDate,
            i.InterviewType,
            i.InterviewName,
            i.Interview_status,
            i.InterviewTime,
            i.InterviewLoc
        })
        .ToList();

            if (!interviews.Any())
                return NotFound("לא נמצאו ריאיונות למשרה זו");


            return Ok(interviews);
        }

        //חיפוש ריאיונות לפי תאריך או טווח תאריכים
        [HttpGet("SearchInterviewsByDate")]
        public IActionResult Search_Interviews_By_Date(DateTime startDate, DateTime endDate)
        {
            var interviews = context.db_interviews.Where
                (i => i.InterviewDate >= startDate && i.InterviewDate <= endDate)
                .Select(i => new
                {
                    i.InterviewId,
                    candidateName = i.Candidate.FirstName + " " + i.Candidate.LastName,
                    JobTitle = i.Job.JobTitle,
                    i.InterviewType,
                    i.InterviewName,
                    i.Interview_status,
                    i.InterviewTime,
                    i.InterviewLoc
                })

                .ToList();
            
            if (!interviews.Any())
                return NotFound("לא נמצאו ריאיונות בתאריכים אלו");

            return Ok(interviews);
        }

        //עדכון סטטוס ריאיון
        //מתודה שמאפשרת לעדכן רק את סטטוס הריאיון (למשל, מ״מתוכנן״ ל״בוצע״ או ״נדחה״), בלי לעדכן את כל הפרטים:

        [HttpPut("UpdateInterviewStatus/{id}")]
        public IActionResult Update_Interview_Status(int id, [FromBody] UpdateInterviewStatusDTO request)
        {
            var interview = context.db_interviews.FirstOrDefault(i => i.InterviewId == id);
            if (interview == null)
                return NotFound("הריאיון לא נמצא.");

            interview.Interview_status = request.NewStatus; // עדכון הסטטוס
            context.SaveChanges();

            return Ok(new
            {
                Message = "סטטוס הריאיון עודכן בהצלחה.",
                InterviewId = interview.InterviewId,
                NewStatus = request.NewStatus
            });
        }

        //שליחת פרטי ריאיון למועמד
        [HttpGet("SendInterviewDetails/{interviewId}")]
        public IActionResult Send_Interview_Details(int interviewId)
        {
            var interview = context.db_interviews.FirstOrDefault(i => i.InterviewId == interviewId);
            if (interview == null)
                return NotFound("הריאיון לא נמצא.");

            // כאן תבנה את ההודעה שתרצה לשלוח למועמד, לדוגמה:
            string message = $"הריאיון שלך נקבע ל-{interview.InterviewDate.ToShortDateString()} בשעה {interview.InterviewTime}.";
            // קריאה לפונקציה למשלוח מייל או SMS למועמד:
            // SendEmail(candidate.Email, "פרטי ריאיון", message);

            return Ok(new { Message = "פרטי הריאיון נשלחו למועמד.", Details = message });
        }





    }
}
