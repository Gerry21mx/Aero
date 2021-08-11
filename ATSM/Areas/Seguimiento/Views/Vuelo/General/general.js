var vuelo, tramo, avion, demora, ruta, tabla, lisTra, lisDem, capacidad, tripulacion;
var vueloHub, cnxHub = false;
$(document).ready(() => {
	vuelo = new Vuelo();
	tramo = new VueloTramo();
	tabla = $('#lista').DataTable({
		processing: true,
		order: [[6, 'asc'], [0, 'asc']],
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Matricula' },
			{
				data: null,
				render: (data, type) => {
					let f = data.SalidaPlataforma !== null ? data.fdSPlataforma : data.Despegue !== null ? data.fdDespegue : data.ItinerarioDespegue !== null ? data.fdIDespegue : data.Salida;
					return moment(f).format(type === 'display' ? 'DD/MM/YYYY HH:mm' : 'YYYYMMDDHHmm');
				}
			},
			{ data: 'Trip' },
			{ data: 'Pierna', visible: false },
			{ data: 'Origen', visible: false },
			{ data: 'Destino', visible: false },
			{ data: 'CodigoVuelo' },
			{ data: 'NoVuelo' },
			{ data: 'TipoVuelo' },
			{ data: 'Ruta' },
			{ data: 'Capitan' },
			{ data: 'Copiloto' },
			{
				data: null,
				render: (data, type) => {
					return data.Cerrado ? 'Cerrado' : 'Abierto';
				}
			},
			{ data: null, visible: false }
		],
		columnDefs: [
			{ targets: [0], className: 'text-end sz-font-775' },
			{ targets: [11, 12], className: 'text-justify sz-font-775' },
			{ targets: '_all', className: 'text-center sz-font-775' }
		]
	});
	$('#TipoReporte').change((e) => {
		let chk = e.target.checked;
		tabla.column(3).visible(chk);
		tabla.column(4).visible(chk);
		tabla.column(5).visible(chk);
	});
	$('#con').click(consultar);
	consultar();
	hubs();
});
function hubs() {
	vueloHub = $.connection.vueloHub;
	vueloHub.client.addVuelo = (datos) => {
		vuelo.setValores(datos);
		//actV();
	}
	vueloHub.client.delVuelo = (idvuelo) => {
		//if (idvuelo > 0) {
		//	var idx = datos.findIndex(i => i.pierna.IdVuelo = idvuelo);
		//	do {
		//		if (idx >= 0)
		//			datos.splice(idx, 1);
		//		idx = datos.findIndex(i => i.pierna.IdVuelo = idvuelo);
		//	} while (idx >= 0);
		//	drawTL();
		//}
	}
	vueloHub.client.vueloAC = (idv, edo) => {
		let i = tabla.data().toArray().findIndex(i => i.IdVuelo === idv);
		if (i > -1) {
			let d = tabla.row(i).data();
			d.Cerrado = edo;
			tabla.row(i).data(d);
			tabla.columns.adjust().draw();
		}
	}
	$.connection.hub.start().done(() => {
		cnxHub = true;
	});
}

function consultar() {
	procc();
	let filtro = {
		trip: $('#Trip').val(),
		idaeronave: parseInt($('#IdAeronave').val()) || 0,
		desde: $('#Desde').val(),
		hasta: $('#Hasta').val(),
		idtipovuelo: parseInt($('#IdTipoVuelo').val()) || 0,
		idorigen: parseInt($('#IdOrigen').val()) || 0,
		iddestino: parseInt($('#IdDestino').val()) || 0,
		idcapitan: parseInt($('#IdCapitan').val()) || 0,
		idcopiloto: parseInt($('#IdCopiloto').val()) || 0,
		estado: parseInt($('#Estado').val()) || null,
		tipo_reporte: $('#TipoReporte').prop('checked'),
	}
	Vuelo.reporte(filtro).then((data) => {
		tabla.clear();
		let datos = data !== null ? data.Data !== null ? data.Data : [] : [];
		if (datos.length > 0) {
			tabla.rows.add(datos);
		}
		tabla.columns.adjust().draw();
		tabla.on('contextmenu.tr', menu);
		$('#loadVuelos').addClass('d-none');
		endPro();
	});
}
function menu(evt) {
	tabla.rows('.selected').deselect();
	evt.preventDefault();
	var dt = tabla.row(evt.target.parentNode).data();
	vuelo.clear();
	vuelo.setValores(dt);
	tabla.row(evt.target.parentNode).select();
	let options = [{
		icon: 'fas fa-location-arrow',
		label: 'Cerrar',
		data: dt,
		action: (option, contextMenuIndex, optionIndex) => {
			vuelo.close();
			superCm.destroyMenu();
		},
		disabled: dt.Cerrado
	},
	{
		icon: 'fas fa-location-arrow',
		label: 'Reabrir',
		data: dt,
		action: (option, contextMenuIndex, optionIndex) => {
			vuelo.reOpen();
			superCm.destroyMenu();
		},
		disabled: !dt.Cerrado
	},
	{
		icon: 'fas fa-edit',
		label: 'Editar',
		action: (option, contextMenuIndex, optionIndex) => {
			window.open(`${window.location.origin}/vuelo?id=${dt.IdVuelo}`);
			superCm.destroyMenu();
		}
	}];
	if (!usuario.air('clVuelo')) {
		options.splice(0, 1);
	}
	if (!usuario.air('raVuelo')) {
		let i = options.findIndex(b => b.label === 'Reabrir');
		options.splice(i, 1);

	}
	var menco = superCm.createMenu(options, evt);
}