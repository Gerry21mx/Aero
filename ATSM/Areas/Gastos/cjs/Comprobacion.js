class Comprobacion extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	async save(documentos) {
		procc();
		let fData = new FormData();
		fData.append('comprobacion', JSON.stringify(this));
		documentos.forEach(d => {
			if (d.pdf !== undefined)
				fData.append(`i_fd_${d.indice}`, d.pdf);
			if (d.xml !== undefined)
				fData.append(`i_fe_${d.indice}`, d.xml);
		});
		let res = await fetch(`${this._apiElement}`, { method: 'POST', body: fData });
		endPro();
		if (res.ok) {
			let post = await res.json();
			this.setValores(post);
			return post;
		}
		else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	async byVuelo(idvuelo, idaccount) {
		let res = await fetch(`${this._apiElement}/byVuelo?idvuelo=${idvuelo}&idaccount=${idaccount}`, { method: 'GET', headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	static async pendientes(idaccount) {
		let res = { ok: false, status: 'Parametro Nulo', statusText: 'IdAccount' };
		if (idaccount !== undefined && idaccount !== null)
			res = await fetch(`${this.Api + this.name}/Pendientes?idaccount=${(idaccount === null ? '' : idaccount)}`, { method: 'GET', headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	static async comprobaciones(idaccount) {
		let res = { ok: false, status: 'Parametro Nulo', statusText: 'IdAccount' };
		if (idaccount !== undefined && idaccount !== null)
			res = await fetch(`${this.Api + this.name}/Comprobaciones?idaccount=${idaccount}`, { method: 'GET', headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
}