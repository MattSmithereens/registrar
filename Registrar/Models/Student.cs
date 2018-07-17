using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Registrar.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }

        public Student(string Name, DateTime Time, int Id = 0)
        {
            this.Id = Id;
            this.Name = Name;
            this.Time = Time;
        }

        public void AddCourse(Course course)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO students_courses (student_id, course_id) VALUES (@StudentId, @CourseId);";

            MySqlParameter student_id = new MySqlParameter();
            student_id.ParameterName = "@StudentId";
            student_id.Value = Id;
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

        public List<Course> GetCourses()  // need to return list of students
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"Select courses.* FROM students
                                INNER JOIN students_courses ON (students.id = students_courses.student_id)
                                INNER JOIN courses ON (students_courses.course_id = courses.id)
                                WHERE student.id = @studentId;";

            MySqlParameter studentIdParameter = new MySqlParameter();
            studentIdParameter.ParameterName = "@studentId";
            studentIdParameter.Value = Id;
            cmd.Parameters.Add(studentIdParameter);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;

            List<Course> courses = new List<Course> { }; // for all entries in DB add a new student object

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                string courseNum = rdr.GetString(2);
                Course newCourse = new Course(name, courseNum, id);
                courses.Add(newCourse);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

            return courses;

        }

        public static Student Find(int id)
        {
            {
                MySqlConnection conn = DB.Connection();
                conn.Open();

                var cmd = conn.CreateCommand() as MySqlCommand;
                cmd.CommandText = @"SELECT * FROM students WHERE id = @thisId;";

                MySqlParameter thisId = new MySqlParameter();
                thisId.ParameterName = "@thisId";
                thisId.Value = id;
                cmd.Parameters.Add(thisId);

                var rdr = cmd.ExecuteReader() as MySqlDataReader;

                int studentId = 0;
                string studentName = "";
                DateTime enroll = DateTime.MinValue;

                while (rdr.Read())
                {
                    studentId = rdr.GetInt32(0);
                    studentName = rdr.GetString(1);
                    enroll = rdr.GetDateTime(2);

                }

                Student foundStudent = new Student(studentName, enroll, studentId);

                conn.Close();
                if (conn != null)
                {
                    conn.Dispose();
                }

                return foundStudent;
            }
        }

        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO students (name, enrollment) VALUES (@studentName, @enrollment);";

            MySqlParameter studentName = new MySqlParameter();
            studentName.ParameterName = "@studentName";
            studentName.Value = this.Name;
            cmd.Parameters.Add(studentName);

            MySqlParameter studentEnrollment = new MySqlParameter();
            studentEnrollment.ParameterName = "@enrollment";
            studentEnrollment.Value = this.Time;
            cmd.Parameters.Add(studentEnrollment);

            cmd.ExecuteNonQuery();
            Id = (int)cmd.LastInsertedId;

            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public static List<Student> GetAll()
        {
            List<Student> allStudents = new List<Student> { };
            MySqlConnection conn = DB.Connection();
            conn.Open();

            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM students;";

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                DateTime time = rdr.GetDateTime(2);

                Student newStudent = new Student(name, time, id);
                allStudents.Add(newStudent);
            }

            conn.Close();

            if (conn != null)
            {
                conn.Dispose();
            }

            return allStudents;
        }

        public void Delete()    // delete one student at a time
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM students WHERE id = @studentId; DELETE FROM students_courses WHERE student_id = @studentId;";

            MySqlParameter studentIdParameter = new MySqlParameter();
            studentIdParameter.ParameterName = "@studentId";
            studentIdParameter.Value = Id;
            cmd.Parameters.Add(studentIdParameter);

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
            cmd.CommandText = @"DELETE FROM students;";

            cmd.ExecuteNonQuery();

            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public override bool Equals(System.Object otherStudent)
        {
            if (!(otherStudent is Student))
            {
                return false;
            }
            else
            {
                Student newStudent = (Student)otherStudent;
                bool idEquality = (this.Id == newStudent.Id);
                bool studentNameEquality = (this.Name == newStudent.Name);

                return (idEquality && studentNameEquality);
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
