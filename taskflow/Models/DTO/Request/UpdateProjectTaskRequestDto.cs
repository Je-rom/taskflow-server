using System.ComponentModel.DataAnnotations;

namespace taskflow.Models.DTO.Request
{
    public class UpdateProjectTaskRequestDto
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Only Max of 255 characters is allowed")]
        public string Name { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(500, ErrorMessage = "Only Max of 500 characters is allowed")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format for StartDate. Please use the format yyyy-MM-dd.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for EndDate. Please use the format yyyy-MM-dd.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        
        [Required]
        [RegularExpression(@"^(TODO|INPROGRESS|COMPLETED)$", ErrorMessage = "Invalid stage value. Only 'TODO', 'INPROGRESS', and 'COMPLETED' are allowed.")]
        public string Stage { get; set; }
        
        [Required]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format.")]
        public Guid UserId { get; set; }
    }
}
