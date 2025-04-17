using klitatOved.Entities;
using klitatOved.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace klitatOved.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        public readonly Context context;

        public CandidatesController(Context context)
        {
            this.context = context;
        }


        //הוספת מועמדים
        [HttpPost("Register")]

        public IActionResult AddCandidate([FromBody] CandidatesEO candidate)
        {
            var existCandidate = context.db_candidates.FirstOrDefault(c => c.IdNumber == candidate.IdNumber);
            if (existCandidate != null)
                return BadRequest("משתמש כבר קיים במערכת");

            if (candidate.IdNumber < 100000000 || candidate.IdNumber > 999999999)
            {
                return BadRequest("אנא הזן מספר זהות תקין בעל 9 ספרות");
            }

            if (string.IsNullOrWhiteSpace(candidate.FirstName) || !Regex.IsMatch(candidate.FirstName, @"^[א-ת]+$"))
                return BadRequest("אנא הזן שם פרטי בעברית");

            if (string.IsNullOrWhiteSpace(candidate.LastName) || !Regex.IsMatch(candidate.LastName, @"^[א-ת]+$"))
                return BadRequest("אנא הזן שם משפחה בעברית");

            if (string.IsNullOrWhiteSpace(candidate.En_FirstName) || !Regex.IsMatch(candidate.En_FirstName, @"^[a-zA-Z]+$"))
                return BadRequest("אנא הזן שם פרטי באנגלית");
            
            if (string.IsNullOrWhiteSpace(candidate.En_LastName) || !Regex.IsMatch(candidate.En_LastName, @"^[a-zA-Z]+$"))
                return BadRequest("אנא הזן שם משפחה באנגלית");

            if (string.IsNullOrWhiteSpace(candidate.Email) || !Regex.IsMatch(candidate.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("אנא הזן כתובת מייל");

            if (string.IsNullOrWhiteSpace(candidate.Phone) || !Regex.IsMatch(candidate.Phone, @"^05[0-9]{8}$"))
                return BadRequest("אנא הזן מספר נייד");

            if (string.IsNullOrWhiteSpace(candidate.UserName) || !Regex.IsMatch(candidate.UserName, @"^[a-zA-Z]+$"))
                return BadRequest("אנא הזן שם משתמש");

            if (string.IsNullOrWhiteSpace(candidate.PasswordHash) || candidate.PasswordHash.Length < 8 && !Regex.IsMatch(candidate.UserName, @"^[a-zA-Z]+$"))
                return BadRequest("אנא הזן סיסמה תקינה. 8 תווים לפחות ואותיות לועזיות");

           
            context.db_candidates.Add(candidate);
            context.SaveChanges();

            return Ok("המועמד נוסף בהצלחה");
        }


        //שליפת מועמדים
        [HttpGet("FindCandidate/{id}")]

        public IActionResult GetCandits(int id)
        {
            var candidate = context.db_candidates.FirstOrDefault(c => c.IdNumber == id);

            if (candidate == null)
                return NotFound("מועמד לא נמצא.");

            var result =
            new
            {
                candidate.IdNumber,
                candidate.FirstName,
                candidate.LastName,
            };

            return Ok(result);

        }

        //עדכון פרטי מועמדים
        [HttpPut("UpdateCandidate/{id}")]
        public IActionResult Update_candidate(int id, [FromBody] CandidatesEO candidate)
        {
            var exist_candidate = context.db_candidates.FirstOrDefault(c => c.IdNumber == id);
            if (exist_candidate == null)
                return NotFound("מועמד לא נמצא");

            //עדכון פרטים לפי הצורך
            exist_candidate.FirstName = candidate.FirstName;
            exist_candidate.LastName = candidate.LastName;
            exist_candidate.En_FirstName = candidate.En_FirstName;
            exist_candidate.En_LastName = candidate.En_LastName;
            exist_candidate.Email = candidate.Email;
            exist_candidate.Phone = candidate.Phone;

           

            context.SaveChanges();
            return Ok("הפרטים עודכנו בהצלחה");
        }

        //חיפוש מועמד
        [HttpGet("SearchCandidate")]
        public IActionResult Search_CCandidate(string firstName, string lastName)
        {
            var candidates = context.db_candidates
                .Where(c => c.FirstName.ToLower().Contains(firstName.ToLower())
                && c.LastName.ToLower().Contains(lastName.ToLower()))
                .ToList();

            return Ok(candidates);
                
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] CandidatesEO candidate)
        {
            if(string.IsNullOrEmpty(candidate.UserName) || string.IsNullOrEmpty(candidate.PasswordHash))
            {
                return BadRequest("אנא הזן שם משתמש וסיסמה ");
            }

            //חיפוש משץמש לפי שם משתמש
            var cndt = context.db_candidates.SingleOrDefault(c => c.UserName == candidate.UserName);

            //אם המשתמש לא נמצא
            if (cndt == null)
                return BadRequest("שם משתמש שגוי");

            //אם הסיסמה לא תואמת את הדרישות
            if (cndt.PasswordHash != candidate.PasswordHash)
                return BadRequest("סיסמה שגויה");

            //ההתחברות הצליחה
            return Ok("ברוך הבא");

        }
    }
   

}
