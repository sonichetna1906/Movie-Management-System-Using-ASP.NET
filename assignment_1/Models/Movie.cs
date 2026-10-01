using System;
using System.ComponentModel.DataAnnotations;

namespace assignment_1.Models
{
	public class Movie
	{
		public int Movie_ID { get; set; }

		[Required]
		public string Movie_name { get; set; }

		[Required]
		public DateTime Release_Date { get; set; }

		[Required]
		public int Cat_ID { get; set; }

		[Required]
		public decimal rate { get; set; }

		public string Cat_Type { get; set; }
	}
}