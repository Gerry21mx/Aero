var tabla;
document.addEventListener('DOMContentLoaded', () => {
	document.getElementById('con').addEventListener('click', (e) => {
		let idAircraft = parseInt(document.getElementById('IdAircraft').value) || 0;
		let idMayor = parseInt(document.getElementById('IdComponenteMayor').value) || 0;
		let idFamilia = parseInt(document.getElementById('IdFamily').value) || 0;
		ItemMenor.filtro(idAircraft, idMayor, idFamilia, null).then(data => {
			loadTable(tabla, data.Data);
		})
	})
})
const tablas = (e) => {
	tabla = $('#tabla').DataTable({
		processing: true,
		rowGroup: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: [],
		buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
		//order: [[3, 'asc']],
		//rowGroup: {
		//	emptyDataGroup: 'Sin Tipo de Cuenta',
		//	dataSrc: 'Tipo.Nombre',
		//	className: 'bg-info'
		//},
		columns: [
			{ data: 'Id' },
			{ data: 'Family' },
			{ data: 'Part', visible: false },
			{ data: 'Description' },
			{ data: 'Serie' },
			{ data: 'PIC' },
			{ data: null },
			{ data: null },
			{ data: null },
			{ data: null },
			{ data: null },
			{ data: null },
			{ data: null }
		],
		columnDefs: [
			//{ targets: [0], className: 'text-end' },
			{ targets: '_all', className: 'text-justify' }
		],
		drawCallback: function (settings) {
			
		}
	});
}