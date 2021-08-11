var biweekly, titulo, tabla, tFaa;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	biweekly = new Biweekly(id);
	if (biweekly.Valid)
		biweekly.write();
	document.getElementById('Codigo').addEventListener('blur', consulta);
	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', registrar);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	tablas();
	Biweekly.getBiweeklys().then(datos => { loadTable(tabla, datos.Data); });
	Biweekly.faa().then(datos => { loadTable(tFaa, datos.Data); });
});
const consulta = (e) => {
	let bw = e.target.value;
	if (bw === '') return;
	procc();
	biweekly.byCadena(bw).then(r => {
		biweekly.write();
		if (!biweekly.Valid) {
			e.target.value = bw;
			document.getElementById('Fecha').value = moment().format('YYYY-MM-DD')
		}
		endPro();
	});
}
const eliminar = () => {
	if (biweekly.Valid) {
		let id = biweekly.Id;
		let res = biweekly.delete();
		MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			biweekly.write();
		}
	}
}
const registrar = () => {
	biweekly.read();
	let res = biweekly.save();
	MsjBox('Bieweekly', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, biweekly, 'Id');
		clear();
	}
}
const clear = () => {
	biweekly.clear();
	biweekly.write();
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
			{
				data: null,
				render: (data, type) => {
					return moment(data.Fecha).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD');
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
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tabla.rows(e.target.closest('tr')).data()[0];
				biweekly.clear();
				biweekly.setValores(data);
				biweekly.write();
			});
		}
	});
	tFaa = $('#tFaa').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		order: [[3,'desc']],
		columns: [
			{ data: 'Tipo' },
			{
				data: 'Biweekly',
				render: (data, type) => {
					return type === 'display' ? `<button data-type="select" class="btn btn-sm btn-outline-dark py-0">${data}</button>` : data;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return moment(data.Fecha1).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD');
				}
			},
			{
				data: null,
				render: (data, type) => {
					return moment(data.Fecha2).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD');
				}
			},
			{
				data: null,
				orderable: false,
				render: (data) => {
					return `<a target="_blank" href="${data.Documentos}"><i class="fas fa-folder-open"></i></a>`
				}
			},
			{
				data: null,
				orderable: false,
				render: (data) => {
					return `<a target="_blank" href="${data.PDF}"><i class="fas fa-file-pdf"></i></a>`
				}
			},
			{
				data: null,
				orderable: false,
				render: (data) => {
					return `<a target="_blank" href="${data.Excel}"><i class="fas fa-file-excel"></i></a>`
				}
			}
		],
		columnDefs: [
			{ targets: [0], className: 'text-start' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="select"]`).click((e) => {
				let data = tFaa.rows(e.target.closest('tr')).data()[0];
				document.getElementById('Codigo').value = data.Biweekly
				document.getElementById('Fecha').value = moment(data.Fecha2).format('YYYY-MM-DD')
			});
		}
	});
}