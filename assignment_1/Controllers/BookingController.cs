using assignment_1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace assignment_1.Controllers
{
	public class BookingController : Controller
	{
		string cs = ConfigurationManager
			.ConnectionStrings["MovieDBConnection"].ConnectionString;


		// =========================================================
		// 1. MY BOOKINGS
		// =========================================================
		public ActionResult Index()
		{
			// Check login
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login", "User");
			}

			int userId = Convert.ToInt32(Session["User_ID"]);

			var bookings = new List<Booking>();

			using (SqlConnection con = new SqlConnection(cs))
			{
				string query = @"SELECT booking_ID,
                                        User_ID,
                                        Cat_ID,
                                        Movie_ID,
                                        no_of_Tickets,
                                        amount
                                 FROM Tbl_Booking
                                 WHERE User_ID = @User_ID";

				SqlCommand cmd = new SqlCommand(query, con);

				cmd.Parameters.AddWithValue("@User_ID", userId);

				con.Open();

				SqlDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					Booking booking = new Booking();

					booking.booking_ID =
						Convert.ToInt32(dr["booking_ID"]);

					booking.User_ID =
						Convert.ToInt32(dr["User_ID"]);

					booking.Cat_ID =
						Convert.ToInt32(dr["Cat_ID"]);

					booking.Movie_ID =
						Convert.ToInt32(dr["Movie_ID"]);

					booking.no_of_Tickets =
						Convert.ToInt32(dr["no_of_Tickets"]);

					booking.amount =
						Convert.ToDecimal(dr["amount"]);

					bookings.Add(booking);
				}
			}

			return View(bookings);
		}


		// =========================================================
		// 2. CREATE BOOKING - GET
		// =========================================================
		public ActionResult Create()
		{
			// Check login
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login", "User");
			}

			ViewBag.Categories = GetCategories();

			ViewBag.Movies = GetMovies();

			return View();
		}


		// =========================================================
		// 3. CREATE BOOKING - POST
		// =========================================================
		[HttpPost]
		public ActionResult Create(Booking booking)
		{
			// Check login
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login", "User");
			}

			int userId = Convert.ToInt32(Session["User_ID"]);

			// Validation
			if (booking.Cat_ID <= 0)
			{
				ModelState.AddModelError(
					"Cat_ID",
					"Please select a category."
				);
			}

			if (booking.Movie_ID <= 0)
			{
				ModelState.AddModelError(
					"Movie_ID",
					"Please select a movie."
				);
			}

			if (booking.no_of_Tickets <= 0)
			{
				ModelState.AddModelError(
					"no_of_Tickets",
					"Tickets must be greater than 0."
				);
			}

			// If validation fails
			if (!ModelState.IsValid)
			{
				ViewBag.Categories = GetCategories();

				if (booking.Cat_ID > 0)
				{
					ViewBag.Movies =
						GetMoviesForCategory(booking.Cat_ID);
				}
				else
				{
					ViewBag.Movies = new List<SelectListItem>();
				}

				return View(booking);
			}


			using (SqlConnection con = new SqlConnection(cs))
			{
				// Get movie rate
				string rateQuery = @"SELECT rate
                                     FROM Tbl_Movie
                                     WHERE Movie_ID = @Movie_ID
                                     AND Cat_ID = @Cat_ID";

				SqlCommand rateCmd =
					new SqlCommand(rateQuery, con);

				rateCmd.Parameters.AddWithValue(
					"@Movie_ID",
					booking.Movie_ID
				);

				rateCmd.Parameters.AddWithValue(
					"@Cat_ID",
					booking.Cat_ID
				);

				con.Open();

				object result = rateCmd.ExecuteScalar();

				if (result == null)
				{
					ModelState.AddModelError(
						"",
						"Selected movie is not available in this category."
					);

					ViewBag.Categories = GetCategories();

					ViewBag.Movies =
						GetMoviesForCategory(booking.Cat_ID);

					return View(booking);
				}

				decimal rate =
					Convert.ToDecimal(result);

				// Calculate amount
				decimal totalAmount =
					rate * booking.no_of_Tickets;


				// Insert booking
				string insertQuery = @"INSERT INTO Tbl_Booking
                                      (
                                          User_ID,
                                          Cat_ID,
                                          Movie_ID,
                                          no_of_Tickets,
                                          amount
                                      )
                                      VALUES
                                      (
                                          @User_ID,
                                          @Cat_ID,
                                          @Movie_ID,
                                          @no_of_Tickets,
                                          @amount
                                      )";

				SqlCommand insertCmd =
					new SqlCommand(insertQuery, con);

				insertCmd.Parameters.AddWithValue(
					"@User_ID",
					userId
				);

				insertCmd.Parameters.AddWithValue(
					"@Cat_ID",
					booking.Cat_ID
				);

				insertCmd.Parameters.AddWithValue(
					"@Movie_ID",
					booking.Movie_ID
				);

				insertCmd.Parameters.AddWithValue(
					"@no_of_Tickets",
					booking.no_of_Tickets
				);

				insertCmd.Parameters.AddWithValue(
					"@amount",
					totalAmount
				);

				insertCmd.ExecuteNonQuery();
			}

			// Go to My Bookings
			return RedirectToAction("Index");
		}


		// =========================================================
		// 4. EDIT BOOKING - GET
		// =========================================================
		public ActionResult Edit(int id)
		{
			// Check login
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login", "User");
			}

			int userId =
				Convert.ToInt32(Session["User_ID"]);

			Booking booking = null;

			using (SqlConnection con = new SqlConnection(cs))
			{
				string query = @"SELECT *
                                 FROM Tbl_Booking
                                 WHERE booking_ID = @booking_ID
                                 AND User_ID = @User_ID";

				SqlCommand cmd =
					new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@booking_ID",
					id
				);

				cmd.Parameters.AddWithValue(
					"@User_ID",
					userId
				);

				con.Open();

				SqlDataReader dr =
					cmd.ExecuteReader();

				if (dr.Read())
				{
					booking = new Booking();

					booking.booking_ID =
						Convert.ToInt32(dr["booking_ID"]);

					booking.User_ID =
						Convert.ToInt32(dr["User_ID"]);

					booking.Cat_ID =
						Convert.ToInt32(dr["Cat_ID"]);

					booking.Movie_ID =
						Convert.ToInt32(dr["Movie_ID"]);

					booking.no_of_Tickets =
						Convert.ToInt32(dr["no_of_Tickets"]);

					booking.amount =
						Convert.ToDecimal(dr["amount"]);
				}
			}

			// Booking does not exist
			if (booking == null)
			{
				return HttpNotFound();
			}

			ViewBag.Categories = GetCategories();

			ViewBag.Movies =
				GetMoviesForCategory(booking.Cat_ID);

			return View(booking);
		}


		// =========================================================
		// 5. EDIT BOOKING - POST
		// =========================================================
		[HttpPost]
		public ActionResult Edit(Booking booking)
		{
			// Check login
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login", "User");
			}

			int userId =
				Convert.ToInt32(Session["User_ID"]);


			// Validation
			if (booking.Cat_ID <= 0)
			{
				ModelState.AddModelError(
					"Cat_ID",
					"Please select a category."
				);
			}

			if (booking.Movie_ID <= 0)
			{
				ModelState.AddModelError(
					"Movie_ID",
					"Please select a movie."
				);
			}

			if (booking.no_of_Tickets <= 0)
			{
				ModelState.AddModelError(
					"no_of_Tickets",
					"Tickets must be greater than 0."
				);
			}


			// If validation fails
			if (!ModelState.IsValid)
			{
				ViewBag.Categories = GetCategories();

				if (booking.Cat_ID > 0)
				{
					ViewBag.Movies =
						GetMoviesForCategory(booking.Cat_ID);
				}
				else
				{
					ViewBag.Movies =
						new List<SelectListItem>();
				}

				return View(booking);
			}


			using (SqlConnection con = new SqlConnection(cs))
			{
				// Get movie rate
				string rateQuery = @"SELECT rate
                                     FROM Tbl_Movie
                                     WHERE Movie_ID = @Movie_ID
                                     AND Cat_ID = @Cat_ID";

				SqlCommand rateCmd =
					new SqlCommand(rateQuery, con);

				rateCmd.Parameters.AddWithValue(
					"@Movie_ID",
					booking.Movie_ID
				);

				rateCmd.Parameters.AddWithValue(
					"@Cat_ID",
					booking.Cat_ID
				);

				con.Open();

				object result =
					rateCmd.ExecuteScalar();

				if (result == null)
				{
					ModelState.AddModelError(
						"",
						"Selected movie is not available in this category."
					);

					ViewBag.Categories =
						GetCategories();

					ViewBag.Movies =
						GetMoviesForCategory(booking.Cat_ID);

					return View(booking);
				}

				decimal rate =
					Convert.ToDecimal(result);

				// Recalculate amount
				decimal totalAmount =
					rate * booking.no_of_Tickets;


				// Update booking
				string updateQuery = @"UPDATE Tbl_Booking
                                       SET Cat_ID = @Cat_ID,
                                           Movie_ID = @Movie_ID,
                                           no_of_Tickets = @no_of_Tickets,
                                           amount = @amount
                                       WHERE booking_ID = @booking_ID
                                       AND User_ID = @User_ID";

				SqlCommand updateCmd =
					new SqlCommand(updateQuery, con);

				updateCmd.Parameters.AddWithValue(
					"@Cat_ID",
					booking.Cat_ID
				);

				updateCmd.Parameters.AddWithValue(
					"@Movie_ID",
					booking.Movie_ID
				);

				updateCmd.Parameters.AddWithValue(
					"@no_of_Tickets",
					booking.no_of_Tickets
				);

				updateCmd.Parameters.AddWithValue(
					"@amount",
					totalAmount
				);

				updateCmd.Parameters.AddWithValue(
					"@booking_ID",
					booking.booking_ID
				);

				updateCmd.Parameters.AddWithValue(
					"@User_ID",
					userId
				);

				updateCmd.ExecuteNonQuery();
			}

			return RedirectToAction("Index");
		}


		// =========================================================
		// 6. DELETE BOOKING
		// =========================================================
		public ActionResult Delete(int id)
		{
			// Check login
			if (Session["User_ID"] == null)
			{
				return RedirectToAction("Login", "User");
			}

			int userId =
				Convert.ToInt32(Session["User_ID"]);


			using (SqlConnection con =
				new SqlConnection(cs))
			{
				string query = @"DELETE FROM Tbl_Booking
                                 WHERE booking_ID = @booking_ID
                                 AND User_ID = @User_ID";

				SqlCommand cmd =
					new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@booking_ID",
					id
				);

				cmd.Parameters.AddWithValue(
					"@User_ID",
					userId
				);

				con.Open();

				cmd.ExecuteNonQuery();
			}

			return RedirectToAction("Index");
		}


		// =========================================================
		// 7. GET ALL CATEGORIES
		// =========================================================
		private List<SelectListItem> GetCategories()
		{
			List<SelectListItem> categories =
				new List<SelectListItem>();

			using (SqlConnection con =
				new SqlConnection(cs))
			{
				string query =
					"SELECT Cat_ID, Cat_Type FROM Tbl_Movie_Category";

				SqlCommand cmd =
					new SqlCommand(query, con);

				con.Open();

				SqlDataReader dr =
					cmd.ExecuteReader();

				while (dr.Read())
				{
					categories.Add(
						new SelectListItem
						{
							Value =
								dr["Cat_ID"].ToString(),

							Text =
								dr["Cat_Type"].ToString()
						}
					);
				}
			}

			return categories;
		}


		// =========================================================
		// 8. GET ALL MOVIES
		// =========================================================
		private List<SelectListItem> GetMovies()
		{
			List<SelectListItem> movies =
				new List<SelectListItem>();

			using (SqlConnection con =
				new SqlConnection(cs))
			{
				string query =
					"SELECT Movie_ID, Movie_name FROM Tbl_Movie";

				SqlCommand cmd =
					new SqlCommand(query, con);

				con.Open();

				SqlDataReader dr =
					cmd.ExecuteReader();

				while (dr.Read())
				{
					movies.Add(
						new SelectListItem
						{
							Value =
								dr["Movie_ID"].ToString(),

							Text =
								dr["Movie_name"].ToString()
						}
					);
				}
			}

			return movies;
		}


		// =========================================================
		// 9. GET MOVIES BY CATEGORY - AJAX
		// =========================================================
		public JsonResult GetMoviesByCategory(int catId)
		{
			var movies =
				GetMoviesForCategory(catId);

			return Json(
				movies,
				JsonRequestBehavior.AllowGet
			);
		}


		// =========================================================
		// 10. GET MOVIES FOR SELECTED CATEGORY
		// =========================================================
		private List<SelectListItem> GetMoviesForCategory(
			int catId)
		{
			List<SelectListItem> movies =
				new List<SelectListItem>();

			using (SqlConnection con =
				new SqlConnection(cs))
			{
				string query = @"SELECT Movie_ID,
                                        Movie_name
                                 FROM Tbl_Movie
                                 WHERE Cat_ID = @Cat_ID";

				SqlCommand cmd =
					new SqlCommand(query, con);

				cmd.Parameters.AddWithValue(
					"@Cat_ID",
					catId
				);

				con.Open();

				SqlDataReader dr =
					cmd.ExecuteReader();

				while (dr.Read())
				{
					movies.Add(
						new SelectListItem
						{
							Value =
								dr["Movie_ID"].ToString(),

							Text =
								dr["Movie_name"].ToString()
						}
					);
				}
			}

			return movies;
		}
	}
}