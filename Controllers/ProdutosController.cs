using Microsoft.AspNetCore.Mvc;
using dotnetWebApiRest.Models;
using System;
using dotnetWebApiRest.Data;
using System.Linq;
using dotnetWebApiRest.HATEOAS;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization; //teste para veririfcar se o push foi com meu nome  mais um teste */

namespace dotnetWebApiRest.Controllers
{
    [Route("v1/[controller]")] //Rotas podem ser usadas para versionar a api e manter o funcionamento de versões diferentes
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public ProdutosController(ApplicationDbContext database){
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/v1/Produtos");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
        }

        [HttpGet]
        public IActionResult Get(){

            var produtos =  database.Produtos.ToList();
            List<ProdutoContainer> produtosHATEOAS = new List<ProdutoContainer>();

            foreach(var prod in produtos){
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = prod;
                produtoHATEOAS.links = HATEOAS.GetActions(prod.Id.ToString());
                produtosHATEOAS.Add(produtoHATEOAS);
            }

            return Ok(produtosHATEOAS); // status code = 200
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id){

            try{
                Produto produto = database.Produtos.First(p => p.Id == id );
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = produto;
                produtoHATEOAS.links = HATEOAS.GetActions(produto.Id.ToString());

                return Ok(produtoHATEOAS); // status code = 200

            }catch(Exception e){
                return BadRequest(new {msg = "Id inválido!"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]  ProdutoTemp pTemp){

            //Validação dos dados//
            if(pTemp.Preco <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O preço do produto não pode ser menor ou igual a zero!"});
            }

            if(pTemp.Nome.Length <= 1){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O nome do produto tem que ser maior que 1 caractere!"});
            }

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

        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produto){
            
            if(produto.Id < 0){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id do produto é inválido"});
            }
            else{
                try{

                    var p = database.Produtos.First(pTemp => pTemp.Id == produto.Id); 

                    if(p != null){

                        p.Nome = produto.Nome != null ? produto.Nome : p.Nome;
                        p.Preco = produto.Preco > 0 ? produto.Preco : p.Preco;

                        database.SaveChanges();
                        return Ok();

                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Produto não encontrado!"}); 
                    }
                
                }catch(Exception e){
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Produto não encontrado!"}); 
                }
            }
        }

        [HttpGet("teste")]
        public IActionResult Teste(){
            return Ok(HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase)).Value);
        }

        public class ProdutoTemp{
            public string Nome {get; set;}
            public float Preco {get; set;}
            
        }

        public class ProdutoContainer{
            public Produto produto {get; set;}
            public Link[] links {get; set;}
        }
    }
}