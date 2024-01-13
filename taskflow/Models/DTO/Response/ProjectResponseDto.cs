using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace taskflow.Models.DTO.Response
{
    public class ProjectResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate {get; set;}
        public DateTime EndDate {get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        
    }
}