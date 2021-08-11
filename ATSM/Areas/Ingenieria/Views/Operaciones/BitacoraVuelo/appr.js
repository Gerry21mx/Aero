var avion, apto, bitacora, tramo, parametro, rcca, tbTramos, tbRCCA;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	bitacora = new Bitacora(id);
	tramo = new BitacoraTramo();
	parametro = new BitacoraParametrosMotor();
	rcca = new BitacoraRCCA();
	avion = new Aircraft();
	apto = new Aeropuerto();
	if (bitacora.Valid)
		bitacora.write();
	tablas();
	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', registrar);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	document.getElementById('Aircraft').addEventListener('blur', valAvi)
	document.querySelectorAll('#Folio,#Aircraft').forEach(e => { e.addEventListener('blur', consulta); })
	document.querySelectorAll('#aOrigen,#aDestino').forEach(ap => {
		ap.addEventListener('blur', (e) => {
			apto.clear();
			if (e.target.id === 'aOrigen') {
				tramo.IdOrigen = 0;
				tramo.Origen = apto.clon();
			}
			else {
				tramo.IdDestino = 0;
				tramo.Destino = apto.clon();
			}
			document.querySelectorAll('#listApto option').forEach(node => {
				if (node.value.toUpperCase() === e.target.value.toUpperCase()) {
					apto.Id = parseInt(node.dataset.id) || 0;
					apto.IATA = node.value;
					e.target.value = node.value;
					return apto;
				}
			});
			if (apto.Id > 0) {
				if (e.target.id === 'aOrigen') {
					tramo.IdOrigen = apto.Id;
					tramo.Origen = apto.clon();
				}
				else {
					tramo.IdDestino = apto.Id;
					tramo.Destino = apto.clon();
				}
			}
			else
				e.target.value = '';
			apto.clear();
		})
	})
	document.getElementById('FechaSalida').addEventListener('blur', (e) => {
		tramo.Fecha = e.target.value;
		valTimes();
	});
	document.querySelectorAll('#tTramosD input[type="time"]').forEach(t => {
		t.addEventListener('blur', (e) => {
			tramo[e.target.id] = e.target.value;
			e.target.value = tramo[e.target.id];
			valTimes();
		})
	})
	document.getElementById('addTr').addEventListener('click', addTra)
	document.getElementById('addRvsm').addEventListener('click', addRvsm)
});
const valAvi = (e) => {
	let cAvi = document.getElementById('Aircraft')
	avion.clear();
	let idCap = 0;
	document.querySelectorAll('#listAircraft option').forEach(node => {
		if (node.value.toUpperCase() === cAvi.value.toUpperCase()) {
			avion.Id = parseInt(node.dataset.id) || 0;
			avion.Matricula = node.value;
			idCap = parseInt(node.dataset.idcapacidad) || 0;
			cAvi.value = node.value;
			return avion;
		}
	});
	if (avion.Id > 0) {
		bitacora.IdAircraft = avion.Id;
		bitacora.Avion = avion.clon();
		document.querySelectorAll('#IdComandante option, #IdPrimerOficial option').forEach(o => {
			if (o.value === '0') return;
			o.classList.add('d-none');
			if ((parseInt(o.dataset.cap1) || 0) === idCap || (parseInt(o.dataset.cap2) || 0) === idCap || (parseInt(o.dataset.cap3) || 0) === idCap)
				o.classList.remove('d-none');
		})
	}
	else
		cAvi.value = '';
}
const valTimes = () => {
	if (tramo.Fecha === null || tramo.Fecha === '') return;
	let duracion;
	if (tramo.SalidaPlataforma !== null && tramo.SalidaPlataforma !== '' && tramo.LlegadaPlataforma !== null && tramo.LlegadaPlataforma !== '') {
		duracion = moment.duration(moment(tramo.Fecha + ' ' + tramo.LlegadaPlataforma).diff(moment(tramo.Fecha + ' ' + tramo.SalidaPlataforma)));
		tramo.Calzo = moment.utc(duracion.as('milliseconds')).format('HH:mm');
		document.getElementById('Calzo').value = tramo.Calzo;
		if (moment.duration(tramo.Calzo).hours() >= 10)
			MsjBox('Alerta de Tiempo de Vuelo','El tiempo Volado Excede las 10 Horas, Revisa Los Tiempos.')
	}
	if (tramo.Despegue !== null && tramo.Despegue !== '' && tramo.Aterrizaje !== null && tramo.Aterrizaje !== '') {
		duracion = moment.duration(moment(tramo.Fecha + ' ' + tramo.Aterrizaje).diff(moment(tramo.Fecha + ' ' + tramo.Despegue)));
		tramo.Tiempo = moment.utc(duracion.as('milliseconds')).format('HH:mm');
		document.getElementById('Tiempo').value = tramo.Tiempo;
		if (moment.duration(tramo.Calzo).hours() >= 10)
			MsjBox('Alerta de Tiempo de Vuelo', 'El tiempo Volado Excede las 10 Horas, Revisa Los Tiempos.')
	}
}
const addTra = (e) => {
	tramo.read('', 'tTramosD');
	tramo.Fecha = document.getElementById('FechaSalida').value;
	if (tramo.IdOrigen <= 0) return;
	if (tramo.IdDestino <= 0) return;
	if (tramo.Fecha === null || tramo.Fecha === '') return;
	if (tramo.SalidaPlataforma === null || tramo.SalidaPlataforma === '') return;
	if (tramo.LlegadaPlataforma === null || tramo.LlegadaPlataforma === '') return;
	if (tramo.Despegue === null || tramo.Despegue === '') return;
	if (tramo.Aterrizaje === null || tramo.Aterrizaje === '') return;
	valTimes();

	let sp = moment(tramo.Fecha + ' ' + tramo.SalidaPlataforma);
	let lp = moment(tramo.Fecha + ' ' + tramo.LlegadaPlataforma);
	lp = lp <= sp ? lp.add(1, 'days') : lp;
	let ds = moment(tramo.Fecha + ' ' + tramo.Despegue);
	let at = moment(tramo.Fecha + ' ' + tramo.Aterrizaje);
	at = at <= ds ? at.add(1, 'days') : at;
	if (ds < sp || ds > lp || ds >= at) {
		MsjBox('Error de Tiempos', 'El Despegue Debe estar <b>Entre</b> la Salida - Llegada / Plataforma y ser <b>Menor</b> al Aterrizaje')
		return;
	}
	if (at <= sp || at > lp || at <= ds) {
		MsjBox('Error de Tiempos', 'El Aterrizaje Debe estar <b>Entre</b> la Salida - Llegada / Plataforma y ser <b>Mayor</b> al Despegue')
		return;
	}
	let limInf = null;
	let idx = bitacora.Tramos.findIndex(t => { return t.Tramo === tramo.Tramo; });
	if (idx > 0)
		limInf = moment(bitacora.Tramos[idx - 1].Fecha + ' ' + bitacora.Tramos[idx - 1].LlegadaPlataforma);
	else if (bitacora.Tramos.length > 0)
		limInf = moment(bitacora.Tramos[bitacora.Tramos.length - 1].Fecha + ' ' + bitacora.Tramos[bitacora.Tramos.length - 1].LlegadaPlataforma);
	if (limInf !== null && (sp < limInf || lp < limInf || ds < limInf || at < limInf)) {
		MsjBox('Error de Tiempos', `Inconsistencia en los tiempos, no pueden ser menores a <b>${limInf.format('DD/MM/YYYY HH:mm')}</b>`)
		return;
	}
	if (idx > -1) {
		bitacora.Tramos[idx] = tramo.clon();
	}
	else {
		let nt = bitacora.Tramos.length + 1;
		document.getElementById('aOrigen').disabled = true;
		tramo.Tramo = nt;
		bitacora.Tramos.push(tramo.clon());
	}
	updDT(tbTramos, tramo, 'Tramo');

	let fec = document.getElementById('Fecha').value;
	tramo.clear();
	tramo.write('', "tTramosD")
	document.getElementById('Fecha').value = fec;
	document.getElementById('aOrigen').value = document.getElementById('aDestino').value;
	document.getElementById('aDestino').value = '';
	document.getElementById('aDestino').focus();

	tramo.IdOrigen = bitacora.Tramos[bitacora.Tramos.length - 1].IdDestino;
	tramo.Origen = clonObject(bitacora.Tramos[bitacora.Tramos.length - 1].Destino);
}
const addRvsm = (e) => {
	rcca.read('', 'tRCCA');
	if (rcca.NoVuelo <= 0) return;
	if (!bitacora.RCCA) bitacora.RCCA = [];
	let idx = bitacora.RCCA.findIndex(r => { return r.No === rcca.No; });
	if (idx > -1)
		bitacora.RCCA[idx] = rcca.clon();
	else {
		rcca.No = bitacora.RCCA.length + 1;
		bitacora.RCCA.push(rcca.clon());
	}
	updDT(tbRCCA, rcca, 'No');

	rcca.clear();
	rcca.write('', "tRCCA")
	document.getElementById('NoVuelo').focus();
}
const consulta = (e) => {
	let fol = parseInt(document.getElementById('Folio').value) || 0;
	let avi = avion.clon();
	if (fol <= 0 || avi.Id <= 0) return;
	clear();
	procc();
	bitacora.byFolioAircraft(fol, avi.Id).then(bita => {
		bitacora.write();
		if (bitacora.Valid) {
			parametro.setValores(bitacora.ParametrosMotor)
			parametro.write('', 'tParMot')
			loadTable(tbTramos, bitacora.Tramos)
			loadTable(tbRCCA, bitacora.RCCA)

			if (bitacora.Tramos.length > 0) {
				document.getElementById('FechaSalida').value = bitacora.Tramos[bitacora.Tramos.length - 1].Fecha;
				document.getElementById('aOrigen').value = bitacora.Tramos[bitacora.Tramos.length - 1].Destino.IATA;
				document.getElementById('aOrigen').disabled = true;
				tramo.IdOrigen = bitacora.Tramos[bitacora.Tramos.length - 1].IdDestino;
				tramo.Origen = clonObject(bitacora.Tramos[bitacora.Tramos.length - 1].Destino);
			}
		}
		else {
			document.getElementById('Folio').value = fol;
			document.getElementById('Aircraft').value = avi.Matricula;
			valAvi();
		}
		endPro();
	});
}
const eliminar = () => {
	if (bitacora.Valid) {
		let id = bitacora.Id;
		let res = bitacora.delete();
		MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tbTramos, 'Id', id);
			bitacora.write();
		}
	}
}
const registrar = () => {
	parametro.read('','tParMot')
	bitacora.read();
	if (bitacora.Cancelada === 0)
		bitacora.ParametrosMotor = parametro.clon();
	else {
		bitacora.Tramos = [];
		bitacora.RCCA = [];
	}
	let res = bitacora.save();
	MsjBox('Bitacora de Vuelo', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid)
		clear()
}
const clear = () => {
	bitacora.clear()
	bitacora.write()

	tramo.clear()
	tramo.write('', "tTramosD")
	tbTramos.clear()
	tbTramos.columns.adjust().draw()
	document.getElementById('aOrigen').value = ''
	document.getElementById('aDestino').value = ''
	document.getElementById('aOrigen').disabled = false

	parametro.clear()
	parametro.write('', "tParMot")

	rcca.clear()
	rcca.write('', "tRCCA")
	tbRCCA.clear()
	tbRCCA.columns.adjust().draw()

	avion.clear();
}
const tablas = () => {
	tbTramos = $('#tTramos').DataTable({
		processing: true,
		//responsive: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: `tr`,
		data: [],
		columns: [
			{ data: 'Id', visible: false },
			{ data: 'Tramo' },
			{ data: 'Origen.IATA' },
			{ data: 'Destino.IATA' },
			{
				data: null,
				render: (data, type) => {
					return moment(data.Fecha).format(type === 'display' ? 'DD/MM/YYYY' : 'YYYYMMDD');
				}
			},
			{ data: 'SalidaPlataforma' },
			{ data: 'LlegadaPlataforma' },
			{ data: 'Calzo' },
			{ data: 'Despegue' },
			{ data: 'Aterrizaje' },
			{ data: 'Tiempo' },
			{ data: 'Demora' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Velocidad ? accounting.formatNumber(data.Velocidad, 0, ',', '.') : '' : data.Velocidad ?? '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Altitud ? accounting.formatNumber(data.Altitud, 0, ',', '.') : '' : data.Altitud ?? '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.CombustibleSalida ? accounting.formatNumber(data.CombustibleSalida, 0, ',', '.') : '' : data.CombustibleSalida ?? '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.CombustibleLlegada ? accounting.formatNumber(data.CombustibleLlegada, 0, ',', '.') : '' : data.CombustibleLlegada ?? '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Peso ? accounting.formatNumber(data.Peso, 0, ',', '.') : '' : data.Peso ?? '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Pasajeros ? accounting.formatNumber(data.Pasajeros, 0, ',', '.') : '' : data.Pasajeros ?? '';
				}
			},
			{
				data: null,
				orderable: false,
				className: 'd-grid py-0 my-0 align-middle',
				//defaultContent: `<div class="btn-group" role="group" aria-label="Basic example">
				//			<button data-type="edit" type="button" class="btn btn-outline-primary py-0"><i class="fas fa-pencil-alt"></i></button>
				//		</div>`
				defaultContent: `<div class="btn-group" role="group" aria-label="Basic example">
							<button data-type="edit" type="button" class="btn btn-outline-primary py-0"><i class="fas fa-pencil-alt"></i></button>
							<button data-type="delete" type="button" class="btn btn-sm btn-outline-danger py-0"><i class="fas fa-trash-alt"></i></button>
						</div>`
			}
		],
		columnDefs: [
			{ targets: [0, 1, 12, 13, 14, 15, 16, 17], className: 'text-end py-0 my-0 align-middle' },
			{ targets: '_all', className: 'text-center py-0 my-0 align-middle', orderable: false }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tbTramos.rows(e.target.closest('tr')).data()[0];
				tramo.clear();
				tramo.setValores(data);
				tramo.write('', 'tTramosD');
				document.getElementById('FechaSalida').value = tramo.Fecha;
				document.getElementById('aOrigen').value = tramo.Origen.IATA;
				document.getElementById('aDestino').value = tramo.Destino.IATA;
				if (tramo.Tramo === 1)
					document.getElementById('aOrigen').disabled = false;
			});
			$(`#${settings.sTableId} button[data-type="delete"]`).click((e) => {
				let data = tbTramos.rows(e.target.closest('tr')).data()[0];
				MsjBox('Eliminar Tramo <i class="fas fa-trash-alt"></i>', `¿Estas seguro que deseas eliminar el Tramo <b>${data.Origen.IATA}-${data.Destino.IATA}</b>?`, 2, (e) => {
					let idx = bitacora.Tramos.findIndex(r => { return r.Tramo === data.Tramo });
					if (idx > -1) {
						bitacora.Tramos.splice(idx, 1);
						let lt;
						bitacora.Tramos.forEach((t, i) => {
							t.Tramo = i + 1;
							if (i > 0) {
								t.IdOrigen = lt.IdDestino
								t.Origen = clonObject(lt.Destino)
							}
							lt = clonObject(t);
						})
						loadTable(tbTramos, bitacora.Tramos)
					}
				})
			});
		}
	});
	tbRCCA = $('#tRCCA').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: `tr`,
		data: [],
		columns: [
			{ data: 'No' },
			{ data: 'NoVuelo' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Desviacion ? 'Si' : 'No' : data.Desviacion;
				}
			},
			{ data: 'Altitud' },
			{ data: 'DIF1' },
			{ data: 'DIF2' },
			{
				data: null,
				orderable: false,
				className: 'd-grid py-0 my-0 align-middle',
				defaultContent: `<div class="btn-group" role="group" aria-label="Basic example">
							<button data-type="edit" type="button" class="btn btn-outline-primary py-0"><i class="fas fa-pencil-alt"></i></button>
							<button data-type="delete" type="button" class="btn btn-sm btn-outline-danger py-0"><i class="fas fa-trash-alt"></i></button>
						</div>`
			}
		],
		columnDefs: [
			{ targets: [0, 3, 4, 5], className: 'text-end py-0 my-0 align-middle' },
			{ targets: '_all', className: 'text-center py-0 my-0 align-middle', orderable: false }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tbRCCA.rows(e.target.closest('tr')).data()[0];
				rcca.clear();
				rcca.setValores(data);
				rcca.write('', 'tRCCA');
				document.getElementById('NoVuelo').focus()
			});
			$(`#${settings.sTableId} button[data-type="delete"]`).click((e) => {
				let data = tbRCCA.rows(e.target.closest('tr')).data()[0];
				MsjBox('Eliminar Registro <i class="fas fa-trash-alt"></i>', `¿Estas seguro que deseas eliminar el Registro No. ${data.No}?`, 2, (e) => {
					let idx = bitacora.RCCA.findIndex(r => { return r.No === data.No });
					if (idx > -1) {
						bitacora.RCCA.splice(idx, 1);
						bitacora.RCCA.forEach((r, i) => { r.No = i + 1; })
						loadTable(tbRCCA, bitacora.RCCA)
					}
				})
			});
		}
	});
}