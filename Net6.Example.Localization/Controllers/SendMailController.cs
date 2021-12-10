using Microsoft.AspNetCore.Mvc;
using Net6.Example.Localization.Services;
using Net6.Example.Localization.Views;
using RazorLight;
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
        public async Task<IActionResult> SendMail(string viewName, string toUserEmail)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = "index";

            if (string.IsNullOrEmpty(toUserEmail))
                return BadRequest("toUserEmail is null");

            var subject = "RazorPage Demo";

            var engine = new RazorLightEngineBuilder()
                            .UseMemoryCachingProvider()
                            .UseEmbeddedResourcesProject(typeof(Program))
                            .Build();

            var model = new EmailModel {  };
            string htmlContent;
            try
            {
                //"Views.Subfolder.A"
                htmlContent = await engine.CompileRenderAsync<string>(viewName, null, null);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }



            var Tos = new List<EmailAddress>();
            Tos.Add(new EmailAddress { Email = toUserEmail });

            var result = await SendMailBySendGrid(subject, null, htmlContent, Tos);

            if (result.IsSuccessStatusCode)
                return Ok();
            else
                return BadRequest();
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
