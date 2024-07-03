using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Quiz_management_system.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Quiz_management_system.Controllers
{
    public class loginController : Controller
    {
        SqlConnection con = new SqlConnection();


        [HttpGet]
        public ActionResult Login()
        { 
            return View();
        }
        void connectionString()
        {
            con.ConnectionString = "Data Source=DESKTOP-ABS14OI;Initial Catalog=databasequiz5;Integrated Security=True";
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(loginModel lm)
        {

            connectionString();
            loginModel lmm = new loginModel();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string query = "select * from registration where user_name = '" + lm.user_name + "' and password = '" + lm.password + "' ";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.Fill(dt);
            }


            if (dt.Rows.Count == 1)
            {

                int register_id = Convert.ToInt32(dt.Rows[0][0].ToString());
                string user_name = dt.Rows[0][1].ToString();


                HttpContext.Session.SetString("register_id", register_id.ToString());
                HttpContext.Session.SetString("user_name", user_name.ToString());

                lmm.user_name = dt.Rows[0][1].ToString();
                lmm.password = dt.Rows[0][4].ToString();
                if (lm.user_select == "Admin")
                {

                    con.Close();
                    return RedirectToAction("Admin_dashboard", "login");
                }

                else if (lm.user_select == "Student")
                {
                    con.Close();
                    return RedirectToAction("s_dashboard", "Student");
                }
                else
                {
                    return null;
                }

            }
            else
            {
                con.Close();
                ViewData["Message"] = "login failed ! ";
            }
            return View("Login");
        }

        public ActionResult sign()
        {
            return View();
        }
        [HttpPost]
        public ActionResult sign(signModel sm)
        {
            connectionString();
            SqlConnection sqlcon = new SqlConnection(con.ConnectionString);
            string query = "insert into registration(user_name,last_name,email,password)values(@user_name, @last_name, @email,@password)";
            SqlCommand cmd = new SqlCommand(query,sqlcon);
            sqlcon.Open();
            cmd.Parameters.AddWithValue("@user_name", sm.user_name);
            cmd.Parameters.AddWithValue("@last_name", sm.last_name);
            cmd.Parameters.AddWithValue("@email", sm.email);
            cmd.Parameters.AddWithValue("@password", sm.password);
            cmd.ExecuteNonQuery();
            sqlcon.Close();
            ViewData["Message"] = "User record  Saved Successfully !";
            return View();
        }
        public ActionResult Admin_dashboard()
        {
            string register_id_is = HttpContext.Session.GetString("register_id");
            return View();
        }
        [HttpGet]
        public ActionResult viewcategory()
        {
            connectionString();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM courses", sqlcon);
                sqlDa.Fill(dt);
            }
            return View(dt);
        }


        [HttpGet]
        public ActionResult v_Question()     
        {
            connectionString();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                sqlcon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM v_question", sqlcon);
                sqlDa.Fill(dt);
            }
            return View(dt);
        }
        public ActionResult Edit_viewquestion(int id)
        {
            connectionString();
            view_questionModel view_question = new view_questionModel();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string query = "SELECT * FROM v_question where question_id = @question_id";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.SelectCommand.Parameters.AddWithValue("@question_id", id);
                sda.Fill(dt);
            }

            Console.WriteLine(dt.Rows.Count);

            if (dt.Rows.Count == 1)
            {
                view_question.question_id = Convert.ToInt32(dt.Rows[0][0].ToString());
                view_question.course_id = Convert.ToInt32(dt.Rows[0][1].ToString());
                view_question.question = dt.Rows[0][2].ToString();
                view_question.option1 = dt.Rows[0][3].ToString();
                view_question.option2 = dt.Rows[0][4].ToString();
                view_question.option3 = dt.Rows[0][5].ToString();
                view_question.option4 = dt.Rows[0][6].ToString();
                view_question.answer = dt.Rows[0][7].ToString();

                return View(view_question);
            }

            else
            {
                return RedirectToAction("v_Question");
            }

            con.Close();
        }
        [HttpPost]
        public ActionResult Edit_viewquestion(view_questionModel vqm)
        {
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                connectionString();
                con.Open();
                SqlCommand cmd = new SqlCommand("editviewquestionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@question_id", vqm.question_id);
                cmd.Parameters.AddWithValue("@question", vqm.question);
                cmd.Parameters.AddWithValue("@option1", vqm.option1);
                cmd.Parameters.AddWithValue("@option2", vqm.option2);
                cmd.Parameters.AddWithValue("@option3", vqm.option3);
                cmd.Parameters.AddWithValue("@option4", vqm.option4);
                cmd.Parameters.AddWithValue("@answer", vqm.answer);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("v_Question");
        }
        public ActionResult Deleteviewquestion(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                connectionString();
                con.Open();
                string query = "Delete from question where question_id = @question_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@question_id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("v_Question");
        }
        [HttpPost]
        public ActionResult Deletequestion(int id, IFormCollection collection)
        {
            return View();
        }

        public ActionResult Add_Question()
        {
            return View(selectsubject());
        }
        public ActionResult Add_Question1()
        {
            return View(selectsubject());
        }
        public  List<Add_QuestionModel> selectsubject()
        {
            List<Add_QuestionModel> selectsubobj = new List<Add_QuestionModel>();
            string connection = "Data Source=DESKTOP-ABS14OI;Initial Catalog=databasequiz5;Integrated Security=True";
            using (SqlConnection sqlcon = new SqlConnection(connection))
            {
                using (SqlCommand sqlcomm = new SqlCommand("Select * from courses"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        sqlcomm.Connection = sqlcon;
                        sqlcon.Open();
                        sda.SelectCommand = sqlcomm;
                        SqlDataReader sdr = sqlcomm.ExecuteReader();
                        while (sdr.Read())
                        {
                            Add_QuestionModel add_subject = new Add_QuestionModel();
                            add_subject.course_id = (int)sdr["course_id"];
                            add_subject.course_name = sdr["course_name"].ToString();
                            selectsubobj.Add(add_subject);

                        }
                    }
                    return selectsubobj;
                }
            }
        }
        
        
        [HttpPost]
        public ActionResult Add_Question(Add_QuestionModel aqm)
        {
            connectionString();
            SqlConnection sqlcon = new SqlConnection(con.ConnectionString);
            string query = "insert into question(course_id,question,option1,option2,option3,option4,answer)values(@course_id, @question,@option1,@option2,@option3,@option4,@answer)";
            SqlCommand cmd = new SqlCommand(query, sqlcon);
            sqlcon.Open();
            cmd.Parameters.AddWithValue("@course_id", aqm.course_id);
            cmd.Parameters.AddWithValue("@question", aqm.question);
            cmd.Parameters.AddWithValue("@option1", aqm.option1);
            cmd.Parameters.AddWithValue("@option2", aqm.option2);
            cmd.Parameters.AddWithValue("@option3", aqm.option3);
            cmd.Parameters.AddWithValue("@option4", aqm.option4);
            cmd.Parameters.AddWithValue("@answer", aqm.answer);
            cmd.ExecuteNonQuery();
            sqlcon.Close();
            return RedirectToAction("Add_Question");
        }

        [HttpPost]
        public ActionResult Add_Question1(Add_QuestionModel aqm)
        {
            if(aqm.course_id == 23)
            {
                Website1.run("23");

            }
            else if (aqm.course_id == 24)
            {
                Website2.run("24");

            }
            else if (aqm.course_id == 25)
            {
                Website3.run("25");

            }
            else if (aqm.course_id == 26)
            {
                Website4.run("26");

            }
            else if (aqm.course_id == 27)
            {
                Website5.run("27");

            }

            return RedirectToAction("Add_Question");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            connectionString();
            View_categoryModel category = new View_categoryModel();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                con.Open();
                string query = "SELECT * FROM category where category_id = @category_id";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                sda.SelectCommand.Parameters.AddWithValue("@category_id", id);
                sda.Fill(dt);
            }

            Console.WriteLine(dt.Rows.Count);

            if (dt.Rows.Count == 1)
            {
                category.category_id = Convert.ToInt32(dt.Rows[0][0].ToString());
                category.category_name = dt.Rows[0][1].ToString();
                return View(category);
            }

            else
            {
                return RedirectToAction("viewcategory");
            }

            con.Close();

        }
        [HttpPost]
        public ActionResult Edit(View_categoryModel category)
        {
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                connectionString();
                con.Open();
                string query = "Update category set category_name = @category_name WHERE category_id = @category_id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@category_id", category.category_id);
                cmd.Parameters.AddWithValue("@category_name", category.category_name);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("viewcategory");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(con.ConnectionString))
            {
                connectionString();
                con.Open();
                SqlCommand cmd = new SqlCommand("deletecategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@category_id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("viewcategory");
        }
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            return View();
        }


    }
}
