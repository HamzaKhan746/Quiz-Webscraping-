using System.ComponentModel.DataAnnotations;
namespace Quiz_management_system.Models
{
    public class loginModel
    {
        public int register_id { get; set; }
        [Required(ErrorMessage = "Please enter user name")]
        public string user_name { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public string password { get; set; }
        [Required(ErrorMessage = "Select any type")]
        public string user_select { get; set; }
       
    }
}

