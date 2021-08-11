class Aeronave extends Base {
	constructor(idAeronave = 0) {
		super('IdAeronave');
		this.IdAeronave = idAeronave;
		this.Valid = false;
		this.getInstance();
	}
	byMatricula(matricula) {
		this.getInstance({ action: 'ByMatricula', value: matricula });
	}
	async tripulacion() {
		let res = await fetch(`${this._apiElement}/Tripulacion?idaeronave=${this.IdAeronave}`, { method: 'GET', headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	async ultimoTramo() {
		let res = await fetch(`${this._apiElement}/UltimoTramo?idaeronave=${this.IdAeronave}`, { method: 'GET', headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	static getAeronaves() {
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
				console.error(`${_this.name}.GetAeronaves()`);
			}
		});
		return res;
	}
}