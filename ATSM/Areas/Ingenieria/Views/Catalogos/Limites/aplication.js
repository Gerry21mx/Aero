var limites, tabla;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	limites = new Limites(id);
	if (limites.Valid)
		limites.write();
	tablas();
	document.getElementById('Codigo').addEventListener('blur', consulta);
	document.querySelector('#reg').addEventListener('click', registrar);
	document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	Limites.getLimites().then(data => {
		loadTable(tabla, data.Data);
	});
});
const consulta = (e) => {
	let cod = e.target.value;
	if (cod === '') return;
	procc();
	limites.byCadena(cod).then(r => {
		limites.write();
		if (!limites.Valid)
			e.target.value = cod;
		endPro();
	});
}
const eliminar = () => {
	if (limites.Valid) {
		let id = limites.Id;
		let res = limites.delete();
		MsjBox('Limites', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			limites.write();
		}
	}
}
const registrar = () => {
	console.log(`Registrar ${moment().format('DD MMM yyyy HH:MM:s')}`);
	limites.read();
	let res = limites.save();
	MsjBox('Limites', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, limites, 'Id');
		clear();
	}
}
const clear = () => {
	limites.clear();
	limites.write();
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
			{ data: 'Definicion' },
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
				limites.clear();
				limites.setValores(data);
				limites.write();
			});
		}
	});
}