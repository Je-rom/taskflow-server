using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace taskflow.Models.DTO
{
    public class CreateProjectRequestDto
    {
        [Required] 
        public string Name {get; set;}


        [Required]
        [DataType(DataType.Text)]
        [MaxLength(500, ErrorMessage = "Only Max of 500 characters is allowed")]
        public string Description {get; set;} 
        
    }
}