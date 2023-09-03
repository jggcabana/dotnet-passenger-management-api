using Qless.Repositories.Enums;
using System.ComponentModel.DataAnnotations;

namespace Qless.WebAPI.RequestModels
{
    public class CreateCardRequest
    {
        public CardTypeEnum CardTypeId { get; set; }

        [RegularExpression(@"(\d{4}|\d{2})-\d{4}-\d{4}", ErrorMessage = "Invalid format for Senior Citizen / PWD ID Number.")]
        public string IdNo { get; set; }
    }
}
