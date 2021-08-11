using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ATSM.Cuentas;

using Newtonsoft.Json;

namespace ATSM.Areas.Cuentas.Controllers.api
{
    public class AccountController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// GET api/<controller>
		public Answer Get() {
			answer.Data = Account.GetAccounts(null);
			return answer;
		}
		// GET api/<controller>/5
		public Answer Get(int id) {
			answer.Data = new Account(id);
			return answer;
		}
		// GET api/<controller>/5/ByTipo
		[Route("api/Account/{tipo}/ByTipo")]
		public Answer GetByTipo(int? tipo) {
			answer.Data = Account.GetAccounts(tipo);
			return answer;
		}
		// GET api/<controller>/Bancos
		[Route("api/Account/Bancos")]
		public string[] GetBancos() {
			SqlCommand comando = new SqlCommand("SELECT DISTINCT Banco FROM Account",DataBase.Conexion());
			var res = DataBase.Query(comando);
			string[] bancos = new string[res.Rows.Count];
			int i = 0;
			foreach(var b in res.Rows) {
				bancos[i] = b.Banco;
				i++;
			}
			return bancos;
		}

		// GET api/<controller>/ByNombre
		[Route("api/Account/ByNombre")]
		public Answer Get(string nombre, int tipo)
		{
			answer.Data = new Account(nombre, tipo);
			return answer;
		}

		// GET api/<controller>/ByUsuario
		[Route("api/Account/ByUsuario")]
		public Answer Get(int idusuario, int tipo)
		{
			answer.Data = new Account(idusuario, tipo);
			return answer;
		}

		//POST api/<controller>
		public Respuesta Post(dynamic datos) {
			Account iClase = JsonConvert.DeserializeObject<Account>(JsonConvert.SerializeObject(datos));
			iClase.GetTipo();
			answer = Funciones.VRoles($"c{iClase.Tipo.RoleName}");
			if (answer.Status) {
				respuesta = iClase.Save();
				if (respuesta.Valid) {
					string nick = datos.Nickname.ToString();
					string pswr = datos.Password.ToString();
					if (!string.IsNullOrEmpty(nick) && !string.IsNullOrEmpty(iClase.Correo) && !string.IsNullOrEmpty(iClase.Nombre)) {
						Perfil per = new Perfil("Gastos");
						Usuario usu = new Usuario(nick);
						if (usu.Valid) {
							iClase.SetUsuario(usu.IdUsuario);
						}
						else if (!string.IsNullOrEmpty(pswr))
						{
							usu.Nombre = iClase.Nombre;
							usu.Correo = iClase.Correo;
							usu.Telefono = iClase.Celular;
							//usu.IdAccount = iClase.Id;
							usu.IdPerfil = per.IdPerfil;
							usu.Activo = true;
							usu.Nickname = nick;
							usu.Password = pswr;
							var rcu=usu.Save();
							if(!rcu.Valid || !string.IsNullOrEmpty(rcu.Error))
								respuesta.Error += rcu.Error;
							else
								iClase.SetUsuario(usu.IdUsuario);
						}
					}
				}
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Account iClase)
		{
			iClase.GetTipo();
			answer = Funciones.VRoles($"d{iClase.Tipo.RoleName}");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}
	}
}
