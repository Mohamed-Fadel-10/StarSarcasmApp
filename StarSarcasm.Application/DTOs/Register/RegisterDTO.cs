using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.DTOs.Register
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب*")]
        [MinLength(3,ErrorMessage = " اقل عدد من الاحرف المسموحة للاسم : 3 احرف "),MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "البريد الالكترونى مطلوب*")]
        [EmailAddress(ErrorMessage = "يرجى  إدخال بريد الكترونى صالح")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "يرجى إدخال بريد إلكتروني صحيح")]
        public string Email { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
        public string Location  { get; set; }

        [Required(ErrorMessage = "كلمة السر مطلوبة*")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون مكونة من 6 أحرف على الأقل")]
        [DataType(DataType.Password,ErrorMessage =
            "يجب أن تحتوي كلمة المرور على أحرف كبيرة وصغيرة، وأرقام، وأحرف خاصة")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="كلمة السر غير متطابقة")]
        public string ConfirmPassword { get; set; }


    }
}
