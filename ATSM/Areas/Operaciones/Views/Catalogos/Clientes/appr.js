var cliente, contacto, actividad, tabla, tContactos, tActividad;
document.addEventListener('DOMContentLoaded', (e) => {
	let id = parseInt(getPar('id')) || 0;
	cliente = new Cliente(id);
	contacto = new ClienteContacto();
	actividad = new ClienteActividad();
	if (cliente.Valid) {
		cliente.write();
		loadTable(tContactos, cliente.Contactos)
		loadTable(tActividad, cliente.Registro)
		document.querySelectorAll('#delAct,#regAct,#delCon,#regCon').forEach(btn => { btn.disabled = !cliente.Valid })
	}
	document.getElementById('Nombre').addEventListener('blur', conClient);
	document.querySelector('#fContact input[id="Nombre"]').addEventListener('blur', conContact);

	document.querySelector('#cle').addEventListener('click', clear);
	document.querySelector('#cleCon').addEventListener('click', cleContact);
	document.querySelector('#cleAct').addEventListener('click', cleActivity);

	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', regClient);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', delClient);

	if (document.querySelector('#regCon'))
		document.querySelector('#regCon').addEventListener('click', regContact);
	if (document.querySelector('#delCon'))
		document.querySelector('#delCon').addEventListener('click', delContact);

	if (document.querySelector('#regAct'))
		document.querySelector('#regAct').addEventListener('click', regActivity);

	tablas();
	Cliente.getClientes().then(datos => { loadTable(tabla, datos.Data); });
})
const conClient = (e) => {
	let cli = e.target.value;
	if (cli === '' || cliente.Valid) return;
	document.querySelectorAll('#delAct,#regAct,#delCon,#regCon').forEach(btn => { btn.disabled = true })
	procc();
	cliente.byCadena(cli).then(r => {
		cliente.write('', 'fClient');
		loadTable(tContactos, cliente.Contactos)
		loadTable(tActividad, cliente.Registro)
		document.querySelectorAll('#delAct,#regAct,#delCon,#regCon').forEach(btn => { btn.disabled = !cliente.Valid })
		if (!cliente.Valid)
			e.target.value = cli;
		endPro();
	});
}
const conContact = (e) => {
	let con = e.target.value;
	if (con === '' || !cliente.Valid || contacto.Valid) return;
	procc();
	contacto.byNombre(cliente.Id, con).then(r => {
		contacto.write('', 'fContact');
		if (!contacto.Valid)
			e.target.value = con;
		endPro();
	});
}

const regClient = (e) => {
	cliente.read('', 'fClient');
	cliente.IdTipo = parseInt(document.querySelector('input[name="IdTipo"]:checked').value) || 0;
	let res = cliente.save();
	MsjBox('Cliente', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, cliente, 'Id');
		//clear();
	}
}
const regContact = (e) => {
	if (!cliente.Valid) return;
	contacto.read('', 'fContact');
	contacto.IdCliente = cliente.Id;
	let res = contacto.save();
	MsjBox('Contactos', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tContactos, contacto, 'Id');
		cleContact();
	}
}
const regActivity = (e) => {
	if (!cliente.Valid) return;
	actividad.read('', 'fActivity')
	actividad.IdCliente = cliente.Id
	actividad.Tipo = document.querySelector('input[name="Tipo"]:checked').value
	let res = actividad.save()
	MsjBox('Registro de Actividad', res.Error !== '' ? res.Error : res.Mensaje)
	if (res.Valid) {
		updDT(tActividad, actividad, 'Id')
		cleActivity()
	}
}

const delClient = (e) => {
	if (cliente.Valid) {
		let id = cliente.Id;
		let res = cliente.delete();
		MsjBox('Cliente', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			cliente.write('', 'fClient');
		}
	}
}
const delContact = (e) => {
	if (contacto.Valid) {
		let id = contacto.Id;
		let res = contacto.delete();
		MsjBox('Contacto', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tContactos, 'Id', id);
			contacto.write('', 'fContact');
		}
	}
}

