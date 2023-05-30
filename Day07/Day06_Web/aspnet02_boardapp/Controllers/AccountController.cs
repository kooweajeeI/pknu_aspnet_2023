using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace aspnet02_boardapp.Controllers
{
    // 사용자 회원가입, 로그인, 로그아웃
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            // Null 검사 추가 체크
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        // public IActionResult Register(RegisterModel model)
        // 비동기가 아니면 return 값은 IActionResult
        // 비동기가 되면 Task<IActionResult>
        public async Task<IActionResult> Register (RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // ASP.NET user - aspnetusers 테이블에 데이터 넣기 위해서
                // 매핑되는 인스턴스 생성
                var user = new IdentityUser()
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                // aspnetusers 테이블에 사용자 데이터를 대입
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 회원가입을 성공했으면 로그인 한 뒤 localhost/Home/Index
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model); // 회원가입 실패하면 그 화면 그대로 유지
        }
    }
}
