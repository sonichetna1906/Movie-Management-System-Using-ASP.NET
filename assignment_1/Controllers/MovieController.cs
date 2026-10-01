using assignment_1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace assignment_1.Controllers
{
	public class MovieController : Controller
	{
		string cs = ConfigurationManager
			.ConnectionStrings["MovieDBConnection"].ConnectionString;


		// STEP 7.1
		// Display all movies
		public ActionResult Index()
		{
			List<Movie> movies = new List<Movie>();

			using (SqlConnection con = new SqlConnection(cs))
			{
				string query = @"SELECT m.Movie_ID,
                                        m.Movie_name,
                                        m.Release_Date,
                                        m.Cat_ID,
                                        m.rate,
                                        c.Cat_Type
                                 FROM Tbl_Movie m
                                 INNER JOIN Tbl_Movie_Category c
                                 ON m.Cat_ID = c.Cat_ID";

				SqlCommand cmd = new SqlCommand(query, con);

				con.Open();

				SqlDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					Movie movie = new Movie();

					movie.Movie_ID = Convert.ToInt32(dr["Movie_ID"]);
					movie.Movie_name = dr["Movie_name"].ToString();
					movie.Release_Date =
						Convert.ToDateTime(dr["Release_Date"]);
					movie.Cat_ID =
						Convert.ToInt32(dr["Cat_ID"]);
					movie.rate =
						Convert.ToDecimal(dr["rate"]);
					movie.Cat_Type =
						dr["Cat_Type"].ToString();

					movies.Add(movie);
				}
			}

			return View(movies);
		}


		// STEP 7.2
		// Get categories for dropdown
		private List<MovieCategory> GetCategories()
		{
			List<MovieCategory> categories =
				new List<MovieCategory>();

			using (SqlConnection con =
				   new SqlConnection(cs))
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

			return categories;
		}


		// STEP 7.3
		// Add Movie page
		public ActionResult Create()
		{
			ViewBag.Categories = GetCategories();

			return View();
		}


		// STEP 7.4
		// Insert Movie
		[HttpPost]
		public ActionResult Create(Movie movie)
		{
			if (ModelState.IsValid)
			{
				using (SqlConnection con =
					   new SqlConnection(cs))
				{
					string query = @"INSERT INTO Tbl_Movie
                                     (Movie_name,
                                      Release_Date,
                                      Cat_ID,
                                      rate)
                                     VALUES
                                     (@Movie_name,
                                      @Release_Date,
                                      @Cat_ID,
                                      @rate)";

					SqlCommand cmd =
						new SqlCommand(query, con);

					cmd.Parameters.AddWithValue(
						"@Movie_name",
						movie.Movie_name);

					cmd.Parameters.AddWithValue(
						"@Release_Date",
						movie.Release_Date);

					cmd.Parameters.AddWithValue(
						"@Cat_ID",
						movie.Cat_ID);

					cmd.Parameters.AddWithValue(
						"@rate",
						movie.rate);

					con.Open();

					cmd.ExecuteNonQuery();
				}

				return RedirectToAction("Index");
			}

			ViewBag.Categories = GetCategories();

			return View(movie);
		}


		// STEP 7.5
		// Edit Movie page
		public ActionResult Edit(int id)
		{
			Movie movie = new Movie();

			using (SqlConnection con =
				   new SqlConnection(cs))
			{
				string query =
					"SELECT * FROM Tbl_Movie " +
					"WHERE Movie_ID=@Movie_ID";

				SqlCommand cmd =
					new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@Movie_ID", id);

				con.Open();

				SqlDataReader dr =
					cmd.ExecuteReader();

				if (dr.Read())
				{
					movie.Movie_ID =
						Convert.ToInt32(dr["Movie_ID"]);

					movie.Movie_name =
						dr["Movie_name"].ToString();

					movie.Release_Date =
						Convert.ToDateTime(
							dr["Release_Date"]);

					movie.Cat_ID =
						Convert.ToInt32(dr["Cat_ID"]);

					movie.rate =
						Convert.ToDecimal(dr["rate"]);
				}
			}

			ViewBag.Categories = GetCategories();

			return View(movie);
		}


		// STEP 7.6
		// Update Movie
		[HttpPost]
		public ActionResult Edit(Movie movie)
		{
			if (ModelState.IsValid)
			{
				using (SqlConnection con =
					   new SqlConnection(cs))
				{
					string query = @"UPDATE Tbl_Movie
                                     SET Movie_name=@Movie_name,
                                         Release_Date=@Release_Date,
                                         Cat_ID=@Cat_ID,
                                         rate=@rate
                                     WHERE Movie_ID=@Movie_ID";

					SqlCommand cmd =
						new SqlCommand(query, con);

					cmd.Parameters.AddWithValue(
						"@Movie_ID",
						movie.Movie_ID);

					cmd.Parameters.AddWithValue(
						"@Movie_name",
						movie.Movie_name);

					cmd.Parameters.AddWithValue(
						"@Release_Date",
						movie.Release_Date);

					cmd.Parameters.AddWithValue(
						"@Cat_ID",
						movie.Cat_ID);

					cmd.Parameters.AddWithValue(
						"@rate",
						movie.rate);

					con.Open();

					cmd.ExecuteNonQuery();
				}

				return RedirectToAction("Index");
			}

			ViewBag.Categories = GetCategories();

			return View(movie);
		}


		// STEP 7.7
		// Delete Movie
		public ActionResult Delete(int id)
		{
			using (SqlConnection con =
				   new SqlConnection(cs))
			{
				string query =
					"DELETE FROM Tbl_Movie " +
					"WHERE Movie_ID=@Movie_ID";

				SqlCommand cmd =
					new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@Movie_ID", id);

				con.Open();

				cmd.ExecuteNonQuery();
			}

			return RedirectToAction("Index");
		}
		// STEP 8.1
		// Search movies by category
		public ActionResult Search(int? catId)
		{
			List<Movie> movies = new List<Movie>();

			// Load categories for dropdown
			ViewBag.Categories = GetCategories();

			// If no category is selected, just show the page
			if (!catId.HasValue)
			{
				return View(movies);
			}

			using (SqlConnection con = new SqlConnection(cs))
			{
				string query = @"SELECT m.Movie_ID,
                                m.Movie_name,
                                m.Release_Date,
                                m.Cat_ID,
                                m.rate,
                                c.Cat_Type
                         FROM Tbl_Movie m
                         INNER JOIN Tbl_Movie_Category c
                         ON m.Cat_ID = c.Cat_ID
                         WHERE m.Cat_ID = @Cat_ID";

				SqlCommand cmd = new SqlCommand(query, con);

				cmd.Parameters.AddWithValue("@Cat_ID", catId.Value);

				con.Open();

				SqlDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					Movie movie = new Movie();

					movie.Movie_ID =
						Convert.ToInt32(dr["Movie_ID"]);

					movie.Movie_name =
						dr["Movie_name"].ToString();

					movie.Release_Date =
						Convert.ToDateTime(dr["Release_Date"]);

					movie.Cat_ID =
						Convert.ToInt32(dr["Cat_ID"]);

					movie.rate =
						Convert.ToDecimal(dr["rate"]);

					movie.Cat_Type =
						dr["Cat_Type"].ToString();

					movies.Add(movie);
				}
			}

			return View(movies);
		}
	}
}