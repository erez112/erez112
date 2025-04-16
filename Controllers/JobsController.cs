using klitatOved.DTO;
using klitatOved.Entities;
using klitatOved.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace klitatOved.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        public readonly Context context;

        public JobsController(Context context)
        {
            this.context = context;
        }

        //הוספת משרה
        [HttpPost("AddJob")]
        public IActionResult Add_Job([FromBody] JobsEO job)
        {
            //בדיקה האם המשרה כבר קיימת לפי שם ותיאור
            var exist_job = context.db_jobs
                .FirstOrDefault(j => j.JobTitle == job.JobTitle
                 );

            if (exist_job != null)
                return BadRequest("משרה כבר קיימת במערכת");

            //בדיקת שדות חובה
            if (string.IsNullOrEmpty(job.JobTitle) || !Regex.IsMatch(job.JobTitle, @"^[א-ת\s]+$"))
                return BadRequest("אנא הזן סוג משרה בעברית ");

            if (string.IsNullOrEmpty(job.JobDescription) || !Regex.IsMatch(job.JobDescription, @"^[א-ת\s.,\-/'\()]+$"))
     return BadRequest("אנא הזן תיאור משרה בעברית");



            if (string.IsNullOrEmpty(job.JobRequirements) || !Regex.IsMatch(job.JobRequirements, @"^[א-ת\s.,\-/'\()]+$"))
                return BadRequest("אנא הזן דרישות תפקיד בעברית");

            if (!int.TryParse(job.RequiredPositions.ToString(), out _))
                return BadRequest("הזן מספר משרות לאיוש");

            job.Organization = "מלון פנינת הגליל";
            job.Job_status = jobStatus.Open.GetDisplayName();
            //איפוס נתוני המועמדים למשרות
            job.TotalApplicants = 0; //כמות המועמדים שהגישו את הצעתם למשרה
            job.HiredCount = 0; // כמות המועמדים שהתקבלו
            job.RejectedCount = 0; // כמות המועמדים שנדחו

            job.CreatedAt = DateTime.Now.Date.Add(DateTime.Now.TimeOfDay).AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);

            job.Fillings_status = "לא מולאה";
            context.db_jobs.Add(job);
            context.SaveChanges();

            return Ok("משרה נוספה בהצלחה");


        }

        //עדכון פרטי משרה
        [HttpPut("UpdateJob/{id}")]
        public IActionResult update_job(int id, [FromBody] UpdateJobDTO updateJob)
        {
            var existingJob = context.db_jobs.FirstOrDefault(j => j.JobId == id);
            if (existingJob == null)
            {
                return NotFound("המשרה לא נמצאה");
            }

            // בדיקות תקינות
            if (string.IsNullOrWhiteSpace(updateJob.JobTitle) ||
                string.IsNullOrWhiteSpace(updateJob.JobDescription) ||
                string.IsNullOrWhiteSpace(updateJob.JobRequirements))
            {
                return BadRequest("כותרת, תיאור ודרישות המשרה לא יכולים להיות ריקים.");
            }

            if (updateJob.TotalApplicants < 0 || updateJob.HiredCount < 0 || updateJob.RejectedCount < 0)
            {
                return BadRequest("המספרים של המועמדים לא יכולים להיות שליליים.");
            }

            // עדכון שדות
            existingJob.JobTitle = updateJob.JobTitle;
            existingJob.JobDescription = updateJob.JobDescription;
            existingJob.JobRequirements = updateJob.JobRequirements;
            existingJob.Job_status = updateJob.Job_status;
            existingJob.Fillings_status = updateJob.Fillings_status;
            existingJob.TotalApplicants = updateJob.TotalApplicants.GetValueOrDefault();
            existingJob.HiredCount = updateJob.HiredCount.GetValueOrDefault(); ;
            existingJob.RejectedCount = updateJob.RejectedCount.GetValueOrDefault(); ;

            context.SaveChanges();
            return Ok("המשרה עודכנה בהצלחה");
        }

        //חיפוש משרה
        [HttpGet("SearchJobs")]
        public IActionResult Search_Jobs(string? jobTitle, string? organization, string? jobStatus)
        {
            var query = context.db_jobs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(jobTitle))
                query = query.Where(j => j.JobTitle.Contains(jobTitle));

            if (!string.IsNullOrWhiteSpace(organization))
                query = query.Where(j => j.Organization.Contains(organization));

            if (!string.IsNullOrWhiteSpace(jobStatus))
                query = query.Where(j => j.Job_status == jobStatus);

            var jobs = query.ToList();

            if (!jobs.Any())
                return NotFound("לא נמצאו משרות מתאימות");

            return Ok(jobs);
        }

        //הסרת משרה - במידה והמשרה מולאה
        [HttpDelete("DeleteJob/{id}")]
        public IActionResult Delete_Job(int id)
        {
            var job = context.db_jobs.FirstOrDefault(j => j.JobId == id);

            if (job == null)
            {
                return NotFound("המשרה לא נמצאה");
            }

            if (job.Job_status == "Filled")
            {
                context.db_jobs.Remove(job);
                context.SaveChanges();
                return Ok("משרה הוסרה בהצלחה");
            }

            return BadRequest("לא ניתן להסיר משרה שלא מולאה");

        }

        [HttpGet("GetJob/{id}")]
        public IActionResult Get_Job(int id)
        {
            var job = context.db_jobs.FirstOrDefault(j => j.JobId == id);

            if (job == null)
            {
                return NotFound("המשרה לא נמצאה");
            }

            return Ok(job);

        }
    }
}
