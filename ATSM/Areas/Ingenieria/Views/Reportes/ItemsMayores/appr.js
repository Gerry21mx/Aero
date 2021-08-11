document.addEventListener('DOMContentLoaded', (e) => {
	document.getElementById('IdComponenteMayor').addEventListener('change', vMayor);
	document.getElementById('IdModelo').addEventListener('change', vModelo);
	tablas();
	document.getElementById('repo').addEventListener('click', reporte);
})
const vMayor = (e) => {
	let idm = parseInt(e.target.value) || 0;
	document.querySelectorAll('#IdModelo option').forEach(om => {
		om.classList.remove('d-none');
		if (idm > 0) {
			let idc = parseInt(om.dataset.idcomponentemayor) || 0;
			if (idm !== idc && om.value !== '0')
				om.classList.add('d-none');
		}
	});
}
const vModelo = (e) => {
	if (document.getElementById('IdComponenteMayor').value !== '1') return;
	let idm = parseInt(e.target.value) || 0;
	document.querySelectorAll('#IdAircraft option').forEach(om => {
		om.classList.remove('d-none');
		if (idm > 0) {
			if ((parseInt(om.dataset.idmodelo) || 0) !== idm)
				om.classList.add('d-none');
		}
	});
}
const reporte = (e) => {
	let idMayor = parseInt(document.getElementById('IdComponenteMayor').value) || 0;
	let idFamilia = parseInt(document.getElementById('IdFamily').value) || 0;
	let idModelo = parseInt(document.getElementById('IdModelo').value) || 0;
	let idTipo = parseInt(document.getElementById('Tipo').value) || 0;
	let ata1 = parseInt(document.getElementById('ATA1').value) || 0;
	ComponenteMenor.query(idMayor, idFamilia, idModelo, idTipo, ata1).then(data => {
		loadTable(tabla, data.Data);
	});
}
const tablas = () => {
	tabla = $('#reporte').DataTable({
		processing: true,
		rowGroup: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: [],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		order: [[0, 'asc']],
		rowGroup: {
			emptyDataGroup: 'Sin Tipo de Cuenta',
			dataSrc: 'IdTipoMenor',
			className: 'bg-info',
			startRender: (rows, group, level) => {
				return `${(group === 1 ? 'Componentes' : group === 2 ? 'Directivas / Boletines de Servicio' : 'Servicios')} ${rows.count()}`;
			}
		},
		columns: [
			{
				data: 'IdTipoMenor',
				visible: false
			},
			{ data: 'Part' },
			{ data: 'Description' },
			{ data: 'Familia' },
			{
				data: null,
				render: (data, type) => {
					return `${data.ATA1}-${data.ATA1}-${data.ATA1}`;
				}
			},
			{
				data: 'Directive',
				className: 'dt-body-nowrap'
			},
			{ data: 'Amendment' },
			{
				data: 'AD_Date',
				className: 'dt-body-nowrap',
				render: (data, type) => {
					return data ? moment(data).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD') : '';
				}
			},
			{ data: 'Efectivity' },
			{
				data: 'ServiceBulletin',
				className: 'dt-body-nowrap'
			},
			{ data: 'Review' },
			{
				data: 'SB_Date',
				className: 'dt-body-nowrap',
				render: (data, type) => {
					return data ? moment(data).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD') : '';
				} },
			{ data: 'Threshold' }
		],
		columnDefs: [
			//{ targets: [0], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		]
	});
}