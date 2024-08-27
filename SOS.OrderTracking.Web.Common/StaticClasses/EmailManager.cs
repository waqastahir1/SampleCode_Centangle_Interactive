using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SOS.OrderTracking.Web.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
namespace SOS.OrderTracking.Web.Common.StaticClasses
{
	//public class EmailManager
	//{
	//    private readonly IConfiguration configuration;

	//    public SmtpEmailManager(IConfiguration configuration)
	//    {
	//        try
	//        {

	//            var emailCred = configuration.GetSection("EmailCred");
	//            UserName = emailCred["UserName"];
	//            Password = emailCred["Password"];
	//            SmtpHost = emailCred["SmtpHost"];


	//        }
	//        catch { }

	//        this.configuration = configuration;
	//    }
	//    public string SmtpHost { get; set; }

	//    public string UserName { get; set; }

	//    public string Password { get; set; }

	//    public async Task SendEmail(string mailTo, string emailBody, string subject)
	//    {
	//        try
	//        {

	//            var apiKey = configuration["SendGridKey"];
	//            var client = new SendGridClient(apiKey);
	//            var from = new EmailAddress("digitalsupport@sospakistan.net", "SOS Digital Support");
	//            var to = new EmailAddress(mailTo);
	//            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, emailBody);
	//            var response = await client.SendEmailAsync(msg);
	//        }
	//        catch (Exception ex)
	//        {
	//            throw new BadRequestException(ex.Message);
	//        }
	//    }


	//    public async Task SendFormattedEmail(string mailTo, IWebHostEnvironment env, string role, string emailConfirmationLink, string userId)
	//    {
	//        if (role == Shared.Constants.Roles.BANK_BRANCH || role == Shared.Constants.Roles.BANK_CPC)
	//        {
	//            role = " as <strong style='color:#1ea653;'>Initiator</strong>";
	//        }
	//        else if (role == Shared.Constants.Roles.BANK_BRANCH_MANAGER || role == Shared.Constants.Roles.BANK_CPC_MANAGER)
	//        {
	//            role = "as <strong style='color:#1ea653;'>Supervisor</strong>";
	//        }
	//        else if (role == Shared.Constants.Roles.BANK_HYBRID)
	//        {
	//            role = "";
	//        }

	//        var filePath = env.WebRootFileProvider.GetFileInfo("Images/SOSLogo.png")?.PhysicalPath;
	//        var imageArray = File.ReadAllBytes(filePath);
	//        string base64ImageRepresentation = Convert.ToBase64String(imageArray);

	//        string htmlBody = $"<p align='center'> <strong style='color:red;'>Welcome to SOS Digital</strong></p><p align='center'> Where excellence is not an exception</p><p> Dear Valued Customer</p><p> Thank you for choosing SOS Pakistan. You can now enjoy expanded access to Cash-in-Transit Services.</p><p> <a href= '{emailConfirmationLink}' target='_blank' >Click here</a> to verify your email\\Username and create your password {role}.<br>Your username is <strong style='color:#1ea653;'>{userId}</strong> for your record.<br><br>The portal link is <a href='https://sosdigital.pk'>https://sosdigital.pk</a> </p><p> Please reach out to our Support Team if you have any questions at ​</p><p> <a href='mailto:digital​support@sospakistan.net' target='_blank'> <strong>digital​support@sospakistan.net</strong> </a> <strong> </strong> or call our <strong>Help Center</strong> <strong> +923000 341 900 </strong> for more information.</p><p><br>
	//        <img height='100' src=\"data:image/png;base64,{base64ImageRepresentation}\"> </p> <p> Thank you! <br><strong style='color:red;'>SOS Digital Team</strong></p><p><br><br><br> Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox <br/> For more information visit our help page or contact us here</p>";
	//        AlternateView avHtml = AlternateView.CreateAlternateViewFromString
	//           (htmlBody, null, MediaTypeNames.Text.Html);



