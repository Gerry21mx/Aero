var componente, titulo, tLim, tMod, limi;
document.addEventListener('DOMContentLoaded', () => {
	limi = new Limite();
	let id = parseInt(getPar('id')) || 0;
	componente = new ComponenteMenor(id);
	tablas();
	if (componente.Valid)
		if (componente.IdTipoMenor === TCOM) {
			componente.write();
			loadTable(tMod, componente.Modelos);
			loadTable(tLim, componente.Limites);
		}
		else {
			e.target.value = '';
			MsjBox('Consulta Componente', 'El Componente NO corresponde al Tipo de Componente.')
		}
	titulo = document.getElementById('tit').innerText;
	document.getElementById('Part').addEventListener('blur', consulta);
	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', registrar);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	document.querySelector('#addMod').addEventListener('click', addMod);
	document.querySelector('#addLim').addEventListener('click', addLim);
	if (document.querySelector('#genpn'))
		document.querySelector('#genpn').addEventListener('click', generate);
	document.getElementById('IdComponenteMayor').addEventListener('change', vMayor);
	document.querySelectorAll('#IdModelo option').forEach(om => { if(om.value !== '0') om.classList.add('d-none'); });
	document.querySelectorAll('#IdFamily option').forEach(of => {
		if (of.value !== '0' && of.dataset[`tm0${TCOM}`] !== 'True')
			of.classList.add('d-none');
	});
});
const generate = (e) => {
	let d = document.getElementById('Directive').value.trim();
	let s = document.getElementById('ServiceBulletin').value.trim();
	if (d !== '' || s !== '')
		ComponenteMenor.generaraPN(d !== '' ? d : s).then(data => {
			if (data.Data)
				document.getElementById('Part').value = d !== '' ? data.Data.PNAD : data.Data.PNSB;
		})
}
const vMayor = (e) => {
	let idm = parseInt(e.target.value) || 0;
	let cMayor = new ComponenteMayor(idm);
	document.querySelectorAll('#IdModelo option').forEach(om => { if (om.value !== '0') om.classList.add('d-none'); });
	document.querySelectorAll('#IdModelo option').forEach(om => {
		let idc = parseInt(om.dataset.idcomponentemayor) || 0;
		if (cMayor.Id === idc || om.value === '0')
			om.classList.remove('d-none');
	});
}
const addMod = () => {
	let idx = document.querySelector(`#IdModelo`).selectedIndex > -1 ? document.querySelector(`#IdModelo`).selectedIndex : 0;
	let idxm = document.querySelector(`#IdComponenteMayor`).selectedIndex > -1 ? document.querySelector(`#IdComponenteMayor`).selectedIndex : 0;
	let oM = {
		IdComponenteMayor: parseInt(document.getElementById(`IdComponenteMayor`).value) || 0,
		IdComponenteMenor: componente.Id,
		IdModelo: parseInt(document.getElementById(`IdModelo`).value) || 0,
		Cantidad: parseInt(document.getElementById(`Cantidad`).value) || 1,
		Modelo: {
			Id: parseInt(document.getElementById(`IdModelo`).value) || 0,
			Nombre: document.querySelector(`#IdModelo`).options[idx].dataset.nombre
		},
		Mayor: {
			Id: parseInt(document.getElementById(`IdComponenteMayor`).value) || 0,
			Codigo: document.querySelector(`#IdComponenteMayor`).options[idxm].dataset.codigo,
			Descripcion: document.querySelector(`#IdComponenteMayor`).options[idxm].dataset.descripcion
		}
	}
	componente.Modelos = componente.Modelos === null ? [] : componente.Modelos;
	let ps = componente.Modelos.findIndex(mod => { return mod.IdModelo === oM.IdModelo });
	if (ps === -1)
		componente.Modelos.push(clonObject(oM));
	else
		componente.Modelos[ps] = clonObject(oM);
	loadTable(tMod, componente.Modelos);
	document.getElementById('IdModelo').value = 0;
	document.getElementById('Cantidad').value = 0;
}
const addLim = () => {
	let idx = document.querySelector(`#IdLimite`).selectedIndex > -1 ? document.querySelector(`#IdLimite`).selectedIndex : 0;
	limi.read('', 'contLim');
	limi.Limit = {
		Id: parseInt(document.querySelector(`#IdLimite`).value) || 0,
		Codigo: document.querySelector(`#IdLimite`).options[idx].dataset.codigo,
		Definicion: document.querySelector(`#IdLimite`).options[idx].dataset.definicion
	};
	componente.Limites = componente.Limites === null ? [] : componente.Limites;
	let ps = componente.Limites.findIndex(lm => { return lm.IdLimite === limi.IdLimite; });
	if (ps === -1)
		componente.Limites.push(limi.clon());
	else
		componente.Limites[ps] = limi.clon();
	loadTable(tLim, componente.Limites);
	limi.clear();
	limi.write('', 'contLim');
}

