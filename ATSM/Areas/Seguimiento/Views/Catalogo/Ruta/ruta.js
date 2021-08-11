var ruta, tramo, lisTab, lisTra;
$(document).ready(() => {
	let id = parseInt(getPar('id')) || 0;
	ruta = new Ruta(id);
	if (ruta.Valid) {
		ruta.write();
		$('#nav-tab a[href="#nav-tab02"]').tab('show');
	}
	tramo = new RutaTramo();
	lisTab = $('#lista').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: Ruta.getRutas(),
		columns: [
			{ data: 'IdRuta' },
			{ data: 'Codigo' },
			{ data: 'Descripcion' },
			{ data: 'TipoVuelo.Descripcion' },
			{ data: 'NoVuelo' },
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
				ruta.clear();
				ruta.setValores(data);
				ruta.write();
				loadTramos();
				$('#IdOrigen').val(ruta.Tramos[ruta.Tramos.length - 1].IdDestino);
				$('#IdOrigen').prop('disabled', true);
				$('#nav-tab a[href="#nav-tab02"]').tab('show');
				endPro();
			});
		}
	});
	lisTra = $('#lisTramos').DataTable({
		processing: true,
		paging: false,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		//dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		dom: "<'row'<'col'tr>>",
		data: [],
		columns: [
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? data.Pierna + 1 : data.Pierna;
				}
			},
			{
				data: null,
				render: (data) => {
					if (data.Origen === null) {
						data.Origen = new Aeropuerto(data.IdOrigen);
					}
					return data.Origen.IATA;
				}
			},
			{
				data: null,
				render: (data) => {
					if (data.Destino === null) {
						data.Destino = new Aeropuerto(data.IdDestino);
					}
					return data.Destino.IATA;
				}
			},
			{
				data: null,
				render: (data) => {
					return data.ItinerarioSalida === null ? '' : data.ItinerarioSalida.substring(0, 5);
				}
			},
			{
				data: null,
				render: (data) => {
					return data.ItinerarioLlegada === null ? '' : data.ItinerarioLlegada.substring(0, 5);
				}
			},
			{ data: 'NoVuelo' },
			{ data: null }
		],
		columnDefs: [
			{
				targets: '_all',
				orderable: false,
				className: 'text-center'
			},
			{
				targets: [-1],
				className: 'text-center',
				defaultContent: `<button tipo="editar" type="button" class="btn btn btn-sm btn-outline-primary pt-0 pb-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		drawCallback: function (settings) {
			$(`#lisTramos button[tipo="editar"]`).click((btn) => {
				let row = lisTra.row($(btn.target).parents('tr'));
				var data = row.data();
				procc();
				tramo.clear();
				tramo.setValores(data);
				tramo.write();
				$('#pNoVuelo').val(tramo.NoVuelo);
				if (tramo.Pierna === 0) {
					$('#IdOrigen').removeAttr('disabled');
				} else {
					$('#IdOrigen').prop('disabled', true);
				}
				endPro();
			});
		}
	});
	$('#reg').click(registrar);
	$('#del').click(eliminar);
	$('#cle').click(clear);
	$('#addTra').click(addTramo);
	document.getElementById('Codigo').addEventListener('blur', consulta);
	loadRutas();
});
function addTramo() {
	let nv = parseInt(document.getElementById('pNoVuelo').value) || 0;
	tramo.read();
	if (tramo.IdOrigen <= 0 || tramo.IdDestino <= 0) {
		MsjBox('Tramo de Ruta', 'Falta algun aeropuerto Origen-Destino');
		return false;
	}
	if (tramo.ItinerarioSalida === '' || tramo.ItinerarioLlegada === '') {
		MsjBox('Tramo de Ruta', 'Faltan Tiempos de Salida o Llegada');
		return false;
	}
	tramo.NoVuelo = nv > 0 ? nv : tramo.NoVuelo;
	tramo.Pierna = tramo.Pierna === 0 ? ruta.Tramos.length : tramo.Pierna;
	let i = ruta.Tramos.findIndex(t => t.Pierna === tramo.Pierna);
	if (i > -1) {
		if (tramo.IdDestino !== tramo.Destino.IdAeropuerto) {
			tramo.Destino = new Aeropuerto(tramo.IdDestino);
			if (tramo.Pierna < (ruta.Tramos.length - 1)) {
				ruta.Tramos[i + 1].IdOrigen = tramo.IdDestino;
				ruta.Tramos[i + 1].Origen = tramo.Destino.clon();
			}
		}
		ruta.Tramos[i] = tramo.clon();
	} else {
		ruta.Tramos.push(tramo.clon());
	}
	loadTramos();
	$('#cTramo input, #cTramo select').val('').blur();
	$('#IdOrigen').val(ruta.Tramos[ruta.Tramos.length - 1].IdDestino);
	$('#IdOrigen').prop('disabled', true);
	tramo.clear();
	document.getElementById('IdDestino').focus();
}
function consulta() {
	let cod = document.getElementById('Codigo').value;
	clear();
	ruta.byCodigo(cod);
	if (ruta.Valid) {
		ruta.write();
		loadTramos();
		$('#IdOrigen').val(ruta.Tramos[ruta.Tramos.length - 1].IdDestino);
		$('#IdOrigen').prop('disabled', true);
	} else {
		document.getElementById('Codigo').value = cod;
	}
}
function loadRutas() {
	let datos = Ruta.getRutas();
	lisTab.clear();
	lisTab.rows.add(datos);
	lisTab.columns.adjust().draw();
}
function loadTramos() {
	if (ruta.Tramos.length === 0 && ruta.Valid) {
		ruta.Tramos = RutaTramo.getTramos(ruta.IdRuta);
	}
	lisTra.clear();
	lisTra.rows.add(ruta.Tramos);
	lisTra.columns.adjust().draw();
}
function eliminar() {
	if (ruta.Valid) {
		let res = ruta.delete();
		MsjBox('Ruta', res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			loadRutas();
			clear();
		}
	}
}
function registrar() {
	ruta.read();
	if (ruta.Codigo === '') {
		MsjBox('Ruta', 'Falta Codigo de Ruta');
		return false;
	} else if (ruta.Descripcion === '') {
		MsjBox('Ruta', 'Falta Descripcion de Ruta');
		return false;
	}
	let res = ruta.save();
	MsjBox('Ruta', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		loadRutas();
		clear();
	}
}
function clear() {
	ruta.clear();
	ruta.write();
	lisTra.clear().columns.adjust().draw();
	$('#cTramo input, #cTramo select').val('').blur();
	$('#IdOrigen').removeAttr('disabled');
}