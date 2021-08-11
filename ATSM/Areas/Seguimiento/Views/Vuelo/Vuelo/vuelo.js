var vuelo, tramo, avion, demora, ruta, tabla, lisTra, lisDem, tripulacion,creLoc;
var vueloHub, cnxHub = false;
$(document).ready(() => {
	tripulacion = [];
	avion = new Aircraft();
	demora = new VueloDemora();
	ruta = new Ruta();
	tablas();
	let id = parseInt(getPar('id')) || 0;
	vuelo = new Vuelo(id);
	tramo = new VueloTramo();
	
	$('#Trip').blur(conV);
	$('#cleV').click(cleV);
	$('#regV').click(regV);
	$('#delV').click(delV);

	$('#cleS,#cleL').click(cleT);
	$('#regS,#regL').click(regT);
	$('#delS,#delL').click(delT);

	$('#IdTipo').change((e) => {
		let tipo = parseInt(e.target.value) || 0;
		if (tipo === 8) {
			$('#PiezasEquipaje').removeClass('d-none');
			$('#PesoEquipaje').removeClass('d-none');
			$('#PiezasCarga').addClass('d-none');
			$('#PesoCarga').addClass('d-none');
		} else {
			$('#PiezasEquipaje').addClass('d-none');
			$('#PesoEquipaje').addClass('d-none');
			$('#PiezasCarga').removeClass('d-none');
			$('#PesoCarga').removeClass('d-none');
		}
	});
	$('#nav-tab03-tab').on('show.bs.tab', swSalida);
	$('#nav-tab04-tab').on('show.bs.tab', swLlegada);
	$('#Pierna').blur(conT);
	$('#IdRuta').change(selRuta);
	$('#addDem').click(addDemora);
	document.getElementById('Aircraft').addEventListener('blur', valAvi)
	hubs();
	if (vuelo.Valid) {
		vuelo.write('', 'nav-tab02');
		loadTable(lisTra, clonObject(vuelo.Tramos))
		$('#nav-tab03-tab').removeClass('disabled');
		$('#nav-tab04-tab').removeClass('disabled');

		let p = parseInt(getPar('p')) || 0;
		if (p > 0) {
			let tra = vuelo.Tramos.find(t => t.Pierna === p);
			tramo.setValores(tra);
			if (tramo.Valid) {
				$('#Aircraft').val(tramo.Aeronave.Matricula);
				tramo.write('', 'nav-tab03');
				tramo.write('', 'nav-tab04');
				$('#nav-tab a[href="#nav-tab03"]').tab('show');
			} else {
				$('#nav-tab a[href="#nav-tab02"]').tab('show');
			}
		} else {
			$('#nav-tab a[href="#nav-tab02"]').tab('show');
		}
	}
	actVuelos();
});
const valAvi = (e) => {
	let cAvi = document.getElementById('Aircraft')
	if (!avion.Valid && avion.Matricula.toUpperCase() !== cAvi.value.toUpperCase()) {
		avion.clear();
		let opsA = Array.from(document.querySelectorAll('#listAircraft option'))
		let idx = opsA.findIndex(o => o.value.toUpperCase() === cAvi.value.toUpperCase())
		if (idx > -1)
			avion.Id = parseInt(opsA[idx].dataset.id) || 0;
		if (avion.Id > 0)
			avion.getInstance();
	}
	if (avion.Id > 0) {
		cAvi.value = avion.Matricula;
		if (!avion.Valid) return;
		$('#Aircraft').val(avion.Matricula);
		if (avion.Modelo.IdCapacidad <= 0) {
			MsjBox('Aeronave Sin Capacidad', 'La Matricula de la Aeronave no tiene asignado Modelo, ni Capacidad de Vuelo, por lo que no se puede Asignar Tripulacion.<br>Pongase en Contacto con el Area de Ingenieria para que le sea asignada la capacidad a la Aeronave.')
			cAvi.value=''
			return;
		}
		document.querySelectorAll('#IdCapitan option, #IdCopiloto option').forEach(o => {
			if (o.value === '0') return;
			o.classList.add('d-none');
			if ((parseInt(o.dataset.cap1) || 0) === avion.Modelo.IdCapacidad || (parseInt(o.dataset.cap2) || 0) === avion.Modelo.IdCapacidad || (parseInt(o.dataset.cap3) || 0) === avion.Modelo.IdCapacidad)
				o.classList.remove('d-none');
		})
		vuelo.CodigoVuelo = avion.Empresa.CodigoVuelo;
		$('#cardVueloS [id="txtEmpresa"]').text(avion.Empresa.Nombre);
		$('#cardVueloS [id="txtCodigo"]').text(vuelo.CodigoVuelo);
		$('#aOrigen').removeAttr('disabled');
		if (vuelo.Tramos.length === 0)
			avion.ultimoTramo().then((data) => {
				if (data !== null)
					$('#aOrigen').val(data.Destino.Valid ? data.Destino.IATA : '');
			});
		else {
			$('#aOrigen').val(vuelo.Tramos[vuelo.Tramos.length - 1].Destino.IATA);
			$('#aOrigen').prop('disabled', true);
		}
	}
	else
		cAvi.value = '';
}
function hubs() {
	vueloHub = $.connection.vueloHub;
	vueloHub.client.addVuelo = (datos) => {
		vuelo.setValores(datos);
		actV();
	}
	vueloHub.client.addTramo = (datos) => {
		if (datos.Valid) {
			if (vuelo.Valid) {
				tramo.clear();
				tramo.setValores(datos)
				let i = vuelo.Tramos.findIndex(i => i.IdTramo === tramo.IdTramo);
				if (i > -1) {
					lisTra.row(i).data(tramo.clon());
				} else {
					lisTra.rows.add(tramo.clon());
				}
				lisTra.columns.adjust().draw();
			}
		}
	}
	vueloHub.client.vueloAC = (idv, edo) => {
		if (edo) {
			let i = tabla.data().toArray().findIndex(i => i.IdVuelo === idv);
			if (i > -1) {
				let d = tabla.row(i).remove().draw();
			}
		} else {
			let i = tabla.data().toArray().findIndex(i => i.IdVuelo === idv);
			if (i > -1) {
				let d = tabla.row(i).data();
				d.Cerrado = edo;
				tabla.row(i).data(d);
			} else {
				let vlo = new Vuelo(idv);
				tabla.rows.add(vlo.clon());
			}
			tabla.columns.adjust().draw();
		}
	}
	$.connection.hub.start().done(() => {
		cnxHub = true;
	});
}
function selRuta() {
	let r = parseInt(document.getElementById('IdRuta').value) || 0;
	ruta = new Ruta(r);
	if (ruta.Valid) {
		ruta.write('', 'nav-tab02');
		vuelo.Tramos = [];
		ruta.Tramos.forEach((tra, idx) => {
			tramo.clear();
			tramo.setValores(tra);
			tramo.ItinerarioDespegue = tra.ItinerarioSalida;
			tramo.ItinerarioAterrizaje = tra.ItinerarioLlegada;
			tramo.ETA = tra.ItinerarioLlegada;
			tramo.Salida = $('#nav-tab02 [id="Salida"]').val();
			tramo.IdAeronave = avion.Id
			vuelo.Tramos.push(tramo.clon());
		});
	}
	loadTable(lisTra, clonObject(vuelo.Tramos))
	//actT();
}
function swSalida(e) {
	$('#cardVueloS span').text('')
	if (!vuelo.Valid) {
		MsjBox('Vuelos', 'Se debe primero Crear el Vuelo')
		return
	}
	cleSL()
	$('#nav-tab03 input[id="Pierna"]').val(vuelo.Tramos.length === 0 ? 1 : (Math.max(...vuelo.Tramos.map(i => i.Pierna)) + 1))
	avion.clear();
	if (!tramo.Valid)
		tramo = clonObject(vuelo.Tramos[vuelo.Tramos.length - 1])
	avion.setValores(tramo.Aeronave)
	valAvi()
	$('#NivelVuelo').val(tramo.NivelVuelo)
	$('#SOB').val(tramo.SOB)
	$('#IdCapitan').val(tramo.IdCapitan)
	$('#IdCopiloto').val(tramo.IdCopiloto)
	$('#nav-tab03 input[id="Salida"]').val(tramo.Llegada !== null ? tramo.Llegada.substring(0, 10) : moment().format('YYYY-MM-DD'))
	$('#aOrigen').val(tramo.Origen.IATA)
	$('#aDestino,#Destino').val(tramo.Destino.IATA)
	$('#cardVueloS span').text('')
	$('#nav-tab03 input[id="NoVuelo"]').val($('#nav-tab02 input[id="NoVuelo"]').val())
	$('#cardVueloS [id="txtTrip"]').text($('#nav-tab02 input[id="Trip"]').val())
	$('#cardVueloS [id="txtTipo"]').text($('#IdTipoVuelo option:selected').text())
	$('#cardVueloS [id="txtEmpresa"]').text(avion.Empresa !== null ? avion.Empresa.Nombre : '')
	$('#cardVueloS [id="txtCodigo"]').text(vuelo.CodigoVuelo)
	$('#cardVueloS [id="txtNo"]').text($('#nav-tab02 input[id="NoVuelo"]').val())
	$('#cardVueloS [id="txtFecha"]').text(moment($('#nav-tab02 input[id="Salida"]').val()).format('dddd DD MMM YYYY'))
	if ($('#IdTipoVuelo option:selected').text() !== 'Seleccionar...')
		$('#cardVueloS [id="txtTipo"]').text($('#IdTipoVuelo option:selected').text())
	if ($('#IdRuta option:selected').text() !== 'Seleccionar...')
		$('#cardVueloS [id="txtRuta"]').text($('#IdRuta option:selected').text())
}
function swLlegada(e) {
	$('#cardVueloL span').text('');
	avion.clear();
	if (!tramo.Valid) {
		MsjBox('Tramo de Vuelo', 'Seleccione el Tramo de Llegada')
		return false;
	}
	avion.setValores(vuelo.Tramos.find(t => t.IdTramo === tramo.IdTramo).Aeronave);
	cleSL();
	if (!avion.Valid) {
		MsjBox('Avion', 'El avion no es valido')
		return false;
	}
	$('#cardVueloL [id="txtTrip"]').text(vuelo.Trip);
	$('#cardVueloL [id="txtPierna"]').text(tramo.Pierna);
	$('#cardVueloL [id="txtFecha"]').text(moment(tramo.Salida).format('dddd DD MMM YYYY'));
	$('#cardVueloL [id="txtMatricula"]').text(avion.Matricula);
	$('#cardVueloL [id="txtEmpresa"]').text(avion.Empresa !== null ? avion.Empresa.Nombre : '');
	$('#cardVueloL [id="txtOrigen"]').text(vuelo.Tramos.find(t => t.IdTramo === tramo.IdTramo) !== undefined ? vuelo.Tramos.find(t => t.IdTramo === tramo.IdTramo).Origen.IATA : '');
	$('#cardVueloL [id="txtCodigo"]').text(vuelo.CodigoVuelo);
	$('#cardVueloL [id="txtTipo"]').text($('#IdTipoVuelo option:selected').text());
	$('#cardVueloL [id="txtNo"]').text(tramo.NoVuelo);
	if ($('#IdTipoVuelo option:selected').text() !== 'Seleccionar...')
		$('#cardVueloL [id="txtTipo"]').text($('#IdTipoVuelo option:selected').text());
}
function actVuelos() {
	$('#loadVuelos').removeClass('d-none');
	Vuelo.getVuelos({ cerrado: false}).then((data) => {
		tabla.clear();
		let datos = data !== null ? data.Data !== null ? data.Data : [] : [];
		if (datos.length > 0) {
			tabla.rows.add(datos);
		}
		tabla.columns.adjust().draw();
		$('#loadVuelos').addClass('d-none');
	});
}