const clear = () => {
	cliente.clear()
	cliente.write('', 'fClient')
	cleContact()
	cleActivity()
	tContactos.clear().draw()
	tActividad.clear().draw()
	document.querySelectorAll('#delAct,#regAct,#delCon,#regCon').forEach(btn => { btn.disabled = true })
}
const cleContact = () => {
	contacto.clear()
	contacto.write('', 'fContact')
}
const cleActivity = () => {
	contacto.clear()
	contacto.write('', 'fActivity')
}
const tablas = () => {
	tabla = $('#tabReg').DataTable({
		processing: false,
		rowGroup: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row justify-content-evenly'<'col-4'l><'col-4'f>><'row justify-content-around'<'col-12't>><'row justify-content-center'<'col-6 text-center'p>>",
		data: [],
		order: [[9, 'asc'], [2, 'asc']],
		rowGroup: {
			dataSrc: ['Grupo'],
			startClassName: 'bg-success fw-bolder',
			endClassName: 'bg-primary',
			startRender: (rows, group) => {
				return `${rows.count()} Grupo ${group}`;
			}
			//endRender: (rows, group, level) => {
			//	let totales = '';
			//	let datos = rows.data().toArray();
			//	let tot = datos.length > 0 ? datos.reduce((a, b) => {
			//		return b.Cerrada ? a + 1 : a;
			//	}, 0) : 0;
			//	totales += `Cerrados ${accounting.formatNumber(tot, 2, ',', '.')} | `;

			//	return `${totales}`;
			//}
		},
		buttons: ['excel', 'print'],
		columns: [
			{ data: 'Id' },
			{ data: 'Nombre' },
			{ data: 'RazonSocial' },
			{ data: 'RFC' },
			{ data: 'Tipo.Tipo' },
			{ data: 'Conmutador' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Activo ? 'Activo' : 'Inactivo' : data.Activo ? 1 : 0;
				}
			},
			{ data: 'Estado' },
			{ data: 'Ciudad' },
			{
				data: 'Grupo',
				visible: false
			},
			{
				data: null,
				orderable: false,
				className: 'd-grid',
				defaultContent: `<button data-type="edit" type="button" class="btn btn-primary btn-primary py-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		columnDefs: [
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tabla.rows(e.target.closest('tr')).data()[0];
				cliente.clear();
				cliente.setValores(data);
				cliente.write('', 'fClient');
				ClienteContacto.getClienteContactos(cliente.Id).then(data => {
					cliente.Contactos = clonObject(data.Data)
					loadTable(tContactos, cliente.Contactos)
				})
				ClienteActividad.getClienteActividads(cliente.Id).then(data => {
					cliente.Registro = clonObject(data.Data)
					loadTable(tActividad, cliente.Registro)
				})
				document.querySelectorAll('#delAct,#regAct,#delCon,#regCon').forEach(btn => { btn.disabled = !cliente.Valid })
				let trigger = document.querySelector('#Alta-tab');
				let tab = new bootstrap.Tab(trigger);
				if (tab)
					tab.show();
			});
			document.querySelectorAll(`#${settings.sTableId}_wrapper:first-child input, select`).forEach(elem => { elem.classList.add('p-0'); })
			document.querySelectorAll(`#${settings.sTableId}_wrapper:last-child .paginate_button`).forEach(elem => { elem.classList.add('py-0'); })
			document.querySelectorAll(`#${settings.sTableId}_wrapper`).forEach(elem => { elem.classList.add('sz-font-8'); })
		}
	})
	tContactos = $('#tContactos').DataTable({
		processing: false,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'#tHead2.row justify-content-evenly'<'col-4'l><'col-4'b><'col-4 mb-1'f>><'row justify-content-around'<'col-12't>><'row justify-content-center'<'col-6 text-center'p>>",
		data: [],
		order: [[1, 'asc'], [0, 'asc']],
		buttons: ['excel', 'print'],
		columns: [
			{ data: 'Nombre' },
			{ data: 'Puesto' },
			{
				data: null,
				render: (data, type) => {
					let contact = `<a href="tel:${data.Telefono}"><i class="fas fa-phone-volume"></i></a>${(data.Extension ? ` <b>Ext. </b> ${data.Extension}` : '')}`
					if (data.Celular)
						contact += ` | <a href="tel:${data.Celular}"><i class="fas fa-mobile-alt"></i></a> | <a href="https://api.whatsapp.com/send?phone=${(data.Celular.indexOf('+') === -1 ? `+52${data.Celular}` : data.Celular)}&text=Buen%20Dia"><i class="fab fa-whatsapp"></i></a>`
					if (data.Otro)
						contact += ` | <a href="tel:${data.Otro}"><i class="fas fa-address-book"></i></a>`
					if (data.Correo)
						contact += ` | <a href="mailto:${data.Correo}?Subject=Buen%20Dia"><i class="fas fa-at"></i></a>`

					return contact;
				}
			}
			//{
			//	data: null,
			//	orderable: false,
			//	className: 'd-grid',
			//	defaultContent: `<button data-type="edit" type="button" class="btn btn-primary btn-primary py-0"><i class="fas fa-pencil-alt"></i></button>`
			//}
		],
		columnDefs: [
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tContactos.rows(e.target.closest('tr')).data()[0];
				contacto.clear();
				contacto.setValores(data);
				contacto.write('', 'fContact');
			});
			document.querySelectorAll(`#${settings.sTableId}_wrapper:first-child input, select`).forEach(elem => { elem.classList.add('p-0'); })
		}
	})
	tActividad = $('#tActividad').DataTable({
		processing: false,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'#tHead3.row justify-content-evenly'<'col-4'l><'col-4'b><'col-4'f>><'row justify-content-around'<'col-12't>><'row justify-content-center'<'col-6 text-center'p>>",
		data: [],
		order: [[0, 'desc']],
		buttons: ['excel', 'print'],
		columns: [
			{
				data: 'Fecha',
				render: (data, type) => {
					return moment(data).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD');
				}
			},
			{ data: 'Tipo' },
			{
				data: 'Comentarios',
				className: 'text-start'
			}
		],
		columnDefs: [
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			document.querySelectorAll(`#${settings.sTableId}_wrapper:first-child input, select`).forEach(elem => { elem.classList.add('p-0'); })
		}
	})
}