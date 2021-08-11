class Aeropuerto extends Base {
	constructor(idAeropuerto = 0) {
		super('IdAeropuerto');
		this.IdAeropuerto = idAeropuerto;
		this.Valid = false;
		this.getInstance();
	}
	byAeropuerto(aeropuerto) {
		this.getInstance({ action: 'ByAeropuerto', value: aeropuerto });
	}
	static getAeropuertos() {
		var res = [];
		let url = `${this.Api + this.name}/`;
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
				console.error(`${_this.name}.getAeropuertos()`);
			}
		});
		return res;
	}
}