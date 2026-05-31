using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace employee.api.Modal
{
    [Table("employeeTbl")]
    public class Employee
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int employeeId { get; set; }
        [Required, MaxLength(50)]
        public string name { get; set; } = String.Empty;
        [Required,MinLength(10), MaxLength(10)]
        public string contactNo { get; set; } = String.Empty;
        public string email { get; set; } = String.Empty;
        public string city { get; set; } = String.Empty;
        public string state { get; set; } = String.Empty;

        public string pincode { get; set; } = String.Empty;

        public string altContactNo { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public int designationId { get; set; } 
        public DateTime createdDate { get; set; }
        public DateTime modifiedDate { get; set; }
        public string role { get; set; } = String.Empty;

    }

    public class LoginDto
    {
        [Required]
        public string email { get; set; } = String.Empty;

        [Required]
        public string contactNo { get; set; } = String.Empty;
    }
}
