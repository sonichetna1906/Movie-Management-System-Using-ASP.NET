using System.ComponentModel.DataAnnotations;

namespace assignment_1.Models
{
	public class User
	{
		public int User_ID { get; set; }

		[Required]
		public string User_Name { get; set; }

		[Required]
		public string Email_ID { get; set; }

		[Required]
		public string User_password { get; set; }

		public string City { get; set; }

		public string PhoneNo { get; set; }
	}
}