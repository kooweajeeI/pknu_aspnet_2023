using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnet02_boardapp.Controllers
{
    // https://localhost:7281/Board/Index
    public class BoardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BoardController(ApplicationDbContext db)
        {
            _db = db;       // 알아서 DB가 연결
        }

        public IActionResult Index()        // 게시판 최초 화면
        {
            IEnumerable<Board> objBoardList = _db.boards.ToList();  // SELECT 쿼리
            return View(objBoardList);
        }

        // https://localhost:7281/Board/Create

        public IActionResult Create()       // 게시판 글쓰기
        {
            return View();
        }

        // Submit이 발생해서 내부로 데이터를 전달 액션

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Board board)
        {
            _db.boards.Add(board);  // INSERT
            _db.SaveChanges();      // COMMIT

            return RedirectToAction("Index", "Board");
        }
    }
}
