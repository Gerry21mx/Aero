var familia, tabla;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	familia = new Family(id);
	if (familia.Valid)
		familia.write();
	tablas();
	document.getElementById('Codigo').addEventListener('blur', consulta);
	document.querySelector('#reg').addEventListener('click', registrar);
	document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	Family.getFamilys().then(data => {
		loadTable(tabla, data.Data);
	});
});
const consulta = (e) => {
	let cod = e.target.value;
	if (cod === '') return;
	procc();
	familia.byCadena(cod).then(r => {
		familia.write();
		if (!familia.Valid)
			e.target.value = cod;
		endPro();
	});
}
const eliminar = () => {
	if (familia.Valid) {
		let id = familia.Id;
		let res = familia.delete();
		MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			familia.write();
		}
	}
}
const registrar = () => {
	console.log(`Registrar ${moment().format('DD MMM yyyy HH:MM:s')}`);
	familia.read();
	let res = familia.save();
	MsjBox('Familia', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, familia, 'Id');
		clear();
	}
}
const clear = () => {
	familia.clear();
	familia.write();
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
			{ data: 'Nombre' },
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
				familia.clear();
				familia.setValores(data);
				familia.write();
			});
		}
	});
}