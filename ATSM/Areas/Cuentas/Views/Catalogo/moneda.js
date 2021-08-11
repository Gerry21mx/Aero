var tabla, moneda;
$(() => {
	let id = getPar('id');
	moneda = new Moneda(id ?? 0);
	if (moneda.Valid) {
		moneda.write();
	}
	tablas();
	$('#Codigo').blur(con);
	$('#cle').click(cle);
	$('#reg').click(reg);
	$('#del').click(del);
});
function con() {
	let cod = document.getElementById('Codigo').value;
	moneda.clear();
	moneda.byCodigo(cod);
	moneda.write();
	if (!moneda.Valid)
		document.getElementById('Codigo').value = cod;
	document.getElementById('Nombre').focus();
}
function reg() {
	procc();
	moneda.read();
	let rs = moneda.save();
	MsjBox('Moneda', rs.Error === '' ? rs.Mensaje : rs.Error);
	if (rs.Valid) {
		updDT(tabla, moneda, "Id");
		cle();
	}
	endPro();
}
function del() {
	procc();
	if (moneda.Valid) {
		let idm = moneda.Id;
		let rs = moneda.delete();
		MsjBox('Moneda', rs.Error === '' ? rs.Mensaje : rs.Error);
		if (rs.Valid) {
			delDT(tabla, "Id", idm);
			cle();
		}
	}
	endPro();
}
function cle() {
	moneda.clear();
	$('input').val('');
}
function tablas() {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: Moneda.getMonedas(),
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		columns: [
			{ data: 'Id' },
			{ data: 'Codigo' },
			{ data: 'Nombre' },
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-end' },
			{ targets: [2, 3], className: 'text-justify' },
			{ targets: '_all', className: 'text-center' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<button tipo="editar" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#tabla button[tipo="editar"]`).click((btn) => {
				let row = tabla.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				moneda.clear();
				moneda.setValores(data);
				moneda.write();
				endPro();
			});
		}
	});
}