using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace LibraryAPI.Controllers {
    public class BaseController : ControllerBase
    {
        protected string? UserID
        {
            get
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return userId;
            }
        }
    }
}
