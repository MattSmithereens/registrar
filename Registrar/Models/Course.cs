using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Registrar.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CourseNum { get; set; }

        public Course(string Name, string CourseNum, int Id = 0)
        {
            this.Id = Id;
            this.Name = Name;
            this.CourseNum = CourseNum;
        }

        public void AddStudent(Student student)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO students_courses (student_id, course_id) VALUES (@StudentId, @CourseId);";

            MySqlParameter student_id = new MySqlParameter();
            student_id.ParameterName = "@StudentId";
            student_id.Value = student.Id;
            cmd.Parameters.Add(student_id);

            MySqlParameter course_id = new MySqlParameter();
            course_id.ParameterName = "@CourseId";
            course_id.Value = Id;
            cmd.Parameters.Add(course_id);

            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public List<Student> GetStudents()  // need to return list of students
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"Select students.* FROM courses
                                JOIN students_courses ON (courses.id = students_courses.course_id)
                                JOIN students ON (students_courses.student_id = students.id)
                                WHERE courses.id = @courseId;";

            MySqlParameter courseIDParameter = new MySqlParameter();
            courseIDParameter.ParameterName = "@courseId";
            courseIDParameter.Value = Id;
            cmd.Parameters.Add(courseIDParameter);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;

            List<Student> students = new List<Student> { }; // for all entries in DB add a new student object

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                DateTime enroll = rdr.GetDateTime(2);
                Student newStudent = new Student(name, enroll, id);
                students.Add(newStudent);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

            return students;
        
        }

        public static Course Find(int id)
        {
            {
                MySqlConnection conn = DB.Connection();
                conn.Open();

                var cmd = conn.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"SELECT * FROM courses WHERE id = @thisId;";

                MySqlParameter thisId = new MySqlParameter();
                thisId.ParameterName = "@thisId";
                thisId.Value = id;
                cmd.Parameters.Add(thisId);

                var rdr = cmd.ExecuteReader() as MySqlDataReader;

                int coursesId = 0;
                string coursesName = "";
                string courseNum = "";

                while (rdr.Read())
                {
                    coursesId = rdr.GetInt32(0);
                    coursesName = rdr.GetString(1);
                    courseNum = rdr.GetString(2);
                }

                Course foundCourse = new Course(coursesName, courseNum, coursesId);

                conn.Close();
                if (conn != null)
                {
                    conn.Dispose();
                }

                return foundCourse;
            }
        }

        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO courses (name, course_code) VALUES (@courseName, @courseCode);";

            MySqlParameter courseName = new MySqlParameter();
            courseName.ParameterName = "@courseName";
            courseName.Value = this.Name;
            cmd.Parameters.Add(courseName);

            MySqlParameter courseCode = new MySqlParameter();
            courseCode.ParameterName = "@courseCode";
            courseCode.Value = this.CourseNum;
            cmd.Parameters.Add(courseCode);

            cmd.ExecuteNonQuery();
            Id = (int)cmd.LastInsertedId;

            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public static List<Course> GetAll()
        {
            List<Course> allCourses = new List<Course> { };
            MySqlConnection conn = DB.Connection();
            conn.Open();

            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM courses;";

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                string courseNum = rdr.GetString(2);

                Course newCourse = new Course(name, courseNum, id);
                allCourses.Add(newCourse);
            }

            conn.Close();

            if (conn != null)
            {
                conn.Dispose();
            }

            return allCourses;
        }

        public void Delete()    // delete one course at a time
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM courses WHERE id = @CourseId; DELETE FROM students_courses WHERE course_id = @CourseId;";

            MySqlParameter cityIdParameter = new MySqlParameter();
            cityIdParameter.ParameterName = "@CourseId";
            cityIdParameter.Value = this.Id;
            cmd.Parameters.Add(cityIdParameter);

            cmd.ExecuteNonQuery();
            if (conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM courses;";

            cmd.ExecuteNonQuery();

            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public override bool Equals(System.Object otherCourse)
        {
            if (!(otherCourse is Course))
            {
                return false;
            }
            else
            {
                Course newCourse = (Course)otherCourse;
                bool idEquality = (this.Id == newCourse.Id);
                bool courseNameEquality = (this.Name == newCourse.Name);

                return (idEquality && courseNameEquality);
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
