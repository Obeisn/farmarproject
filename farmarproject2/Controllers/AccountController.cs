using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using farmarproject2.Models;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;

namespace farmarproject2.Controllers
{

    public static class TempName
    {
        public static string name { get; set; }
        public static string username { get; set; }
        public static List<float> Chart1 { get; set; }
    }

    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private String TempUser = "";
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 這不會計算為帳戶鎖定的登入失敗
            // 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (Request.Cookies["MemberID"] == null)
                    {
                        HttpCookie Cookie = new HttpCookie("MemberID", model.Email);
                        Cookie.Expires = DateTime.Now.AddDays(1); //設置Cookie到期時間
                        HttpContext.Response.Cookies.Add(Cookie);
                    }
                    else
                    {
                        Request.Cookies.Remove("MemberID");
                        //--------
                        HttpCookie Cookie = new HttpCookie("MemberID", model.Email);
                        Cookie.Expires = DateTime.Now.AddDays(1); //設置Cookie到期時間
                        HttpContext.Response.Cookies.Add(Cookie);
                    }
                    global::farmarproject2.Models.farmarEntities1 farmar = new farmarEntities1();
                    TempName.name = farmar.AspNetUsers.Select(o => o).Where(c => c.Email == model.Email).SingleOrDefault().UserIg;
                    TempName.username = farmar.AspNetUsers.Select(o => o).Where(c => c.Email == model.Email).SingleOrDefault().FamName;
                    return RedirectToAction("homepage", "Home");

                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "登入失敗。");
                    return View(model);
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult FLogin(string returnUrl)
        {
            if (Request.Cookies["ServerID"] != null)
            {
                if (Request.Cookies["ServerID"].Value == "famar123")
                {
                    return RedirectToAction("Index", "Serice");
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult FLogin(ServiceAccount model, string returnUrl)
        {
            if (model.Account == null || model.Psword == null)
            {
                ModelState.AddModelError("", "登入失敗。");
                return View(model);
            }

            farmarEntities1 farmarEntities1 = new farmarEntities1();
            var tempPS = farmarEntities1.ServiceAccounts.Where(p => p.Account == model.Account).Select(a => a.Psword).SingleOrDefault();
            if (tempPS == model.Psword)
            {
                TempUser = model.Account;
                ServiceLogin service = new ServiceLogin()
                {
                    userID = TempUser,
                    LoginTime = DateTime.Now,
                    ContentA = $"{model.Account}登入",
                    State = "登入",
                    Type = "用戶"
                };
                farmarEntities1.ServiceLogins.Add(service);
                farmarEntities1.SaveChanges();
                TempData["TempName"] = model.Account;

                //存取cookie
                if (Request.Cookies["ServerID"] == null)
                {
                    HttpCookie Cookie = new HttpCookie("ServerID", model.Account);
                    Cookie.Expires = DateTime.Now.AddDays(1); //設置Cookie到期時間
                    HttpContext.Response.Cookies.Add(Cookie);
                }
                else
                {
                    Request.Cookies.Remove("ServerID");
                    //--------
                    HttpCookie Cookie = new HttpCookie("ServerID", model.Account);
                    Cookie.Expires = DateTime.Now.AddDays(1); //設置Cookie到期時間
                    HttpContext.Response.Cookies.Add(Cookie);
                }
                return RedirectToAction("Index", "Serice");
            }
            else
            {
                ModelState.AddModelError("", "登入失敗。");
                return View(model);
            }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 需要使用者已透過使用者名稱/密碼或外部登入進行登入
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 下列程式碼保護兩個因素碼不受暴力密碼破解攻擊。 
            // 如果使用者輸入不正確的代碼來表示一段指定的時間，則使用者帳戶 
            // 會有一段指定的時間遭到鎖定。 
            // 您可以在 IdentityConfig 中設定帳戶鎖定設定
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代碼無效。");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, HttpPostedFileBase File1)
        {
            ApplicationUser user;
            if (ModelState.IsValid)
            {
                var tempimg = "";

                if (File1 != null)
                {

                    string filejpg = Path.GetExtension(File1.FileName);
                    tempimg = $"{model.Email}{filejpg}";
                    string routeimg = Path.Combine(Server.MapPath("~/image"), $"{model.Email}{filejpg}");
                    File1.SaveAs(routeimg);
                    user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber, UserIg = tempimg, FamName = model.FamName };
                    TempName.name = tempimg;
                }
                else
                {
                    user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber, UserIg = "No-image-available.jpg", FamName = model.FamName };
                    TempName.name = "No-image-available.jpg";
                }
                List<string> mailname = new List<string>();
                mailname.Add(model.Email);
                TempName.username = model.FamName;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    SendMailByGmail(mailname, "碼農是好農註冊通知", MailMessage("會員註冊", $"恭喜您成為碼農是好農的一員～～～", user.FamName));
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                    // 傳送包含此連結的電子郵件
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "確認您的帳戶", "請按一下此連結確認您的帳戶 <a href=\"" + callbackUrl + "\">這裏</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // 不顯示使用者不存在或未受確認
                    return View("ForgotPasswordConfirmation");
                }

                // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                // 傳送包含此連結的電子郵件
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "重設密碼", "請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // 不顯示使用者不存在
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id,model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 要求重新導向至外部登入提供者
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 產生並傳送 Token
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 若使用者已經有登入資料，請使用此外部登入提供者登入使用者
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // 若使用者沒有帳戶，請提示使用者建立帳戶
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 從外部登入提供者處取得使用者資訊
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string id)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if (Request.Cookies["MemberID"] != null)
            {
                HttpCookie Cookie = new HttpCookie("MemberID");
                Cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(Cookie);
            }
            return RedirectToAction("Index", "Home");

        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            base.Dispose(disposing);
        }

        private void SendMailByGmail(List<string> MailList, string Subject, string Body)
        {
            MailMessage msg = new MailMessage();
            //收件者，以逗號分隔不同收件者 ex "test@gmail.com,test2@gmail.com"
            msg.To.Add(string.Join(",", MailList.ToArray()));
            msg.From = new MailAddress("msit119test@gmail.com", "碼農管理員", System.Text.Encoding.UTF8);
            //郵件標題 
            msg.Subject = Subject;
            //郵件標題編碼  
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            //郵件內容
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
            msg.Priority = MailPriority.Normal;//郵件優先級 
                                               //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
            #region 其它 Host
            /*
             *  outlook.com smtp.live.com port:25
             *  yahoo smtp.mail.yahoo.com.tw port:465
            */
            #endregion
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
            //設定你的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("msit119test@gmail.com", "lkk123456");
            //Gmial 的 smtp 使用 SSL
            MySmtp.EnableSsl = true;
            MySmtp.Send(msg);
        }

        public string MailMessage(string title, string content, string name)
        {
            string t = $"<table border='0' cellpadding='0' cellspacing='0' height='100%' style='min-width:348px' width='100%'><tbody><tr height='32px'></tr><tr align='center'><td><table border='0' cellpadding='0' cellspacing='0' style='max-width:600px'><tbody><tr><td><table bgcolor='#D94235' border='0' cellpadding='0' cellspacing='0' style='min-width:332px;max-width:600px;border:1px solid #f0f0f0;border-bottom:0;border-top-left-radius:3px;border-top-right-radius:3px'width='100%'><tbody><tr><td colspan='3' height='72px'></td></tr><tr><td width='32px'></td><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:24px;color:#ffffff;line-height:1.25;min-width:300px'>{title}</td><td width='32px'></td></tr><tr><td colspan='3' height='18px'></td></tr></tbody></table></td></tr><tr><td><table bgcolor='#FAFAFA' border='0' cellpadding='0' cellspacing='0' style='min-width:332px;max-width:600px;border:1px solid #f0f0f0;border-bottom:1px solid #c0c0c0;border-top:0;border-bottom-left-radius:3px;border-bottom-right-radius:3px' width='100%'><tbody><tr height='16px'><td rowspan='3' width='32px'></td><td></td><td rowspan='3' width='32px'></td></tr><tr><td><table border='0' cellpadding='0' cellspacing='0' style='min-width:300px'><tbody><tr><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5;padding-bottom:4px'>{name}您好:</td></tr><tr><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5;padding:4px 0'>{content}</td></tr><tr><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:13px;color:#202020;line-height:1.5;padding-top:28px'>碼農是好農敬上</td></tr><tr height='16px'></tr><tr><td><table style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:12px;color:#b9b9b9;line-height:1.5'><tbody><tr><td>請勿回覆這封電子郵件。如需詳細資訊，請聯絡管理員</td></tr></tbody></table></td></tr></tbody></table></td></tr><tr height='32px'></tr></tbody></table></td></tr><tr height='16'></tr><tr><td style='max-width:600px;font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:10px;color:#bcbcbc;line-height:1.5'></td></tr></tbody></table></td></tr></tbody></table>";
            return t;
        }

        #region Helper
        // 新增外部登入時用來當做 XSRF 保護
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}