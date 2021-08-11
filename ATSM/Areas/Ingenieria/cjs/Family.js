class Family extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	async byCadena(cadena) {
		let res = await fetch(`${(this._apiElement)}/ByCadena?cadena=${cadena}`);
		if (res.ok) {
			let post = await res.json();
			if (post.Data) {
				this.setValores(clonObject(post.Data));
				if (this.onGetInstance !== undefined && this.onGetInstance instanceof Function)
					this.onGetInstance();
			}
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	static async getFamilys() {
		let res = await fetch(`${(this.Api + this.name)}`);
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
}