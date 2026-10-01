using System.ComponentModel.DataAnnotations;

namespace assignment_1.Models
{
	public class Booking
	{
		public int booking_ID { get; set; }

		[Required]
		public int User_ID { get; set; }

		[Required]
		public int Cat_ID { get; set; }

		[Required]
		public int Movie_ID { get; set; }

		[Required]
		public int no_of_Tickets { get; set; }

		public decimal amount { get; set; }
	}
}