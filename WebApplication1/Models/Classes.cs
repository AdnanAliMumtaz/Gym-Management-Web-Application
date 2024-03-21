using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Areas.Identity.Data;

namespace WebApplication1.Models
{
    public class Classes
    {
        [Key]
        public int ClassID { get; set; }

        public string ClassName { get; set; }
        public string Description { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        //[DataType(DataType.Time)]
        public TimeSpan Duration { get; set; }

        // Navigation properties for related entities
        //public ICollection<Member> Members { get; set; }
        //public ICollection<Employee> Trainers { get; set; }

        public List<Member> Members { get; set; }
        public List<Employee> Trainers { get; set; }

        // Foreign Key
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
