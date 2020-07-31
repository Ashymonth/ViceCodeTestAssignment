using System.ComponentModel.DataAnnotations;

namespace CrudTestAssignment.DAL.Models
{
    public class UserViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}