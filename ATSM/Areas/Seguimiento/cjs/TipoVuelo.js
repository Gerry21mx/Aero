class TipoVuelo extends Base {
	constructor(idTipo = 0) {
		super('IdTipo');
		this.IdTipo = idTipo;
		this.Valid = false;
		this.getInstance();
	}
	byTipo(tipo) {
		this.getInstance({ action: 'ByTipo', value: tipo });
	}
	static getTipos() {
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
				console.error(`${_this.name}.getTipos()`);
			}
		});
		return res;
	}
}