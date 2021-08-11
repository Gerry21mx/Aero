using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class Status {
		public int Id { get; set; }
		public string Nombre { get; set; }
		public Status(int? id = null) {
			Id = id ?? 0;
			switch (id) {
				case 1:
				Nombre = "Open";
				break;
				case 2:
				Nombre = "Once";
				break;
				case 3:
				Nombre = "Term";
				break;
				case 4:
				Nombre = "Repetitive";
				break;
				case 5:
				Nombre = "Superceded";
				break;
				default:
				Id = 0;
				Nombre = "";
				break;
			}
		}
	}
}