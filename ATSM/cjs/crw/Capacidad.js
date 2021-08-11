class Capacidad extends Base {
	constructor(idCapacidad = 0) {
		super('IdCapacidad');
		this.IdCapacidad = idCapacidad;
		this.Valid = false;
		this.getInstance();
	}
	byCapacidad(capacidad) {
		this.getInstance({ action: 'ByCapacidad', value: capacidad });
	}
	static async getCapacidades() {
		let res = await fetch(`${this.Api}${this.name}`, { method: 'GET', headers: { 'Content-Type': 'application/json' } });
		if (res.ok) {
			let post = await res.json();
			return post.Data;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	//		LocalDB
	//static readCrewsDB(arrayData) {
	//	if (indexedDB) {
	//		//var db;
	//		let request = indexedDB.open('iCrewDB', 1);
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