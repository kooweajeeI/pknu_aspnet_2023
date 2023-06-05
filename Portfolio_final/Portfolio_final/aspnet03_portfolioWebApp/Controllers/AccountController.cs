using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace aspnet02_boardapp.Controllers
{
    // 사용자 회원가입, 로그인, 로그아웃

    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            // (Alt+Enter ) -> 생성자 생성 -> Null 검사 추가 체크
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }


        [HttpGet]
        public IActionResult Register()
        {
            ViewData["NoScroll"] = "true";
            return View();
            // 화면만 띄우면 되기 때문에 다른 거 필요 없음
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 비동기 사용 안할 경우
        // public IActionResult Register(RegisterModel model) -> Return 값은 IActionResult
        // 비동기 사용할 경우 
        // Return 값은 Task<IActionResult>
        public async Task<IActionResult> Register(RegisterModel model)
        {
            ModelState.Remove("PhoneNumber"); // PhoneNumber는 입력값 검증에서 제거

            if (ModelState.IsValid) // 데이터를 제대로 입력해서 검증 성공하면 다음 단계 진행
            {
                // ASP.Net user- aspnetusers테이블에 데이터 넣기 위해 매핑되는 인스턴스 생성
                var user = new IdentityUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                // aspusers 테이블에 사용자 데이터를 대입
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 회원가입 성공 시 로그인한 뒤 홈 화면실행
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["succeed"] = "회원가입 성공";
                    // isPersistent -> 계속 로그인하고 있을 것인가? No(ex. 10분 경과시 자동 로그아웃 되는 설정)
                    return RedirectToAction("Index", "Home"); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model); // 회원가입 실패 시 해당 화면 그대로 유지
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["NoScroll"] = "true";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // 파라미터 : 아이디, 비밀번호, isPersistent = RememberMe, 실패 시 계정 잠그기
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // ToDo : 로그인 후 토스트메시지 띄우기
                    TempData["succeed"] = "로그인 성공";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "로그인 실패");
            }

            return View(model); // 입력 검증 안된 경우(실패한 경우) 해당 화면에서 대기
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            // ToDo : 로그아웃 후 토스트메시지 띄우기
            TempData["succeed"] = "로그아웃 성공";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string userName)
        {
            ViewData["NoScroll"] = "true";
            Debug.WriteLine(userName);

            var curUser = await _userManager.FindByNameAsync(userName);

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
                var user = await _userManager.FindByNameAsync(model.Email);

                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password); // 비밀번호 변경

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // 프로필변경 성공 시 로그인한 뒤 홈 화면실행
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["succeed"] = "프로필 변경 성공";
                    // isPersistent -> 계속 로그인하고 있을 것인가? No(ex. 10분 경과시 자동 로그아웃 되는 설정)
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model); // 프로필변경 실패 시 해당 화면 그대로 유지
        }
        

    }
}