	//        await SendEmail(mailTo, htmlBody, "Welcome to SOS Digital");
	//    }
	//}

	public class SmtpEmailManager
	{

		public SmtpEmailManager(IConfiguration configuration)
		{
			try
			{
				var emailCred = configuration.GetSection("EmailCred");
				UserName = emailCred["UserName"];
				Password = emailCred["Password"];
				SmtpHost = emailCred["SmtpHost"];
				Port = Convert.ToInt32(emailCred["Port"]);

			}
			catch { }
		}
		public string SmtpHost { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }
		public int Port { get; set; }

		public async Task SendEmail(string mailTo, string emailBody, string subject)
		{

			try
			{

				var client = new SmtpClient
				{
					Host = SmtpHost,
					// Port = 465,
					Port = Port,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(UserName, Password)
				};

				MailMessage mail = new MailMessage
				{
					From = new MailAddress(UserName),
					Subject = subject,
					IsBodyHtml = true,
					Body = emailBody
				};

				mail.To.Add(mailTo);

				await client.SendMailAsync(mail);
				//logger.LogInformation("mail Sent");
			}
			catch (Exception ex)
			{
				//logger.LogInformation(ex.Message);
				throw new BadRequestException(ex.ToString());
			}
		}
		public async Task SendEmail2(string mailTo, string emailBody, string subject, MemoryStream stream)
		{

			try
			{

				var client = new SmtpClient
				{
					Host = SmtpHost,
					// Port = 465,
					Port = Port,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(UserName, Password)
				};
				System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(stream, "Receipt.pdf", "application/pdf");

				MailMessage mail = new MailMessage
				{
					From = new MailAddress(UserName),
					Subject = subject,
					IsBodyHtml = true,
					Body = emailBody,
					//Attachments= attachment
				};
				mail.Attachments.Add(attachment);
				mail.To.Add(mailTo);

				await client.SendMailAsync(mail);
				//logger.LogInformation("mail Sent");
			}
			catch (Exception ex)
			{
				//logger.LogInformation(ex.Message);
				throw new BadRequestException(ex.ToString());
			}
		}
		public async Task SendEmail(string mailTo, string emailBody, string subject, MemoryStream stream)
		{

			try
			{
				var message = new MimeMessage();

				message.From.Add(new MailboxAddress("SOS Digital", UserName));
				message.To.Add(new MailboxAddress("", mailTo));
				message.Subject = subject;

				var bodyBuilder = new BodyBuilder();
				bodyBuilder.HtmlBody = emailBody;

				bodyBuilder.Attachments.Add("Receipt.pdf",stream.ToArray());
				//var filename = "Receipt.pdf";
				//var attachmentPart = new MimePart(MediaTypeNames.Application.Pdf)
				//{
				//	Content = new MimeContent(stream),
				//	ContentTransferEncoding = ContentEncoding.Base64,
				//	FileName = filename
				//};
				//bodyBuilder.Attachments.Add(attachmentPart);

				//var attachments = new List<MimeEntity>
				//{
				//	MimeEntity.Load(new MimeKit.ContentType("application", "pdf"), stream)
				//};
				//foreach (var item in attachments)
				//	bodyBuilder.Attachments.Add(item);


				message.Body = bodyBuilder.ToMessageBody();
				var smtp = new MailKit.Net.Smtp.SmtpClient();
					smtp.Connect(SmtpHost, Port);
					smtp.Authenticate(UserName, Password);
					await smtp.SendAsync(message);
					await smtp.DisconnectAsync(true);
			}
			catch (Exception ex)
			{
				throw new BadRequestException(ex.ToString());
			}
		}

