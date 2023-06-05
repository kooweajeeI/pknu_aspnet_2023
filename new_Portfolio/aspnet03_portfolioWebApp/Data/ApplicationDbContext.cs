using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using aspnet02_boardapp.Models;
using Microsoft.EntityFrameworkCore;
using aspnet03_portfolioWebApp.Models;

namespace aspnet02_boardapp.Data
{
    public class ApplicationDbContext : IdentityDbContext  
        // 1. ASP.Net Identity - DbContext에서 IdentityDbContext로 변경(DbContext 쓰는 것과 동일함)
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Board> Boards { get; set; }

        // 포트폴리오 DB로 관리하기 위한 모델
        public DbSet<PortfolioModel> Portfolios { get; set; }

        //public DbSet<aspnet03_portfolioWebApp.Models.TempPortfolioModel>? TempPortfolioModel { get; set; }
    }
}
