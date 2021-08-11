var datosT, user, pw1, pw2, swPw, tabla;
$(document).ready(function () {
	swPw = false;
	user = new Usuario();
	$('#Nickname').blur(con);
	$('#cle').click(cle);
	$('#reg').click(reg);
	$('#del').click(del);
	tablas();
	Usuario.getUsuarios('Seguimiento').then(data => {
		datosT = data;
		acTu();
	});
});
function acTu() {
	tabla.clear();
	tabla.rows.add(datosT);
	tabla.columns.adjust().draw();
}
function valPsw() {
	pw1 = $('#Password').val();
	pw2 = $('#Password2').val();
	if (pw1 === pw2) {
		swPw = true;
	} else {
		swPw = false;
	}
}
function con(uid = 0) {
	procc();
	let un = $('#Nickname').val();
	cle();
	user.Nickname = un;
	user.UserId = typeof (uid) === 'number' ? uid : 0;
	user.getInstance();
	if (!user.Valid)
		user.byNickName(un);
	document.querySelector('#Password').disabled = false;
	document.querySelector('#Password2').disabled = false;
	if (user.Valid) {
		user.write();
		document.querySelector('#Password').disabled = true;
		document.querySelector('#Password2').disabled = true;
	} else {
		$('#Nickname').val(un);
	}
	endPro();
}
function cle() {
	user.clear();
	pw1 = "";
	pw2 = "";
	swPw = false;
	$('input').val('');
	$('#Activo').prop('checked', false);
	$('#iDpERFIL').val(0);
}
function reg() {
	procc();
	valPsw();
	if (!swPw) {
		MsjBox('Registro de Usuarios', 'Las Contraseñas No Coinciden.');
		$('#Password,#Password2').val('');
		$('#Password').focus();
		return;
	}
	if ($('#Nickname').val() === '') {
		MsjBox('Registro de Usuarios', 'Falta el nombre del Usuario.');
		$('#Nickname').val('');
		$('#Nickname').focus();
		return;
	}
	user.read();
	var res = user.save();
	MsjBox('Usuarios', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		addOrModify(user);
		cle();
	}
	endPro();
}
function del() {
	procc();
	if (user.Valid) {
		var res = user.delete();
		MsjBox('Usuarios', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			addOrModify(user);
			cle();
		}
	}
	endPro();
}
function addOrModify(registro) {
	var idx = tabla.data().toArray().findIndex(item => item.UserId === registro.UserId);
	if (idx > -1) {
		tabla.row(idx).data(clonObject(registro));
	} else {
		tabla.row.add(clonObject(registro));
	}
	tabla.columns.adjust().draw();
}
function tablas() {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{ data: 'Nickname' },
			{ data: 'Nombre' },
			{ data: 'Perfil.Nombre' },
			{
				data: null,
				render: (data, type, row) => {
					return data.Activo ? 'Activo' : 'Inactivo';
				}
			},
			{ data: null }
		],
		columnDefs: [
			{ targets: [0, 1], className: 'text-start' },
			{ targets: [2], orderable: false, className: 'text-center' },
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
				$('input[type="password"]').val('');
				user.clear();
				user.setValores(data);
				user.write();
				document.querySelector('#Password').disabled = true;
				document.querySelector('#Password2').disabled = true;
			});
		}
	});
}