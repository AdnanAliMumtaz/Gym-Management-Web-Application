using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Data;


public class Employee
{
    [Key]
    public int EmployeeID { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    [Range(typeof(DateTime), "1900-01-01", "2003-12-31", ErrorMessage = "Employee must be at least 18 years old.")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [DataType(DataType.MultilineText)]
    public string Address { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [DataType(DataType.PhoneNumber)]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid phone number format.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [UniqueEmail(ErrorMessage = "Email already exists.")]
    public string Email { get; set; }

    // Foreign Key
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser ApplicationUser { get; set; }
}

public enum Gender
{
    Male,
    Female,
    Other
}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var context = (WebDbContext)validationContext.GetService(typeof(WebDbContext));
        var instance = validationContext.ObjectInstance as Employee;

        if (context.Employees.Any(e => e.Email == instance.Email && e.EmployeeID != instance.EmployeeID))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
