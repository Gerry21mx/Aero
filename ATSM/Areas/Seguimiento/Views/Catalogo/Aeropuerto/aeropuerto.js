var aeropuerto, lisTab;
$(document).ready(() => {
	let id = parseInt(getPar('id')) || 0;
	aeropuerto = new Aeropuerto(id);
	if (aeropuerto.Valid) {
		aeropuerto.write();
		$('#nav-tab a[href="#nav-alta"]').tab('show');
	}
	lisTab = $('#lista').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: Aeropuerto.getAeropuertos(),
		columns: [
			{ data: 'IdAeropuerto' },
			{ data: 'Nombre' },
			{ data: 'ICAO' },
			{ data: 'IATA' },
			{ data: 'Pais' },
			{ data: 'Estado' },
			{ data: 'Latitud' },
			{ data: 'Longitud' },
			{ data: 'Elevacion' },
			{
				data: null,
				render: (data, type) => {
					return data.Abre === null ? '' : data.Abre.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return data.Cierra === null ? '' : data.Cierra.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Activo ? 'Activo' : 'Inactivo' : data.Activo ? 1 : 0;
				}
			},
			{ data: null }
		],
		columnDefs: [
			{ targets: [0, 8], className: 'text-end pr-4' },
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
				aeropuerto.clear();
				aeropuerto.setValores(data);
				aeropuerto.write();
				$('#nav-tab a[href="#nav-alta"]').tab('show');
				endPro();
			});
		}
	});
	$('#reg').click(registrar);
	$('#del').click(eliminar);
	$('#cle').click(clear);
	$('#Nombre,#ICAO,#IATA').blur(consulta);
	actualizar();
});
function consulta() {
	aeropuerto.read();
	if (aeropuerto.Nombre !== '') {
		aeropuerto.byAeropuerto(aeropuerto.Nombre);
	} else if (aeropuerto.ICAO) {
		aeropuerto.byAeropuerto(aeropuerto.ICAO);
	} else if (aeropuerto.IATA !== '') {
		aeropuerto.byAeropuerto(aeropuerto.IATA);
	}
	if (aeropuerto.Valid) {
		aeropuerto.write();
	}
}
function actualizar() {
	let datos = Aeropuerto.getAeropuertos();
	lisTab.clear();
	lisTab.rows.add(datos);
	lisTab.columns.adjust().draw();
}
function eliminar() {
	if (aeropuerto.Valid) {
		let res = aeropuerto.delete();
		MsjBox('Aeropuerto', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			actualizar();
			clear();
		}
	}
}
function registrar() {
	aeropuerto.read();
	let res = aeropuerto.save();
	MsjBox('Aeropuerto', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		actualizar();
		clear();
	}
}
function clear() {
	aeropuerto.clear();
	aeropuerto.write();
}