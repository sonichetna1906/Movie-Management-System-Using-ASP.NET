using assignment_1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace assignment_1.Controllers
{
	public class MovieCategoryController : Controller
	{
		string cs = ConfigurationManager
			.ConnectionStrings["MovieDBConnection"]
			.ConnectionString;


		// DISPLAY
		public ActionResult Index()
		{
			List<MovieCategory> categories =
				new List<MovieCategory>();

			using (SqlConnection con =	new SqlConnection(cs))
			{
				string query =
					"SELECT * FROM Tbl_Movie_Category";

				SqlCommand cmd =
					new SqlCommand(query, con);

				con.Open();

				SqlDataReader dr =
					cmd.ExecuteReader();

				while (dr.Read())
				{
					MovieCategory category =
						new MovieCategory();

					category.Cat_ID =
						Convert.ToInt32(dr["Cat_ID"]);

					category.Cat_Type =
						dr["Cat_Type"].ToString();

					categories.Add(category);
				}
			}

			return View(categories);
		}


		// INSERT - GET
		public ActionResult Create()
		{
			return View();
		}


		// INSERT - POST
		[HttpPost]
		public ActionResult Create(MovieCategory category)
		{
			if (ModelState.IsValid)
			{
				using (SqlConnection con =
					new SqlConnection(cs))
				{
					string query =
						"INSERT INTO Tbl_Movie_Category " +
						"(Cat_Type) VALUES (@Cat_Type)";

					SqlCommand cmd =
						new SqlCommand(query, con);

					cmd.Parameters.AddWithValue(
						"@Cat_Type",
						category.Cat_Type);

					con.Open();

					cmd.ExecuteNonQuery();
				}

				return RedirectToAction("Index");
			}

			return View(category);
		}


		// UPDATE - GET
		public ActionResult Edit(int id)
		{
			MovieCategory category =
				new MovieCategory();

			using (SqlConnection con =
				new SqlConnection(cs))
			{
				string query =
					"SELECT * FROM Tbl_Movie_Category " +
					"WHERE Cat_ID=@Cat_ID";

				SqlCommand cmd =
					new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@Cat_ID", id);

				con.Open();

				SqlDataReader dr =
					cmd.ExecuteReader();

				if (dr.Read())
				{
					category.Cat_ID =
						Convert.ToInt32(dr["Cat_ID"]);

					category.Cat_Type =
						dr["Cat_Type"].ToString();
				}
			}

			return View(category);
		}


		// UPDATE - POST
		[HttpPost]
		public ActionResult Edit(MovieCategory category)
		{
			if (ModelState.IsValid)
			{
				using (SqlConnection con =
					new SqlConnection(cs))
				{
					string query =
						"UPDATE Tbl_Movie_Category " +
						"SET Cat_Type=@Cat_Type " +
						"WHERE Cat_ID=@Cat_ID";

					SqlCommand cmd =
						new SqlCommand(query, con);

					cmd.Parameters.AddWithValue(
						"@Cat_ID",
						category.Cat_ID);

					cmd.Parameters.AddWithValue(
						"@Cat_Type",
						category.Cat_Type);

					con.Open();

					cmd.ExecuteNonQuery();
				}

				return RedirectToAction("Index");
			}

			return View(category);
		}


		// DELETE
		public ActionResult Delete(int id)
		{
			using (SqlConnection con =
				new SqlConnection(cs))
			{
				string query =
					"DELETE FROM Tbl_Movie_Category " +
					"WHERE Cat_ID=@Cat_ID";

				SqlCommand cmd =
					new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@Cat_ID", id);

				con.Open();

				cmd.ExecuteNonQuery();
			}

			return RedirectToAction("Index");
		}
	}
}