using System.ComponentModel.DataAnnotations;

namespace hallocDoc.ViewDataModels
{
    public class forgotpass
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        /*public bool emailSent { get; set; }*/

        /*public bool emailSent
        {
            get { return emailSent; }
            set { emailSent = false; }
        }*/
    }
}
