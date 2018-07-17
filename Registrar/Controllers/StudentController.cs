using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Registrar.Models;

namespace Registrar.Controllers
{
    public class StudentController : Controller
    {
        [HttpGet("new-student")]
        public ActionResult Create()
        {
            return View(Course.GetAll());
        }

        [HttpPost("new-student")]
        public ActionResult CreatePost()
        {
            string name = Request.Form["name"];
            string enrollDate = Request.Form["enrollment"];
            int courseId = int.Parse(Request.Form["course"]);

            DateTime date = Convert.ToDateTime(enrollDate);

            Student newStudent = new Student(name, date);
            newStudent.Save();
            newStudent.AddCourse(Course.Find(courseId));

            return RedirectToAction("ViewAll");
        }

        [HttpGet("view-students")]
        public ActionResult ViewAll()
        {
            List<Student> allStudents = Student.GetAll();
            return View(allStudents);
        }

        [HttpGet("student/{id}/details")]
        public ActionResult Details(int id)
        {
            Student newStudent = Student.Find(id);
            return View(newStudent);
        }
    }
}