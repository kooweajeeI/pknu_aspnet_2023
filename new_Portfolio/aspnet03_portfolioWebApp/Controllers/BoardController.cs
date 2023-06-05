using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace aspnet02_boardapp.Controllers
{
    public class BoardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BoardController(ApplicationDbContext db)
        {
            _db = db; // 알아서 DB가 연결됨
        }

        // 게시판 최초화면 리스트
        // startcount = 1, 11, 21, 31, ...
        // endcount = 10, 20, 30, 41, ...
        public IActionResult Index(int page = 1)
        {
            ViewData["NoScroll"] = "true"; // 게시판은 메인스크롤이 안생김

            // EntityFramework로 작업하는 방식
            // IEnumerable<Board> objBoardList = _db.Boards.ToList(); // SELECT * 쿼리

            // SQL 쿼리로 작업하는 방식
            // var objBoardList = _db.Boards.FromSql($"SELECT * FROM boards").ToList();
            
            // 제일 첫번째 페이지, 제일 마지막 페이지
            var totalCount = _db.Boards.Count(); // 12
            var pageSize = 10; // 게시판 한페이지 10개씩 리스트
            var totalPage = totalCount / pageSize; // 1

            if (totalCount % pageSize > 0) { totalPage++; } // 2

            // 제일 첫번째 페이지, 제일 마지막 페이지
            var countPage = 10;
            var startPage = ((page - 1) / countPage) * countPage + 1; // 12데이터일때 1페이지 startPage = 1
            var endPage = startPage + countPage - 1; // 10
            if (totalPage < endPage) endPage = totalPage; // 10

            int startCount = ((page - 1) * countPage) + 1; // 1, 11
            int endCount = startCount + (pageSize - 1);  // 10, 20

            // HTML화면에서 사용하기 위해서 선언 == ViewData, TempData 동일한 역할
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.StartCount = startCount; // 230525. 게시판 번호를 위해 추가

            var StartCount = new MySqlParameter("startCount", startCount);
            var EndCount = new MySqlParameter("endCount", endCount);

            var objBoardList = _db.Boards.FromSql($"CALL New_PagingBoard({StartCount}, {EndCount})").ToList();

            return View(objBoardList);
        }

        // https://localhost:7177/Board/Create
        // GetMethod로 화면을 URL로 부를때 쓰는 액션

        // 게시판 글쓰기
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["NoScroll"] = "true";

            return View();
        }

        // submit이 발생해서 내부로 데이터를 전달하는 액션
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Board board)
        {
            board.PostDate = DateTime.Now; // 현재 저장하는 일시를 할당

            _db.Boards.Add(board); // INSERT
            _db.SaveChanges(); // COMMIT

            TempData["succeed"] = "새 게시글이 저장되었습니다."; // 성공메시지

            return RedirectToAction("Index", "Board");
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            ViewData["NoScroll"] = "true";

            if (Id == null || Id == 0)
            {
                return NotFound(); // Error.cshtml이 표시됨
            }

            var board = _db.Boards.Find(Id); // SELECT * FROM board WHERE Id = @id

            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Board board)
        {
            board.PostDate = DateTime.Now;
            _db.Boards.Update(board); // UPDATE query 실행
            _db.SaveChanges(); // COMMIT

            TempData["succeed"] = "게시글이 수정되었습니다."; // 성공메시지

            return RedirectToAction("Index", "Board");

        }

        [HttpGet]
        public IActionResult Delete(int? Id)
        {
            ViewData["NoScroll"] = "true";

            // HttpGet Edit Action의 로직과 동일
            if (Id == null || Id == 0)
            {
                return NotFound(); // Error.cshtml이 표시됨
            }

            var board = _db.Boards.Find(Id); // SELECT * FROM board WHERE Id = @id

            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        [HttpPost]
        public IActionResult DeletePost(int? Id)
        {
            var board = _db.Boards.Find(Id);

            if (board == null)
            {
                return NotFound();
            }

            _db.Boards.Remove(board); // DELETE 쿼리
            _db.SaveChanges(); // COMMIT

            TempData["succeed"] = "게시글이 삭제되었습니다."; // 성공메시지

            return RedirectToAction("Index", "Board");
        }

        [HttpGet]
        public IActionResult Details(int? Id)
        {
            ViewData["NoScroll"] = "true";

            // HttpGet Edit Action의 로직과 동일
            if (Id == null || Id == 0)
            {
                return NotFound(); // Error.cshtml이 표시됨
            }

            var board = _db.Boards.Find(Id); // SELECT * FROM board WHERE Id = @id

            if (board == null)
            {
                return NotFound();
            }

            // 조회수 1 증가, update, commit
            board.ReadCount++;
            _db.Boards.Update(board);
            _db.SaveChanges();

            return View(board);
        }
    }
}
