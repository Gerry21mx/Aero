var demora, lisTab;
$(document).ready(() => {
	let id = parseInt(getPar('id')) || 0;
	demora = new Demora(id);
	if (demora.Valid) {
		demora.write();
		$('#nav-tab a[href="#nav-tab02"]').tab('show');
	}
	lisTab = $('#lista').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: Demora.getDemoras(),
		columns: [
			{ data: 'IdDemora' },
			{ data: 'Codigo' },
			{ data: 'Clasificacion' },
			{ data: 'Descripcion' },
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Activo ? 'Activo' : 'Inactivo' : data.Activo ? 1 : 0;
				}
			},
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-end' },
			{ targets: [2,3], className: 'text-justify' },
			{ targets: '_all', className: 'text-center' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<button tipo="editar" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#lista button[tipo="editar"]`).click((btn) => {
				let row = lisTab.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				demora.clear();
				demora.setValores(data);
				demora.write();
				$('#nav-tab a[href="#nav-tab02"]').tab('show');
				endPro();
			});
		}
	});
	$('#reg').click(registrar);
	$('#del').click(eliminar);
	$('#cle').click(clear);
	actualizar();
});
function actualizar() {
	let datos = Demora.getDemoras();
	lisTab.clear();
	lisTab.rows.add(datos);
	lisTab.columns.adjust().draw();
}
function eliminar() {
	if (demora.Valid) {
		let res = demora.delete();
		MsjBox('Demora', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			actualizar();
			clear();
		}
	}
}
function registrar() {
	demora.read();
	let res = demora.save();
	MsjBox('Demora', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		actualizar();
		clear();
	}
}
function clear() {
	demora.clear();
	demora.write();
}