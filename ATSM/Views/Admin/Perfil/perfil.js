var arbol = { data: null };
var datosT = [];
var perfil, rol;
var tabla;
var swe = false;
$(document).ready(function () {
	perfil = new Perfil();
	rol = new Rol();
	let dA = Rol.getRoles();
	arbol.data = [];
	dA.forEach(rol => {
		if (rol.Area === 'Libre') return;
		arbol.data.push({
			id: rol.RoleId,
			parent: rol.Padre > 0 ? rol.Padre.toString() : '#',
			text: rol.Descripcion + ` (${rol.RoleName})`,
			icon: rol.Icon,
			li_attr: {},
			a_attr: {},
			state: {
				disabled: false,
				opened: true,
				selected: false
			}
		});
	});
	datosT = Perfil.getPerfiles();
	$('#perfiles').jstree({
		core: arbol,
		checkbox: {
			three_state: false
		},
		plugins: ["wholerow", "checkbox", "types", "themes"]
	});
	$('#perfiles').on('select_node.jstree', function (x, y) {
		if (!swe) {
			var cascada = getHijos(y.node.id);
			if (y.node.state.selected) {
				$('#perfiles').jstree(true).select_node(cascada);
			}
		}
	});
	$('#perfiles').on('deselect_node.jstree', function (x, y) {
		var cascada = getHijos(y.node.id);
		if (!y.node.state.selected) {
			$('#perfiles').jstree(true).deselect_node(cascada);
		}
	});
	$('#Nombre').blur(con);
	$('#reg').click(reg);
	$('#cle').click(cle);
	$('#Herencia').prop('checked',true).change((e) => { swe = !e.target.checked; });
	tablas();
	wres();
});
$(window).resize(wres);
function wres() {
	let h = $(window).height();
	let r = 70 * h / 100;
	$('#clp').height(r);
}
function getHijos(id = '#') {
	var hijos = arbol.data.filter(node => node.parent === id);
	hijos.forEach(nodo => {
		var at = getHijos(nodo.id);
		at.forEach(item => { hijos.push(item); });
	})
	return hijos;
}
function con(idp = 0) {
	var nombre = $('#Nombre').val();
	perfil = new Perfil(typeof (idp) === 'number' ? idp : 0);
	if (!perfil.Valid) {
		perfil.byNombre(nombre);
	}
	if (perfil.Valid) {
		perfil.write();
		var cascada = [];
		perfil.Roles.forEach(rol => cascada.push(rol.RoleId));
		$('#perfiles').jstree(true).select_node(cascada);
	} else {
		cle();
		$('#Nombre').val(nombre);
	}
}
function reg() {
	perfil.read();
	perfil.Roles = [];
	$("#perfiles").jstree('get_checked', null, true).forEach(rolId => {
		rol.clear();
		rol.RoleId = rolId;
		perfil.Roles.push(rol.clon());
	});
	var res = perfil.save();
	if (res.Valid) {
		MsjBox('Perfiles de Acceso', res.Error !== '' ? res.Error : res.Mensaje);
		addOrModify(perfil);
		cle();
	}
}
function cle() {
	perfil.clear();
	$('input').val('');
	$('#Activo').prop('checked', false);
	$('#perfiles').jstree(true).deselect_all();
}
function addOrModify(registro) {
	var idx = tabla.data().toArray().findIndex(item => item.IdPerfil === registro.IdPerfil);
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
		rowGroup: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: datosT,
		order: [[3, 'asc'],[0, 'asc']],
		rowGroup: {
			dataSrc: ['Area'],
			startClassName: 'bg-success fw-bolder',
			startRender: (rows, group) => {
				return `${group} (${rows.count()})`;
			}
		},
		columns: [
			{ data: 'Nombre' },
			{ data: 'Descripcion' },
			{
				data: null,
				render: (data, type, row) => {
					return data.Activo ? 'Activo' : 'Inactivo';
				}
			},
			{ data: 'Area', visible: false },
			{
				data: null
			}
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
				perfil.clear();
				perfil.setValores(data);
				perfil.write();
				var cascada = [];
				perfil.Roles.forEach(rol => cascada.push(rol.RoleId));
				swe = true;
				$('#perfiles').jstree(true).select_node(cascada);
				swe = false;
			});
		}
	});
}