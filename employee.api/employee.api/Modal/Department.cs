using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace employee.api.Modal
{
    [Table("departmentTbl")]
    public class Department
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int departmentId { get; set; }
        [Required, MaxLength(50)]
        public string departmentName { get; set; } = String.Empty;
        public bool isActive { get; set; }
    }
}
