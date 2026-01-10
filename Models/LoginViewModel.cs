using System.ComponentModel.DataAnnotations;

namespace MaharaFinalVersion.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = null!;

        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; } = false;
    }
}
