using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using ATSM.Almacen;
using ATSM.Cuentas;
using ATSM.Ingenieria;
using ATSM.Seguimiento;
//using ATSM.Tripulaciones;

using WebMatrix.WebData;

namespace ATSM {
	public static class Campos {
		public static IHtmlString SelectUnidadMedida(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdUnidadMedida" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Unidad de Medidad" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<UnidadMedida> umes = UnidadMedida.GetUnidadMedidas();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num{clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-codigo=\"\" data-nombre=\"\" data-mayores=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var ume in umes) {
				if (!ume.Activo)
					continue;
				select += $"<option value=\"{ume.Id}\" data-codigo=\"{ume.Codigo}\" data-descripcion=\"{ume.Descripcion}\">{ume.Codigo}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoIdUnidadMedida\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectCategoria(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdCategoria" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Categoria" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Categoria> categorias = Categoria.GetCategorias();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num{clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-codigo=\"\" data-nombre=\"\" data-mayores=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var cat in categorias) {
				if (!cat.Activo)
					continue;
				select += $"<option value=\"{cat.Id}\" data-codigo=\"{cat.Codigo}\" data-descripcion=\"{cat.Descripcion}\">{cat.Codigo}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoIdCategoria\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectPosition(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdPosicion" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Posicion" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Position> posiciones = Position.GetPositions();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num{clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-codigo=\"\" data-nombre=\"\" data-mayores=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var pos in posiciones) {
				if (!pos.Activo)
					continue;
				select += $"<option value=\"{pos.Id}\" data-codigo=\"{pos.Codigo}\" data-nombre=\"{pos.Nombre}\" data-mayores=\"{pos.Mayores}\">{pos.Codigo}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoPosicion\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectAircraft(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdAircraft" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Avion" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Aircraft> avion = Aircraft.GetAircrafts();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num{clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-matricula=\"\" data-idempresa=\"\" data-estado=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var avi in avion) {
				if (!avi.Estado)
					continue;
				Modelo mod = new Modelo();
				avi.SetMayores();
				avi.SetModelo();
				select += $"<option value=\"{avi.Id}\" data-matricula=\"{avi.Matricula}\" data-idempresa=\"{avi.IdEmpresa}\" data-estado=\"{avi.Estado}\" data-idmodelo=\"{avi.Modelo.Id}\" data-modelo=\"{avi.Modelo.Nombre}\" data-idcapacidad=\"{avi.Modelo.IdCapacidad}\">{avi.Matricula}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoAircraft\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectEmpresa(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdEmpresa" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Empresa" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Empresa> empresas = Empresa.GetEmpresas();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-nombre=\"\" data-codigovuelo=\"\" data-tipo=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var emp in empresas) {
				select += $"<option value=\"{emp.IdEmpresa}\" data-nombre=\"{emp.Nombre}\" data-codigovuelo=\"{emp.CodigoVuelo}\" data-tipo=\"{emp.Tipo}\">{emp.Nombre}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoEmpresa\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectMonedas(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdMoneda" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Moneda" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			var demoras = Moneda.GetMonedas();
			List<Moneda> tipos = Moneda.GetMonedas();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" >{(label ? "" : labelTag)}...</option>";
			foreach (var mon in tipos) {
				select += $"<option value=\"{mon.Id}\" data-codigo=\"{mon.Codigo}\" data-nombre=\"{mon.Nombre}\">{mon.Codigo}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoMoneda\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectAccountTypes(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdTipo" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Tipo" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			var demoras = AccountType.GetAccountTypes();
			List<AccountType> tipos = AccountType.GetAccountTypes();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" >{(label ? "" : labelTag)}...</option>";
			foreach (var sal in tipos) {
				select += $"<option value=\"{sal.Id}\">{sal.Nombre}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoAccountType\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectSaldos(bool label = false, string id = "", string clases = "", bool? combustibles = false, string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdSaldo" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Saldo" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Saldo> saldos = Saldo.GetSaldos();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" >{(label ? "" : labelTag)}...</option>";
			foreach (var sal in saldos) {
				if (sal.Combustible != combustibles && combustibles != null) {
					continue;
				}
				select += $"<option value=\"{sal.Id}\" data-codigo=\"{sal.Codigo}\" data-nombre=\"{sal.Nombre}\" data-combustible=\"{sal.Combustible}\">{sal.Nombre}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoSaldo\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectCrews(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false, int? nivel = null) {
			id = string.IsNullOrEmpty(id) ? "IdCrew" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Crew" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			var crews = Tripulaciones.Crew.GetCrews(true);
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" >{(label ? "" : labelTag)}...</option>";
			foreach (var cap in crews) {
				if (nivel != null) {
					cap.SetCapacidades();
					if (nivel == 1 && (cap.Nivel_1 != 1 && cap.Nivel_2 != 1 && cap.Nivel_3 != 1)) {
						continue;
					}
					if (nivel == 2 && (cap.Nivel_1 != 2 && cap.Nivel_2 != 2 && cap.Nivel_3 != 2)) {
						continue;
					}
				}
				select += $"<option value=\"{cap.IdCrew}\">{cap.Nombre}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoCrew\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectCapacidad(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdCapacidad" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Capacidad" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Capacidad> capacidades = Capacidad.GetCapacidades();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-nombre=\"\" data-descripcion=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var cap in capacidades) {
				select += $"<option value=\"{cap.Id}\" data-nombre=\"{cap.Nombre}\" data-descripcion=\"{cap.Descripcion}\">{cap.Nombre}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoCapacidad\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectModelo(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdModelo" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Modelo" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Modelo> modelos = Modelo.GetModelos();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-nombre=\"\" data-fabricante=\"\" data-idcapacidad=\"\" data-idcomponentemayor=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var mod in modelos) {
				if (!mod.Activo)
					continue;
				select += $"<option value=\"{mod.Id}\" data-nombre=\"{mod.Nombre}\" data-fabricante=\"{mod.Fabricante}\" data-idcapacidad=\"{mod.IdCapacidad}\" data-idcomponentemayor=\"{mod.IdComponenteMayor}\">{mod.Nombre}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoModelo\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectFamily(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdFamily" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Familia" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Family> familias = Family.GetFamilys(2);
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-nombre=\"\" data-fabricante=\"\" data-idcapacidad=\"\" data-idcomponentemayor=\"\" data-tm01=\"\" data-tm02=\"\" data-tm03=\"\">{(label ? "" : labelTag)}...</option>";
			string comp = "<optgroup label=\"Componentes\">";
			string adsb = "<optgroup label=\"AS's/SB's\">";
			string srv = "<optgroup label=\"Servicios\">";
			foreach (var fam in familias) {
				if (!fam.Activo)
					continue;
				if (fam.TM01 == true) {
					comp += $"<option value=\"{fam.Id}\" data-codigo=\"{fam.Codigo}\" data-nombre=\"{fam.Nombre}\" data-tm01=\"{fam.TM01}\" data-tm02=\"{fam.TM02}\" data-tm03=\"{fam.TM03}\">{fam.Nombre}</option>";
				}
				if (fam.TM02 == true) {
					adsb += $"<option value=\"{fam.Id}\" data-codigo=\"{fam.Codigo}\" data-nombre=\"{fam.Nombre}\" data-tm01=\"{fam.TM01}\" data-tm02=\"{fam.TM02}\" data-tm03=\"{fam.TM03}\">{fam.Nombre}</option>";
				}
				if (fam.TM03 == true) {
					srv += $"<option value=\"{fam.Id}\" data-codigo=\"{fam.Codigo}\" data-nombre=\"{fam.Nombre}\" data-tm01=\"{fam.TM01}\" data-tm02=\"{fam.TM02}\" data-tm03=\"{fam.TM03}\">{fam.Nombre}</option>";
				}
			}
			comp += "</optgroup>";
			adsb += "</optgroup>";
			srv += "</optgroup>";
			select += comp + adsb + srv;
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoFamily\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectLimites(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdLimites" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Limites" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<Limites> limites = Limites.GetLimites();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-codigo=\"\" data-definicion=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var lim in limites) {
				select += $"<option value=\"{lim.Id}\" data-codigo=\"{lim.Codigo}\" data-definicion=\"{lim.Definicion}\">{lim.Codigo} - {lim.Definicion}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoLimites\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectComponentesMayores(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false, bool longDes = false) {
			id = string.IsNullOrEmpty(id) ? "IdComponenteMayor" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Componente Mayor" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			List<ComponenteMayor> comay = ComponenteMayor.GetComponentesMayores();
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" data-codigo=\"\" data-descripcion=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var cm in comay) {
				select += $"<option value=\"{cm.Id}\" data-codigo=\"{cm.Codigo}\" data-descripcion=\"{cm.Descripcion}\">{(longDes?cm.Descripcion:cm.Codigo)}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoComponentesMayores\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectDemora(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdDemora" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Demora" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "Demora" : clases;
			var demoras = Demora.GetDemoras();
			RespuestaQuery Clasificacion = DataBase.Query(new SqlCommand("SELECT DISTINCT LTRIM(RTRIM(Clasificacion)) AS Clasificacion FROM Demora WHERE Activo=1 ORDER BY Clasificacion", DataBase.Conexion()));
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\" codigo=\"\">{(label ? "" : labelTag)}...</option>";
			foreach (var cla in Clasificacion.Rows) {
				string cdem = (string)cla.Clasificacion;
				//if (string.IsNullOrEmpty(cdem)) { continue; }
				select += $"<optgroup label=\"{cla.Clasificacion}\">";
				foreach (var dem in demoras) {
					if (dem.Activo && dem.Clasificacion.Trim() == cdem ) {
						select += $"<option value=\"{dem.IdDemora}\" codigo=\"{dem.Codigo}\">{dem.Codigo} - {dem.Descripcion}</option>";
					}
				}
				select += $"</optgroup>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoDemora\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectAeropuerto(bool label = false, string id = "", string clases = "", string labelTag = null, bool info = false, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdAeropuerto" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Aeropuerto" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			RespuestaQuery Tipos = DataBase.Query(new SqlCommand("SELECT IdAeropuerto,Nombre, ICAO, IATA FROM Aeropuerto WHERE Activo=1 ORDER BY IATA", DataBase.Conexion()));
			string select = (label || info) ? $"<div class=\"input-group input-group-sm\">" : "";
			if (label) {
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\">{(label ? "" : labelTag)}...</option>";
			foreach (var pro in Tipos.Rows) {
				select += $"<option value=\"{pro.IdAeropuerto}\" data-nombre=\"{pro.Nombre}\" data-icao=\"{pro.ICAO}\" data-iata=\"{pro.IATA}\">{pro.IATA}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<button class=\"btn btn-sm btn-outline-primary\" type=\"button\" tipo=\"infoAeropuerto\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></button>";
			}
			select += (label || info) ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectTipoVuelo(bool label = false, string id = "", string clases = "", string labelTag = null, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdTipoVuelo" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Tipo Vuelo" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			RespuestaQuery Tipos = DataBase.Query(new SqlCommand("SELECT IdTipo,Descripcion FROM TipoVuelo WHERE Activo=1 ORDER BY Descripcion", DataBase.Conexion()));
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\">{(label ? "Seleccionar" : labelTag)}...</option>";
			foreach (var pro in Tipos.Rows) {
				select += $"<option value=\"{pro.IdTipo}\">{pro.Descripcion}</option>";
			}
			select += $"</select> ";
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectRuta(bool label = false, string id = "", string clases = "", bool info = false, string labelTag = null, bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdRuta" : id;
			labelTag = string.IsNullOrEmpty(labelTag) ? "Ruta" : labelTag;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			RespuestaQuery Tipos = DataBase.Query(new SqlCommand("SELECT IdRuta, Codigo, Descripcion FROM Ruta WHERE Activo=1 ORDER BY Codigo", DataBase.Conexion()));
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\">{(label ? "Seleccionar" : labelTag)}...</option>";
			foreach (var pro in Tipos.Rows) {
				select += $"<option value=\"{pro.IdRuta}\">{pro.Codigo}</option>";
			}
			select += $"</select> ";
			if (info) {
				select += $"<label class=\"input-group-text btn btn-sm btn-primary\" tipo=\"infoRuta\" id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></label>";
			}
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		public static IHtmlString SelectPerfil(bool label = false, string id = "", string clases = "", bool required = false) {
			id = string.IsNullOrEmpty(id) ? "IdPerfil" : id;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			RespuestaQuery Perfiles = DataBase.Query(new SqlCommand("SELECT IdPerfil,Nombre FROM Perfil WHERE Activo=1 ORDER BY Nombre", DataBase.Conexion()));
			string select = "";
			if (label) {
				select += $"<div class=\"input-group input-group-sm\">";
				select += $"<label class=\"input-group-text\" for=\"{ id }\">Perfil</label>";
			}
			select += $"<select class=\"form-select num {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
			select += $"<option selected value=\"0\">{(label ? "Seleccionar" : "Perfil")}...</option>";
			foreach (var pro in Perfiles.Rows) {
				select += $"<option value=\"{pro.IdPerfil}\">{pro.Nombre}</option>";
			}
			select += $"</select> ";
			select += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(select);
			return html;
		}
		/// <summary>
		/// Genera un Select (Lista Desplegable) con las Aeronaves Activas
		/// </summary>
		/// <param name="label">Especifica si se incluye un Label para el Campo o No</param>
		/// <param name="id">Id del Campo</param>
		/// <param name="value">Define el Valor a Asignar en los Options del Select 1.- IdAeronave(Default) 2.- Matricula</param>
		/// <param name="clases">Clases CSS que se deseen Agregar al Select</param>
		/// <param name="required">Especifica si el campo es Required o No</param>
		/// <returns></returns>
		//public static IHtmlString SelectAeronave(bool label = false, string id = "IdAeronave", int value = 1, string clases = "", string labelTag = null, bool info = false, bool required = false) {
		//	clases = string.IsNullOrEmpty(clases) ? "" : clases;
		//	labelTag = string.IsNullOrEmpty(labelTag) ? "Aeronave" : labelTag;
		//	RespuestaQuery Aeronaves = DataBase.Query(new SqlCommand("SELECT IdAeronave, Matricula FROM Aeronave WHERE Estado=1 ORDER BY Matricula", DataBase.Conexion("BD_MTTO")));
		//	string select = "";
		//	if (label) {
		//		select += $"<div class=\"input-group input-group-sm\">";
		//		select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
		//	}
		//	select += $"<select class=\"form-select {(value == 1 ? "num" : "")} {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
		//	select += $"<option selected value=\"{(value == 1 ? "0" : "")}\">{(label ? "Seleccionar" : labelTag)}...</option>";
		//	foreach (var avi in Aeronaves.Rows) {
		//		select += $"<option value=\"{(value == 1 ? avi.IdAeronave : avi.Matricula)}\">{avi.Matricula}</option>";
		//	}
		//	select += $"</select> ";
		//	select += label ? $"</div>" : "";
		//	IHtmlString html = MvcHtmlString.Create(select);
		//	return html;
		//}
		//public static IHtmlString ModeloAeronave(bool label = false, string id = "IdModelo", int value = 1, string clases = "", string labelTag = null, bool info = false, bool required = false) {
		//	clases = string.IsNullOrEmpty(clases) ? "" : clases;
		//	labelTag = string.IsNullOrEmpty(labelTag) ? "Aeronave" : labelTag;
		//	RespuestaQuery Aeronaves = DataBase.Query(new SqlCommand("SELECT IdAeronave, Matricula FROM Aeronave WHERE Estado=1 ORDER BY Matricula", DataBase.Conexion("BD_MTTO")));
		//	string select = "";
		//	if (label) {
		//		select += $"<div class=\"input-group input-group-sm\">";
		//		select += $"<label class=\"input-group-text\" for=\"{ id }\">{labelTag}</label>";
		//	}
		//	select += $"<select class=\"form-select {(value == 1 ? "num" : "")} {clases}\" id=\"{ id }\" {(required ? "required" : "")}>";
		//	select += $"<option selected value=\"{(value == 1 ? "0" : "")}\">{(label ? "Seleccionar" : labelTag)}...</option>";
		//	foreach (var avi in Aeronaves.Rows) {
		//		select += $"<option value=\"{(value == 1 ? avi.IdAeronave : avi.Matricula)}\">{avi.Matricula}</option>";
		//	}
		//	select += $"</select> ";
		//	select += label ? $"</div>" : "";
		//	IHtmlString html = MvcHtmlString.Create(select);
		//	return html;
		//}
		public static IHtmlString Switch(string id = null, string oAct = null, string oInact = null, string LabelFija = null, bool defa = false) {
			id = string.IsNullOrEmpty(id) ? "Activo" : id;
			if (oAct == null) {
				oAct = id;
			}
			if (oInact == null) {
				if (id.IndexOf("Activo") > -1) {
					oInact = "Inactivo";
				}
			}
			string cadena = "<div class=\"form-check form-switch\">";
			cadena += $"<input type=\"checkbox\"{(defa?" checked='true'":"")} class=\"form-check-input\" id=\"{id}\" data-activo=\"{oAct}\" data-inactivo=\"{oInact}\" data-fija=\"{(string.IsNullOrEmpty(LabelFija) ? "false" : "true")}\">";
			cadena += $"<label class=\"form-check-label\" for=\"{id}\">{(string.IsNullOrEmpty(LabelFija) ? defa? oAct: (oInact ?? oAct) : LabelFija)}</label>";
			cadena += $"</div>";
			return MvcHtmlString.Create(cadena);
		}
		public static IHtmlString GetInput(string id, bool label = false, int maxLength = 0, string placeHolder = "", string clases = "", string type = "", string value = "", bool enabled = true, bool required = false, string txtLabel = "", string min = null, string max = null, bool info = false) {
			id = string.IsNullOrEmpty(id) ? "" : id;
			clases = string.IsNullOrEmpty(clases) ? "" : clases;
			type = string.IsNullOrEmpty(type) ? "text" : type;
			placeHolder = string.IsNullOrEmpty(placeHolder) ? id : placeHolder;
			txtLabel = string.IsNullOrEmpty(txtLabel) ? placeHolder : txtLabel;
			string cadena = "";
			if (label) {
				cadena += $"<div class=\"input-group input-group-sm\">";
				cadena += $"<span class=\"input-group-text\">{txtLabel}</span>";
			}
			string strMaxLength = maxLength > 0 ? $"maxlength=\"{maxLength}\"" : "";
			string strMin = min != null ? $"min=\"{min}\"" : "";
			string strMax = max != null ? $"max=\"{max}\"" : "";
			string strRequire = required ? "required" : "";
			string strValue = string.IsNullOrEmpty(value) ? "" : $"value=\"{value}\"";
			cadena += $"<input type=\"{type}\" id=\"{ id }\" list=\"list{ id }\" class=\"form-control form-control-sm {clases}\" placeholder=\"{placeHolder}\" {strMaxLength} {strMin} {strMax} {strValue} {strRequire} {(!enabled ? "disabled" : "")} >";
			if (info) {
				cadena += $"<span class=\"input-group-text btn btn-sm btn-primary\" data-tipo=\"informacion\" data-id=\"i{id}\"><i class=\"fas fa-info-circle\"></i></span>";
			}
			cadena += label ? $"</div>" : "";
			IHtmlString html = MvcHtmlString.Create(cadena);
			return html;
		}
		public static IHtmlString BtnReg(string id = null, string roleName = null, bool? enabled = null) {
			if (roleName != null) {
				if (!Usuario.CurrentInRole(roleName) && WebSecurity.CurrentUserId != 1) {
					return MvcHtmlString.Create("");
				}
			}
			id = string.IsNullOrEmpty(id) ? "reg" : id;
			bool deshabilitado = enabled ?? true;
			string cadena = $"<button id=\"{id}\" class=\"btn btn-primary btn-sm w-100\" {(!deshabilitado ? "disabled" : "")}><i class=\"fas fa-save\"></i> Registrar</button>";
			return MvcHtmlString.Create(cadena);
		}
		public static IHtmlString BtnDel(string id = null, string roleName = null, bool? enabled = null) {
			if (roleName != null) {
				if (!Usuario.CurrentInRole(roleName) && WebSecurity.CurrentUserId!=1) {
					return MvcHtmlString.Create("");
				}
			}
			id = string.IsNullOrEmpty(id) ? "del" : id;
			bool deshabilitado = enabled ?? true;
			string cadena = $"<button id=\"{id}\" class=\"btn btn-danger btn-sm w-100\" {(!deshabilitado ? "disabled" : "")}><i class=\"far fa-trash-alt\"></i> Eliminar</button>";
			return MvcHtmlString.Create(cadena);
		}
		public static IHtmlString BtnClo(string id = null, string roleName = null, bool? enabled = null) {
			if (roleName != null) {
				if (!Usuario.CurrentInRole(roleName) && WebSecurity.CurrentUserId != 1) {
					return MvcHtmlString.Create("");
				}
			}
			id = string.IsNullOrEmpty(id) ? "clo" : id;
			bool deshabilitado = enabled ?? true;
			string cadena = $"<button id=\"{id}\" class=\"btn btn-success btn-sm w-100\" {(!deshabilitado ? "disabled" : "")}><i class=\"fas fa-door-closed\"></i> Cerrar</button>";
			return MvcHtmlString.Create(cadena);
		}
		public static IHtmlString BtnCan(string id = null, string roleName = null, bool? enabled = null) {
			if (roleName != null) {
				if (!Usuario.CurrentInRole(roleName) && WebSecurity.CurrentUserId != 1) {
					return MvcHtmlString.Create("");
				}
			}
			id = string.IsNullOrEmpty(id) ? "cancelar" : id;
			bool deshabilitado = enabled ?? true;
			string cadena = $"<button id=\"{id}\" class=\"btn btn-secondary btn-sm w-100\" {(!deshabilitado ? "disabled" : "")}><i class=\"fas fa-ban\"></i> Cancelar</button>";
			return MvcHtmlString.Create(cadena);
		}
		public static IHtmlString BtnCle(string id = null, bool? enabled = null) {
			id = string.IsNullOrEmpty(id) ? "cle" : id;
			bool deshabilitado = enabled ?? true;
			string cadena = $"<button id=\"{id}\" class=\"btn btn-outline-primary btn-sm w-100\" {(!deshabilitado ? "disabled" : "")}>Limpiar <i class=\"fas fa-broom\"></i></button>";
			return MvcHtmlString.Create(cadena);
		}
	}
}