class Saldo extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	byCodigo(tipo) {
		this.getInstance({ action: 'ByCodigo', value: tipo });
	}
	static getSaldos() {
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
				console.error(`${_this.name}.getSaldos()`);
			}
		});
		return res;
	}
}