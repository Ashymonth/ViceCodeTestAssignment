using System;
using System.ComponentModel.DataAnnotations;

namespace CrudTestAssignment.Api.Api.V1.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(64)]
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
