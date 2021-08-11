using System;
using System.Data.SqlClient;

namespace ATSM {
	public class Nodo {
		public string id { get; set; }
		public string parent { get; set; }
		public string text { get; set; }
		public string icon { get; set; }
		public StateNode state { get; set; }
		//public List<Nodo> children = new List<Nodo>();
		public dynamic li_attr { get; set; }
		public dynamic a_attr { get; set; }
		public Nodo(int roleId) {
			Inicializar();
			if (roleId > 0) {
				SqlCommand comando = new SqlCommand("SELECT * FROM webpages_Roles WHERE RoleId=@rid", DataBase.Conexion());
				comando.Parameters.AddWithValue("@rid", roleId);
				var res = DataBase.Query(comando);
				if (res.Valid) {
					var reg = res.Row;
					id = reg.RoleId;
					parent = reg.Padre;
					text = reg.Descripcion;
				}
			}
		}
		public Nodo(string Id = "", string Parent = "#", string Text = "", string Icon = "") {
			Inicializar();
			id = Id;
			parent = Parent;
			text = Text;
			icon = Icon;
		}
		public Nodo(int Id = 0, int Parent = 0, string Text = "", string Icon = "") {
			Inicializar();
			id = Convert.ToString(Id);
			parent = Parent > 0 ? Convert.ToString(Parent) : "#";
			text = Text;
			icon = Icon;
		}
		private void Inicializar() {
			id = "";
			parent = "";
			text = "";
			icon = "";
			state = new StateNode();
			li_attr = new { };
			a_attr = new { };
		}
	}

	public class StateNode {
		public bool opened { get; set; }
		public bool disabled { get; set; }
		public bool selected { get; set; }
		public StateNode(bool ope = false, bool dis = false, bool sel = false) {
			opened = ope;
			disabled = dis;
			selected = sel;
		}
	}
}