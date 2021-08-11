var avion, titulo, tabla;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	avion = new Aircraft(id);
	if (avion.Valid)
		avion.write();
	document.getElementById('Matricula').addEventListener('blur', consulta);
	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', registrar);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	tablas();
	Aircraft.getAircrafts().then(datos => { loadTable(tabla, datos.Data); });
});
const consulta = (e) => {
	let mat = e.target.value;
	if (mat === '') return;
	procc();
	avion.byCadena(mat).then(r => {
		avion.write();
		if (!avion.Valid)
			e.target.value = mat;
		endPro();
	});
}
const eliminar = () => {
	if (avion.Valid) {
		let id = avion.Id;
		let res = avion.delete();
		MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			avion.write();
		}
	}
}
const registrar = () => {
	avion.read();
	let res = avion.save();
	MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, avion, 'Id');
		clear();
	}
}
const clear = () => {
	avion.clear();
	avion.write();
}
const tablas = () => {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Id' },
			{ data: 'Matricula' },
			{ data: 'Empresa.Nombre' },
			{ data: 'JumpSeat' },
			{ data: 'Pasajeros' },
			{ data: 'Seguro' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Estado ? 'Activo' : 'Inactivo' : data.Estado ? 1 : 0;
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
			{ targets: [2], className: 'text-start' },
			{ targets: [0, 3, 4], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click((e) => {
				let data = tabla.rows(e.target.closest('tr')).data()[0];
				avion.clear();
				avion.setValores(data);
				avion.write();
			});
		}
	});
}