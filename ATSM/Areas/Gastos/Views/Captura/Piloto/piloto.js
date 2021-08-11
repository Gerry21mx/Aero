var tabla, tablaGastos, vuelo, comprobacion, gasto, bsOffcanvas, tipGto, documentos, cuenta;
const monedas = Moneda.getMonedas();
const saldos = Saldo.getSaldos();
document.addEventListener('DOMContentLoaded', () => {
	cuenta = new Account();
	if (!usuario.Valid)
		usuario.getFirmado().then(x => {
			cuenta.byUsuario(usuario.IdUsuario, act).then(x => {
				if (cuenta.Valid)
					vuelosPnd();
			});
		});
	else
		cuenta.byUsuario(usuario.IdUsuario, act).then(x => {
			if (cuenta.Valid)
				vuelosPnd();
		});
	documentos = [];
	TipoGasto.getTipoGastos().then(tipos => tipGto = tipos.Data);
	vuelo = new Vuelo();
	comprobacion = new Comprobacion();
	gasto = new Gasto();
	tables();
	document.querySelectorAll('#LTS_Combustible, #Galones, input[id="Subtotal"], input[id="P_IVA"], input[id="Total"], #ImpHospedaje').forEach(de => de.addEventListener('blur', calcula));
	document.querySelectorAll('button[name="addg"]').forEach(btn => btn.addEventListener('click', addGasto));
	bsOffcanvas = new bootstrap.Offcanvas(document.getElementById('offTrips'));
	document.querySelectorAll('input[name="IdTipoGasto"]').forEach(radio => {
		radio.addEventListener('change', activeForm);
	});
	document.querySelectorAll('#IdSaldo option[data-combustible="True"]').forEach(eOp => { eOp.classList.add('d-none') });
	document.querySelectorAll('a[data-bs-toggle="tab"]').forEach(e => {
		e.addEventListener('shown.bs.tab', (event) => {
			actTGas(event.target.id === 'nav-lista-tab');
		});
	});
	document.getElementById('reg').addEventListener('click', reg);
	document.getElementById('del').addEventListener('click', del);
});
document.body.addEventListener('mousemove', (e) => {
	let ofc = bootstrap.Offcanvas.getInstance(document.getElementById('offTrips'));
	if (ofc && e.clientX < 10) {
		if (!ofc._isShown)
			ofc.show();
	}
})
const vuelosPnd = () => {
	Comprobacion.pendientes(cuenta.Id).then((data) => {
		loadTable(tabla, data.Data);
	}).catch(console.log);
}
const actTGas = (allG = false) => {
	if (!comprobacion.Gastos)
		comprobacion.Gastos = [];
	if (allG) {
		loadTable(tablaGastos, comprobacion.Gastos);
		tablaGastos.columns().visible(true);
		tablaGastos.columns([11, 12]).visible(false);
	} else {
		let tg = parseInt(document.querySelector('input[name="IdTipoGasto"]:checked').value) || 0;
		let aGas = comprobacion.Gastos.filter(g => g.IdTipoGasto === tg);
		loadTable(tablaGastos, aGas);
	}
	resumen();
}
const activeForm = (r) => {
	let tipo = parseInt(r.target.value) || 0;
	document.querySelectorAll('div[data-ocul="true"]').forEach(cont => {
		cont.classList.add('d-none');
		cont.querySelector('input,select').value = '';
	});
	document.getElementById('divDoc').classList.remove('d-none');
	document.getElementById('divCom').classList.add('d-none');
	document.getElementById('Subtotal').disabled = false;
	document.getElementById('Total').disabled = false;
	document.querySelectorAll('#IdSaldo option[data-combustible="False"]').forEach(eOp => { eOp.classList.remove('d-none') });
	document.querySelectorAll('#IdSaldo option[data-combustible="True"]').forEach(eOp => { eOp.classList.add('d-none') });
	tablaGastos.columns().visible(true);
	tablaGastos.columns([11, 12]).visible(false);
	switch (tipo) {
		case 2:
			document.getElementById('Total').disabled = true;
			document.querySelectorAll('#IdAeropuerto,#Factura,#Subtotal,#Subtotal,#P_IVA,#IVA,#fileFactura,#fileXml').forEach(cont => {
				cont.closest('div.col-12').classList.remove('d-none');
			});
			tablaGastos.columns([4, 8, 9]).visible(false);
			break;
		case 3:
			document.getElementById('Subtotal').disabled = true;
			document.querySelectorAll('#IdAeropuerto,#Factura,#Subtotal,#ImpHospedaje,#Subtotal,#P_IVA,#IVA,#fileFactura,#fileXml').forEach(cont => {
				cont.closest('div.col-12').classList.remove('d-none');
			});
			tablaGastos.columns([8, 9]).visible(false);
			break;
		case 4:
			document.getElementById('divCom').classList.remove('d-none');
			document.getElementById('Subtotal').disabled = true;
			document.querySelectorAll('#IdAeropuerto,#Factura,#Subtotal,#Subtotal,#P_IVA,#IVA,#LTS_Combustible,#Galones,#PPL,#fileFactura,#fileXml').forEach(cont => {
				cont.closest('div.col-12').classList.remove('d-none');
			});
			document.querySelectorAll('#IdSaldo option[data-combustible="False"]').forEach(eOp => { eOp.classList.add('d-none') });
			document.querySelectorAll('#IdSaldo option[data-combustible="True"]').forEach(eOp => { eOp.classList.remove('d-none') });
			tablaGastos.columns([4]).visible(false);
			break;
		default:
			document.getElementById('divDoc').classList.add('d-none');
			tablaGastos.columns([0, 1, 3, 4, 5, 8, 9]).visible(false);
	}
	actTGas();
};
const regDocs = (indice) => {
	let data = {
		indice: indice,
		pdf: document.querySelector("#fileFactura").files[0],
		xml: document.querySelector("#fileXml").files[0]
	};
	if (data.pdf === undefined && data.xml === undefined) return false;
	let pos = documentos.findIndex(doc => doc.indice === indice);
	if (pos > -1)
		documentos[pos] = data;
	else
		documentos.push(data);
}
const resumen = () => {
	tipGto.forEach(tgto => {
		saldos.forEach(sldo => {
			monedas.forEach(mnda => {
				let tot = comprobacion.Gastos.reduce((a, b) => {
					if (b.IdTipoGasto === tgto.Id)
						if (b.IdSaldo === sldo.Id)
							if (b.IdMoneda === mnda.Id)
								return a + b.Total;
							else
								return a;
						else
							return a;
					else
						return a;
				}, 0);
				if (document.querySelector(`#s${tgto.Id}_${sldo.Id}_${mnda.Id}`) && tot > 0)
					document.querySelector(`#s${tgto.Id}_${sldo.Id}_${mnda.Id}`).innerText = accounting.formatMoney(tot);
			});
		});
	});
	saldos.forEach(sldo => {
		monedas.forEach(mnda => {
			let tot = comprobacion.Gastos.reduce((a, b) => {
				if (b.IdSaldo === sldo.Id)
					if (b.IdMoneda === mnda.Id)
						return a + b.Total;
					else
						return a;
				else
					return a;
			}, 0);
			if (document.querySelector(`#t_${sldo.Id}_${mnda.Id}`) && tot > 0)
				document.querySelector(`#t_${sldo.Id}_${mnda.Id}`).innerText = accounting.formatMoney(tot);
		});
		if (sldo.Combustible) {
			let toL = comprobacion.Gastos.reduce((a, b) => {
				if (b.IdSaldo === sldo.Id)
					return a + b.LTS_Combustible;
				else
					return a;
			}, 0);
			if (document.querySelector(`#l_${sldo.Id}`) && toL > 0)
				document.querySelector(`#l_${sldo.Id}`).innerText = accounting.formatNumber(toL, 2, ',', '.');
		}
	});
	let totL = comprobacion.Gastos.reduce((a, b) => {
		return a + b.LTS_Combustible;
	}, 0);
	document.querySelector(`#t_l`).innerText = accounting.formatNumber(totL,2,',','.');
}
function reg() {
	comprobacion.IdVuelo = vuelo.IdVuelo;
	comprobacion.IdAccount = cuenta.Id;
	comprobacion.Cuenta = cuenta.clon();
	comprobacion.Observaciones = document.getElementById('Observaciones').value;
	comprobacion.save(documentos).then(rSave => {
		//MsjBox('Comprobacion de Gastos', rSave.Error === '' ? rSave.Mensaje : rSave.Error);
		console.log(rSave.Error === '' ? rSave.Mensaje : rSave.Error);
		if (rSave.Valid) {
			delDT(tabla, "IdVuelo", comprobacion.IdVuelo);
			clearForm();
		}
	});
}
function del() {
	procc();
	if (comprobacion.Valid) {
		let rs = comprobacion.delete();
		MsjBox('Comprobacion de Gastos', rs.Error === '' ? rs.Mensaje : rs.Error);
		if (rs.Valid) {
			delDT(tabla, "IdVuelo", comprobacion.IdVuelo);
			clearForm();
		}
	}
	endPro();
}
function addGasto(e) {
	comprobacion.Gastos = (comprobacion.Gastos === null || comprobacion.Gastos === undefined) ? [] : comprobacion.Gastos;
	gasto.read();
	let tGs = parseInt(document.querySelector('input[name="IdTipoGasto"]:checked').value) || 0;
	gasto.IdTipoGasto = tGs;
	tid = tipGto.findIndex(t => t.Id === gasto.IdTipoGasto);
	gasto.Tipo = clonObject(tipGto[tid]);
	let idx = document.querySelector(`#IdMoneda`).selectedIndex > -1 ? document.querySelector(`#IdMoneda`).selectedIndex : 0;
	gasto.Moneda = {
		Id: gasto.IdMoneda,
		Codigo: document.querySelector(`#IdMoneda`).options[idx].dataset.codigo,
		Nombre: document.querySelector(`#IdMoneda`).options[idx].dataset.nombre
	};
	idx = document.querySelector(`#IdSaldo`).selectedIndex > -1 ? document.querySelector(`#IdSaldo`).selectedIndex : 0;
	gasto.Saldo = {
		Id: gasto.IdSaldo,
		Codigo: document.querySelector(`#IdSaldo`).options[idx].dataset.codigo,
		Nombre: document.querySelector(`#IdSaldo`).options[idx].dataset.nombre,
		Combustible: document.querySelector(`#IdSaldo`).options[idx].dataset.combustible === 'True'
	};
	idx = document.querySelector(`#IdAeropuerto`).selectedIndex > -1 ? document.querySelector(`#IdAeropuerto`).selectedIndex : 0;
	gasto.Aeropuerto = {
		IdAeropuerto: gasto.IdAeropuerto,
		Nombre: document.querySelector(`#IdAeropuerto`).options[idx].dataset.nombre,
		ICAO: document.querySelector(`#IdAeropuerto`).options[idx].dataset.icao,
		IATA: document.querySelector(`#IdAeropuerto`).options[idx].dataset.iata
	};
	gasto.Aeropuerto.Nombre = gasto.IdAeropuerto === 0 ? '' : gasto.Aeropuerto.Nombre;
	switch (gasto.IdTipoGasto) {
		case 1: {
			if (gasto.Concepto === '' || gasto.Total === 0 || gasto.IdMoneda === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.Concepto === '' ? 'Concepto' : gasto.Total === 0 ? 'Total' : gasto.IdMoneda === 0 ? 'Moneda' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
		}
			break;
		case 2: {
			if (gasto.IdAeropuerto === 0 || gasto.Factura === '' || gasto.Concepto === '' || gasto.Subtotal === 0 || gasto.IdMoneda === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.IdAeropuerto === 0 ? 'Aeropuerto' : gasto.Factura === '' ? 'Factura' : gasto.Concepto === '' ? 'Concepto' : gasto.Subtotal === 0 ? 'Subtotal' : gasto.IdMoneda === 0 ? 'Moneda' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
		} break;
		case 3: {
			if (gasto.IdAeropuerto === 0 || gasto.Factura === '' || gasto.Concepto === '' || gasto.Subtotal === 0 || gasto.IdMoneda === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.IdAeropuerto === 0 ? 'Aeropuerto' : gasto.Factura === '' ? 'Factura' : gasto.Concepto === '' ? 'Concepto' : gasto.Subtotal === 0 ? 'Subtotal' : gasto.IdMoneda === 0 ? 'Moneda' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
		} break;
		case 4: {
			gasto.Concepto = 'Combustible';
			if (gasto.IdAeropuerto === 0 || gasto.Factura === '' || gasto.LTS_Combustible === 0 || gasto.IdSaldo === 0) {
				MsjBox('Falta Informacion', `El Campo <b>${(gasto.IdAeropuerto === 0 ? 'Aeropuerto' : gasto.Factura === '' ? 'Factura' : gasto.LTS_Combustible === 0 ? 'Litros' : 'Saldo')}</b> No puede quedar Vacio`);
				return false;
			}
		} break;
	}
	let idg = -1;
	if (gasto.Id > 0)
		idg = comprobacion.Gastos.findIndex(g => g.Id === gasto.Id);
	else
		idg = idg === -1 ? comprobacion.Gastos.findIndex(g => g.Consecutivo === gasto.Consecutivo) : idg;
	if (idg > -1)
		comprobacion.Gastos[idg] = gasto.clon();
	else {
		gasto.Consecutivo = comprobacion.Gastos.length;
		comprobacion.Gastos.push(gasto.clon());
	}
	actTGas();
	regDocs(gasto.Consecutivo);
	gasto.clear();
	gasto.write();
	document.querySelector("#fileFactura").value = '';
	document.querySelector("#fileXml").value = '';
	document.getElementById(tGs === 1 ? 'Concepto' : 'IdAeropuerto').focus();
}
function calcula(e) {
	let tg = parseInt(document.querySelector('input[name="IdTipoGasto"]:checked').value) || 0;
	let contenedor = e.target.closest('div[role="tabpanel"]');
	let piv = parseFloat(contenedor.querySelector('input[id="P_IVA"]').value) || 0;
	if (tg === 2) {
		let sub = parseFloat(contenedor.querySelector('input[id="Subtotal"]').value) || 0;
		let iva = sub * (piv / 100);
		let tot = sub + iva;
		contenedor.querySelector('input[id="IVA"]').value = iva.toFixed(2);
		contenedor.querySelector('input[id="Total"]').value = tot.toFixed(2);
	}

	if (tg === 3) {
		let hos = parseFloat(contenedor.querySelector('input[id="ImpHospedaje"]').value) || 0;
		let tot = parseInt(contenedor.querySelector('input[id="Total"]').value) || 0;
		let xtr = tot - hos;
		let sub = parseFloat((xtr / (1 + (piv / 100))).toFixed(2));
		let iva = xtr - sub;
		contenedor.querySelector('input[id="Subtotal"]').value = sub.toFixed(2);
		contenedor.querySelector('input[id="IVA"]').value = iva.toFixed(2);
	}

	if (tg === 4) {
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
function clearForm() {
	document.querySelectorAll('#tGtos td,#tComb td').forEach(c => { c.innerText = ''; });
	gasto.clear();
	gasto.write();
	comprobacion.clear();
	comprobacion.write();
	comprobacion.Gastos = [];
	documentos = [];
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
				clearForm();
				bsOffcanvas.hide();
				let selected = !e.currentTarget.classList.contains('selected');
				document.querySelectorAll('#flgInfo span').forEach(e => { e.innerText = ''; });
				document.getElementById('nav-gastos-tab').classList.add('disabled');
				if (!selected)
					return false;
				document.getElementById('nav-gastos-tab').classList.remove('disabled');
				let data = tabla.row(e.currentTarget).data();
				vuelo = new Vuelo(data.IdVuelo);
				let ttri = vuelo.Trip;
				let ncap = vuelo.Capitan.Nombre.toLowerCase();
				let ncop = vuelo.Copiloto.Nombre.toLowerCase();
				let ruta = `${vuelo.Tramos[0].Origen.IATA}`;
				let aptoIni = { id: vuelo.Tramos[0].IdOrigen, nombre: vuelo.Tramos[0].Origen.Nombre, iata: vuelo.Tramos[0].Origen.IATA, icao: vuelo.Tramos[0].Origen.ICAO };
				if (vuelo.IdCapitan !== cuenta.IdCrew) {
					let idx = vuelo.Tramos.findIndex(t => t.IdCapitan === cuenta.IdCrew);
					if (idx > -1) {
						ncap = vuelo.Tramos[idx].Capitan.Nombre.toLowerCase();
						ncop = vuelo.Tramos[idx].Copiloto.Nombre.toLowerCase();
						ttri += ` - ${vuelo.Tramos[idx].Pierna}`;
						ruta = `${vuelo.Tramos[idx].Origen.IATA}`;
						aptoIni.id = vuelo.Tramos[idx].IdOrigen;
						aptoIni.nombre = vuelo.Tramos[idx].Origen.Nombre;
						aptoIni.iata = vuelo.Tramos[idx].Origen.IATA;
						aptoIni.icao = vuelo.Tramos[idx].Origen.ICAO;
					}
					else {
						MsjBox('Error en Vuelo', 'Este vuelo no contiene tramos volados por ti, reportalo a Control de Tripulaciones');
						return false;
					}
				}
				document.getElementById('txtTrip').innerText = ttri;
				document.getElementById('txtFecha').innerText = moment(vuelo.Salida).format('DD MMM YYYY');
				document.getElementById('txtMatricula').innerText = vuelo.Aeronave.Matricula;
				document.getElementById('txtCapitan').innerText = ncap;
				document.getElementById('txtCopiloto').innerText = ncop;
				var aptos = [{ id: 0, iata: '...' }, aptoIni];
				vuelo.Tramos.forEach(i => {
					if (i.Destino !== null) {
						if (i.IdCapitan === cuenta.IdCrew) {
							ruta += `-${i.Destino.IATA}`;
							let idx = aptos.findIndex(a => a.id === i.IdDestino);
							if (idx === -1)
								aptos.push({ id: i.IdDestino, nombre: i.Destino.nombre, iata: i.Destino.IATA, icao: i.Destino.ICAO });
						}
					}
				});
				document.getElementById('IdAeropuerto').innerHTML = '';
				aptos.forEach(a => {
					document.getElementById('IdAeropuerto').innerHTML += `<option value="${a.id}" data-nombre="${a.nombre}" data-iata="${a.iata}" data-icao="${a.icao}">${a.iata}</option>`;
				});
				document.getElementById('txtRuta').innerText = ruta;
				document.querySelectorAll('.dataTables_filter').forEach(e => {
					e.style.marginTop = 0;
					e.classList.add('text-nowrap');
				});
			});
		}
	});
	tablaGastos = $('#gastos').DataTable({
		processing: true,
		rowGroup: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-sm-4'l><'col-12 col-sm-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		order: [[10, 'asc'], [11, 'asc'], [0, 'asc'], [7, 'asc'], [2, 'asc']],
		rowGroup: {
			dataSrc: ['Tipo.Nombre', 'Saldo.Nombre'],
			startClassName: 'bg-success fw-bolder',
			endClassName: 'bg-secondary',
			startRender: (rows, group) => {
				return `${rows.count()} Gastos ${group}`;
			},
			endRender: (rows, group, level) => {
				let totales = '';
				monedas.forEach((moneda) => {
					let datos = rows.data().toArray();
					let tot = datos.length > 0 ? datos.reduce((a, b) => {
						return b.IdMoneda === moneda.Id ? a + b.Total : a;
					}, 0) : 0;
					totales += ` ${moneda.Codigo} ${accounting.formatMoney(tot)} | `;
				});
				return `${totales}`;
			}
		},
		columns: [
			{
				data: 'Aeropuerto.IATA',
				visible: false
			},
			{
				data: 'Factura',
				visible: false
			},
			{ data: 'Concepto' },
			{
				data: 'Subtotal',
				visible: false,
				render: (data, type) => {
					if (data > 0)
						return type === 'display' ? accounting.formatMoney(data) : data;
					else
						return '';
				}
			},
			{
				data: 'ImpHospedaje',
				visible: false,
				render: (data, type) => {
					if (data > 0)
						return type === 'display' ? accounting.formatMoney(data) : data;
					else
						return '';
				}
			},
			{
				data: null,
				visible: false,
				render: (data, type) => {
					if (data.IVA > 0)
						return type === 'display' ? `${accounting.formatMoney(data.IVA)} (${data.P_IVA}%)` : data.IVA;
					else
						return '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? `${accounting.formatMoney(data.Total)}` : data.Total;
				}
			},
			{ data: 'Moneda.Codigo' },
			{
				data: 'LTS_Combustible',
				visible: false
			},
			{
				data: null,
				visible: false,
				render: (data, type) => {
					if (data.IdTipoGasto === 4)
						return type === 'display' ? `${accounting.formatMoney(data.Total / data.LTS_Combustible)}` : data.Total;
					else
						return '';
				}
			},
			{ data: 'Tipo.Nombre', visible: false },
			{ data: 'Saldo.Nombre', visible: false },
			{
				data: null,
				defaultContent: `<div class="btn-group" role="group" aria-label="Basic example">
  <button type="button" class="btn btn-sm btn-primary py-0" data-type="edit"><i class="fas fa-pen-square"></i></button>
  <button type="button" class="btn btn-sm btn-primary py-0" data-type="delete"><i class="fas fa-trash-alt"></i></button>
</div>` }
		],
		columnDefs: [
			{ targets: [2], className: 'text-start' },
			{ targets: [3, 4, 5, 6, 8, 9], className: 'text-end' },
			{ targets: '_all', className: 'text-center', orderable: false }
		],
		drawCallback: (settings) => {
			$(`#${settings.sTableId} button[data-type="edit"]`).on("click", (e) => {
				let data = tablaGastos.rows(e.target.closest('tr')).data()[0];
				gasto.clear();
				gasto.setValores(data);
				gasto.write();
				let radio = document.querySelector(`input[name="IdTipoGasto"][value="${gasto.IdTipoGasto}"]`);
				radio.checked = true
				let evt = new Event('change');
				radio.dispatchEvent(evt);
				let tab = bootstrap.Tab.getInstance(document.getElementById('nav-gastos-tab'));
				tab.show();
			});
			$(`#${settings.sTableId} button[data-type="delete"]`).on("click", (e) => {
				let data = tablaGastos.rows(e.target.closest('tr')).data()[0];
				let idx = comprobacion.Gastos.findIndex(g => { g.Consecutivo === data.Consecutivo });
				comprobacion.Gastos.splice(idx, 1);
				actTGas();
			});
		}
	});
}