		public async Task SendFormattedEmail(string mailTo, IWebHostEnvironment env, string role, string emailConfirmationLink, string userId)
		{
			if (role == Shared.Constants.Roles.BANK_BRANCH || role == Shared.Constants.Roles.BANK_CPC)
			{
				role = " as <strong style='color:#1ea653;'>Initiator</strong>";
			}
			else if (role == Shared.Constants.Roles.BANK_BRANCH_MANAGER || role == Shared.Constants.Roles.BANK_CPC_MANAGER)
			{
				role = "as <strong style='color:#1ea653;'>Supervisor</strong>";
			}
			else if (role == Shared.Constants.Roles.BANK_HYBRID)
			{
				role = "";
			}



			//<p> <img width='86' height='86' src=\"cid:SOSLogo\" align='left' hspace='12' alt='photograph1' /></p>
			var cid = Guid.NewGuid().ToString(); ;
			string htmlBody = $"<p align='center'> <strong style='color:red;'>Welcome to SOS Digital</strong></p><p align='center'> Where excellence is not an exception</p><p> Dear Valued Customer</p><p> Thank you for choosing SOS Pakistan. You can now enjoy expanded access to Cash-in-Transit Services.</p><p> <a href= '{emailConfirmationLink}' target='_blank' >Click here</a> to verify your email\\Username and create your password {role}.<br>Your username is <strong style='color:#1ea653;'>{userId}</strong> for your record.<br><br>The portal link is <a href='https://sosdigital.pk'>https://sosdigital.pk</a> </p><p> Please reach out to our Support Team if you have any questions at ​</p><p> <a href='mailto:digital​support@sospakistan.com' target='_blank'> <strong>digital​support@sospakistan.com</strong> </a> <strong> </strong> or call our <strong>Help Center</strong> <strong> +923000 341 900 </strong> for more information.</p><p><br><img height='100' src=\"cid:{cid}\"> </p> <p> Thank you! <br><strong style='color:red;'>SOS Digital Team</strong></p><p><br><br><br> Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox <br/> For more information visit our help page or contact us here</p>";
			AlternateView avHtml = AlternateView.CreateAlternateViewFromString
			   (htmlBody, null, MediaTypeNames.Text.Html);

			LinkedResource inline = new LinkedResource("wwwroot/images/SOSLogo.png", MediaTypeNames.Image.Jpeg);
			inline.ContentId = cid;
			avHtml.LinkedResources.Add(inline);

			MailMessage mail = new MailMessage();
			mail.AlternateViews.Add(avHtml);

			var filePath = env.WebRootFileProvider.GetFileInfo("Images/SOSLogo.png")?.PhysicalPath;

			System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(filePath);
			att.ContentDisposition.Inline = true;

			mail.From = new MailAddress(UserName);
			mail.To.Add(mailTo);
			mail.Subject = "Welcome to SOS Digital";
			//mail.Body = $"<p> <img width='86' height='86' src=\"cid:{att.ContentId}\" align='left' hspace='12' alt='photograph2' /></p><p align='center'> <strong></strong></p><p align='center'> <strong style='color:red;'>Welcome to SOS Digital</strong></p><p align='center'> Where excellence is not an exception</p><p> Dear Valued Customer</p><p> Thank you for choosing SOS Pakistan. You can now enjoy expanded access to Cash-in-Transit Services.</p><p> <a href= '{emailConfirmationLink}' target='_blank' >Click here</a> verify your email\\Username and create your password as  <strong style='color:#1ea653;'>{role}</strong>. You username is  <strong style='color:#1ea653;'>{userId}</strong> for your record. <a href='https://sosdigital.pk'>https://sosdigital.pk</a></p><p> Please reach out to our Support Team if you have any questions at ​</p><p><a href='mailto:digital​support@sospakistan.net' target='_blank'> <strong>digital​support@sospakistan.net</strong> </a> <strong> </strong> or call our <strong>Help Center</strong> <strong> +923000 341 900 </strong> for more information.</p><p> Thank you!</p><p> <strong style='color:red;'>SOS Digital Team</strong></p><p><br><br><br> Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox <br/> For more information visit our help page or contact us here</p>";

			mail.IsBodyHtml = true;
			mail.Attachments.Add(att);

			var client = new SmtpClient
			{
				Host = SmtpHost,
				// Port = 465,
				Port = Port,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(UserName, Password)
			};
			await client.SendMailAsync(mail);
		}
		public async Task SendShipmentScanComplaintFormattedEmail(string mailTo, IWebHostEnvironment env, string complainant, string complainantEmail, string shipmentNumber, string complaintRemarks, string link)
		{

			var cid = Guid.NewGuid().ToString(); 
			string htmlBody = $"<p align='center'> <strong style='color:red;'>Welcome to SOS Digital</strong></p><p align='center'> Where excellence is not an exception</p>" +
				$"<p>This is an automated notification to inform you that a new complaint has been launched by a customer.</p>" +
				$"<p>Details of the complaint are as follows:</p>" +

				$"<p>" +
				$"Customer Name: <strong>{complainant}</strong><br>" +
				$"Customer Email: <strong>{complainantEmail}</strong><br>" +
				$"Shipment Number: <strong>{shipmentNumber}</strong><br>" +
				$"Complaint Comments: <strong>{complaintRemarks}</strong><br>" +
				$"Complaint Number: <strong>{shipmentNumber}</strong>" +
				$"</p>" +

				$"<p>The complaints link is <a href='{link}'>{link}</a> </p>" +
				$"<p>Please take the necessary steps to investigate and address this complaint promptly.​</p>" +
				$"<p><br><img height='100' src=\"cid:{cid}\"> </p> <p> Thank you! <br><strong style='color:red;'>SOS Digital Team</strong></p>" +
				$"<p><br><br><br> Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox <br/> For more information visit our help page or contact us here</p>";
			AlternateView avHtml = AlternateView.CreateAlternateViewFromString
			   (htmlBody, null, MediaTypeNames.Text.Html);

			LinkedResource inline = new LinkedResource("wwwroot/images/SOSLogo.png", MediaTypeNames.Image.Jpeg);
			inline.ContentId = cid;
			avHtml.LinkedResources.Add(inline);

			MailMessage mail = new MailMessage();
			mail.AlternateViews.Add(avHtml);

			var filePath = env.WebRootFileProvider.GetFileInfo("Images/SOSLogo.png")?.PhysicalPath;

			System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(filePath);
			att.ContentDisposition.Inline = true;

			mail.From = new MailAddress(UserName);
			mail.To.Add(mailTo);
			mail.Subject = "Shipment Scan Complaint Initiated in Portal";
			//mail.Body = $"<p> <img width='86' height='86' src=\"cid:{att.ContentId}\" align='left' hspace='12' alt='photograph2' /></p><p align='center'> <strong></strong></p><p align='center'> <strong style='color:red;'>Welcome to SOS Digital</strong></p><p align='center'> Where excellence is not an exception</p><p> Dear Valued Customer</p><p> Thank you for choosing SOS Pakistan. You can now enjoy expanded access to Cash-in-Transit Services.</p><p> <a href= '{emailConfirmationLink}' target='_blank' >Click here</a> verify your email\\Username and create your password as  <strong style='color:#1ea653;'>{role}</strong>. You username is  <strong style='color:#1ea653;'>{userId}</strong> for your record. <a href='https://sosdigital.pk'>https://sosdigital.pk</a></p><p> Please reach out to our Support Team if you have any questions at ​</p><p><a href='mailto:digital​support@sospakistan.net' target='_blank'> <strong>digital​support@sospakistan.net</strong> </a> <strong> </strong> or call our <strong>Help Center</strong> <strong> +923000 341 900 </strong> for more information.</p><p> Thank you!</p><p> <strong style='color:red;'>SOS Digital Team</strong></p><p><br><br><br> Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox <br/> For more information visit our help page or contact us here</p>";

			mail.IsBodyHtml = true;
			mail.Attachments.Add(att);

			var client = new SmtpClient
			{
				Host = SmtpHost,
				// Port = 465,
				Port = Port,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(UserName, Password)
			};
			await client.SendMailAsync(mail);
		}
	}

}
