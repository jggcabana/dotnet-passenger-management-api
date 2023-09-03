using System.ComponentModel.DataAnnotations;

namespace Qless.WebAPI.RequestModels
{
    public class LoadCardRequest
    {
        [Required]
        [Range(0, 10000, ErrorMessage = "Value must be from 0 to 10000.")]
        public decimal NewBalance { get; set; }
    }
}
