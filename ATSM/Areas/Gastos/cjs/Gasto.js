class Gasto extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	async byVuelo(idvuelo, idaccount) {
		//let res = await postAction(`${this._apiElement}/byVuelo`, filtro);
		let res = await fetch(`${this._apiElement}/byVuelo`, { method: 'GET', body: { idvuelo: idvuelo, idaccount: idaccount }, headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
}