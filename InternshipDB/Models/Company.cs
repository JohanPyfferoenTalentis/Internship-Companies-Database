using System.ComponentModel.DataAnnotations;

namespace InternshipDB.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Display(Name = "Sector")]
        public string? Sector { get; set; }

        [Display(Name = "Person in Charge")]
        public string? PersonInCharge { get; set; }

        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Internship Period")]
        public string? InternshipPeriod { get; set; }

        [Display(Name = "Information")]
        public string? Information { get; set; }
    }
}
