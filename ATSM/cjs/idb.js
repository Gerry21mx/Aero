const iDB = indexedDB;
if (iDB) {
	var db;
	const request = iDB.open('iAtsmDB', 1);
	request.onsuccess = () => {
		db = request.result;
		loadDataSeguimiento();
		loadDataCrews();
		loadDataCuentas();
		loadDataMtto();
	}
	request.onupgradeneeded = () => {
		db = request.result;
		createTablesCrew();
		createTablesSeguimiento();
		createTablesCuentas();
		createTablesMtto();
	}
	request.onerror = (error) => {
		console.log('Error: ', error);
	}
	//	Crews
	const createTablesCrew = () => {
		//	Tabla Crew
		var osCrews = db.createObjectStore('Crew', { keyPath: 'IdCrew' });
		osCrews.createIndex('Nombre', 'Nombre', { unique: false });
		osCrews.createIndex('IdCapacidad_1', 'IdCapacidad_1', { unique: false });
		osCrews.createIndex('IdCapacidad_2', 'IdCapacidad_2', { unique: false });
		osCrews.createIndex('IdCapacidad_3', 'IdCapacidad_3', { unique: false });
		osCrews.createIndex('Nivel_1', 'Nivel_1', { unique: false });
		osCrews.createIndex('Nivel_2', 'Nivel_2', { unique: false });
		osCrews.createIndex('Nivel_3', 'Nivel_3', { unique: false });
		osCrews.createIndex('Licencia', 'Licencia', { unique: false });

		//	Tabla Capacidad
		var osCapacidad = db.createObjectStore('Capacidad', { keyPath: 'IdCapacidad' });
		osCapacidad.createIndex('Nombre', 'Nombre', { unique: false });
	}
	const loadDataCrews = () => {
		Crew.getCrews().then((datos) => {
			var objectStore = db.transaction(['Crew'], 'readwrite').objectStore('Crew');
			datos.forEach((row, idx) => {
				let reqAdd = objectStore.put(row);
			});
		});
		Capacidad.getCapacidades().then((datos) => {
			var objectStore = db.transaction(['Capacidad'], 'readwrite').objectStore('Capacidad');
			datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
		});
	}
	//	Vuelos
	const createTablesSeguimiento = () => {
		//	Tabla Aeropuertos
		var osAeropuertos = db.createObjectStore('Aeropuerto', { keyPath: 'IdAeropuerto' });
		osAeropuertos.createIndex('Nombre', 'Nombre', { unique: false });
		osAeropuertos.createIndex('ICAO', 'ICAO', { unique: false });
		osAeropuertos.createIndex('IATA', 'IATA', { unique: false });
		osAeropuertos.createIndex('Pais', 'Pais', { unique: false });
		osAeropuertos.createIndex('Activo', 'Activo', { unique: false });
		//	Tabla Demora
		var osDemora = db.createObjectStore('Demora', { keyPath: 'IdDemora' });
		osDemora.createIndex('Codigo', 'Codigo', { unique: false });
		osDemora.createIndex('Activo', 'Activo', { unique: false });
		//	Tabla Ruta
		var osRuta = db.createObjectStore('Ruta', { keyPath: 'IdRuta' });
		osRuta.createIndex('Codigo', 'Codigo', { unique: false });
		osRuta.createIndex('Activo', 'Activo', { unique: false });
		//	Tabla TipoVuelo
		var osTipoVuelo = db.createObjectStore('TipoVuelo', { keyPath: 'IdTipo' });
		osTipoVuelo.createIndex('Descripcion', 'Descripcion', { unique: false });
		osTipoVuelo.createIndex('Activo', 'Activo', { unique: false });
	}
	const loadDataSeguimiento = () => {
		//	Aeropuertos
		var datos = Aeropuerto.getAeropuertos()
		var objectStore = db.transaction(['Aeropuerto'], 'readwrite').objectStore('Aeropuerto');
		datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
		//	Demoras
		datos = Demora.getDemoras();
		objectStore = db.transaction(['Demora'], 'readwrite').objectStore('Demora');
		datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
		//	Rutas
		datos = Ruta.getRutas();
		objectStore = db.transaction(['Ruta'], 'readwrite').objectStore('Ruta');
		datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
		//	Tipo de Vuelo
		datos = TipoVuelo.getTipos()
		objectStore = db.transaction(['TipoVuelo'], 'readwrite').objectStore('TipoVuelo');
		datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
	}
	//	Cuentas
	const createTablesCuentas = () => {
		//	Tabla Moneda
		var osMoneda = db.createObjectStore('Moneda', { keyPath: 'Id' });
		osMoneda.createIndex('Codigo', 'Codigo', { unique: false });
		//	Tabla Saldos
		var osSaldos = db.createObjectStore('Saldo', { keyPath: 'Id' });
		osSaldos.createIndex('Codigo', 'Codigo', { unique: false });
	}
	const loadDataCuentas = () => {
		//	Moneda
		var datos = Moneda.getMonedas()
		var objectStore = db.transaction(['Moneda'], 'readwrite').objectStore('Moneda');
		datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
		//	Saldo
		datos = Saldo.getSaldos();
		objectStore = db.transaction(['Saldo'], 'readwrite').objectStore('Saldo');
		datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
	}
	//	Mantenimiento
	const createTablesMtto = () => {
		//	Tabla Aeronave
		var osAeronave = db.createObjectStore('Aeronave', { keyPath: 'IdAeronave' });
		osAeronave.createIndex('Matricula', 'Matricula', { unique: false });
		osAeronave.createIndex('IdModelo', 'IdModelo', { unique: false });
		osAeronave.createIndex('Estado', 'Estado', { unique: false });
		osAeronave.createIndex('Empresa', 'Empresa', { unique: false });
	}
	const loadDataMtto = () => {
		//	Aeronave
		Aircraft.getAircrafts().then(datos => {
			var objectStore = db.transaction(['Aeronave'], 'readwrite').objectStore('Aeronave');
			datos.forEach((row, idx) => { let reqAdd = objectStore.put(row); });
		})
	} 
}