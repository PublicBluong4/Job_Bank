using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bluong4_Job_Bank.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


//Name: Ba Binh Luong,
//Done for: Project 1 Part A and B
//Last Modified: 2020 September 28 6:00pm

namespace Bluong4_Job_Bank.Data
{
    public static class JBSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new JobBankContext(serviceProvider.GetRequiredService<DbContextOptions<JobBankContext>>()))
            {
                //Add Occupation to database
                if(!context.Occupations.Any())
                {
                    context.Occupations.AddRange(
                        new Occupation
                        {
                            Title = "Software Engineer and Designer"
                        },
                        new Occupation
                        {
                            Title = "Electrician"
                        },
                        new Occupation
                        {
                            Title = "Business Management"
                        }

                        );
                    context.SaveChanges();
                }

                //Add Position to database
                if (!context.Positions.Any())
                {
                    context.Positions.AddRange(
                        new Position
                        {
                            Name = "Web Developer",
                            Description = "Build website and services relate to website",
                            Salary = 70000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Software Engineer and Designer").ID
                        },
                        new Position
                        {
                            Name = "BackEnd Developer",
                            Description = "Has knownledge in C# and Python language",
                            Salary = 100000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Software Engineer and Designer").ID
                        },
                        new Position
                        {
                            Name = "FrontEnd Developer",
                            Description = "Build UI, has knownledge in HTML, CSS, Bootstrap, JQuery",
                            Salary = 70000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Software Engineer and Designer").ID
                        },


                        new Position
                        {
                            Name = "Industry Electrician Level 1",
                            Description = "Barchelor degree in Electrician or related field",
                            Salary = 90000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Electrician").ID
                        },
                        new Position
                        {
                            Name = "Home Electrician",
                            Description = "Barchelor degree in Electrician or related field",
                            Salary = 80000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Electrician").ID
                        },
                        new Position
                        {
                            Name = "Customer support for electric equipment shop",
                            Description = "Barchelor degree in Electrician or related field",
                            Salary = 60000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Electrician").ID
                        },


                        new Position
                        {
                            Name = "Project Manager",
                            Description = "Five years experience in Project Management",
                            Salary = 120000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Business Management").ID
                        },
                        new Position
                        {
                            Name = "Marketing Manager",
                            Description = "Five years experience in Sale and Project Management",
                            Salary = 130000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Business Management").ID
                        },
                        new Position
                        {
                            Name = "Sale Manager",
                            Description = "Bachelor Degree in Business Management",
                            Salary = 120000.00m,
                            OccupationID = context.Occupations.FirstOrDefault(o => o.Title == "Business Management").ID
                        }

                        );
                    context.SaveChanges();
                }

                //Add Posting to database
                if (!context.Postings.Any())
                {
                    context.Postings.AddRange(
                        new Posting
                        {
                            NumberOpen = 1,
                            ClosingDate = DateTime.Parse("2020-12-01"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "Web Developer").ID
                        },
                        new Posting
                        {
                            NumberOpen = 2,
                            ClosingDate = DateTime.Parse("2020-12-02"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "BackEnd Developer").ID
                        },
                        new Posting
                        {
                            NumberOpen = 3,
                            ClosingDate = DateTime.Parse("2020-12-03"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "FrontEnd Developer").ID
                        },
                        new Posting
                        {
                            NumberOpen = 4,
                            ClosingDate = DateTime.Parse("2020-12-04"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "Industry Electrician Level 1").ID
                        },
                        new Posting
                        {
                            NumberOpen = 5,
                            ClosingDate = DateTime.Parse("2020-12-05"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "Home Electrician").ID
                        },
                        new Posting
                        {
                            NumberOpen = 6,
                            ClosingDate = DateTime.Parse("2020-12-06"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "Customer support for electric equipment shop").ID
                        },
                        new Posting
                        {
                            NumberOpen = 7,
                            ClosingDate = DateTime.Parse("2020-12-07"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "Project Manager").ID
                        },
                        new Posting
                        {
                            NumberOpen = 8,
                            ClosingDate = DateTime.Parse("2020-12-08"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "Marketing Manager").ID
                        },
                        new Posting
                        {
                            NumberOpen = 9,
                            ClosingDate = DateTime.Parse("2020-12-09"),
                            StartDate = DateTime.Now,
                            PositionID = context.Positions.FirstOrDefault(p => p.Name == "Sale Manager").ID
                        }

                        );
                    context.SaveChanges();
                }
                //Add Retraining Program to database
                if(!context.RetrainingPrograms.Any())
                {
                    context.RetrainingPrograms.AddRange(
                        new RetrainingProgram
                        {
                            Name = "First Aid"
                        },
                        new RetrainingProgram
                        {
                            Name = "CPR"
                        },
                        new RetrainingProgram
                        {
                            Name = "Computer Programming"
                        },
                        new RetrainingProgram
                        {
                            Name = "Life Skills"
                        },
                        new RetrainingProgram
                        {
                            Name = "Machine Learning"
                        },
                        new RetrainingProgram
                        {
                            Name = "AI"
                        },
                        new RetrainingProgram
                        {
                            Name = "Algorithm"
                        }
                        );
                    context.SaveChanges();
                }

                //Add Applicant to database

                if (!context.Applicants.Any())
                {
                    context.Applicants.AddRange(
                        new Applicant
                        {
                            FirstName = "Binh",
                            MiddleName = "Ba",
                            LastName = "Luong",
                            SIN = 123456789,
                            Phone = 1234567890,
                            Email = "bluong4@ncstudents.niagaracollege.ca"
                        },
                        new Applicant
                        {
                            FirstName = "Binh1",
                            MiddleName = "Ba",
                            LastName = "Luong1",
                            SIN = 123456789,
                            Phone = 1234567890,
                            Email = "bluong41@ncstudents.niagaracollege.ca"
                        }
                        ,
                        new Applicant
                        {
                            FirstName = "Binh4",
                            MiddleName = "Ba",
                            LastName = "Luong4",
                            SIN = 122222222,
                            Phone = 6666666666,
                            Email = "bluong42@ncstudents.niagaracollege.ca"
                        }
                        ,
                        new Applicant
                        {
                            FirstName = "Binh6",
                            MiddleName = "Ba",
                            LastName = "Luong6",
                            SIN = 166666666,
                            Phone = 6666666666,
                            Email = "bluong46@ncstudents.niagaracollege.ca",
                            RetrainingProgramID = context.RetrainingPrograms.FirstOrDefault(r => r.Name == "Computer Programming").ID

                        }
                         ,
                        new Applicant
                        {
                            FirstName = "Binh8",
                            MiddleName = "Ba",
                            LastName = "Luong8",
                            SIN = 166666666,
                            Phone = 6666666666,
                            Email = "bluong48@ncstudents.niagaracollege.ca",
                            RetrainingProgramID = context.RetrainingPrograms.FirstOrDefault(r => r.Name == "Machine Learning").ID

                        },
                        new Applicant
                        {
                            FirstName = "Peter",
                            LastName = "Pan",
                            SIN = 166666666,
                            Phone = 6666666666,
                            Email = "ppeter@ncstudents.niagaracollege.ca",
                            RetrainingProgramID = context.RetrainingPrograms.FirstOrDefault(r => r.Name == "Machine Learning").ID

                        }
                        ,
                        new Applicant
                        {
                            FirstName = "Jacob",
                            LastName = "Taylor",
                            SIN = 166666666,
                            Phone = 6666666666,
                            Email = "jtailor@ncstudents.niagaracollege.ca",
                            RetrainingProgramID = context.RetrainingPrograms.FirstOrDefault(r => r.Name == "Machine Learning").ID

                        },
                        new Applicant
                        {
                            FirstName = "Mike",
                            LastName = "Mickey",
                            SIN = 166666666,
                            Phone = 6666666666,
                            Email = "mmike@ncstudents.niagaracollege.ca",
                            RetrainingProgramID = context.RetrainingPrograms.FirstOrDefault(r => r.Name == "Machine Learning").ID

                        }
                        ,
                        new Applicant
                        {
                            FirstName = "Mood",
                            LastName = "Mood",
                            SIN = 166666666,
                            Phone = 6666666666,
                            Email = "mmood@ncstudents.niagaracollege.ca",
                            RetrainingProgramID = context.RetrainingPrograms.FirstOrDefault(r => r.Name == "Machine Learning").ID

                        }

                        ); ;
                    context.SaveChanges();
                }

                //Add Application to database

                if (!context.Applications.Any())
                {
                    context.Applications.AddRange(
                        new Application
                        {
                            Comments = "Urgent, Need work from home. Require internet connection at least 50Mbps.",
                            PostingID = 1,
                            ApplicantID = context.Applicants.FirstOrDefault(app => app.LastName == "Luong").ID
                        },
                        new Application
                        {
                            Comments = "Not Urgent, Need work from home. Require internet connection at least 50Mbps.",
                            PostingID = 2,
                            ApplicantID = context.Applicants.FirstOrDefault(app => app.LastName == "Luong1").ID
                        }
                        ,
                        new Application
                        {
                            Comments = "Start work at 29 December 2020. Require internet connection at least 50Mbps.",
                            PostingID = 2,
                            ApplicantID = context.Applicants.FirstOrDefault(app => app.LastName == "Luong4").ID
                        }

                        );
                    context.SaveChanges();
                }
                if (!context.Skills.Any())
                {
                    context.Skills.AddRange(
                        new Skill
                        {
                            Name = "Communication Skills."
                        },
                        new Skill
                        {
                            Name = "Organizational Skills."
                        },
                        new Skill
                        {
                            Name = "Writing."
                        },
                        new Skill
                        {
                            Name = "Customer Service."
                        },
                        new Skill
                        {
                            Name = "Microsoft Excel."
                        },
                        new Skill
                        {
                            Name = "Problem Solving."
                        },
                        new Skill
                        {
                            Name = "Planning."
                        },
                        new Skill
                        {
                            Name = "Microsoft Office."
                        },
                        new Skill
                        {
                            Name = "Research."
                        },
                        new Skill
                        {
                            Name = "Detail-Oriented."
                        },
                        new Skill
                        {
                            Name = "Building effective Relationships."
                        },
                        new Skill
                        {
                            Name = "Project Management."
                        },
                        new Skill
                        {
                            Name = "Computer Skills."
                        },
                        new Skill
                        {
                            Name = "Quanlity Assurance and Control."
                        },
                        new Skill
                        {
                            Name = "Troubleshooting."
                        });
                    context.SaveChanges();
                }
                
            }
        }
    }
}
