using EmprestimoLivros.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EmprestimoLivros.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
        {
        }

        public DbSet<EmprestimosModel> Emprestimos { get; set; }          
        public DbSet<UsuarioModel> Usuarios { get; set; }
    }
}
