using klitatOved.Entities;
using klitatOved.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace klitatOved.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicatesController : ControllerBase
    {
            public readonly Context context;

            public ApplicatesController(Context context)
            {
                this.context = context;
            }

        //בדיקה ראשונית של גיט
        //מתודה להגשת מועמדות
        [HttpPost("ApplyForJob")]
        public IActionResult Apply_For_Job([FromForm] JobApplicationsEO jobApplication)
        {
            var existApply = context.db_aplicatesJob.FirstOrDefault(ap => ap.CandidateId == jobApplication.CandidateId
            && ap.JobId == jobApplication.JobId);
            if (existApply != null)
                return BadRequest("הוגשה מועמדות למשרה זו");


            if (jobApplication.CandidateId < 100000000 || jobApplication.CandidateId > 999999999)
            {
                return BadRequest("אנא הזן מספר זהות תקין בעל 9 ספרות");
            }

            if (string.IsNullOrWhiteSpace(jobApplication.Email) || !Regex.IsMatch(jobApplication.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("הזן כתובת מייל תקינה");

            if (string.IsNullOrWhiteSpace(jobApplication.PhoneNumber) || !Regex.IsMatch(jobApplication.PhoneNumber, @"^05[0-9]{8}$"))
                return BadRequest("אנא הזן מספר נייד");

            if (string.IsNullOrWhiteSpace(jobApplication.ResumePath))
                return BadRequest("אנא צרף קובץ קורות חיים");

            if (!Uri.IsWellFormedUriString(jobApplication.ResumePath, UriKind.RelativeOrAbsolute))
                return BadRequest("שם קובץ לא חוקי");

            if (Path.GetInvalidFileNameChars().Any(invalidChar => jobApplication.ResumePath.Contains(invalidChar)))
                return BadRequest("שם הקובץ מכיל תווים אסורים");

            string fileExtension = Path.GetExtension(jobApplication.ResumePath).ToLower();
            string[] allowedExtensions = { ".pdf", ".doc", ".docx" };


            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("PDF או DOC, DOCX הקובץ חייב להיות בפורמט");

            if (!Enum.IsDefined(typeof(Status), jobApplication.Progress_Status))
                jobApplication.Progress_Status = Status.New.GetDescription();

            jobApplication.ApplicationDate = DateTime.Now;
            context.db_aplicatesJob.Add(jobApplication);
             context.SaveChanges();

            return Ok("מועמדותך נקלטה בהצלחה");
        }

        [HttpDelete("RemoveApply/{id}")]
        public IActionResult Delete_apply(int id)
        {
            //קריאת מזהה המשתמש המחובר
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("משתמש לא מחובר");

            //חיפוש המועמדות
            var apply = context.db_aplicatesJob.FirstOrDefault(c => c.Id == id);

            if (apply == null)
                return NotFound("מועמדות למשרה לא נמצאה");

            //בדיקה שהמשתמש המחובר הוא המועמד
            if (apply.CandidateId.ToString() != userId)
                return Unauthorized("אין לך הרשאה לבטל מועמדות זו");

            //הסרת המועמדות
            context.db_aplicatesJob.Remove(apply);
            context.SaveChanges();
            return NoContent();
        }

    }
}
