using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class forgotpass
    {
        [EmailAddress]
        [RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]{1,4})$", ErrorMessage = "Domain is not real..")]
        public string Email { get; set; } = null!;

        /*public bool emailSent { get; set; }*/

        /*public bool emailSent
        {
            get { return emailSent; }
            set { emailSent = false; }
        }*/
    }
}
