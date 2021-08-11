var modelo, tabla;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	modelo = new Modelo(id);
	if (modelo.Valid)
		modelo.write();
	tablas();
	document.getElementById('Nombre').addEventListener('blur', consulta);
	document.querySelector('#reg').addEventListener('click', registrar);
	document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	Modelo.getModelos().then(data => {
		loadTable(tabla, data.Data);
	});
	document.getElementById('IdComponenteMayor').addEventListener('change', (e) => {
		let idc = parseInt(e.target.value) || 0;
		let componente = new ComponenteMayor(idc);
		document.querySelectorAll(`.row[data-tipo="Limite"]`).forEach(r => { r.classList.add('d-none'); });
		componente.Limites.forEach(lim => {
			document.querySelector(`.row[data-idlimite="${lim.Id}"]`).classList.remove('d-none');
		});
	})
});
const consulta = (e) => {
	let cod = e.target.value;
	if (cod === '') return;
	procc();
	modelo.byCadena(cod).then(r => {
		modelo.write();
		if (!modelo.Valid)
			e.target.value = cod;
		endPro();
	});
}
const eliminar = () => {
	if (modelo.Valid) {
		let id = modelo.Id;
		let res = modelo.delete();
		MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			modelo.write();
		}
	}
}
const registrar = () => {
	console.log(`Registrar ${moment().format('DD MMM yyyy HH:MM:s')}`);
	modelo.read();
	modelo.Limites = [];
	document.querySelectorAll(`.row[data-tipo="Limite"]`).forEach(r => {
		if (!r.classList.contains('d-none')) {
			let idLim = parseInt(r.dataset.idlimite) || 0;
				let oLim = {
				IdLimite: idLim,
				Activo: document.getElementById(`LimAct_${idLim}`).checked,
				IdComponenteMayor: parseInt(document.getElementById('IdComponenteMayor').value) || 0,
				IdComponenteMenor: -1,
				Horas: parseInt(r.querySelector("#Horas").value) || 0,
				Ciclos: parseInt(r.querySelector("#Ciclos").value) || 0,
				Dias: parseInt(r.querySelector("#Dias").value) || 0
			}
			let idx = modelo.Limites.findIndex(l => { return l.IdLimite === oLim.IdLimite; });
			if (idx > -1)
				modelo.Limites[idx] = clonObject(oLim);
			else
				modelo.Limites.push(clonObject(oLim));
		}
	});
	let res = modelo.save();
	MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, modelo, 'Id');
		clear();
	}
}
const clear = () => {
	modelo.clear();
	modelo.write();
	document.querySelectorAll(`.row[data-tipo="Limite"]`).forEach(r => {
		r.classList.add('d-none');
		r.querySelectorAll('input').forEach(i => { i.value = ''; });
	});
}
const tablas = () => {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Id' },
			{ data: 'Nombre' },
			{ data: 'Fabricante' },
			{ data: 'Capacidad.Nombre' },
			{ data: 'ComponenteMayor.Codigo' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Activo ? 'Activo' : 'Inactivo' : data.Activo ? 1 : 0;
				}
			},
			{
				data: null,
				orderable: false,
				className: 'd-grid',
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
				clear();
				modelo.clear();
				modelo.setValores(data);
				modelo.write();
				let evt = new Event('change');
				document.getElementById('IdComponenteMayor').dispatchEvent(evt);
				data.Limites.forEach(lim => {
					document.querySelector(`#LimAct_${lim.IdLimite}`).checked = lim.Activo;
					document.querySelector(`.row[data-idlimite="${lim.IdLimite}"]`).querySelector(`input[id="Horas"]`).value = lim.Horas;
					document.querySelector(`.row[data-idlimite="${lim.IdLimite}"]`).querySelector(`input[id="Ciclos"]`).value = lim.Ciclos;
					document.querySelector(`.row[data-idlimite="${lim.IdLimite}"]`).querySelector(`input[id="Dias"]`).value = lim.Dias;
				});
			});
		}
	});
}