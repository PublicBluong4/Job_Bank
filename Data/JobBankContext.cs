using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluong4_Job_Bank.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


//Name: Ba Binh Luong,
//Done for: Project 1 Part A and B
//Last Modified: 2020 September 28 6:00pm

namespace Bluong4_Job_Bank.Data
{
    public class JobBankContext: DbContext
    {
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public JobBankContext(DbContextOptions<JobBankContext> options, IHttpContextAccessor httpContextAccessor) 
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
            //UserName = (UserName == null) ? "Unknown" : UserName;
            UserName = UserName ?? "Unknown";
        }
        public JobBankContext(DbContextOptions<JobBankContext> options)
            : base(options)
        {
            UserName = "SeedData";
        }

        public DbSet<Occupation> Occupations { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Posting> Postings { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ApplicantSkill> ApplicantSkills { get; set; }
        public DbSet<RetrainingProgram> RetrainingPrograms { get; set; }
        public DbSet<SkillPostion> SkillPostions { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<PostingDocument> PostingDocuments { get; set; }
        public DbSet<ApplicantPhoto> ApplicantPhotos { get; set; }
        public DbSet<UploadedPhoto> UploadedPhotos { get; set; }
        public DbSet<ApplicantDocument> ApplicantDocuments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Set a default schema
            modelBuilder.HasDefaultSchema("JB");

            //Add a unique constraint to the Position Name property
            modelBuilder.Entity<Position>()
                .HasIndex(p => p.Name)
                .IsUnique();
            modelBuilder.Entity<Posting>()
                .HasIndex(post => new { post.PositionID, post.ClosingDate })
                .IsUnique();

            //Add a unique constraint to the Email address in Aplicant

            modelBuilder.Entity<Applicant>()
                .HasIndex(app => app.Email)
                .IsUnique()
                .HasName("IX_Unique_Applicant_email");
            //Add a unique composite constraint of Posting ID and ApplicantID in Application

            modelBuilder.Entity<Application>()
                .HasIndex(app => new { app.PostingID, app.ApplicantID })
                .IsUnique()
                .HasName("IX_Unique_Applicatation");

            //Add a unique contraint for Name of Skill table

            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.Name)
                .IsUnique();

            //Add composite primary Key for ApplicantSkill table
            modelBuilder.Entity<ApplicantSkill>()
                .HasKey(a => new { a.SkillID, a.ApplicantID });

            //Add a unique constraint for name of RetrainingProgram table.
            modelBuilder.Entity<RetrainingProgram>()
                .HasIndex(r => r.Name)
                .IsUnique();

            //Add Composite primarey key for SkillPosition table
            modelBuilder.Entity<SkillPostion>()
                .HasKey(s => new { s.PositionID, s.SkillID });

            //Restriction foreign key relationships Cascade Delete
            modelBuilder.Entity<Occupation>()
                .HasMany<Position>(o => o.Positions)
                .WithOne(p => p.Occupation)
                .HasForeignKey(p => p.OccupationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Position>()
                .HasMany<Posting>(p => p.Postings)
                .WithOne(post => post.Position)
                .HasForeignKey(post => post.PositionID)
                .OnDelete(DeleteBehavior.Restrict);

            //Restriction foreign key for Application and Applicant
            modelBuilder.Entity<Posting>()
                .HasMany<Application>(p => p.Applications)
                .WithOne(app => app.Posting)
                .HasForeignKey(app => app.PostingID)
                .OnDelete(DeleteBehavior.Restrict);

            //Allow cascade delete from Applicant have Application
            //modelBuilder.Entity<Applicant>()
            //    .HasMany<Application>(app => app.Applications)
            //    .WithOne(app => app.Applicant)
            //    .HasForeignKey(app => app.ApplicantID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //Restrict deleting a Skill that has any ApplicantSkill belong to
            modelBuilder.Entity<Skill>()
                .HasMany<ApplicantSkill>(s => s.ApplicantSkills)
                .WithOne(app => app.Skill)
                .HasForeignKey(app => app.SkillID)
                .OnDelete(DeleteBehavior.Restrict);

            //Restric deleting a Skill that has any SkillPostion belong to
            modelBuilder.Entity<Skill>()
                .HasMany<SkillPostion>(s => s.SkillPostions)
                .WithOne(sk => sk.Skill)
                .HasForeignKey(sk => sk.SkillID)
                .OnDelete(DeleteBehavior.Restrict);

            //Restrict deleting a Retraining Program that has any Applicant belong to
            modelBuilder.Entity<RetrainingProgram>()
                .HasMany<Applicant>(r =>r.Applicants)
                .WithOne(app => app.RetrainingProgram)
                .HasForeignKey(app => app.RetrainingProgramID)
                .OnDelete(DeleteBehavior.Restrict);

            //Restrict deleting a Skill that has any AplicantSkill belong to
            //modelBuilder.Entity<ApplicantSkill>()
            //    .HasOne(a => a.Skill)
            //    .WithMany(s => s.ApplicantSkills)
            //    .HasForeignKey(a => a.SkillID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //Add this so you DO get Cascade Delete
            modelBuilder.Entity<UploadedPhoto>()
                .HasOne<PhotoContent>(p => p.PhotoContentFull)
                .WithOne(p => p.PhotoFull)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UploadedPhoto>()
                .HasOne<PhotoContent>(p => p.PhotoContentThumb)
                .WithOne(p => p.PhotoThumb)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }

    }
}
