using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace taskflow.Models.DTO.Request
{
    public class ProjectRequestDto
    {
        [Required] 
        public string Name {get; set;}


         [Required]
        [DataType(DataType.Text)]
        [MaxLength(500, ErrorMessage = "Only Max of 500 characters is allowed")]
        public string Description {get; set;} 
        
    }
}