const consulta = (e) => {
	let part = e.target.value;
	if (part === '') return;
	procc();
	componente.byCadena(part).then(r => {
		if (componente.Valid)
			if (componente.IdTipoMenor === TCOM) {
				componente.write();
				if (!componente.Valid)
					e.target.value = part;
				else {
					loadTable(tMod, componente.Modelos);
					loadTable(tLim, componente.Limites);
				}
			}
			else {
				e.target.value = '';
				MsjBox('Consulta Componente', 'El Componente NO corresponde al Tipo de Componente.')
			}
		endPro();
	});
}
const eliminar = () => {
	if (componente.Valid) {
		let id = componente.Id;
		let res = componente.delete();
		MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			componente.write();
		}
	}
}
const registrar = () => {
	componente.read();
	componente.IdTipoMenor = TCOM;
	let res = componente.save();
	MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		clear();
	}
}
const clear = () => {
	componente.clear();
	componente.write();
	tLim.clear();
	tMod.clear();
	tLim.columns.adjust().draw();
	tMod.columns.adjust().draw();
}
const tablas = () => {
	tLim = $('#tLim').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col'tr>>",
		data: [],
		columns: [
			{ data: 'Limit.Codigo' },
			{
				data: 'Horas',
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data,0,',','.') : data;
				}
			},
			{
				data: 'Ciclos',
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data, 0, ',', '.') : data;
				}
			},
			{
				data: 'Dias',
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data, 0, ',', '.') : data;
				}
			},
			{
				data: null,
				orderable: false,
				className: 'd-grid',
				defaultContent: `<button data-type="edit" type="button" class="btn btn-primary btn-primary py-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		columnDefs: [
			{ targets: [0], className: 'text-center' },
			{ targets: [1, 2, 3], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tLim.rows(e.target.closest('tr')).data()[0];
				limi.setValores(data);
				limi.write('', 'contLim');
			});
		}
	});
	tMod = $('#tMod').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col'tr>>",
		data: [],
		columns: [
			{ data: 'Mayor.Codigo' },
			{ data: 'Modelo.Nombre' },
			{
				data: 'Cantidad',
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data,0,',','.') : data;
				}
			},
			{
				data: null,
				orderable: false,
				className: 'd-grid',
				defaultContent: `<div class="btn-group btn-group-sm" role="group" aria-label="Botones">
  <button data-type="edit" type="button" class="btn btn-primary btn-primary py-0"><i class="fas fa-pencil-alt"></i></button>
  <button data-type="delete" type="button" class="btn btn-primary btn-danger py-0"><i class="fas fa-trash-alt"></i></button>
</div>`
			}
		],
		columnDefs: [
			{ targets: [0], className: 'text-center' },
			{ targets: [1, 2], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tMod.rows(e.target.closest('tr')).data()[0];
				document.getElementById('IdModelo').value = data.IdModelo;
				document.getElementById('Cantidad').value = data.Cantidad;
			});
			$(`#${settings.sTableId} button[data-type="delete"]`).click((e) => {
				let data = tMod.rows(e.target.closest('tr')).data()[0];
				let i = componente.Modelos.findIndex(l => { return l.IdModelo === data.IdModelo });
				if (i > -1)
					componente.Modelos.splice(i, 1);
				loadTable(tMod, componente.Modelos);
			});
		}
	});
}