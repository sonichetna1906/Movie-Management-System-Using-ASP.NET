using System.ComponentModel.DataAnnotations;

namespace assignment_1.Models
{
	public class MovieCategory
	{
		public int Cat_ID { get; set; }

		[Required]
		public string Cat_Type { get; set; }
	}
}