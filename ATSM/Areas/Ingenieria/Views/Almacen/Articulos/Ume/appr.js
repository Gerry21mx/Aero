var unimed, tabla;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	unimed = new UnidadMedida(id);
	if (unimed.Valid)
		unimed.write();
	tablas();
	document.getElementById('Codigo').addEventListener('blur', consulta);
	document.getElementById('Descripcion').addEventListener('blur', consulta);
	document.querySelector('#reg').addEventListener('click', registrar);
	document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	UnidadMedida.getUnidadMedidas().then(data => { loadTable(tabla, data.Data); });
});
const consulta = (e) => {
	let cod = document.getElementById('Codigo').value;
	let des = document.getElementById('Descripcion').value;
	if (cod === '' || unimed.Valid) return;
	procc();
	unimed.byCadena(cod).then(r => {
		unimed.write();
		if (!unimed.Valid) {
			document.getElementById('Codigo').value = cod;
			document.getElementById('Descripcion').value = des;
		}
		endPro();
	});
}
const eliminar = () => {
	if (unimed.Valid) {
		let id = unimed.Id;
		let res = unimed.delete();
		MsjBox('UnidadMedida', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			unimed.write();
		}
	}
}
const registrar = () => {
	unimed.read();
	let res = unimed.save();
	MsjBox('UnidadMedida', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, unimed, 'Id');
		clear();
	}
}
const clear = () => {
	unimed.clear();
	unimed.write();
}
const tablas = (e) => {
	tabla = $('#tabReg').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-sm-6'l><'col-12 col-sm-6'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{ data: 'Id' },
			{ data: 'Codigo' },
			{ data: 'Descripcion' },
			//{
			//	data: null,
			//	render: (data, type) => {
			//		return data.Tipo ? data.Tipo.Codigo : '';
			//	}
			//},
			{
				data: null,
				render: (data) => { return data.Activo ? 'Activo' : 'Inactivo'; }
			},
			{ data: null }
		],
		columnDefs: [
			{
				targets: [-1],
				orderable: false,
				defaultContent: `<button tipo="editar" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-pencil-alt"></i></button>`
			},
			{ targets: [0], className: 'text-end' },
			{ targets: [2], className: 'text-justify' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#tabReg button[tipo="editar"]`).click((btn) => {
				let row = tabla.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				unimed.clear();
				unimed.setValores(data);
				unimed.write();
				endPro();
			});
		}
	});
}