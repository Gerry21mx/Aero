var pic, tabla, coMen;
document.addEventListener('DOMContentLoaded', () => {
	coMen = new ComponenteMenor();
	let id = parseInt(getPar('id')) || 0;
	pic = new PIC(id);
	if (pic.Valid)
		pic.write();
	tablas();
	clear();
	//document.getElementById('Nombre').addEventListener('blur', consulta);
	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', registrar);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', eliminar);
	if (document.querySelector('#cle'))
		document.querySelector('#cle').addEventListener('click', clear);
	
	document.getElementById('IdComponente').addEventListener('change', (e) => {
		let idc = parseInt(e.target.value) || 0;
		document.querySelectorAll('#IdModelo option').forEach(om => {
			let mcm = parseInt(om.dataset.idcomponentemayor) || 0;
			if (idc === mcm || om.value === '0')
				om.classList.remove('d-none')
			else
				om.classList.add('d-none');
		});
	})
	document.getElementById('IdModelo').addEventListener('blur', conCoMen);
	document.getElementById('IdMenor').addEventListener('blur', (e) => {
		if (e.target.value === '') return;
		let op = document.querySelector(`#list${e.target.id} option[value="${e.target.value}"]`);
		if (op) {
			let idc = parseInt(op.dataset.id) || 0;
			let idmy = parseInt(document.getElementById('IdComponente').value) || 0;
			let idmdl = parseInt(document.getElementById('IdModelo').value) || 0;
			PIC.MenorEnPICMayorModelo(idmy, idmdl, idc).then(data => {
				if (data.Data) {
					let im = data.Data.Menor.Modelos.findIndex(m => { return m.IdModelo === idmdl; });
					if (im > -1) {
						if (data.Data.Count < data.Data.Menor.Modelos[im].Cantidad) {
							coMen.setValores(data.Data.Menor);
							e.target.value = coMen.Part;
						} else {
							coMen.clear();
							MsjBox("Componente en PIC","Se ha alcanzado el Numero MAXIMO permitido de Componentes en el Modelo, Revise la Definicion del Componente Menor.")
						}
					}
					else {
						coMen.clear();
						MsjBox("Componente en PIC","El Componente no esta asignado al Modelo.")
					}
				}
			})
		} else {
			MsjBox("Componente en PIC","El Componente no esta asignado al Modelo.")
		}
		if (!coMen.Valid)
			e.target.value = '';
	});
	document.getElementById('addCom').addEventListener('click', addCom)
	document.querySelectorAll('#IdComponente, #IdModelo, #ATA1').forEach(s => { s.addEventListener('change', conEx) })
});
const conEx = (e) => {
	let my = parseInt(document.getElementById('IdComponente').value) || 0;
	let md = parseInt(document.getElementById('IdModelo').value) || 0;
	let at = parseInt(document.getElementById('ATA1').value) || 0;
	if (my > 0 && md > 0 && at > 0)
		PIC.getByMMA(my, md, at).then(data => {
			if (data.Data !== null)
				loadTable(tabla, data.Data)
		})
}
const addCom = (e) => {
	pic.Componentes = pic.Componentes === null ? [] : pic.Componentes;
	let ps = pic.Componentes.findIndex(com => { return com.Id === coMen.Id });
	if (ps === -1)
		pic.Componentes.push(coMen.clon());
	else
		pic.Componentes[ps] = coMen.clon();
	loadTable(tComponentes, pic.Componentes);
	document.getElementById('IdMenor').value = '';
	coMen.clear();
	document.getElementById('IdMenor').focus();

}
const conCoMen = (e) => {
	let idm = parseInt(document.getElementById('IdModelo').value) || 0;
	if (idm > 0)
		ComponenteMenor.getByModelo(idm).then(data => {
			document.getElementById('listIdMenor').innerHTML = '';
			data.Data.forEach(cm => {
				let ola = document.createElement('option');
				ola.value = cm.Part;
				ola.dataset.id = cm.Id;
				ola.dataset.description = cm.Description;
				document.getElementById('listIdMenor').append(ola);
			});
		});
}
const consulta = (e) => {
	let cod = e.target.value;
	if (cod === '') return;
	procc();
	pic.byCadena(cod).then(r => {
		pic.write();
		if (!pic.Valid)
			e.target.value = cod;
		endPro();
	});
}
const eliminar = () => {
	if (pic.Valid) {
		let id = pic.Id;
		let res = pic.delete();
		MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			pic.write();
		}
	}
}
const registrar = () => {
	pic.read();
	let res = pic.save();
	MsjBox('PIC', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, pic, 'Id');
		clear();
	}
}
const clear = () => {
	pic.clear();
	pic.write();
	document.querySelectorAll('#IdModelo option').forEach(om => { if (om.value !== '0') om.classList.add('d-none') });
	tabla.clear().columns.adjust().draw();
	tComponentes.clear().columns.adjust().draw();
}
const tablas = () => {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Mayor.Descripcion' },
			{ data: 'Modelo.Nombre' },
			{ data: 'ATA1' },
			{ data: 'Posicion.Nombre' },
			{ data: 'Consecutivo' },
			{ data: 'Descripcion' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Activo ? 'Activo' : 'Inactivo' : data.Activo ? 1 : 0;
				}
			},
			{
				data: null,
				orderable: false,
				defaultContent: `<button data-type="edit" type="button" class="btn btn-sm btn-outline-primary py-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		columnDefs: [
			{ targets: [2,4], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tabla.rows(e.target.closest('tr')).data()[0];
				pic.clear();
				pic = new PIC(data.Id);
				pic.write();
				loadTable(tComponentes, pic.Componentes);
			});
		}
	});
	tComponentes = $('#tComponentes').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col'tr>>",
		data: [],
		columns: [
			{
				data: 'Part',
				className: 'text-center'
			},
			{ data: 'Description' },
			{
				data: null,
				orderable: false,
				className: 'text-center',
				defaultContent: `<button data-type="delete" type="button" class="btn btn-sm btn-danger py-0"><i class="fas fa-trash-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="delete"]`).click((e) => {
				let data = tComponentes.rows(e.target.closest('tr')).data()[0];
				let idx = pic.Componentes.findIndex(l => { return l.Id === data.Id });
				if (idx > -1)
					pic.Componentes.splice(idx, 1);
				loadTable(tComponentes, pic.Componentes);
			});
		}
	});
}