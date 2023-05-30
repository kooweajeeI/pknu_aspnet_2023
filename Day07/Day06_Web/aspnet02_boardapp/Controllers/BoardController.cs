using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;

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

        // startcount = 1, 11, 21..
        // endcount = 10, 20, 30..
        public IActionResult Index(int page = 1)        // 게시판 최초 화면
        {
            // IEnumerable<Board> objBoardList = _db.boards.ToList();  // SELECT 쿼리
            // var objBoardList = _db.boards.FromSql($"SELECT * FROM boards").ToList();

            var totalCount = _db.boards.Count();
            var pageSize = 10;  // 게시판 한 페이지 10개씩 리스트
            var totalPage = totalCount / pageSize;

            if (totalCount % pageSize > 0) { totalPage++; }     // 나머지 페이지가 있으면 전체페이지 1증가

            var countPage = 10;
            var startPage = ((page - 1) / countPage) * countPage + 1;
            var endPage = startPage + countPage - 1;
            if (totalPage < endPage) endPage = totalPage;

            int startCount = ((page - 1) * countPage) + 1;
            int endCount = startCount + (pageSize - 1);

            // HTML화면에서 사용하기 위해서 선언 == ViewData, TempData 동일한 역할
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;

            var StartCount = new MySqlParameter("StartCount", startCount);
            var EndCount = new MySqlParameter("EndCount", endCount);

            var objNoteList = _db.boards.FromSql($"CALL NEW_PagingBoard({StartCount}, {EndCount})").ToList();

            return View(objNoteList);
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
            try
            {
                board.PostDate = DateTime.Now;  // 현재 저장하는 일시 할당
                _db.boards.Add(board);  // INSERT
                _db.SaveChanges();      // COMMIT

                TempData["succeed"] = "새 게시글이 저장되었습니다.";        // 성공메세지
            }
            catch (Exception)
            {
                TempData["error"] = "게시글 작성에 오류가 발생하였습니다.";        // 성공메세지
            }
            
            return RedirectToAction("Index", "Board");
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0) 
            {
                return NotFound();
            }

            var board = _db.boards.Find(Id);    // SELECT * FROM board WHERE Id = @id

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
            _db.boards.Update(board);   // update query 실행
            _db.SaveChanges();  // COMMIT

            TempData["succeed"] = "게시글이 수정되었습니다.";        // 성공메세지

            return RedirectToAction("Index", "Board");
        }

        [HttpGet]
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var board = _db.boards.Find(Id);    // SELECT * FROM board WHERE Id = @id

            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? Id)
        {
            var board = _db.boards.Find(Id);

            if (board == null)
            {
                return NotFound();
            }

            _db.boards.Remove(board);   // DELETE query
            _db.SaveChanges();  // COMMIT

            TempData["succeed"] = "게시글을 삭제했습니다.";        // 성공메세지

            return RedirectToAction("Index", "Board");
        }

        [HttpGet]
        public IActionResult Details(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var board = _db.boards.Find(Id);    // SELECT * FROM board WHERE Id = @id

            if (board == null)
            {
                return NotFound();
            }

            board.ReadCount++;
            _db.boards.Update(board);
            _db.SaveChanges();

            return View(board);
        }

    }
}
