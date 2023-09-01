using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class LoginRegisterModel : BaseNopModel
    {
        public RegisterModel RegisterModel { get; set; }
        public LoginModel LoginModel { get; set; }
        public int Type { get; set; }
    }
}