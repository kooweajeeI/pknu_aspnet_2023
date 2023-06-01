using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace aspnet02_boardapp.Controllers
{
    // 사용자 회원가입, 로그인, 로그아웃
    public class AccountController : Controller
    {      
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            // 생성자 마법사로 만드세요.
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        // https://localhost:7066/Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // public IActionResult Register(RegisterModel model)
        // 비동기가 아니면 return 값은 IActionResult
        // 비동기가 되면 Task<IActionResult>
        public async Task<IActionResult> Register(RegisterModel model)
        {
            ModelState.Remove("PhoneNumber");  // PhoneNumber는 입력값 검증에서 제거
            if (ModelState.IsValid)  // 데이터를 제대로 입력해서 검증 성공하면
            {
                // ASP.NET user - aspnetusers 테이블에 데이터 넣기 위해서
                // 매핑되는 인스턴스를 생성
                var user = new IdentityUser { 
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber  // 핸드폰 번호 추가
                };

                // aspnetusers 테이블에 사용자 데이터를 대입
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 회원가입을 성공했으면 로그인한 뒤 localhost:7066/Home/Index로 가라
                    await signInManager.SignInAsync(user, isPersistent: false);
                    // TODO : 회원가입 후 토스트메시지 띄우기
                    TempData["succeed"] = "회원가입 성공했습니다.";  // 성공 메시지
                    return RedirectToAction("Index", "Home"); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);  // 회원가입을 실패하면 그 화면 그대로 유지
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // 파라미터 순서 : 아이디, 패스워드, isPersistent = RememberMe, 실패할 때 계정 잠그기
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // TODO : 로그인 후 토스트메시지 띄우기
                    TempData["succeed"] = "로그인했습니다.";  // 성공 메시지
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "로그인 실패!");
            }

            return View(model);  // 입력검증이 실패하면 화면에 그대로 대기
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // TODO : 로그아웃 후 토스트메시지 띄우기
            await signInManager.SignOutAsync();  // 로그아웃
            TempData["succeed"] = "로그아웃했습니다.";  // 성공 메시지
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string userName)
        {
            Debug.WriteLine(userName);

            var curUser = await userManager.FindByNameAsync(userName);

            if (curUser == null)
            {
                TempData["error"] = "사용자가 없습니다.";
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterModel
            {
                Email = curUser.Email,
                PhoneNumber = curUser.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, model.Password);  // 비밀번호 변경은 어렵다!!              
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    TempData["succeed"] = "프로필변경 성공했습니다."; 
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);  // 프로필 변경 실패하면 화면유지.
        }
    }
}
