var avion, idFam, idCoMay, idTipoMenor;
document.addEventListener('DOMContentLoaded', (e) => {
	idFam = 0;
	let id = parseInt(getPar('id')) || 0;
	avion = new Aircraft(id);
	if (avion.Valid)
		avion.write();
	document.getElementById('IdAircraft').addEventListener('change', cAircraft);
	document.querySelectorAll(`li.nav-item[data-tipo="mMay"]`).forEach(m => {
		m.classList.add('d-none');
		m.querySelector(`a.nav-link[data-bs-target^="#collapse"]`).classList.add('disabled');
	})
	tablas();
	document.querySelectorAll('li.nav-item[data-tipo="mMay"]').forEach(l => { l.addEventListener('click', selPos); })
	document.querySelectorAll('a[data-selector="tipomenores"]').forEach(lnk => {
		lnk.addEventListener('click', (e) => {
			document.querySelectorAll('[data-selector="tipomenores"]:not(.collapsed)').forEach(c => {
				if (c.dataset.tipo !== e.target.dataset.tipo) {
					let ic = bootstrap.Collapse.getInstance(c);
					ic.hide();
				}
			})
			if (!avion.Valid) return;
			idTipoMenor = parseInt(e.currentTarget.dataset.idtipo) || 0;
			tabla.columns().every((idx) => {
				//if (idx === 0) return;
				tabla.column(idx).visible(true);
			});
			switch (idTipoMenor) {
				case 1:
					document.getElementById('titConTipo').innerText = 'Componentes';
					tabla.column(5).visible(false);
					tabla.column(7).visible(false);
					tabla.column(8).visible(false);
					tabla.column(9).visible(false);
					tabla.column(10).visible(false);
					tabla.column(11).visible(false);
					tabla.column(12).visible(false);
					tabla.column(13).visible(false);
					break;
				case 2:
					document.getElementById('titConTipo').innerText = `AD's/SB's`;
					tabla.column(1).visible(false);
					tabla.column(2).visible(false);
					tabla.column(3).visible(false);
					break;
				case 3:
					document.getElementById('titConTipo').innerText = 'Servicios';
					tabla.column(2).visible(false);
					tabla.column(3).visible(false);
					tabla.column(5).visible(false);
					tabla.column(7).visible(false);
					tabla.column(8).visible(false);
					tabla.column(9).visible(false);
					tabla.column(10).visible(false);
					tabla.column(11).visible(false);
					tabla.column(12).visible(false);
					tabla.column(13).visible(false);
					break;
				default:
			}
		});
	})
	document.querySelectorAll('a[data-selector="familias"]').forEach(lnk => {
		lnk.addEventListener('click', (e) => {
			if (!avion.Valid) return;
			document.getElementById('titConFam').innerText = e.currentTarget.innerText;
			idFam = parseInt(e.currentTarget.dataset.idfamilia) || 0;
			reporte(e);
		});
	})
	document.getElementById('rep').addEventListener('click', reporte);
	document.getElementById('pdf_general').addEventListener('click', general);
	document.getElementById('pdf_afac').addEventListener('click', afac);
	document.getElementById('recalc').addEventListener('click', (e) => {
		if (avion.Valid)
			Aircraft.recalcular(avion.Id).then(res => {
				MsjBox('Recalcular', `Recalculado en ${(res.Data.Milisegundos / 1000).toFixed(2)} segundos`)
				console.log(res);
			})

	})
})
const afac = (e) => {
	let idItemMayor = parseInt(document.getElementById('idMayor').value) || 0;
	let url = `${window.location.origin}/Ingenieria/Reportes/PDF_AFAC?idComponenteMayor=${idCoMay}&idTipoMenor=${idTipoMenor}&idFamilia=${idFam}&idAircraft=${avion.Id}&idItemMayor=${idItemMayor}`;
	console.log(url);
	window.open(url, '_blank');
}
const general = (e) => {
	let idItemMayor = parseInt(document.getElementById('idMayor').value) || 0;
	window.open(`${window.location.origin}/Ingenieria/Reportes/PDF_General?idComponenteMayor=${idCoMay}&idTipoMenor=${idTipoMenor}&idFamilia=${idFam}&idAircraft=${avion.Id}&idItemMayor=${idItemMayor}`, '_blank');
}
const vuelos = async (e) => {
	if (!avion.Valid) return;
	let res = await fetch(`${window.location.origin}/Ingenieria/Reportes/vuelos`);
	if (res.ok) {
		let html = await res.text();
		document.getElementById()
	} else
		console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
}
const selPos = (e) => {
	if (!avion.Valid) return;
	let ds = e.currentTarget.dataset;
	idCoMay = parseInt(ds.idmayor) || 0;
	document.getElementById('titConMay').innerText = ds.descripcion
	let idm = parseInt(ds.idmayor) || 0;
	if (idm !== 1)
		document.getElementById('idMayor').innerHTML = `<option selected value="0">...</option>`;
	else
		document.getElementById('idMayor').innerHTML = ``;
	avion.Mayores.forEach(may => {
		if (may.IdComponente !== idm) return;
		let op = document.createElement('option');
		op.value = may.Id;
		op.innerText = `${may.Posicion.Nombre} (${may.Serie})`;
		document.getElementById('idMayor').appendChild(op);
	})
}
const cAircraft = (e) => {
	let id = parseInt(e.target.value) || 0;
	avion = new Aircraft(id);
	if (avion.Valid) {
		document.getElementById('mat').innerText = avion.Matricula;
		document.getElementById('empresa').innerText = avion.Empresa.Nombre;

		document.querySelectorAll(`li.nav-item[data-tipo="mMay"]`).forEach(m => {
			m.classList.add('d-none');
			m.querySelector(`a.nav-link[data-bs-target^="#collapse"]`).classList.add('disabled');
		});
		document.querySelectorAll('div[id^="contenido"]').forEach(dc => { dc.innerHTML = ''; });
		avion.Mayores.forEach(may => {
			let xOpm = document.querySelector(`li.nav-item[data-idmayor="${may.IdComponente}"]`);
			if (xOpm) {
				xOpm.classList.remove('d-none');
				xOpm.querySelector(`a.nav-link[data-bs-target="#collapse${may.Componente.Codigo}"]`).classList.remove('disabled');
			}

			let dv = document.createElement('div');
			dv.classList.add('mb-0', 'text-xs', 'font-weight-bold', 'text-gray-600');
			dv.innerHTML = `Serie <span class="text-dark">${may.Serie}</span> ${(may.IdComponente != 1 ? `(<span class="text-dark">${may.Posicion.Nombre}</span>)` : '')}`;
			document.getElementById(`contenido${may.IdComponente}`).appendChild(dv);

			dv = document.createElement('div');
			dv.classList.add('mb-0', 'text-xs', 'font-weight-bold', 'text-gray-600');
			dv.innerHTML = `TSN <span class="text-dark">${may.TSN}</span><br>CSN <span class="text-dark">${may.CSN}</span>`;
			document.getElementById(`contenido${may.IdComponente}`).appendChild(dv);

			dv = document.createElement('div');
			dv.classList.add('mb-0', 'text-xs', 'font-weight-bold', 'text-gray-600');
			dv.innerHTML = `Modelo <span class="text-dark">${may.Modelo !== null ? may.Modelo.Nombre : ''}</span><br>Capacidad <span class="text-dark">${may.Modelo !== null ?may.Modelo.Capacidad.Nombre:''}</span>`;
			document.getElementById(`contenido${may.IdComponente}`).appendChild(dv);

			dv = document.createElement('hr');
			dv.classList.add('pb-0')
			document.getElementById(`contenido${may.IdComponente}`).appendChild(dv);
		})
	}
}
const reporte = (e) => {
	let idAircraft = parseInt(document.getElementById('IdAircraft').value) || 0;
	tabla.clear();
	if (idAircraft > 0) {
		let idMayor = parseInt(document.getElementById('idMayor').value) || 0;
		ItemMenor.filtro(idAircraft, idCoMay, idMayor, idFam).then(data => {
			loadTable(tabla, data.Data);
		});
	}
}
const tablas = () => {
	tabla = $('#reporte').DataTable({
		processing: true,
		//rowGroup: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-6'B><'col-6'p>>",
		data: [],
		buttons: ['copy', 'excel'],
		order: [[0, 'asc'], [5, 'asc']],
		//rowGroup: {
		//	emptyDataGroup: 'Sin Familia',
		//	dataSrc: 'Family',
		//	className: 'bg-info',
		//	startRender: (rows, group, level) => {
		//		//return `<b>${(group === 1 ? 'Componentes' : group === 2 ? 'Directivas / Boletines de Servicio' : 'Servicios')}</b> (${rows.count()})`;
		//		return group;
		//	}
		//},
		columns: [
			//{
			//	data: 'Family',
			//	//data: null,
			//	visible: false
			//},
			{
				data: null,
				render: (data, type) => {
					let comd = data.IdTipoMenor === 1 ? 'Componente' : data.IdTipoMenor === 2 ? 'ADSB' : 'Servicio';
					return type !== 'display' ? data.Id : `<a href="${window.location.origin}/Ingenieria/Items/${comd}?id=${data.Id}" target="_blank">${data.Id}</a>`;
				}
			},
			{ data: 'Part' },
			{ data: 'Serie' },
			{ data: 'PIC' },
			{
				data: null,
				render: (data, type) => {
					return `${data.ATA1}-${data.ATA2}-${data.ATA3}`;
				}
			},
			//{ data: 'Family' },
			{
				data: 'Directive',
				className: 'dt-body-nowrap'
			},
			{
				data: 'Description',
				width: '15%'
			},
			{ data: 'Amendment' },
			{
				data: 'AD_Date',
				className: 'dt-body-nowrap',
				render: (data, type) => {
					return data ? moment(data).format(type === 'display' ? 'DD MMM YYYY' : 'YYYYMMDD') : '';
				}
			},
			{
				data: 'Efectivity',
				width: '15%'
			},
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
				}
			},
			{ data: 'Threshold' },
			{
				data: null,
				render: (data, type) => {
					let cad = '';
					if (data.Horas > 0)
						cad += `HR: ${(data.Limite_Individual_Horas > 0 ? data.Limite_Individual_Horas : data.Horas)}<br>`;
					if (data.Ciclos > 0)
						cad += `CY: ${(data.Limite_Individual_Ciclos > 0 ? data.Limite_Individual_Ciclos : data.Ciclos)}<br>`;
					if (data.Dias > 0)
						cad += `DY: ${(data.Limite_Individual_Dias > 0 ? data.Limite_Individual_Dias : data.Dias)}<br>`;
					return cad;
				}
			},
			{
				data: null,
				render: (data, type) => {
					let cad = '';
					if (data.Horas > 0)
						cad += `HR: ${data.Horas_Last}<br>`;
					if (data.Ciclos > 0)
						cad += `CY: ${data.Ciclos_Last}<br>`;
					if (data.Fecha_Last !== null)
						cad += moment(data.Fecha_Last).format('DD/MM/YYYY');
					return cad;
				}
			},
			{
				data: null,
				render: (data, type) => {
					let cad = '';
					if (data.Horas > 0)
						cad += `HR: ${data.Horas_Next}<br>`;
					if (data.Ciclos > 0)
						cad += `CY: ${data.Ciclos_Next}<br>`;
					if (data.Fecha_Next !== null)
						cad += moment(data.Fecha_Next).format('DD/MM/YYYY');
					return cad;
				}
			},
			{
				data: null,
				render: (data, type) => {
					let cad = '';
					if (data.Horas > 0)
						cad += `HR: ${data.Horas_Remain}<br>`;
					if (data.Ciclos > 0)
						cad += `CY: ${data.Ciclos_Remain}<br>`;
					if (data.Dias > 0)
						cad += `DY: ${data.Dias_Remain}<br>`;
					return cad;
				}
			}
		],
		columnDefs: [
			{ targets: '_all', className: 'text-center' }
		]
	});
}