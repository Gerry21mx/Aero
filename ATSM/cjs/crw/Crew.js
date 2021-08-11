class Crew extends Base {
	constructor(idCrew = 0) {
		super('IdCrew');
		this.IdCrew = idCrew;
		this.Valid = false;
		this.getInstance();
	}
	static async getCrews(idcapacidad, capacidad) {
		var res = [];
		let datos = {
			idCapacidad: typeof (idcapacidad) === 'number' ? idcapacidad : parseInt(idcapacidad) || 0,
			capacidad: (capacidad === undefined ? typeof (idcapacidad) === 'string' ? idcapacidad : '' : capacidad)
		};
		let data = await postAction(`${this.Api + this.name}`, datos);
		res = data.Data;
		return res;
	}
	//		LocalDB
	//static readCrewsDB(arrayData) {
	//	if (indexedDB) {
	//		//var db;
	//		let request = indexedDB.open('iAtsmDB', 1);
	//		request.onsuccess = () => {
	//			let db = request.result;
	//			let transaction = db.transaction(['Crew']);
	//			let objectStore = transaction.objectStore('Crew');
	//			let requestOC = objectStore.openCursor();
	//			requestOC.onsuccess = (e) => {
	//				const cursor = e.target.result
	//				if (cursor) {
	//					arrayData.push(cursor.value);
	//					cursor.continue();
	//				}
	//			}
	//		}
	//	}
	//}
}