class JumpSeat extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	async tramoNmbre(idtramo, nombre) {
		let res = await fetch(`${this._apiElement}`, { method: 'GET', body: { idtramo: idtramo, nombre: nombre }, headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	static async  getJumpSeats(idtramo) {
		let res = await fetch(`${this.Api + this.name}/ByTramo`, { method: 'GET', body: { idtramo: idtramo }, headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
}