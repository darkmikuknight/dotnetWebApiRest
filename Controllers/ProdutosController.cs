using Microsoft.AspNetCore.Mvc;
using dotnetWebApiRest.Models;
using System;
using dotnetWebApiRest.Data;
using System.Linq;

namespace dotnetWebApiRest.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public ProdutosController(ApplicationDbContext database){
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get(){

            var produtos =  database.Produtos.ToList();
            return Ok(produtos); // status code = 200
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                var produto = database.Produtos.First(p => p.Id == id );
                 return Ok(produto); // status code = 200

            }catch(Exception e){
                return BadRequest(new {msg = "Id inválido!"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]  ProdutoTemp pTemp){

            Produto p = new Produto();
            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;
            database.Produtos.Add(p);
            database.SaveChanges();

            Response.StatusCode = 201; //Nesse caso poderia deixa a resposta em branco ""
            return new ObjectResult(new {info = "Você registrou um novo produto", Produto = pTemp});
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id){

            try{
                Produto produto = database.Produtos.First(p => p.Id == id );
                database.Produtos.Remove(produto);
                database.SaveChanges();

                return Ok("Produto Deletado!"); // status code = 200

            }catch(Exception e){
                return BadRequest(new {msg = "Id inválido!"});
            }  
        }

        public class ProdutoTemp{
            public string Nome {get; set;}
            public float Preco {get; set;}
            
        }
    }
}