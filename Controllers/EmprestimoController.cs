using ClosedXML.Excel;
using EmprestimoLivros.Data;
using EmprestimoLivros.Models;
using EmprestimoLivros.Services.SessaoService;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EmprestimoLivros.Controllers
{
    public class EmprestimoController : Controller
    {
        readonly private AppDbContext _dbContext;
        readonly private ISessaoInterface _sessaoInterface;

        public EmprestimoController(AppDbContext dbContext, ISessaoInterface sessaoInterface)
        {
            _dbContext = dbContext;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult Index()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if(usuario == null)
            {
                return RedirectToAction("Login", "Login");

            }

            IEnumerable<EmprestimosModel> emprestimos = _dbContext.Emprestimos;
            return View(emprestimos);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");

            }

            return View();
        }

        [HttpGet]
        public IActionResult Editar(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");

            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            EmprestimosModel emprestimo = _dbContext.Emprestimos.FirstOrDefault(x => x.Id == id); 

            if(emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        [HttpGet]
        public IActionResult Excluir(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");

            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            EmprestimosModel emprestimo = _dbContext.Emprestimos.FirstOrDefault(x => x.Id == id);

            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }
        
        [HttpGet]
        public IActionResult Exportar()
        {
            var dados = GetDados();

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.AddWorksheet(dados, "Dados Empréstimos");

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spredsheetml.sheet", "Empréstimo.xls");
                }
            };
        }

        private DataTable GetDados()
        {
            DataTable dadatable = new DataTable();

            dadatable.TableName = "Dados empréstimos";
            dadatable.Columns.Add("Recebedor", typeof(string));
            dadatable.Columns.Add("Fornecedor", typeof(string));
            dadatable.Columns.Add("Livro", typeof(string));
            dadatable.Columns.Add("Data Empréstimo", typeof(DateTime));

            var dados = _dbContext.Emprestimos.ToList();

            if (dados.Count > 0)
            {
                dados.ForEach(emprestimos =>
                {
                    dadatable.Rows.Add(emprestimos.Recebedor, emprestimos.Fornecedor, emprestimos.LivroEmprestado, emprestimos.DataEmprestimo);
                });
            }

            return dadatable;
        }

        [HttpPost]
        public IActionResult Cadastrar(EmprestimosModel emprestimos)
        {
            if (ModelState.IsValid) 
            {
                emprestimos.DataEmprestimo = DateTime.Now;

                _dbContext.Emprestimos.Add(emprestimos);
                _dbContext.SaveChanges();

                TempData["MensagemSucesso"] = "Cadastro realizado com sucesso!";

                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Editar(EmprestimosModel emprestimos)
        {
            if (ModelState.IsValid)
            {
                var emprestimoDb = _dbContext.Emprestimos.Find(emprestimos.Id);

                emprestimoDb.Recebedor = emprestimos.Recebedor;
                emprestimoDb.Fornecedor = emprestimos.Fornecedor;
                emprestimoDb.LivroEmprestado = emprestimos.LivroEmprestado;

                _dbContext.Emprestimos.Update(emprestimoDb);
                _dbContext.SaveChanges();

                TempData["MensagemSucesso"] = "Edição realizada com sucesso!";

                return RedirectToAction("Index");
            }

            return View(emprestimos);
        }

        [HttpPost]
        public IActionResult Excluir(EmprestimosModel emprestimos)
        {
            if(emprestimos == null)
            {
                return NotFound();
            }

            _dbContext.Emprestimos.Remove(emprestimos);
            _dbContext.SaveChanges();

            TempData["MensagemSucesso"] = "Remoção realizada com sucesso!";

            return RedirectToAction("Index");
        }
    }
}
