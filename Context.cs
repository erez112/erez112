using klitatOved.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> option):base(option)
        {

        }

        public Context()
        {

        }

        protected override void OnModelCreating(ModelBuilder model_builder)
        {
            //קשר בין מועמדות למועמד - יחיד לרבים
            model_builder.Entity<JobApplicationsEO>()
                .HasOne(job => job.Candidate)
                .WithMany(c => c.CandidatesApplyJob)
                .HasForeignKey(j => j.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            //קשר בין מועמדות למשרה - יחיד לרבים
            model_builder.Entity<JobApplicationsEO>()
                .HasOne(j => j.Job)
                .WithMany(jb => jb.JobGetCandidate)
                .HasForeignKey(j => j.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            //קשר בין ריאיונות למועמד - יחיד לרבים
            model_builder.Entity<InterviewsEO>()
                .HasOne(i => i.Candidate)
                .WithMany(c => c.candidtateGetInterview)
                .HasForeignKey(i => i.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            //קשר בין ריאיונות למשרה - יחיד לרבים
            model_builder.Entity<InterviewsEO>()
                .HasOne(i => i.Job)
                .WithMany(jb => jb.jobGetInterview)
                .HasForeignKey(i => i.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public virtual DbSet<CandidatesEO> db_candidates { get; set; }
        public virtual DbSet<JobsEO> db_jobs { get; set; }
        public virtual DbSet<InterviewsEO> db_interviews { get; set; }
        public virtual DbSet<JobApplicationsEO> db_aplicatesJob { get; set; }
    }
}
