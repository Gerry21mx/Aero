class Ruta extends Base {
	constructor(idRuta = 0) {
		super('IdRuta');
		this.IdRuta = idRuta;
		this.Valid = false;
		this.getInstance();
	}
	byCodigo(codigo) {
		this.getInstance({ action: 'ByCodigo', value: codigo });
	}
	static getRutas() {
		var res = [];
		let url = `${this.Api + this.name}/`;
		$.ajax({
			async: false,
			url: url,
			dataType: 'json',
			type: 'GET',
			contentType: "application/json; charset=utf-8",
			success: function (data) {
				if (data.Data !== null) {
					res = data.Data;
				}
			},
			error: function (x, y, z) {
				console.error(`${_this.name}.getTipos()`);
			}
		});
		return res;
	}
}
class RutaTramo extends Base {
	constructor(idRutaTramo = 0) {
		super('IdRutaTramo');
		this.IdRutaTramo = idRutaTramo;
		this.Valid = false;
		this.getInstance();
	}
	byPierna(pierna) {
		//this.getInstance({ action: 'ByPierna', value: `${this.IdRuta}/${pierna}` });
		this.getInstance({ action: '', value: `${this.IdRuta}-${pierna}` });
	}
	static getTramos(idRuta) {
		var res = [];
		let url = `${this.Api + this.name}/${idRuta}/Tramos`;
		var _this = this;
		$.ajax({
			async: false,
			url: url,
			dataType: 'json',
			type: 'GET',
			contentType: "application/json; charset=utf-8",
			success: function (data) {
				if (data.Data !== null) {
					res = data.Data;
				}
			},
			error: function (x, y, z) {
				console.error(`${_this.name}.getTramos()`);
			}
		});
		return res;
	}
}