var componenteMayor, tabla, tCapacidades, tLimites;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	componenteMayor = new ComponenteMayor(id);
	if (componenteMayor.Valid)
		componenteMayor.write();
	tablas();
	document.getElementById('Codigo').addEventListener('blur', consulta);
	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', registrar);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	document.querySelector('#addCap').addEventListener('click', addCap);
	document.querySelector('#addLim').addEventListener('click', addLim);
	ComponenteMayor.getComponentesMayores().then(data => {
		loadTable(tabla, data.Data);
	});

	let triggerTabList = [].slice.call(document.querySelectorAll('#ComponenteMayor button'));
	triggerTabList.forEach((triggerEl) => {
		let tabTrigger = new bootstrap.Tab(triggerEl);
		triggerEl.addEventListener('click', (event) => {
			event.preventDefault();
			tabTrigger.show();
		});
	});
});
const addCap = () => {
	let idx = document.querySelector(`#IdCapacidad`).selectedIndex > -1 ? document.querySelector(`#IdCapacidad`).selectedIndex : 0;
	let oC = {
		IdComponenteMayor: 0,
		IdCapacidad: parseInt(document.querySelector(`#IdCapacidad`).value) || 0,
		Cantidad: parseInt(document.querySelector(`#Cantidad`).value) || 1,
		Capacidad: {
			Id: parseInt(document.querySelector(`#IdCapacidad`).value) || 0,
			Nombre: document.querySelector(`#IdCapacidad`).options[idx].dataset.nombre,
			Descripcion: document.querySelector(`#IdCapacidad`).options[idx].dataset.descripcion
		}
	}
	componenteMayor.Capacidades = componenteMayor.Capacidades === null ? [] : componenteMayor.Capacidades;
	let ps = componenteMayor.Capacidades.findIndex(cap => { return cap.IdCapacidad === oC.IdCapacidad });
	if (ps === -1)
		componenteMayor.Capacidades.push(clonObject(oC));
	else
		componenteMayor.Capacidades[ps] = clonObject(oC);
	loadTable(tCapacidades, componenteMayor.Capacidades);
}
const addLim = () => {
	let idx = document.querySelector(`#IdLimites`).selectedIndex > -1 ? document.querySelector(`#IdLimites`).selectedIndex : 0;
	let oL = {
		Id: parseInt(document.querySelector(`#IdLimites`).value) || 0,
		Codigo: document.querySelector(`#IdLimites`).options[idx].dataset.codigo,
		Definicion: document.querySelector(`#IdLimites`).options[idx].dataset.definicion
	}
	componenteMayor.Limites = componenteMayor.Limites === null ? [] : componenteMayor.Limites;
	let ps = componenteMayor.Limites.findIndex(cap => { return cap.Id === oL.Id });
	if (ps === -1)
		componenteMayor.Limites.push(clonObject(oL));
	else
		componenteMayor.Limites[ps] = clonObject(oL);
	loadTable(tLimites, componenteMayor.Limites);
}
const consulta = (e) => {
	let cod = e.target.value;
	if (cod === '') return;
	procc();
	componenteMayor.byCadena(cod).then(r => {
		componenteMayor.write();
		if (!componenteMayor.Valid)
			e.target.value = cod;
		endPro();
	});
}
const eliminar = () => {
	if (componenteMayor.Valid) {
		let id = componenteMayor.Id;
		let res = componenteMayor.delete();
		MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			componenteMayor.write();
		}
	}
}
const registrar = () => {
	componenteMayor.read();
	let res = componenteMayor.save();
	MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, componenteMayor, 'Id');
		clear();
	}
}
const clear = () => {
	componenteMayor.clear();
	componenteMayor.write();
}
const tablas = () => {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Id' },
			{ data: 'Codigo' },
			{ data: 'Descripcion' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Activo ? 'Activo' : 'Inactivo' : data.Activo ? 1 : 0;
				}
			},
			{
				data: null,
				render: (data, type) => {
					let caps = [];
					if (data.Capacidades !== null)
						caps = data.Capacidades.reduce((a, b) => { return a + `${b.Capacidad.Nombre}, ` }, '');
					return caps.length > 0 ? caps.substring(0, caps.length - 2) : '';
				}
			},
			{
				data: null,
				render: (data, type) => {
					let lims = [];
					if (data.Limites !== null)
						lims = data.Limites.reduce((a, b) => { return a + `${b.Codigo}, ` }, '');
					return lims.length > 0 ? lims.substring(0, lims.length - 2) : '';
				}
			},
			{
				data: null,
				orderable: false,
				defaultContent: `<button data-type="edit" type="button" class="btn btn-sm btn-outline-primary py-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		columnDefs: [
			{ targets: [0], className: 'text-end' },
			{ targets: [2, 3], className: 'text-justify' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tabla.rows(e.target.closest('tr')).data()[0];
				componenteMayor.clear();
				componenteMayor.setValores(data);
				componenteMayor.write();
				loadTable(tCapacidades, componenteMayor.Capacidades);
				loadTable(tLimites, componenteMayor.Limites);
				let btn = document.querySelector('#Alta');
				let evt = new Event('click');
				btn.dispatchEvent(evt);
				var triggerFirstTabEl = document.querySelector('#Alta-tab');
				bootstrap.Tab.getInstance(triggerFirstTabEl).show();
			});
		}
	});
	tCapacidades = $('#tCapacidades').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col'tr>>",
		data: [],
		columns: [
			{
				data: null,
				className: 'text-center',
				render: (data, type) => {
					return data.Capacidad.Nombre;
				}
			},
			{
				data: 'Cantidad',
				className: 'text-end'
			},
			{
				data: null,
				orderable: false,
				className: 'text-center',
				defaultContent: `<div class="btn-group" role="group">
					<button data-type="edit" type="button" class="btn btn-sm btn-primary py-0"><i class="fas fa-pencil-alt"></i></button>
					<button data-type="delete" type="button" class="btn btn-sm btn-danger py-0"><i class="fas fa-trash-alt"></i></button>
				</div>`
			}
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tCapacidades.rows(e.target.closest('tr')).data()[0];
				document.getElementById('IdCapacidad').value = data.IdCapacidad;
				document.getElementById('Cantidad').value = data.Cantidad;
			});
			$(`#${settings.sTableId} button[data-type="delete"]`).click((e) => {
				let data = tLimites.rows(e.target.closest('tr')).data()[0];
				let idx = componenteMayor.Capacidades.findIndex(c => { return c.IdCapacidad === data.IdCapacidad });
				if (idx > -1)
					componenteMayor.Capacidades.splice(idx, 1);
				loadTable(tLimites, componenteMayor.Capacidades);
			});
		}
	});
	tLimites = $('#tLimites').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col'tr>>",
		data: [],
		columns: [
			{
				data: 'Definicion',
				className: 'text-center'
			},
			{
				data: null,
				orderable: false,
				className: 'text-center',
				defaultContent: `<button data-type="delete" type="button" class="btn btn-sm btn-danger py-0"><i class="fas fa-trash-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="delete"]`).click((e) => {
				let data = tLimites.rows(e.target.closest('tr')).data()[0];
				let idx = componenteMayor.Limites.findIndex(l => { return l.Id === data.Id });
				if (idx > -1)
					componenteMayor.Limites.splice(idx, 1);
				loadTable(tLimites, componenteMayor.Limites);
			});
		}
	});
}