var tRepo;
$(() => {
	tRepo = $('#tRepo').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		order: [/*[0, 'asc'], */[5, 'asc']],
		//rowGroup: {
		//	dataSrc: 0
		//},
		columns: [
			{
				data: 'Matricula',
				className: 'text-nowrap '
			},
			{ data: 'Trip' },
			{ data: 'Pierna' },
			{ data: 'Origen' },
			{ data: 'Destino' },
			{
				data: null,
				render: (data, type) => {
					return moment(data.fdSPlataforma).format(type === 'display' ? 'DD/MM/YYYY HH:mm' : 'YYYYMMDDHHmm');
				}
			},
			{
				data: null,
				render: (data, type) => {
					return moment(data.fdLPlataforma).format(type === 'display' ? 'DD/MM/YYYY HH:mm' : 'YYYYMMDDHHmm');
				}
			},
			{
				data: null,
				render: (data, type) => {
					let msp = moment(data.fdSPlataforma);
					let mlp = moment(data.fdLPlataforma);
					let m = mlp.diff(msp, 'minutes');
					let h = Math.floor(m / 60);
					m = m % 60;
					return type === 'display' ? `${(h < 10 ? `0${h}` : h)}:${(m < 10 ? `0${m}` : m)}` : (data.MinCalzo / 60);
				}
			},
			{
				data: null,
				render: (data, type) => {
					let md = moment(data.fdDespegue);
					let ma = moment(data.fdAterrizaje);
					let m = ma.diff(md, 'minutes');
					let h = Math.floor(m / 60);
					m = m % 60;
					return type === 'display' ? `${(h < 10 ? `0${h}` : h)}:${(m < 10 ? `0${m}` : m)}` : (data.MinCalzo / 60);
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data.PiezasCarga, 0, ',', '.') : data.PiezasCarga ?? 0;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data.PesoCarga, 0, ',', '.') : data.PesoCarga ?? 0;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data.CombustibleCargado, 0, ',', '.') : data.CombustibleCargado ?? 0;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data.CombustibleRampa, 0, ',', '.') : data.CombustibleRampa ?? 0;
				}
			},
			{
				data: null,
				render: (data, type) => {
					return type === 'display' ? accounting.formatNumber(data.CombustibleRemanente, 0, ',', '.') : data.CombustibleRemanente ?? 0;
				}
			},
			{
				data: null,
				render: (data, type) => {
					let cg = (data.CombustibleRampa == null ? 0 : data.CombustibleRampa) - (data.CombustibleRemanente == null ? 0 : data.CombustibleRemanente);
					return type === 'display' ? accounting.formatNumber(cg,2,',','.') : cg;
				}
			},
			{
				data: 'Capitan',
				className: 'text-nowrap text-capitalize',
				render: (data) => {
					return data !== null ? data.toLowerCase().replace(/\w\S*/g, (w) => (w.replace(/^\w/, (c) => c.toUpperCase()))) : '';
				}
			},
			{ data: 'Ruta' },
			{ data: 'TipoVuelo' }
		],
		columnDefs: [
			{ targets: [9, 10, 11, 12, 13, 14], className: 'text-end' },
			{ targets: [15, 17], className: 'text-start' },
			{ targets: '_all', className: 'text-center' }
		],
		footerCallback: (tfoot, data, start, end, display) => {
			//					SUBTOTALES
			tMinCalzo = tRepo.column(7, { page: 'current' }).data().reduce(function (a, b) {
				let msp = moment(b.fdSPlataforma);
				let mlp = moment(b.fdLPlataforma);
				let m = mlp.diff(msp, 'minutes');
				return a + m;
			}, 0);
			let shC = Math.floor(tMinCalzo / 60);
			let smC = tMinCalzo % 60;
			tMinVuelo = tRepo.column(8, { page: 'current' }).data().reduce(function (a, b) {
				let msp = moment(b.fdDespegue);
				let mlp = moment(b.fdAterrizaje);
				let m = mlp.diff(msp, 'minutes');
				return a + (m);
			}, 0);
			let shV = Math.floor(tMinVuelo / 60);
			let smV = tMinVuelo % 60;

			stPiezasCarga = tRepo.column(9, { page: 'current' }).data().reduce(function (a, b) {
				return a + (b.PiezasCarga ?? 0);
			}, 0);
			stPesoCarga = tRepo.column(10, { page: 'current' }).data().reduce(function (a, b) {
				return a + (b.PesoCarga ?? 0);
			}, 0);
			stCombustibleCargado = tRepo.column(11, { page: 'current' }).data().reduce(function (a, b) {
				return a + (b.CombustibleCargado ?? 0);
			}, 0);
			stCombustibleRampa = tRepo.column(12, { page: 'current' }).data().reduce(function (a, b) {
				return a + (b.CombustibleRampa ?? 0);
			}, 0);
			stCombustibleRemanente = tRepo.column(13, { page: 'current' }).data().reduce(function (a, b) {
				return a + (b.CombustibleRemanente ?? 0);
			}, 0);
			stCombustibleGastado = tRepo.column(14, { page: 'current' }).data().reduce(function (a, b) {
				let cg = (data.CombustibleRampa == null ? 0 : data.CombustibleRampa) - (data.CombustibleRemanente == null ? 0 : data.CombustibleRemanente);
				return a + cg;
			}, 0);

			shC = accounting.formatNumber(shC, 0, ',', '.');
			smC = accounting.formatNumber(smC, 0, ',', '.');
			shV = accounting.formatNumber(shV, 0, ',', '.');
			smV = accounting.formatNumber(smV, 0, ',', '.');
			$(tRepo.column(7).footer()).html(`${shC} H<br>${smC} m`);
			$(tRepo.column(8).footer()).html(`${shV} H<br>${smV} m`);
			$(tRepo.column(9).footer()).html(accounting.formatNumber(stPiezasCarga, 0, ',', '.'));
			$(tRepo.column(10).footer()).html(accounting.formatNumber(stPesoCarga, 0, ',', '.'));
			$(tRepo.column(11).footer()).html(accounting.formatNumber(stCombustibleCargado, 0, ',', '.'));
			$(tRepo.column(12).footer()).html(accounting.formatNumber(stCombustibleRampa, 0, ',', '.'));
			$(tRepo.column(13).footer()).html(accounting.formatNumber(stCombustibleRemanente, 0, ',', '.'));
			$(tRepo.column(14).footer()).html(accounting.formatNumber(stCombustibleGastado, 0, ',', '.'));
			//					TOTALES
			tMinCalzo = tRepo.column(7).data().reduce(function (a, b) {
				let msp = moment(b.fdSPlataforma);
				let mlp = moment(b.fdLPlataforma);
				let m = mlp.diff(msp, 'minutes');
				return a + m;
			}, 0);
			let thC = Math.floor(tMinCalzo / 60);
			let tmC = tMinCalzo % 60;
			tMinVuelo = tRepo.column(8).data().reduce(function (a, b) {
				let msp = moment(b.fdDespegue);
				let mlp = moment(b.fdAterrizaje);
				let m = mlp.diff(msp, 'minutes');
				return a + (m);
			}, 0);
			let thV = Math.floor(tMinVuelo / 60);
			let tmV = tMinVuelo % 60;
			tPiezasCarga = tRepo.column(9).data().reduce(function (a, b) {
					return a + (b.PiezasCarga ?? 0);
			}, 0);
			tPesoCarga = tRepo.column(10).data().reduce(function (a, b) {
				return a + (b.PesoCarga ?? 0);
			}, 0);
			tCombustibleCargado = tRepo.column(11).data().reduce(function (a, b) {
				return a + (b.CombustibleCargado ?? 0);
			}, 0);
			tCombustibleRampa = tRepo.column(12).data().reduce(function (a, b) {
				return a + (b.CombustibleRampa ?? 0);
			}, 0);
			tCombustibleRemanente = tRepo.column(13).data().reduce(function (a, b) {
				return a + (b.CombustibleRemanente ?? 0);
			}, 0);
			tCombustibleGastado = tRepo.column(14).data().reduce(function (a, b) {
				let cg = (data.CombustibleRampa == null ? 0 : data.CombustibleRampa) - (data.CombustibleRemanente == null ? 0 : data.CombustibleRemanente);
				return a + cg;
			}, 0);

			thC = accounting.formatNumber(thC, 0, ',', '.');
			tmC = accounting.formatNumber(tmC, 0, ',', '.');
			thV = accounting.formatNumber(thV, 0, ',', '.');
			tmV = accounting.formatNumber(tmV, 0, ',', '.');
			$('#tCz').html(`${thC} H<br>${tmC} m`);
			$('#tVl').html(`${thV} H<br>${tmV} m`);
			$('#tPzc').html(accounting.formatNumber(tPiezasCarga, 0, ',', '.'));
			$('#tPsc').html(accounting.formatNumber(tPesoCarga, 0, ',', '.'));
			$('#tCc').html(accounting.formatNumber(tCombustibleCargado, 0, ',', '.'));
			$('#tCra').html(accounting.formatNumber(tCombustibleRampa, 0, ',', '.'));
			$('#tCre').html(accounting.formatNumber(tCombustibleRemanente, 0, ',', '.'));
			$('#tCga').html(accounting.formatNumber(tCombustibleGastado, 0, ',', '.'));
		}
	});
	$('#res,#des').click(report)
});
function report(e) {
	procc();
	let filtro = {
		trip: $('#Trip').val(),
		idaeronave: parseInt($('#IdAeronave').val()) || 0,
		desde: $('#Desde').val(),
		hasta: $('#Hasta').val(),
		idorigen: parseInt($('#IdOrigen').val()) || 0,
		iddestino: parseInt($('#IdDestino').val()) || 0,
		idtipovuelo: parseInt($('#IdTipoVuelo').val()) || 0,
		iddemora: parseInt($('#IdDemora').val()) || 0,
		idruta: parseInt($('#IdDemora').val()) || 0,
		idcapacidad: parseInt($('#IdCapacidad').val()) || 0,
		tipo_reporte: e.target.id === 'des'
	}
	filtro.desde = filtro.desde === '' ? null : filtro.desde;
	filtro.hasta = filtro.hasta === '' ? null : filtro.hasta;
	Vuelo.reporte(filtro).then((data) => {
		tRepo.clear();
		let datos = data !== null ? data.Data !== null ? data.Data : [] : [];
		if (datos.length > 0) {
			tRepo.rows.add(datos);
		}
		tRepo.columns.adjust().draw();
		//tRepo.on('contextmenu.tr', menu);
		//$('#loadVuelos').addClass('d-none');
		endPro();
	});
}