using System.ComponentModel.DataAnnotations;

namespace taskflow.Models.DTO.Request
{
    public class CreateWorkspaceRequestDto
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "Only Max of 255 characters is allowed")]
        public string Name { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(500, ErrorMessage = "Only Max of 500 characters is allowed")]
        public string Description { get; set; }
        
        //[Required(ErrorMessage = "The Members field is required.")]
      //  [Array(typeof(string), ArrayLength = 10, ErrorMessage = "The Members field must contain an array of 10 email addresses.")]
        //[EmailAddress(ErrorMessage = "The Members field must contain valid email addresses.")]
        public string[]? Members { get; set; }
        
    }
}