/*------------VUELO------------*/
function actV() {
	if (!vuelo.Valid) return;
	let i = tabla.data().toArray().findIndex(i => i.IdVuelo === vuelo.IdVuelo);
	if (i > -1) {
		tabla.row(i).data(vuelo.clon());
	} else {
		tabla.rows.add(vuelo.clon());
	}
	tabla.columns.adjust().draw();
}
function conV() {
	let t = document.getElementById('Trip').value;
	cleV();
	vuelo.byTrip(t);
	if (vuelo.Valid) {
		vuelo.write('', 'nav-tab02');
		loadTable(lisTra, clonObject(vuelo.Tramos))
		//actT();
		$('#nav-tab03-tab').removeClass('disabled');
	} else {
		document.getElementById('Trip').value = t;
	}
}
function delV() {
	if (vuelo.Valid) {
		let res = vuelo.delete();
		MsjBox('Vuelo', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			actVuelos();
			cleV();
		}
	}
}
function regV() {
	vuelo.read('', 'nav-tab02');
	let res = vuelo.save();
	MsjBox('Vuelo', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		actV();
		cleV();
	}
}
function cleV() {
	$('input,textarea,select').val('');
	$('select.num').val(0);
	vuelo.clear();
	vuelo.write();
	tramo.clear();
	tramo.write('', 'nav-tab03');
	tramo.write('', 'nav-tab04');
	lisTra.clear().columns.adjust().draw();
	$('#nav-tab04-tab,#nav-tab03-tab').addClass('disabled');
	$('#nav-tab02 input[id="Salida"]').val(moment().format('YYYY-MM-DD'));
}
/*------------TRAMO------------*/
function conT() {
	let p = parseInt(document.getElementById('Pierna').value) || 0
	let m = document.querySelector('#Aircraft').value
	let avi = avion.clon();
	if (vuelo.Valid && p > 0) {
		tramo.write('', 'nav-tab03');
		tramo.IdVuelo = vuelo.IdVuelo;
		tramo.byPierna(p);
		if (tramo.Valid) {
			tramo.write('', 'nav-tab03');
			tramo.write('', 'nav-tab04');
			$('#nav-tab04-tab,#nav-tab03-tab').removeClass('disabled');
			$('#aOrigen').val(tramo.Origen.IATA);
			$('##aDestino,#Destino,#Destino').val(tramo.Destino.IATA);
			$('#Aircraft').val(tramo.Aeronave.Matricula);
		} else {
			document.getElementById('Pierna').value = p;
			document.querySelector('#Aircraft').value = m;
			if (vuelo.Tramos.length > 0) {
				let tAn = vuelo.Tramos[vuelo.Tramos.length - 1];
				$('#aOrigen').val(tAn.Origen.IATA);
				$('#aOrigen').prop('disabled', true);
				$('#IdCapitan').val(tAn.IdCapitan);
				$('#IdCopiloto').val(tAn.IdCopiloto);
				$('#Salida').val(tAn.Llegada);
			}
		}
	}
}
function delT() {
	if (tramo.Valid) {
		let res = tramo.delete();
		MsjBox('Tramo', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			cleT();
		}
	}
}
function regT(e) {
	let o = e.target.id === 'regS' ? 3 : 4;
	if (o === 4 && tramo.IdTramo <= 0) {
		MsjBox('Llegada', 'El tramo que intentas Actualizar no Existe, debes primero registrar la salida');
		$('#nav-tab a[href="#nav-tab03"]').tab('show');
		return;
	}
	tramo.read('', `nav-tab0${o}`);
	tramo.IdVuelo = vuelo.IdVuelo;
	tramo.IdDestinoOriginal = tramo.IdDestino;
	let res = tramo.save();
	MsjBox('Vuelo', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		let i = vuelo.Tramos.findIndex(t => t.Pierna === tramo.Pierna);
		if (i > -1) {
			vuelo.Tramos[i] = tramo.clon();
		} else {
			vuelo.Tramos.push(tramo.clon());
		}
		loadTable(lisTra, clonObject(vuelo.Tramos))
		//actT();
		tramo.clear();
		cleSL();
		$('#nav-tab a[href="#nav-tab02"]').tab('show');
	}
}
function cleT() {
	tramo.clear();
	cleSL();
	$('#nav-tab a[href="#nav-tab02"]').tab('show');
	$(`#nav-tab04-tab ${(vuelo.Tramos.length > 0 ? '' : ', #nav-tab03-tab')}`).addClass('disabled');
}
function cleSL() {
	$('#nav-tab03 input,#nav-tab03 textarea,#nav-tab03 select').val('');
	$('#nav-tab03 select.num').val(0);
	$('#nav-tab04 input,#nav-tab04 textarea,#nav-tab04 select').val('');
	$('#nav-tab04 select.num').val(0);
	$('#cardVueloS span').text('');
	$('#cardVueloL span').text('');
}
/*------------DEMORA------------*/
function actD() {
	lisDem.clear();
	lisDem.rows.add(clonObject(tramo.Demoras));
	lisDem.columns.adjust().draw();
}
function addDemora() {
	demora.read();
	if (demora.IdDemora === 0)
		return;
	demora.Codigo = $('#IdDemora option:selected').attr('codigo');
	let i = tramo.Demoras.findIndex(x => x.Consecutivo === demora.Consecutivo);
	if (i > -1) {
		tramo.Demoras[i] = demora.clon();
	} else {
		demora.Consecutivo = tramo.Demoras.length;
		tramo.Demoras.push(demora.clon());
	}
	actD();
	demora.clear();
	demora.write();
}
/*------------TABLAS------------*/
function tablas() {
	tabla = $('#lista').DataTable({
		processing: true,
		order: [[6, 'asc'], [0, 'asc']],
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{
				data: null,
				render: (data) => {
					return data.Aeronave !== null ? data.Aeronave.Matricula : '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					let f = data.SalidaPlataforma !== null ? data.fdSPlataforma : data.Despegue !== null ? data.fdDespegue : data.ItinerarioDespegue !== null ? data.fdIDespegue : data.Salida;
					return moment(f).format(type === 'display' ? 'DD/MM/YYYY HH:mm' : 'YYYYMMDDHHmm');
				}
			},
			{ data: 'Trip' },
			{ data: null, visible: false },
			{ data: null, visible: false },
			{ data: null, visible: false },
			{ data: 'CodigoVuelo' },
			{ data: 'NoVuelo' },
			{ data: 'Tipo.Descripcion', className: 'sz-font-7' },
			{
				data: null,
				render: (data, type) => {
					return data.Ruta !== null ? data.Ruta.Codigo : '';
				}
			},
			{ data: 'Capitan.Nombre', className: 'sz-font-7' },
			{ data: 'Copiloto.Nombre', className: 'sz-font-7' },
			{
				data: null,
				render: (data, type) => {
					return data.Cerrado ? 'Cerrado' : 'Abierto';
				}
			},
			{ data: null }
		],
		columnDefs: [
			{ targets: [4], className: 'text-end' },
			{ targets: [10, 11], className: 'text-justify' },
			{ targets: '_all', className: 'text-center' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<div class="btn-group" role="group" aria-label="Basic example">
					<button tipo="editar" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-pencil-alt"></i></button>
					${(usuario.air('clVuelo') ? '<button tipo="cerrar" type="button" class="btn btn btn-sm btn-danger pt-0 pb-0"><i class="fas fa-lock"></i></button>' : '')}
				</div>`
			}
		],
		drawCallback: function (settings) {
			$(`#lista button[tipo="editar"]`).click(async (btn) => {
				let row = tabla.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				vuelo.clear();
				vuelo.setValores(data);
				vuelo.write('', 'nav-tab02');
				$('#loadTramos').removeClass('d-none');
				await VueloTramo.getTramosVuelo(vuelo.IdVuelo).then((data) => {
					if (data === null) { console.log('Error VueloTramo.getTramosVuelo: vuelo.js_74') }
					if (data.Data !== null) {
						vuelo.Tramos = clonObject(data.Data);
						loadTable(lisTra, clonObject(vuelo.Tramos))
					}
					$('#loadTramos').addClass('d-none');
				});
				$('#nav-tab a[href="#nav-tab02"]').tab('show');
				$('#nav-tab03-tab').removeClass('disabled');
				endPro();
			});
			$(`#lista button[tipo="cerrar"]`).click((btn) => {
				let row = tabla.row($(btn.target).parents('tr'));
				var data = row.data();
				vuelo.clear();
				vuelo.setValores(data);
				vuelo.close();
				cleV();
			});
		}
	});
	lisTra = $('#lisTramos').DataTable({
		processing: true,
		paging: false,
		order: [[1, 'asc']],
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		//dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		dom: "tr",
		data: [],
		columns: [
			{ data: 'Aeronave.Matricula' },
			{ data: 'Pierna' },
			{ data: 'Origen.IATA' },
			{ data: 'Destino.IATA' },
			{
				data: null,
				render: (data, type) => {
					return data.Salida === null ? '<i class="fas fa-clock"></i>' : moment(data.Salida).format(type === 'display' ? 'DD/MM/YYYY' : 'YYYYMMDD');
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.ItinerarioDespegue === null ? '<i class="fas fa-clock"></i>' : data.ItinerarioDespegue.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.SalidaPlataforma === null ? '<i class="fas fa-clock"></i>' : data.SalidaPlataforma.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.Despegue === null ? '<i class="fas fa-clock"></i>' : data.Despegue.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.Llegada === null ? '<i class="fas fa-clock"></i>' : moment(data.Llegada).format(type === 'display' ? 'DD/MM/YYYY' : 'YYYYMMDD');
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.ItinerarioAterrizaje === null ? '<i class="fas fa-clock"></i>' : data.ItinerarioAterrizaje.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.LlegadaPlataforma === null ? '<i class="fas fa-clock"></i>' : data.LlegadaPlataforma.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.Aterrizaje === null ? '<i class="fas fa-clock"></i>' : data.Aterrizaje.substring(0, 5);
				}
			},
			{
				data: null,
				className: 'sz-font-65',
				render: (data, type) => {
					return data.Capitan === null ? '' : data.Capitan.Nombre;
				}
			},
			{
				data: null,
				className: 'sz-font-65',
				render: (data, type) => {
					return data.Copiloto === null ? '' : data.Copiloto.Nombre;
				}
			},
			{ data: null }
		],
		columnDefs: [
			{ targets: [1], className: 'text-end', orderable: false },
			{ targets: [0, 2, 3], className: 'text-center', orderable: false },
			{ targets: [4, 5, 6, 7], className: 'bgc-cadetblue text-center', orderable: false },
			{ targets: [8, 9, 10, 11], className: 'bgc-sandybrown text-center', orderable: false },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<div class="btn-group" role="group" aria-label="Basic example">
  <button tipo="ediSal" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-plane-departure"></i></button>
  <button tipo="ediLle" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-plane-arrival"></i></button>
</div>`
			}
		],
		drawCallback: function (settings) {
			$(`#lisTramos button[tipo="ediSal"]`).click((btn) => {
				let row = lisTra.row($(btn.target).parents('tr'));
				var data = row.data();
				$('#nav-tab04-tab').removeClass('disabled');
				setTramo(data.IdTramo)
				$('#nav-tab03-tab').tab('show');
			});
			$(`#lisTramos button[tipo="ediLle"]`).click((btn) => {
				let row = lisTra.row($(btn.target).parents('tr'));
				var data = row.data();
				$('#nav-tab04-tab').removeClass('disabled');
				setTramo(data.IdTramo)
				$('#nav-tab04-tab').tab('show');
			});
			const setTramo = (idt) => {
				let idx = vuelo.Tramos.findIndex(t => { return t.IdTramo === idt; })
				if (idx === -1) {
					MsjBox('Tramo', 'No es posible editar el Tramo, ha ocurrido un Error.')
					cleV();
					$('#nav-tab01-tab').tab('show');
					return;
				}
				procc();
				tramo.clear();
				tramo.setValores(clonObject(vuelo.Tramos[idx]));
				tramo.write('', 'nav-tab03');
				tramo.write('', 'nav-tab04');
				endPro();
			}
		}
	});
	lisDem = $('#lisDem').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Codigo' },
			{ data: 'Observaciones' },
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-center' },
			{ targets: [1], className: 'text-justify' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<button tipo="editar" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-pencil-alt"></i></button>
<button tipo="quitar" type="button" class="btn btn btn-sm btn-outline-danger pt-0 pb-0"><i class="fas fa-trash-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#lisDem button[tipo="editar"]`).click((btn) => {
				let row = lisDem.row($(btn.target).parents('tr'));
				var data = row.data();
				demora.clear();
				demora.setValores(data);
				demora.write();
			});
			$(`#lisDem button[tipo="quitar"]`).click((btn) => {
				let row = lisDem.row($(btn.target).parents('tr'));
				var data = row.data();
				let i = tramo.Demoras.findIndex(e => e.Consecutivo = data.Consecutivo);
				if (i > -1) {
					tramo.Demoras.splice(i, 1);
					actD();
				}
			});
		}
	});
}