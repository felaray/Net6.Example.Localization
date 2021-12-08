using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Net6.Example.Localization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IStringLocalizer<TodoController> _localizer;

        public TodoController(IStringLocalizer<TodoController> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 測試語言包
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SayHi()
        {
            var data = new
            {
                txt = _localizer["hello"],

            };

            return Ok(data);
        }

        /// <summary>
        /// 以cookie設定語言
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("cookie")]
        public IActionResult setCultureInCookie(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect("~/");
        }

        /// <summary>
        /// 移除語言設定
        /// </summary>
        /// <returns></returns>
        [HttpDelete("cookie")]
        public IActionResult delelteCultureInCookie()
        {
            Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);

            return LocalRedirect("~/");
        }
    }
}
