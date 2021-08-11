var tabla, cuenta, usuVinc;
$(() => {
	usuVinc = new Usuario();
	let id = getPar('id');
	cuenta = new Account(id ?? 0);
	if (cuenta.Valid)
		cuenta.write();
	tablas();
	$('#IdCrew').change(con);
	$('#cle').click(cle);
	$('#reg').click(reg);
	$('#del').click(del);
	$('#Nickname').blur(vUsu);
});
function vUsu() {
	let nn = $('#Nickname').val();
	usuVinc.clear();
	usuVinc.byNickName(nn);
	if (usuVinc.Valid) {
		$('#Password,#Password2').attr('disabled', true);
	}
}
function con() {
	let idc = $('#IdCrew').val();
	let nom = $('#IdCrew option:selected').text();
	cuenta.clear();
	cuenta.byNombre(nom, act).then(data => {
		cuenta.write();
		if (!cuenta.Valid) {
			$('#IdCrew').val(idc);
			cuenta.IdCrew = idc;
		}
		document.getElementById('Banco').focus();
	});
}
function reg() {
	if (document.getElementById('Password').value !== document.getElementById('Password2').value) {
		MsjBox('Cuenta Piloto', 'Las Contraseñas NO Coinciden');
		return;
	}
	procc();
	cuenta.read();
	cuenta.Nombre = $('#IdCrew option:selected').text();
	cuenta.IdTipo = act;
	cuenta.Nickname = document.getElementById('Nickname').value;
	cuenta.Password = document.getElementById('Password').value;
	let rs = cuenta.save();
	MsjBox('Cuentas', rs.Error === '' ? rs.Mensaje : rs.Error);
	if (rs.Valid) {
		updDT(tabla, cuenta, "Id");
		cle();
	}
	endPro();
}
function del() {
	procc();
	if (cuenta.Valid) {
		let idm = cuenta.Id;
		let rs = cuenta.delete();
		MsjBox('Cuentas', rs.Error === '' ? rs.Mensaje : rs.Error);
		if (rs.Valid) {
			delDT(tabla, "Id", idm);
			cle();
		}
	}
	endPro();
}
function cle() {
	cuenta.clear();
	$('input').val('');
	$('select').val('0');
	$('#Activo').prop('checked', false).change();
}
function tablas () {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: Account.getAccountsByTipo(act),
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		order: [[1, 'asc']],
		columns: [
			{ data: 'Id', visible: false },
			{
				data: 'Nombre',
				className: 'text-nowrap'
			},
			{ data: 'Banco' },
			{ data: 'Cuenta' },
			{ data: 'CLABE' },
			{ data: 'AMEX' },
			{ data: 'Celular' },
			{ data: 'Correo' },
			{
				data: 'Activo',
				render: (data) => {
					return data ? 'Activo' : 'Inactivo';
				}
			},
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-end' },
			{ targets: [1, 2], className: 'text-justify' },
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
				cuenta.clear();
				cuenta.setValores(data);
				cuenta.write();
				$('#nav-alta-tab').tab('show');
				endPro();
			});
		}
	});
}
