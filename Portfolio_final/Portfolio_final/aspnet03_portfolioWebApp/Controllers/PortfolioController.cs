using aspnet02_boardapp.Data;
using aspnet03_portfolioWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace aspnet03_portfolioWebApp.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _db;

        // 파일업로드용 웹환경을 위한 객체 (필수)
        private readonly IWebHostEnvironment _environment;

        public PortfolioController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["NoScroll"] = "true";
            var list = _db.Portfolios.ToList(); // SELECT *
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["NoScroll"] = "true";
            // PortfolioModel이 아닌 TempPortfolioModel을 선택해야 함
            return View();
        }

        [HttpPost]
        public IActionResult Create(TempPortfolioModel temp)
        {
            // 파일 업로드, Temp에서 원래 있는 Model로 변경 후 DB 저장
            if (ModelState.IsValid)
            {
                // 파일 업로드되면 새로운 이미지 파일명 받음
                string upFileName = UploadImageFile(temp);

                // TempPortfolioModel에서 실제 DB처리하기 위한 PortfolioModel로 변경
                var portfolio = new PortfolioModel()
                {
                    Division = temp.Division,
                    Title = temp.Title,
                    Description = temp.Description,
                    Url = temp.Url,
                    FileName = upFileName // 핵심
                };

                _db.Portfolios.Add(portfolio);
                _db.SaveChanges();

                TempData["succeed"] = "포트폴리오 저장완료";
                return RedirectToAction("Index", "Portfolio");
            }

            return View(temp);
        }

        // Routing이나 Get/Post랑 관계 없음
        private string UploadImageFile(TempPortfolioModel temp)
        {
            var resultFileName = "";
            try
            {
                if (temp.PortfolioImage != null)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads"); // wwwroot 밑에 uploads라는 폴더 존재
                    resultFileName = Guid.NewGuid() + "_" + temp.PortfolioImage.FileName; // 중복 이미지 파일명 제거
                    string filePath = Path.Combine(uploadFolder, resultFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        temp.PortfolioImage.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
            return resultFileName;
        }
    }
}
