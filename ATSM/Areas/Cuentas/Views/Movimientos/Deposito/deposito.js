var tabla, tant, tfon, deposito, cuenta, cuentas;
$(() => {
	let id = getPar('id');
	let m = getPar('m');
	cuenta = new Account(id ?? 0);
	deposito = new Transaccion();
	if (cuenta.Valid) {
		cuenta.write();
		if (m === 1)
			$('#nav-fondo-tab').tab('show');
		if (m === 2)
			$('#nav-anticipo-tab').tab('show');
	}
	cuentas = Account.getAccounts();
	tablas();
	$('#clef,#clea').click(cle);
	$('#regf,#rega').click(reg);
});
function reg(b) {
	let tm = b.target.id === 'regf' ? 1 : 2;
	procc();
	if (!cuenta.Valid) {
		MsjBox('Cuenta No Valida', 'Valide la Cuenta Por Favor, ya que no es Valida.');
		cle();
	}
	deposito.read('', tm === 1 ? 'nav-fondo' :'nav-anticipo');
	deposito.IdAccount = cuenta.Id;
	deposito.IdMovimiento = tm;
	deposito.IdTipo = act;
	let rs = deposito.save();
	MsjBox('Cuentas', rs.Error === '' ? rs.Mensaje : rs.Error);
	if (rs.Valid) {
		updDT(tm === 1 ? tfon : tant, deposito, "Id");
		$(`#${(tm === 1 ? 'nav-fondo' : 'nav-anticipo')} input`).val('');
		$(`#${(tm === 1 ? 'nav-fondo' : 'nav-anticipo')} select`).val(0);
	}
	endPro();
}
function cle() {
	deposito.clear();
	$('input').val('');
	$(`#nav-fondo-tab, #nav-anticipo-tab`).addClass('disabled');
	$(`#nav-lista-tab`).tab('show');
}

function tablas() {
	tabla = $('#tabla').DataTable({
		processing: true,
		rowGroup: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: cuentas,
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		order: [[3, 'asc']],
		rowGroup: {
			emptyDataGroup: 'Sin Tipo de Cuenta',
			dataSrc: 'Tipo.Nombre',
			className: 'bg-info'
		},
		columns: [
			{ data: 'Id' },
			{
				data: 'Nombre',
				className: 'text-nowrap'
			},
			{ data: 'Estacion' },
			{ data: 'Tipo.Nombre', visible: false },
			{ data: 'Celular' },
			{ data: 'Correo' },
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-end' },
			{ targets: '_all', className: 'text-justify' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
	<button tipo="fondo" type="button" class="btn btn btn-sm btn-primary"><i class="fas fa-piggy-bank"></i> Fondo</button>
	<button tipo="anticipo" type="button" class="btn btn btn-sm btn-success"><i class="fas fa-money-check-alt"></i> Anticipo</button>
</div>`
			}
		],
		drawCallback: function (settings) {
			$(`#tabla button`).click((btn) => {
				let t = $(btn.target).attr('tipo');
				let row = tabla.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				cuenta.clear();
				cuenta.setValores(data);
				let n = cuenta.Nombre;
				cuenta.Nombre = cuenta.Nombre.toLowerCase();
				cuenta.write('', 'nav-fondo');
				cuenta.write('', 'nav-anticipo');
				cuenta.Nombre = n;
				$('span[id="TipoNombre"]').text(cuenta.Tipo.Nombre);
				$(`#nav-fondo-tab, #nav-anticipo-tab`).removeClass('disabled');
				$(`#nav-${t}-tab`).tab('show');
				let dfon = Transaccion.getTransacciones(cuenta.Id, 1);
				let dant = Transaccion.getTransacciones(cuenta.Id, 2);
				loadTable(tfon, dfon);
				loadTable(tant, dant);
				endPro();
			});
		}
	});
	tfon = $('#tfon').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: [],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		order: [[0, 'asc']],
		columns: [
			{
				data: null,
				className: 'text-center',
				render: (data, type) => {
					return moment(data.Fecha).format(type === 'display' ? 'DD/MM/YYYY' : 'YYYYMMDD');
				}
			},
			{ data: 'Saldo.Nombre' },
			{
				data: 'Monto', className: 'text-end',
				render: (data, type) => {
					return type === 'display' ? accounting.formatMoney(data, '$', 2, ',', '.') : data;
				}
			},
			{ data: 'Moneda.Codigo' },
			{ data: 'Observaciones' },
			{ data: null }
		],
		columnDefs: [
			{ targets: '_all', className: 'text-center' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<button tipo="editar" type="button" class="btn btn btn-sm btn-primary"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#tfon button`).click((btn) => {
				let row = tfon.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				deposito.clear();
				deposito.setValores(data);
				deposito.write('', 'nav-fondo');
				endPro();
			});
		}
	});
	tant = $('#tant').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: [],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		order: [[0, 'asc']],
		columns: [
			{
				data: null,
				className: 'text-center',
				render: (data, type) => {
					return moment(data.Fecha).format(type === 'display' ? 'DD/MM/YYYY' : 'YYYYMMDD');
				}
			},
			{ data: 'Saldo.Nombre' },
			{
				data: 'Monto', className: 'text-end',
				render: (data, type) => {
					return type === 'display' ? accounting.formatMoney(data, '$', 2, ',', '.') : data;
				}
			},
			{ data: 'Moneda.Codigo' },
			{ data: 'Observaciones' },
			{ data: null }
		],
		columnDefs: [
			{ targets: '_all', className: 'text-center' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<button tipo="editar" type="button" class="btn btn btn-sm btn-primary"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#tant button`).click((btn) => {
				let row = tant.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				deposito.clear();
				deposito.setValores(data);
				deposito.write('', 'nav-anticipo');
				endPro();
			});
		}
	});
}