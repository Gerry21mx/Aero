var articulo, tabla;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	articulo = new Articulo(id);
	if (articulo.Valid)
		articulo.write();
	tablas();
	document.getElementById('Part').addEventListener('blur', consulta);
	document.querySelector('#reg').addEventListener('click', registrar);
	document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	$('#sFoto').click(() => { $('#foto').click(); });
	$('#foto').change(valPic);
	Articulo.getVArticulos().then(data => { loadTable(tabla, data.Data); });
});
const valPic = (e) => {
	if (e.target.value !== '') {
		$('#sFoto').removeClass('btn-outline-success');
		$('#sFoto').addClass('btn-success');
	} else {
		$('#sFoto').addClass('btn-outline-success');
		$('#sFoto').removeClass('btn-success');
	}
}
const consulta = (e) => {
	let cod = document.getElementById('Codigo').value;
	let des = document.getElementById('Descripcion').value;
	if (cod === '' || articulo.Valid) return;
	procc();
	articulo.byCadena(cod).then(r => {
		articulo.write();
		if (!articulo.Valid) {
			document.getElementById('Codigo').value = cod;
			document.getElementById('Descripcion').value = des;
		}
		endPro();
	});
}
const eliminar = () => {
	if (articulo.Valid) {
		let id = articulo.Id;
		let res = articulo.delete();
		MsjBox('Articulo', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			articulo.write();
		}
	}
}
const registrar = () => {
	procc();
	articulo.read();
	let dataString = new FormData();
	dataString.append('Foto', $('#foto')[0].files[0]);
	dataString.append('json', articulo.json());
	let res = articulo.save(dataString).then(res => {
		MsjBox('Articulo', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			let tar = articulo.clon()
			if (articulo.UnidadMedida) {
				tar.CodigoUM = articulo.UnidadMedida.Codigo;
				tar.DescripcionUM = articulo.UnidadMedida.Descripcion;
			}
			if (articulo.Categoria) {
				tar.CodigoCat = articulo.Categoria.Codigo;
				tar.DescripcionCat = articulo.Categoria.Descripcion;
			}
			updDT(tabla, tar, 'Id');
			clear();
		}
		endPro();
	}).catch(res => {
		endPro();
	})
}
const clear = () => {
	articulo.clear();
	articulo.write();
}
const tablas = (e) => {
	tabla = $('#tabReg').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{ data: 'Id' },
			{ data: 'Part' },
			{ data: 'Description' },
			{ data: 'CodigoCat' },
			{ data: 'CodigoUM' },
			{
				data: null,
				render: (data, type, row) => {
					return data.Seriado ? 'Si' : 'No';
				}
			},
			{
				data: null,
				render: (data, type, row) => {
					return data.Caducidad ? 'Si' : 'No';
				}
			},
			{
				data: null,
				render: (data, type, row) => {
					return data.Calibracion ? 'Si' : 'No';
				}
			},
			{
				data: null,
				render: (data, type, row) => {
					return data.Activo ? 'Si' : 'No';
				}
			},
			{ data: 'Minimo' },
			{ data: 'Maximo' },
			{ data: 'Reorden' },
			{ data: 'Equivalencia_Unitaria' },
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-end' },
			{ targets: [1], className: 'text-nowrap text-center' },
			{ targets: [2], className: 'text-justify' },
			{ targets: [8, 9, 10, 11, 12], orderable: false },
			{ targets: '_all', className: 'text-center' },
			{
				targets: [-1],
				orderable: false,
				className: 'text-center',
				defaultContent: `<button tipo="editar" type="button" class="btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#tabReg button[tipo="editar"]`).click((btn) => {
				let row = tabla.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				articulo.clear();
				articulo.setValores(data)
				articulo.write();
				trigger = document.querySelector('#alta-tab')
				tab = new bootstrap.Tab(trigger)
				tab.show()
				endPro();
			});
		},
		createdRow: (row, data, dataIndex) => {
			if (data.Seriado)
				$(row).attr('style', 'background-color:#12e2ef;');
		}
	});
}