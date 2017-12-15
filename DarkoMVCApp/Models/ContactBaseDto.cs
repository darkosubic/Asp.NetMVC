using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DarkoMVCApp.Models
{
    public class ContactBaseDto
    {
        [Display(Name = "LBL_FirstName", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "LBL_FirstName")]
        public string FirstName { get; set; }
        [Display(Name = "LBL_LastName", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "LBL_FirstName")]
        public string LastName { get; set; }
        [Display(Name = "LBL_Telephone", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "LBL_FirstName")]
        public string Telephone { get; set; }
        [Display(Name = "LBL_Mail", ResourceType = typeof(Resources.Resources))]
        [EmailAddress(ErrorMessageResourceName = "VLD_MailValid", ErrorMessageResourceType = typeof(Resources.Resources))]
        public string Mail { get; set; }
    }
}