using System.ComponentModel.DataAnnotations;

namespace taskflow.Models.DTO.Request
{
    public class ProjectTaskRequestDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Only a maximum of 255 characters is allowed")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Only a maximum of 500 characters is allowed")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format for StartDate. Please use the format yyyy-MM-dd.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for EndDate. Please use the format yyyy-MM-dd.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
    }
}
