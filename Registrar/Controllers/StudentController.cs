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

            Course newCourse = Course.Find(courseId);
            Student newStudent = new Student(name, date);
            newStudent.Save();
            newStudent.AddCourse(newCourse);

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

        [HttpGet("student/{id}/update")]
        public ActionResult Edit(int id)
        {
            Student newStudent = Student.Find(id);
            return View(newStudent);
        }

        [HttpPost("student/{id}/update")]
        public ActionResult EditDetails(int id)
        {
            string newName = Request.Form["newName"];
            int courseId = int.Parse(Request.Form["newCourse"]);
            Course newCourse = Course.Find(courseId);
            Student newStudent = Student.Find(id);
            newStudent.Edit(newName);
            newStudent.AddCourse(newCourse);
            return RedirectToAction("ViewAll");
        }

        [HttpPost("course/{id}/delete")]
        public ActionResult Delete(int id)
        {
            Course newCourse = Course.Find(id);
            newCourse.Delete();
            return RedirectToAction("ViewAll");
        }
    }
}