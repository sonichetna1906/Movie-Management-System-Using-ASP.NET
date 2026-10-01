using assignment_1.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace assignment_1.Controllers
{
	public class UserController : Controller
	{
		string cs = ConfigurationManager
			.ConnectionStrings["MovieDBConnection"]
			.ConnectionString;


		// =========================
		// REGISTER - GET
		// =========================
		public ActionResult Create()
		{
			return View();
		}


		// =========================
		// REGISTER - POST
		// =========================
		[HttpPost]
		public ActionResult Create(User user)
		{
			if (ModelState.IsValid)
			{
				using (SqlConnection con = new SqlConnection(cs))
				{
					string query = @"INSERT INTO Tbl_User
                                    (User_Name, Email_ID, User_password, City, PhoneNo)
                                    VALUES
                                    (@User_Name, @Email_ID, @User_password, @City, @PhoneNo)";

					SqlCommand cmd = new SqlCommand(query, con);

					cmd.Parameters.AddWithValue(
						"@User_Name", user.User_Name);

					cmd.Parameters.AddWithValue(
						"@Email_ID", user.Email_ID);

					cmd.Parameters.AddWithValue(
						"@User_password", user.User_password);

					cmd.Parameters.AddWithValue(
						"@City", user.City);

					cmd.Parameters.AddWithValue(
						"@PhoneNo", user.PhoneNo);

					con.Open();
					cmd.ExecuteNonQuery();
				}

				return RedirectToAction("Login");
			}

			return View(user);
		}


		// =========================
		// LOGIN - GET
		// =========================
		public ActionResult Login()
		{
			return View();
		}


		// =========================
		// LOGIN - POST
		// =========================
		[HttpPost]
		public ActionResult Login(string Email_ID, string User_password)
		{
			using (SqlConnection con = new SqlConnection(cs))
			{
				string query = @"SELECT User_ID, User_Name, Email_ID
                                 FROM Tbl_User
                                 WHERE Email_ID = @Email_ID
                                 AND User_password = @User_password";

				SqlCommand cmd = new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@Email_ID", Email_ID);

				cmd.Parameters.AddWithValue(
					"@User_password", User_password);

				con.Open();

				SqlDataReader dr = cmd.ExecuteReader();

				if (dr.Read())
				{
					Session["User_ID"] =
						Convert.ToInt32(dr["User_ID"]);

					Session["User_Name"] =
						dr["User_Name"].ToString();

					return RedirectToAction("Profile");
				}
			}

			ViewBag.Message = "Invalid Email or Password";

			return View();
		}


		// =========================
		// PROFILE - GET
		// =========================
		public ActionResult Profile()
		{
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login");
			}

			User user = new User();

			using (SqlConnection con = new SqlConnection(cs))
			{
				string query = @"SELECT *
                                 FROM Tbl_User
                                 WHERE User_ID = @User_ID";

				SqlCommand cmd = new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@User_ID",
					Session["User_ID"]);

				con.Open();

				SqlDataReader dr = cmd.ExecuteReader();

				if (dr.Read())
				{
					user.User_ID =
						Convert.ToInt32(dr["User_ID"]);

					user.User_Name =
						dr["User_Name"].ToString();

					user.Email_ID =
						dr["Email_ID"].ToString();

					user.User_password =
						dr["User_password"].ToString();

					user.City =
						dr["City"].ToString();

					user.PhoneNo =
						dr["PhoneNo"].ToString();
				}
			}

			return View(user);
		}


		// =========================
		// PROFILE UPDATE - POST
		// =========================
		[HttpPost]
		public ActionResult Profile(User user)
		{
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login");
			}

			if (ModelState.IsValid)
			{
				using (SqlConnection con = new SqlConnection(cs))
				{
					string query = @"UPDATE Tbl_User
                                     SET User_Name = @User_Name,
                                         Email_ID = @Email_ID,
                                         User_password = @User_password,
                                         City = @City,
                                         PhoneNo = @PhoneNo
                                     WHERE User_ID = @User_ID";

					SqlCommand cmd = new SqlCommand(query, con);

					cmd.Parameters.AddWithValue(
						"@User_ID",
						Session["User_ID"]);

					cmd.Parameters.AddWithValue(
						"@User_Name",
						user.User_Name);

					cmd.Parameters.AddWithValue(
						"@Email_ID",
						user.Email_ID);

					cmd.Parameters.AddWithValue(
						"@User_password",
						user.User_password);

					cmd.Parameters.AddWithValue(
						"@City",
						user.City);

					cmd.Parameters.AddWithValue(
						"@PhoneNo",
						user.PhoneNo);

					con.Open();

					cmd.ExecuteNonQuery();
				}

				return RedirectToAction("Profile");
			}

			return View(user);
		}


		// =========================
		// LOGOUT
		// =========================
		public ActionResult Logout()
		{
			Session.Clear();

			return RedirectToAction("Login");
		}
	}
}