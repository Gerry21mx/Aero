using System;

using System.Web;
using System.Web.Http;

using WebMatrix.WebData;
using System.Net.Mail;
using System.Web.Security;
using System.Net;

namespace ATSM.Controllers.api.gen {
	public class UsuarioController : ApiController {
		private Answer answer = new Answer();
		private Respuesta respuesta = new Respuesta();

		// POST api/<controller>/Air
		[Route("api/Usuario/Air")]
		[HttpPost]
		public bool PostAir(dynamic rolename) {
			return Usuario.CurrentInRole(rolename.ToString());
		}

		// GET api/<controller>
		[Route("api/Usuario")]
		[HttpGet]
		public Answer Get(string area) {
			answer = Funciones.VRoles("Usuario");
			if (answer.Status) {
				var usuarios = Usuario.GetUsuarios(area);
				foreach(var u in usuarios) {
					u.GetPerfil();
				}
				answer.Data = usuarios;
			}
			return answer;
		}

		// GET api/<controller>/5
		public Answer Get(int id) {
			//answer = Funciones.VRoles("Usuario");
			//if (answer.Status) {
				answer.Data = new Usuario(id);
			//}
			return answer;
		}

		// GET api/<controller>/ufir
		[Route("api/Usuario/ufir")]
		public Answer GetUfir() {
			if(WebSecurity.IsAuthenticated)
				answer.Data = new Usuario(WebSecurity.CurrentUserId);
			return answer;
		}

