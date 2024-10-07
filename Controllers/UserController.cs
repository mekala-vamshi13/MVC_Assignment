using System;
using System.Linq;
using System.Web.Mvc;
using User_Management.Models;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using BCrypt.Net;

namespace User_Management.Controllers
{
    public class UserController : Controller
    {
        MvcDbContext db = new MvcDbContext();

        // GET: User
        public ActionResult Index()
        {
            var element = db.UserDetails.ToList();
            // Console.WriteLine(element);

            return View(element);
        }
        // GET: User
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginView model)
        {
            if (ModelState.IsValid)
            {
                // Fetch user details by email
                var userDetail = db.UserDetails.FirstOrDefault(u => u.Email == model.Email);

                if (userDetail != null)
                {
                    // Fetch password details using the user ID
                    var passwordDetail = db.PasswordDetails.FirstOrDefault(p => p.UserID == userDetail.Id);

                    // Check if password detail exists and verify the password
                    if (passwordDetail != null && BCrypt.Net.BCrypt.Verify(model.PasswordDetails, passwordDetail.Password))
                    {
                        // Successful login: redirect to index or set authentication
                        return RedirectToAction("Index", "User");
                    }
                }

                // If credentials are invalid, show an error message
                ViewBag.Message = "Invalid credentials. Please try again.";
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]

        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the user by email
                var userDetail = db.UserDetails.FirstOrDefault(u => u.Email == model.Email);

                if (userDetail != null)
                {
                    // Generate a reset token
                    var token = Guid.NewGuid().ToString(); 

                    // Prepare the reset password link
                    var urlHelper = new UrlHelper(Request.RequestContext);
                    SendResetPasswordEmail(model.Email, userDetail.Id, Request, urlHelper);

                    ViewBag.Message = "An email has been sent to your address with instructions to reset your password.";
                }
                else
                {
                    // Email not registered, add an error message
                    ModelState.AddModelError("Email", "This email address is not registered.");
                    // Optionally, set a message to be displayed
                    ViewBag.Message = "This email address is not registered.";
                }
            }

            return View(model);
        }



        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserDetail model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingUser = db.UserDetails.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    // Email already exists, add an error message
                    ModelState.AddModelError("Email", "Email already exists. Please try resetting your password.");
                    return View(model); // Return the model to display validation message
                }

                // Add new user if email does not exist
                db.UserDetails.Add(model);
                db.SaveChanges();

                // The ID is now available in model.Id after saving the entity to the database
                var userId = model.Id;

                // Call the email function to send email after registration
                var urlHelper = new UrlHelper(Request.RequestContext);
                SendResetPasswordEmail(model.Email, userId, Request, urlHelper);

                ViewBag.Message = "Data Inserted";
                return RedirectToAction("Login");
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = db.UserDetails.Where(x => x.Id == id).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(UserDetail model)
        {
            if (ModelState.IsValid)
            {


                var data = db.UserDetails.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.DOB = model.DOB;
                    data.FirstName = model.FirstName;
                    data.LastName = model.LastName;
                    data.Gender = model.Gender;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            //if fails
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var data = db.UserDetails.Where(x => x.Id == id).FirstOrDefault();
            db.UserDetails.Remove(data);
            db.SaveChanges();
            ViewBag.Message = " Data Deleted";
            return RedirectToAction("index");

        }
        public ActionResult Details(int id)
        {
            var data = db.UserDetails.Where(x => x.Id == id).FirstOrDefault();
            return View(data);
        }



        // Static function to send an email
        private static void SendResetPasswordEmail(string recipientEmail, int userId, HttpRequestBase request, UrlHelper urlHelper)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("mekalavamshi855@gmail.com"),
                    Subject = "Reset Password",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                // Generate a reset token (you can create your token logic here)
                string resetToken = Guid.NewGuid().ToString();
                string scheme = request.IsSecureConnection ? "https" : "http";
                string resetLink = urlHelper.Action("ResetPassword", "Home", new { userId = userId, token = resetToken }, scheme);

                mailMessage.Body = $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
        public ActionResult ResetPassword(int userId, string token)
        {
            // Find the user using the userId
            var userDetail = db.UserDetails.FirstOrDefault(u => u.Id == userId);

            if (userDetail == null)
            {
                ViewBag.Message = "Invalid user.";
                return View();
            }

            // You can also validate the token here if necessary

            // Return the view for the user to reset the password
            return View(userDetail);
        }


[HttpPost]
    public ActionResult ResetPassword(int userId, string token, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ViewBag.Message = "Passwords do not match.";
            return View();
        }

        var userDetail = db.UserDetails.FirstOrDefault(u => u.Id == userId);

        if (userDetail == null)
        {
            ViewBag.Message = "Invalid reset request.";
            return View();
        }

        // Update the user's password
        var passwordDetail = db.PasswordDetails.FirstOrDefault(p => p.UserID == userId);
        if (passwordDetail == null)
        {
            // Hash the new password before saving
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            passwordDetail = new PasswordDetail
            {
                UserID = userId,
                Password = hashedPassword
            };
            db.PasswordDetails.Add(passwordDetail);
        }
        else
        {
            // Hash the new password before updating
            passwordDetail.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        }

        db.SaveChanges();
        ViewBag.Message = "Password has been updated successfully.";

        return RedirectToAction("Login");
    }


}
}