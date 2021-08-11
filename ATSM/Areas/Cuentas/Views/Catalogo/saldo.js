var tabla, saldo;
$(() => {
	let id = getPar('id');
	saldo = new Saldo(id ?? 0);
	if (saldo.Valid) {
		saldo.write();
	}
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: Saldo.getSaldos(),
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		columns: [
			{ data: 'Id' },
			{ data: 'Codigo' },
			{ data: 'Nombre' },
			{
				data: null,
				className: 'text-center',
				render: (data, type) => {
					return data.Combustible ? 'Si' : 'No';
				}
			},
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
				saldo.clear();
				saldo.setValores(data);
				saldo.write();
				endPro();
			});
		}
	});
	$('#Codigo').blur(con);
	$('#cle').click(cle);
	$('#reg').click(reg);
	$('#del').click(del);
});
function con() {
	let cod = document.getElementById('Codigo').value;
	saldo.clear();
	saldo.byCodigo(cod);
	saldo.write();
	if (!saldo.Valid)
		document.getElementById('Codigo').value = cod;
	document.getElementById('Nombre').focus();
}
function reg() {
	procc();
	saldo.read();
	let rs = saldo.save();
	MsjBox('Saldo', rs.Error === '' ? rs.Mensaje : rs.Error);
	if (rs.Valid) {
		updDT(tabla, saldo, "Id");
		cle();
	}
	endPro();
}
function del() {
	procc();
	if (saldo.Valid) {
		let idm = saldo.Id;
		let rs = saldo.delete();
		MsjBox('Saldo', rs.Error === '' ? rs.Mensaje : rs.Error);
		if (rs.Valid) {
			delDT(tabla, "Id", idm);
			cle();
		}
	}
	endPro();
}
function cle() {
	saldo.clear();
	$('input').val('');
	$('#Reembolsable').prop('checked', false).change()
}