var tabla, tabla1, tabla2, tabla3, tabla4, vuelo, comprobacion, gasto, bsOffcanvas;
document.addEventListener('DOMContentLoaded', () =>{
	vuelo = new Vuelo();
	comprobacion = new Comprobacion();
	gasto = new Gasto();
	tables();
	Comprobacion.pendientes().then((data) => {
		loadTable(tabla, data.Data);
	}).catch(console.log);
	document.querySelectorAll('#LTS_Combustible, #Galones, input[id="Subtotal"], input[id="P_IVA"], input[id="Total"], #ImpHospedaje').forEach(de => de.addEventListener('blur', calcula));
	document.querySelectorAll('button[name="addg"]').forEach(btn => btn.addEventListener('click', addGasto));
	bsOffcanvas = new bootstrap.Offcanvas(document.getElementById('offTrips'));
});
function addGasto(e) {
	let op = parseInt(e.target.dataset.tg) || 0;
	gasto.clear();
	comprobacion.Gastos = (comprobacion.Gastos === null || comprobacion.Gastos === undefined) ? [] : comprobacion.Gastos;
	let cont = e.target.closest('div[role="tabpanel"]').id;
	gasto.read('', cont);
	const setMS = () => {
		let osM = document.querySelector(`#${cont} select[id="IdMoneda"]`);
		gasto.Moneda = {};
		gasto.Moneda.Id = gasto.IdMoneda;
		gasto.Moneda.Codigo = osM.options[osM.selectedIndex].dataset.codigo;
		gasto.Moneda.Nombre = osM.options[osM.selectedIndex].dataset.nombre;
		let osS = document.querySelector(`#${cont} select[id="IdSaldo"]`);
		gasto.Saldo = {};
		gasto.Saldo.Id = gasto.IdSaldo;
		gasto.Saldo.Codigo = osS.options[osS.selectedIndex].dataset.codigo;
		gasto.Saldo.Nombre = osS.options[osS.selectedIndex].dataset.nombre;
		gasto.Saldo.Combustible = osS.options[osS.selectedIndex].dataset.combustible;
		let osA = document.querySelector(`#${cont} select[id="IdAeropuerto"]`);
		gasto.Aeropuerto = {};
		if (osA) {
			gasto.Aeropuerto.IdAeropuerto = gasto.IdAeropuerto;
			gasto.Aeropuerto.Nombre = osA.options[osA.selectedIndex].dataset.nombre;
			gasto.Aeropuerto.ICAO = osA.options[osA.selectedIndex].dataset.icao;
			gasto.Aeropuerto.IATA = osA.options[osA.selectedIndex].dataset.iata;
		}
	}
	switch (op) {
		case 1: {
			if (gasto.Concepto === '' || gasto.Total === 0 || gasto.IdMoneda === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.Concepto === '' ? 'Concepto' : gasto.Total === 0 ? 'Total' : gasto.IdMoneda === 0 ? 'Moneda' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
			gasto.IdTipoGasto = 1;
			setMS();
			updDT(tabla1, gasto);
			}
			break;
		case 2: {
			if (gasto.IdAeropuerto === 0 || gasto.Factura === '' || gasto.Concepto === '' || gasto.Subtotal === 0 || gasto.IdMoneda === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.IdAeropuerto === 0 ? 'Aeropuerto' : gasto.Factura === '' ? 'Factura' : gasto.Concepto === '' ? 'Concepto' : gasto.Subtotal === 0 ? 'Subtotal' : gasto.IdMoneda === 0 ? 'Moneda' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
			gasto.IdTipoGasto = 2;
			setMS();
			updDT(tabla2, gasto);
		} break;
		case 3: {
			if (gasto.IdAeropuerto === 0 || gasto.Factura === '' || gasto.Concepto === '' || gasto.Subtotal === 0 || gasto.IdMoneda === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.IdAeropuerto === 0 ? 'Aeropuerto' : gasto.Factura === '' ? 'Factura' : gasto.Concepto === '' ? 'Concepto' : gasto.Subtotal === 0 ? 'Subtotal' : gasto.IdMoneda === 0 ? 'Moneda' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
			gasto.IdTipoGasto = 3;
			setMS();
			updDT(tabla3, gasto);
		} break;
		case 4: {
			gasto.Concepto = 'Combustible';
			if (gasto.IdAeropuerto === 0 || gasto.Factura === '' || gasto.LTS_Combustible === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.IdAeropuerto === 0 ? 'Aeropuerto' : gasto.Factura === '' ? 'Factura' : gasto.LTS_Combustible === 0 ? 'Litros' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
			gasto.IdTipoGasto = 4;
			setMS();
			updDT(tabla4, gasto);
		} break;
	}
	comprobacion.Gastos.push(gasto.clon());
	gasto.clear();
	gasto.write('', cont);
}
function calcula(e) {
	let contenedor = e.target.closest('div[role="tabpanel"]');
	let piv = 0;
	if (contenedor.querySelector('input[id="P_IVA"]'))
		piv = parseFloat(contenedor.querySelector('input[id="P_IVA"]').value) || 0;
	if (contenedor.id === 'nav-operacion') {
		let sub = parseFloat(contenedor.querySelector('input[id="Subtotal"]').value) || 0;
		let iva = sub * (piv / 100);
		let tot = sub + iva;
		contenedor.querySelector('input[id="IVA"]').value = iva.toFixed(2);
		contenedor.querySelector('input[id="Total"]').value = tot.toFixed(2);
	}

	if (contenedor.id === 'nav-concom') {
		let hos = parseFloat(contenedor.querySelector('input[id="ImpHospedaje"]').value) || 0;
		let tot = parseInt(contenedor.querySelector('input[id="Total"]').value) || 0;
		let xtr = tot - hos;
		let sub = parseFloat((xtr / (1 + (piv / 100))).toFixed(2));
		let iva = xtr - sub;
		contenedor.querySelector('input[id="Subtotal"]').value = sub.toFixed(2);
		contenedor.querySelector('input[id="IVA"]').value = iva.toFixed(2);
	}

	if (contenedor.id === 'nav-combustible') {
		let lit = parseFloat(contenedor.querySelector('input[id="LTS_Combustible"]').value) || 0;
		let gal = parseFloat(contenedor.querySelector('input[id="Galones"]').value) || 0;
		if (e.target.id == 'Galones') {
			lit = gal * 3.78541;
			contenedor.querySelector('input[id="LTS_Combustible"]').value = lit.toFixed(2);
		} else {
			gal = lit * 0.264172;
			contenedor.querySelector('input[id="Galones"]').value = gal.toFixed(2);
		}
		let tot = parseInt(contenedor.querySelector('input[id="Total"]').value) || 0;
		let sub = parseFloat((tot / (1 + (piv / 100))).toFixed(2));
		let iva = tot - sub;
		document.querySelector('#PPL').value = (sub / lit).toFixed(2);
		contenedor.querySelector('input[id="Subtotal"]').value = sub.toFixed(2);
		contenedor.querySelector('input[id="IVA"]').value = iva.toFixed(2);
	}
}
function tables() {
	tabla = $('#tVuelos').DataTable({
		processing: true,
		select: { style: 'single' },
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12'l>><'row'<'col-12'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		order: [[1, 'desc']],
		columns: [
			{
				data: 'Trip',
				className: 'text-center'
			},
			{
				data: 'Salida',
				className: 'text-nowrap text-capitalize',
				render: (data, type) => {
					return moment(data).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD');
				}
			}
		],
		drawCallback: (settings) => {
			$(`#${settings.sTableId} tbody tr`).click((e) => {
				bsOffcanvas.hide();
				let selected = !e.currentTarget.classList.contains('selected');
				$('#flgInfo span').text('');
				$('select[id="IdAeropuerto"]').html('');
				$('#nav-sincom-tab,#nav-operacion-tab,#nav-concom-tab,#nav-combustible-tab').addClass('disabled');
				if (!selected)
					return false;
				$('#nav-sincom-tab,#nav-operacion-tab,#nav-concom-tab,#nav-combustible-tab').removeClass('disabled');
				let data = tabla.row(e.currentTarget).data();
				vuelo = new Vuelo(data.IdVuelo);
				$('#txtTrip').text(vuelo.Trip);
				$('#txtFecha').text(moment(vuelo.Salida).format('DD MMM YYYY'));
				$('#txtMatricula').text(vuelo.Aeronave.Matricula);
				$('#txtCapitan').text(vuelo.Capitan.Nombre.toLowerCase());
				$('#txtCopiloto').text(vuelo.Copiloto.Nombre.toLowerCase());
				var ruta = `${vuelo.Tramos[0].Origen.IATA}`;
				var aptos = [{ id: 0, iata: '...' }, { id: vuelo.Tramos[0].IdOrigen, nombre: vuelo.Tramos[0].Origen.Nombre, iata: vuelo.Tramos[0].Origen.IATA, icao: vuelo.Tramos[0].Origen.ICAO }];
				vuelo.Tramos.forEach(i => {
					if (i.Destino !== null) {
						ruta += `-${i.Destino.IATA}`;
						let idx = aptos.findIndex(a => a.id === i.IdDestino);
						if (idx === -1)
							aptos.push({ id: i.IdDestino, nombre: i.Destino.nombre, iata: i.Destino.IATA, icao: i.Destino.ICAO });
					}
				});
				aptos.forEach(a => {
					let oApto = `<option value="${a.id}" data-nombre="${a.nombre}" data-iata="${a.iata}" data-icao="${a.icao}">${a.iata}</option>`;
					$(oApto).appendTo('#nav-operacion select[id="IdAeropuerto"]');
					$(oApto).appendTo('#nav-concom select[id="IdAeropuerto"]');
					$(oApto).appendTo('#nav-combustible select[id="IdAeropuerto"]');
				});
				$('#txtRuta').text(ruta);
				formaTable();
			});
		}
	});
	tabla1 = $('#t1').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-sm-4'l><'col-12 col-sm-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{
				data: 'Concepto',
				className: 'text-nowrap'
			},
			{
				data: 'Total',
				className: 'text-end',
				render: (data, type) => {
					return type === 'display' ? accounting.formatMoney(data) : data;
				}
			},
			{ data: 'Moneda.Codigo', className: 'text-center' },
			{ data: 'Saldo.Nombre', className:'text-center' },
		],
		drawCallback: (settings) => {
			$(`#${settings.sTableId} tbody tr`).on("contextmenu",(e) => {
				alert('menu derecho')
			});
		}
	});
	tabla2 = $('#t2').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-sm-4'l><'col-12 col-sm-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Aeropuerto.IATA' },
			{ data: 'Factura' },
			{ data: 'Concepto' },
			{ data: 'Moneda.Codigo' },
			{ data: 'Saldo.Nombre' },
			{
				data: 'Subtotal',
				render: (data, type) => {
					return type === 'display' ? accounting.formatMoney(data) : data;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? `${accounting.formatMoney(data.IVA)} (${data.P_IVA}%)` : data.IVA;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? `${accounting.formatMoney(data.Total)}` : data.Total;
				}
			}
		],
		columnDefs: [
			{ targets: [2], className: 'text-start' },
			{ targets: [5, 6, 7], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: (settings) => {
			$(`#${settings.sTableId} tbody tr`).on("contextmenu", (e) => {
				alert('menu derecho');
			});
		}
	});
	tabla3 = $('#t3').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-sm-4'l><'col-12 col-sm-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Aeropuerto.IATA' },
			{ data: 'Factura' },
			{ data: 'Concepto' },
			{ data: 'Moneda.Codigo' },
			{ data: 'Saldo.Nombre' },
			{
				data: 'Subtotal',
				render: (data, type) => {
					return type === 'display' ? accounting.formatMoney(data) : data;
				}
			},
			{
				data: 'ImpHospedaje',
				render: (data, type) => {
					return type === 'display' ? accounting.formatMoney(data) : data;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? `${accounting.formatMoney(data.IVA)} (${data.P_IVA}%)` : data.IVA;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? `${accounting.formatMoney(data.Total)}` : data.Total;
				}
			}
		],
		columnDefs: [
			{ targets: [2], className: 'text-start' },
			{ targets: [5, 6, 7], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: (settings) => {
			$(`#${settings.sTableId} tbody tr`).on("contextmenu", (e) => {
				alert('menu derecho');
			});
		}
	});
	tabla4 = $('#t4').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-sm-4'l><'col-12 col-sm-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Aeropuerto.IATA' },
			{ data: 'Factura' },
			{ data: 'LTS_Combustible' },
			{
				data: null,
				render: (data, type) => {
					let ppl = data.Subtotal / data.Litros;
					return type === 'display' ? accounting.formatMoney(ppl) : ppl;
				}
			},
			{
				data: 'Subtotal',
				render: (data, type) => {
					return type === 'display' ? accounting.formatMoney(data) : data;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? `${accounting.formatMoney(data.IVA)} (${data.P_IVA}%)` : data.IVA;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? `${accounting.formatMoney(data.Total)}` : data.Total;
				}
			},
			{ data: 'Moneda.Codigo' },
			{ data: 'Saldo.Nombre' }
		],
		drawCallback: (settings) => {
			$(`#${settings.sTableId} tbody tr`).on("contextmenu", (e) => {
				alert('menu derecho');
			});
		}
	});
}
function formaTable() {
	document.querySelectorAll('.dataTables_filter').forEach(e => {
		e.style.marginTop = 0;
		e.classList.add('text-nowrap');
	});
}