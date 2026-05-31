using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace employee.api.Modal
{

    [Table("designationTbl")]
    public class Designation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int designationId { get; set; }
       
        public int departmentId { get; set; }
        [Required, MaxLength(50)]
        public string designationName { get; set; } = String.Empty;
    }
}
