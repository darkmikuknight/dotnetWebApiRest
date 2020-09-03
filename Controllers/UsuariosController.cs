using Microsoft.AspNetCore.Mvc;
using dotnetWebApiRest.Models;
using dotnetWebApiRest.Data;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;

namespace dotnetWebApiRest.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public UsuariosController(ApplicationDbContext database){
            this.database = database;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario){
            
            //Necessário validar algumas informações//

            database.Add(usuario);
            database.SaveChanges();
            return Ok(new {msg = "Usuário cadastrado com sucesso"});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credenciais){
            
            try{
                Usuario usuario = database.usuarios.First(User => User.Email.Equals(credenciais.Email));

                //Acho um usuário com cadastro válido
                if(usuario != null){

                    //Senha está correta
                    if(usuario.Senha.Equals(credenciais.Senha)){
                        
                        //Chave de segurança
                        string chaveDeSeguranca = "esse_sim_e_meu_teste_com_jwt";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciasDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("mail", usuario.Email.ToString()));
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                        var JWT = new JwtSecurityToken(
                            issuer: "teste_jwt", //Quem está fornecendo o jwt pro usuário
                            expires: DateTime.Now.AddHours(1),  //Tempo de validade do jwt
                            audience: "usuario_comum",
                            signingCredentials: credenciasDeAcesso,
                            claims: claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                        //return Ok("Login realizado com sucesso!");
                    }
                    else{
                        //Senha incorreta
                        Response.StatusCode = 401;
                        return new ObjectResult("");
                    }
                }
                else{
                    //Não existe usuáiro 
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                }
            }catch(Exception e){
                Response.StatusCode = 401;
                return new ObjectResult("");
            }

            
        }
    }
}