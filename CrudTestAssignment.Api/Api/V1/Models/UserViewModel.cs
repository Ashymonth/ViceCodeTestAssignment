using System.ComponentModel.DataAnnotations;

namespace CrudTestAssignment.Api.Api.V1.Models
{
    public class UserViewModel
    {
        [Required]
        [MaxLength(63)]
        public string Name { get; set; }
    }
}