		// POST api/<controller>
		[Route("api/Usuario")]
		[HttpPost]
		public Respuesta Post(Usuario iClase) {
			answer = Funciones.VRoles("cUsuario");
			if (answer.Status) {
				return iClase.Save();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// DELETE api/<controller>/5
		public Respuesta Delete(Usuario iClase) {
			answer = Funciones.VRoles("dUsuario");
			if (answer.Status) {
				return iClase.Delete();
			}
			respuesta.Error = answer.Message;
			return respuesta;
		}

		// GET api/<controller>/nickname/ByNickName
		[Route("api/Usuario/{nickname}/ByNickName")]
		public Answer GetByNickName(string nickname) {
			answer = Funciones.VRoles("Usuario");
			if (answer.Status) {
				answer.Data = new Usuario(nickname);
			}
			return answer;
		}

		// GET api/<controller>/IsAuthenticated
		[Route("api/Usuario/IsAuthenticated")]
		[HttpGet]
		public Answer GetIsAuthenticated() {
			answer.Data = WebSecurity.IsAuthenticated;
			return answer;
		}

		// GET api/<controller>/LogOff
		[Route("api/Usuario/LogOff")]
		[HttpGet]
		public Respuesta GetLogOff() {
			WebSecurity.Logout();
			respuesta.Valid = true;
			respuesta.Mensaje = "Hasta Pronto.";
			return respuesta;
		}

		// POST api/<controller>/usu/Recovery
		[Route("api/Usuario/{usu}/Recovery")]
		[HttpGet]
		public Respuesta GetRecovery(string usu) {
			respuesta.Error = $"No ha podido ser procesada la solicitud.<br>El Usuario <b>{usu}</b> no Existe.";
			if (!string.IsNullOrEmpty(usu)) {
				respuesta.Error = "";
				string correo = DataBase.QueryValue($"SELECT Correo FROM Usuario WHERE Nickname = '{usu}'").ToString();
				if (!string.IsNullOrEmpty(correo)) {
					string token = WebSecurity.GeneratePasswordResetToken(usu, 15);
					string link = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}/Login/Restablecer/Index?token={token}&usuario={usu}";
					MailMessage email = new MailMessage();
					email.To.Add(new MailAddress(correo));
					email.From = new MailAddress("desarrollo@aeronavestsm.com");
					email.Subject = "Recuperar Contraseña";
					email.SubjectEncoding = System.Text.Encoding.UTF8;
					email.Body = $"Se ha solicitado la Recuperacion de la Contraseña, <a href=\"{link}\" target=\"_blank\">Click Aqui</a> para recuperar tu contraseña de acceso al Sistema.<br>Si tu no has Solicitado la Recuperacion de la Contraseña, favor de Omitir este correo y notificarlo al Administrador del Sistema.";
					email.BodyEncoding = System.Text.Encoding.UTF8;
					email.IsBodyHtml = true;
					email.Priority = MailPriority.High;

					SmtpClient smtp = new SmtpClient();
					smtp.Host = "mail.aeronavestsm.com";
					smtp.Port = 26;
					smtp.EnableSsl = false;
					smtp.UseDefaultCredentials = false;
					smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
					smtp.Credentials = new NetworkCredential("desarrollo@aeronavestsm.com", "ITtsmac266.48#");

					try {
						smtp.Send(email);
						smtp.Dispose();
						email.Dispose();
						respuesta.Mensaje = "Si el usuario existe, se enviara un correo de recuperacion al correo registrado del Usuario.";
						respuesta.Valid = true;
					}
					catch (Exception ex) {
						respuesta.Error = "Error enviando correo electrónico: <br>" + ex.Message;
					}
				}
			}
			return respuesta;
		}

		// POST api/<controller>/InRole
		[Route("api/Usuario/InRole")]
		[HttpPost]
		public Answer PostInRole(dynamic datos) {
			var Datos = new { IdUsuario = (int)datos.IdUsuario, NickName = (string)datos.NickName, RolName = (string)datos.RolName };
			if (!string.IsNullOrEmpty(Datos.RolName)) {
				Usuario usu = new Usuario();
				if (Datos.IdUsuario > 0) {
					usu = new Usuario(Datos.IdUsuario);
				}
				if (!string.IsNullOrEmpty(Datos.NickName)) {
					usu = new Usuario(Datos.NickName);
				}
				if (usu.Valid) {
					answer.Data = usu.InRole(Datos.RolName);
				}
			}
			return answer;
		}

		// POST api/<controller>/LogMeIn
		[Route("api/Usuario/LogMeIn")]
		[HttpPost]
		public Answer PostLogMeIn(dynamic datos) {
			string msj = "";
			System.Web.HttpContext context = System.Web.HttpContext.Current;
			string ipAddress = "";// context.Request.ServerVariables["REMOTE_ADDR"];
			string nick = (string)datos.NickName;
			string psw = (string)datos.Password;
			if (!string.IsNullOrEmpty(nick) && !string.IsNullOrEmpty(psw)) {
				if (WebSecurity.Login(nick, psw)) {
					answer.Status = true;
					answer.Message = "Se ha iniciado sesion correctamente.";
					Usuario usu = new Usuario(nick);
					usu.SetLastLogin(ipAddress);
					answer.Data = new { logged = true, user = usu };
					return answer;
				}
				else {
					msj = "No se ha podido iniciar sesion.<br>Usuario o Contraseña Incorrectos";
				}
			}
			else {
				msj = "Falta informacion para el inicio de la sesion.<br>Falta Usuario o Contraseña.";
			}
			answer.Data = new { logged = false, Message = msj };
			return answer;
		}

		// POST api/<controller>/ResetPsw
		[Route("api/Usuario/ResetPsw")]
		[HttpPost]
		public Respuesta PostResetPsw(dynamic datos) {
			var jsn = new { usu = (string)datos.usu, token = (string)datos.token, psw = (string)datos.psw };
			respuesta.Mensaje = $"Falta Informacion para Cambiar el Password <i class=\"fas fa-bug fa-5x\"></i>";
			if (!string.IsNullOrEmpty(jsn.usu) && !string.IsNullOrEmpty(jsn.token) && !string.IsNullOrEmpty(jsn.psw)) {
				respuesta.Valid = WebSecurity.ResetPassword(jsn.token, jsn.psw);
				if (respuesta.Valid) {
					respuesta.Mensaje = $"<i class=\"fas fa-check-double fa-2x\"></i> La Contraseña ha sido Cambiada";
				}
				else {
					respuesta.Error = $"<i class=\"fas fa-bug fa-2x\"></i> Algo ha salido mal y la Contraseña no ha podido ser cambiada Intentelo Nuevamente.";
				}
			}
			return respuesta;
		}
	}
}
