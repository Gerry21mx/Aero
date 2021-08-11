class Transaccion extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	static getTransacciones(cuenta, movimiento) {
		var res = [];
		let url = `${this.Api + this.name}/Transaccions`;
		var _this = this;
		$.ajax({
			async: false,
			url: url,
			dataType: 'json',
			type: 'POST',
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify({ idcuenta: cuenta, idmovimiento: movimiento }),
			success: function (data) {
				if (data.Data !== null) {
					res = data.Data;
				}
			},
			error: function (x, y, z) {
				console.error(`${_this.name}.getTransaccions()`);
			}
		});
		return res;
	}
}