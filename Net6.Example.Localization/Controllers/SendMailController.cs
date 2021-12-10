using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net6.Example.Localization.Pages;
using Net6.Example.Localization.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Net6.Example.Localization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMailController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IViewRenderService _viewRenderService;
        private readonly IConfiguration _configuration;
        private readonly string apikey;
        public SendMailController(IHttpContextAccessor contextAccessor, IViewRenderService viewRenderService, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _viewRenderService = viewRenderService;

            _configuration = configuration;
            apikey = _configuration["SendGridKey"];
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(string viewName)
        {
            var subject = string.Format("Jobdone Test");

            //JsonSerializer.Serialize(model);

            var htmlContent = await _viewRenderService.RenderToStringAsync(viewName, new IndexModel
            {

            });
           
            var Tos = new List<EmailAddress>();
            Tos.Add(new EmailAddress { Email = "felaray@fab26.cyou" });

            var Bccs = new List<EmailAddress>();
            //Bccs.Add(new EmailAddress { Email = "carol@pyramius.com" });
            Bccs.Add(new EmailAddress { Email = "felaray@gmail.com" });
            subject = "[Test] FelarayTest";
            await SendMailBySendGrid(subject, null, htmlContent, Tos, Bccs);
            return Ok();
        }

        private async Task<Response> SendMailBySendGrid(string Subject, string PlainTextContent, string HtmlContent, List<EmailAddress> MailTos, List<EmailAddress> MailBCCs = null)
        {
            var client = new SendGridClient(apikey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("admin@fab26.cyou", "Fab26 Admin"),

                Subject = Subject,
                PlainTextContent = PlainTextContent,
                HtmlContent = HtmlContent
            };

            //收件者
            msg.AddTos(MailTos);
            //密件副本 如果包含收件者會寄不出去

            if (MailBCCs != null)
            {
                var cc = MailBCCs.Except(MailTos);
                if (cc.Count() > 0)
                    msg.AddBccs(cc.ToList());
            }


            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}
