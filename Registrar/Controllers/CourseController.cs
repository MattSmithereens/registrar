using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Registrar.Models;

namespace Registrar.Controllers
{
    public class CourseController : Controller
    {
        [HttpGet("new-course")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost("new-course")]
        public ActionResult CreatePost()
        {
            string name = Request.Form["name"];
            Course newCourse = new Course(name);
            newCourse.Save();

            return RedirectToAction("ViewAll");
        }

        [HttpGet("view-courses")]
        public ActionResult ViewAll()
        {
            List<Course> allCourses = Course.GetAll();
            return View(allCourses);
        }

        [HttpGet("course/{id}/details")]
        public ActionResult Details(int id)
        {
            Course newCourse = Course.Find(id);
            return View(newCourse);
        }

        [HttpGet("course/{id}/update")]
        public ActionResult Edit(int id)
        {
            Course newCourse = Course.Find(id);
            return View(newCourse);
        }

        //[HttpPost("course/{id}/update")]
        //public ActionResult EditDetails(int id)
        //{
        //    string newName = Request.Form["newName"];
        //    Course newCourse = Course.Find(id);
        //    newCourse.Edit(newName);
        //    return RedirectToAction("ViewAll");
        //}

        [HttpPost("course/{id}/delete")]
        public ActionResult Delete(int id)
        {
            Course newCourse = Course.Find(id);
            newCourse.Delete();
            return RedirectToAction("ViewAll");
        }
    }
}