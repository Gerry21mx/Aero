class Account extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	async byNombre(nombre, tipo) {
		let res = await fetch(`${this._apiElement}/ByNombre?nombre=${nombre}&tipo=${tipo}`);
		if (res.ok) {
			let post = await res.json();
			this.setValores(post.Data);
			if (this.onGetInstance !== undefined && this.onGetInstance instanceof Function) {
				this.onGetInstance();
			}
			return post;
		} else {
			console.log(`Error Fetched: ${ res.status } - ${ res.statusText }`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${ res.status } - ${ res.statusText }`, Data: null };
		}
	}
	async byUsuario(idusuario, tipo) {
		let res = await fetch(`${this._apiElement}/ByUsuario?idusuario=${idusuario}&tipo=${tipo}`);
		if (res.ok) {
			let post = await res.json();
			this.setValores(post.Data);
			if (this.onGetInstance !== undefined && this.onGetInstance instanceof Function) {
				this.onGetInstance();
			}
			return post;
		} else {
			console.log(`Error Fetched: ${ res.status } - ${ res.statusText }`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${ res.status } - ${ res.statusText }`, Data: null };
		}
	}
	static getAccounts() {
		var res = [];
		let url = `${this.Api + this.name}`;
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
				console.error(`${_this.name}.getAccounts()`);
			}
		});
		return res;
	}
	static getAccountsByTipo(tipo = 0) {
		var res = [];
		let url = `${this.Api + this.name}/${tipo}/ByTipo`;
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
				console.error(`${_this.name}.getAccountsByTipo()`);
			}
		});
		return res;
	}
}