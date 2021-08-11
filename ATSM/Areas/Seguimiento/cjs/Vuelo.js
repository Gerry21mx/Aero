class Vuelo extends Base {
	constructor(idVuelo = 0) {
		super('IdVuelo');
		this.IdVuelo = idVuelo;
		this.Valid = false;
		this.getInstance();
	}
	byTrip(trip) {
		this.getInstance({ action: 'ByTrip', value: trip });
	}
	close() {
		$.ajax({
			type: "PUT",
			url: `${this._apiElement}/Close`,
			data: JSON.stringify({ idVuelo: this.IdVuelo }),
			dataType: 'json',
			contentType: "application/json; charset=utf-8",
			success: (response) => {
				let res = response;
			},
			error: (jqXHR, textStatus, errorThrown) => {
				console.error(`Vuelo Close Error: ${jqXHR.responseText}`);
			}
		});
	}
	reOpen() {
		$.ajax({
			type: "PUT",
			url: `${this._apiElement}/ReOpen`,
			data: JSON.stringify({ idVuelo: this.IdVuelo }),
			dataType: 'json',
			contentType: "application/json; charset=utf-8",
			success: (response) => {
				let res = response;
			},
			error: (jqXHR, textStatus, errorThrown) => {
				console.error(`Vuelo Close Error: ${jqXHR.responseText}`);
			}
		});
	}
	static async getVuelos(filtro) {
		let res = await postAction(`${this.Api + this.name}/Vuelos`, filtro);
		return res;
	}
	static async reporte(filtro) {
		let res = await postAction(`${this.Api + this.name}/Reporte`, filtro);
		return res;
	}
}
class VueloTramo extends Base {
	constructor(idTramo = 0) {
		super('IdTramo');
		this.IdTramo = idTramo;
		this.Valid = false;
		this.getInstance();
	}
	byPierna(pierna) {
		this.getInstance({ action: 'ByPierna', value: `${this.IdVuelo}/${pierna}` });
	}
	static async getTramosVuelo(idVuelo) {
		let res = await getAction(`${this.Api + this.name}/${idVuelo}/Tramos`);
		return res;
	}
	static async timeline(filtro) {
		let res = await postAction(`${this.Api + this.name}/TimeLine`, filtro);
		return res;
	}
}
class VueloDemora extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
	}
	static async getDemorasTramo(idTramo) {
		let res = await getAction(`${this.Api + this.name}/${idTramo}/ByTramo`);
		return res;
	}
}