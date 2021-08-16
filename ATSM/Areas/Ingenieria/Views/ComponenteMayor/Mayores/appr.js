var itemMayor, titulo, tabla, oTime;
document.addEventListener('DOMContentLoaded', () => {
	oTime = new Tiempos();
	titulo = document.getElementById('tit').innerText;
	$('#Arbol').jstree({
		core: {
			themes: {
				stripes: 'true'
			},
			multiple: false,
			animation: 0
		},
		//checkbox: {
		//	three_state: false
		//},
		//plugins: ['wholerow', 'checkbox', 'types', 'themes', 'search']
		plugins: ['wholerow', 'types', 'themes', 'search']
	});
	$('#Arbol').on('select_node.jstree', select);
	document.querySelectorAll('#IdPosicion option').forEach(o => {
		if (o.dataset.mayores === 'False')
			o.remove();
	});
	document.querySelectorAll('#IdModelo option').forEach(om => {
		let idc = parseInt(om.dataset.idcomponentemayor) || 0;
		if (COMAY !== idc && om.value !== '0')
			om.remove();
	});
	document.getElementById('IdModelo').addEventListener('change', (e) => {
		let idm = parseInt(e.target.value) || 0;
		let modelo = new Modelo(idm);
		document.querySelectorAll(`div[data-tipo="Tiempos"]`).forEach(d => { d.classList.add('d-none') })
		modelo.Limites.forEach(lim => {
			if (lim.Activo)
				document.querySelector(`div[data-idlimite="${lim.IdLimite}"]`).classList.remove('d-none')
			//else
			//	document.querySelector(`div[data-idlimite="${lim.IdLimite}"]`).classList.add('d-none')
		})
	})
	if (COMAY !== 1) {
		document.getElementById('IdAircraft').disabled = true;
		document.querySelectorAll('#IdAircraft option').forEach(avi => { avi.classList.add('d-none'); });
		document.getElementById('IdModelo').addEventListener('change', (e) => {
			let os = e.target.options[e.target.selectedIndex];
			let capM = parseInt(os.dataset.idcapacidad) || 0;
			document.getElementById('IdAircraft').value = 0;
			document.querySelectorAll('#IdAircraft option').forEach(avi => {
				let capA = parseInt(avi.dataset.idcapacidad) || 0;
				if (capM === capA)
					avi.classList.remove('d-none');
				else
					avi.classList.add('d-none');
			});
		});
	}
	if (document.getElementById('IdEstadoMayor'))
		document.getElementById('IdEstadoMayor').addEventListener('change', (e) => {
			if (e.target.value === '1')
				document.getElementById('IdAircraft').disabled = false;
			else {
				document.getElementById('IdAircraft').disabled = true;
				document.getElementById('IdAircraft').value = 0;
			}
		});
	if (document.querySelector('#reg'))
		document.querySelector('#reg').addEventListener('click', registrar);
	if (document.querySelector('#del'))
		document.querySelector('#del').addEventListener('click', eliminar);
	document.querySelector('#cle').addEventListener('click', clear);
	tablas();
	ItemMayor.getItemMayors(COMAY).then(datos => { loadTable(tabla, datos.Data); });
	document.querySelectorAll('input[type="checkbox"][id^="LimInd"]').forEach(chk => {
		chk.addEventListener('change', (e) => {
			let cnt = e.target.closest('div[data-tipo="Tiempos"]');
			if (e.target.checked)
				cnt.querySelector('.row[data-limit="individuales"]').classList.remove('d-none');
			else
				cnt.querySelector('.row[data-limit="individuales"]').classList.add('d-none');
		});
	});
	//		TIEMPOS
	document.getElementById('TSN').addEventListener('blur', (e) => {
		let evt = new Event('blur');
		document.querySelectorAll(`input[id*="Lst_"]`).forEach(i => { i.dispatchEvent(evt); });
	})
	document.querySelectorAll('div[data-tipo="Tiempos"]').forEach(tmp => {
		let idl = parseInt(tmp.dataset.idlimite) || 0;
		tmp.querySelectorAll(`input[id*="Lst_"],input[id*="Elp_"]`).forEach(e => {
			e.addEventListener('blur', (e) => {
				let campo = e.target.id.substring(0, 3);
				let ixl = itemMayor.Limites.findIndex(l => { return l.Id === idl });
				let lim = 0;
				if (document.getElementById(`LimInd_${idl}`).checked)
					lim = parseFloat(document.getElementById(`${campo}Ind_${idl}`).value) || 0;
				else if (ixl > -1)
					lim = itemMayor.Limites[ixl][(campo === 'Hrs' ? 'Horas' : campo === 'Dia' ? 'Dias' : 'Ciclos')];
				let tpo = campo === 'Hrs' ? 'TSN' : 'CSN';
				let ta = parseFloat(document.getElementById(tpo).value) || 0;
				let sn = document.getElementById(campo + 'Elp_' + idl);
				if (campo !== 'Dia') {
					let ult = parseFloat(document.getElementById(`${campo}Lst_${idl}`).value) || 0;
					let elp = ta - ult;
					let nxt = ult + lim;
					let rem = nxt - ta;
					document.getElementById(campo + 'Elp_' + idl).value = elp;
					document.getElementById(campo + 'Nxt_' + idl).value = nxt;
					document.getElementById(campo + 'Rem_' + idl).value = rem;
				}
				else {
					let lc = moment(document.getElementById(`${campo}Lst_${idl}`).value);
					sn.value = parseInt(moment().diff(lc, 'days')) || 0;
					if (lim > 0) {
						document.getElementById(`DiaNxt_${idl}`).value = lc.add(lim, 'days').format('YYYY-MM-DD');
						document.getElementById(`DiaRem_${idl}`).value = lim - sn.value;
					}
				}
			})
		})
	});
	let idim = parseInt(getPar('id')) || 0;
	itemMayor = new ItemMayor(idim);
	if (itemMayor.Valid) {
		setItem(itemMayor);

		let someTabTriggerEl = document.querySelector('#Alta')
		let tab = new bootstrap.Tab(someTabTriggerEl)
		tab.show()
	}
});
const select = (e, data) => {
	var data = data.node.data;
	let idTipo = parseInt(data.idtipo) || 0;
	let idMenor = parseInt(data.iditemmenor) || 0;
	if (idMenor > 0) {
		let url = `${window.location.origin}/Ingenieria/ComponenteMenor/${(idTipo === 1 ? 'Componente' : idTipo === 2 ? 'ADSB' : 'Servicio')}?id=${idMenor}`;
		window.open(url, '_blank')
	}
}
//const consulta = (e) => {
//	let mat = e.target.value;
//	if (mat === '') return;
//	procc();
//	itemMayor.byCadena(mat).then(r => {
//		itemMayor.write();
//		if (!itemMayor.Valid)
//			e.target.value = mat;
//		endPro();
//	});
//}
const eliminar = () => {
	if (itemMayor.Valid) {
		let id = itemMayor.Id;
		let res = itemMayor.delete();
		MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid) {
			delDT(tabla, 'Id', id);
			itemMayor.write();
		}
	}
}
const registrar = () => {
	itemMayor.read();
	itemMayor.IdComponente = COMAY;
	if (COMAY === 1) {
		itemMayor.IdEstadoMayor = 1;	//	Estado Instalado
		itemMayor.IdPosicion = 1;		//	Posicion Unica
	}
	document.querySelectorAll('div[data-tipo="Tiempos"]').forEach(tl => {
		if (tl.classList.contains('d-none')) return
		let idlimite = parseInt(tl.dataset.idlimite) || 0;
		oTime.clear()
		oTime.IdLimite = idlimite;
		oTime.IdItemMayor = itemMayor.Id;
		oTime.Limite_Individual_Horas = tl.querySelector(`#HrsInd_${idlimite}`) ? (parseInt(tl.querySelector(`#HrsInd_${idlimite}`).value) || 0) : null;
		oTime.Limite_Individual_Ciclos = tl.querySelector(`#CicInd_${idlimite}`) ? (parseInt(tl.querySelector(`#CicInd_${idlimite}`).value) || 0) : null;
		oTime.Limite_Individual_Dias = parseInt(tl.querySelector(`#DiaInd_${idlimite}`).value) || 0;
		oTime.Horas_Last = tl.querySelector(`#HrsLst_${idlimite}`) ? parseInt(tl.querySelector(`#HrsLst_${idlimite}`).value) || 0 : null;
		oTime.Ciclos_Last = tl.querySelector(`#CicLst_${idlimite}`) ? parseInt(tl.querySelector(`#CicLst_${idlimite}`).value) || 0 : null;
		oTime.Fecha_Last = tl.querySelector(`#DiaLst_${idlimite}`).value;
		oTime.Horas_TSO_Instalacion = tl.querySelector(`#HrsTso_${idlimite}`) ? parseInt(tl.querySelector(`#HrsTso_${idlimite}`).value) || 0 : null;
		oTime.Ciclos_TSO_Instalacion = tl.querySelector(`#CicTso_${idlimite}`) ? parseInt(tl.querySelector(`#CicTso_${idlimite}`).value) || 0 : null;
		oTime.Dias_TSO_Instalacion = parseInt(tl.querySelector(`#DiaTso_${idlimite}`).value) || 0;
		oTime.Horas_Elapsed = tl.querySelector(`#HrsElp_${idlimite}`) ? parseInt(tl.querySelector(`#HrsElp_${idlimite}`).value) || 0 : null;
		oTime.Ciclos_Elapsed = tl.querySelector(`#CicElp_${idlimite}`) ? parseInt(tl.querySelector(`#CicElp_${idlimite}`).value) || 0 : null;
		oTime.Dias_Elapsed = parseInt(tl.querySelector(`#DiaElp_${idlimite}`).value) || 0;
		oTime.Horas_Next = tl.querySelector(`#HrsNxt_${idlimite}`) ? parseInt(tl.querySelector(`#HrsNxt_${idlimite}`).value) || 0 : null;
		oTime.Ciclos_Next = tl.querySelector(`#CicNxt_${idlimite}`) ? parseInt(tl.querySelector(`#CicNxt_${idlimite}`).value) || 0 : null;
		oTime.Fecha_Next = tl.querySelector(`#DiaNxt_${idlimite}`).value;
		oTime.Horas_Remain = tl.querySelector(`#HrsRem_${idlimite}`) ? parseInt(tl.querySelector(`#HrsRem_${idlimite}`).value) || 0 : null;
		oTime.Ciclos_Remain = tl.querySelector(`#CicRem_${idlimite}`) ? parseInt(tl.querySelector(`#CicRem_${idlimite}`).value) || 0 : null;
		oTime.Dias_Remain = parseInt(tl.querySelector(`#DiaRem_${idlimite}`).value) || 0;
		itemMayor.Tiempos = itemMayor.Tiempos === null ? [] : itemMayor.Tiempos;
		let idt = itemMayor.Tiempos.findIndex(idl => { return idl.Limite.IdLimite === oTime.IdLimite; });
		if (idt > -1)
			itemMayor.Tiempos[idt] = clonObject(oTime);
		else
			itemMayor.Tiempos.push(clonObject(oTime));
	});
	let res = itemMayor.save();
	MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		updDT(tabla, itemMayor, 'Id');
		clear();
	}
}
const clear = () => {
	itemMayor.clear();
	itemMayor.write();
	document.querySelectorAll('input').forEach(i => { i.value = ''; })
	document.querySelectorAll('input.num').forEach(i => { i.value = '0'; })
	document.querySelectorAll('input[type="checkbox"]').forEach(i => {
		i.checked = false;
		let e = new Event('change');
		i.dispatchEvent(e);
	})
}
const setItem = (item) => {
	item.write();
	document.getElementById('IdModelo').dispatchEvent(new Event('change'))
	item.Tiempos.forEach(t => {
		if (document.querySelector(`#HrsInd_${t.IdLimite}`))
			document.querySelector(`#HrsInd_${t.IdLimite}`).value = t.Limite_Individual_Horas;
		if (document.querySelector(`#CicInd_${t.IdLimite}`))
			document.querySelector(`#CicInd_${t.IdLimite}`).value = t.Limite_Individual_Ciclos;
		if (document.querySelector(`#DiaInd_${t.IdLimite}`))
			document.querySelector(`#DiaInd_${t.IdLimite}`).value = t.Limite_Individual_Dias;

		if (document.querySelector(`#HrsLst_${t.IdLimite}`))
			document.querySelector(`#HrsLst_${t.IdLimite}`).value = t.Horas_Last;
		if (document.querySelector(`#CicLst_${t.IdLimite}`))
			document.querySelector(`#CicLst_${t.IdLimite}`).value = t.Ciclos_Last;
		if (document.querySelector(`#DiaLst_${t.IdLimite}`))
			document.querySelector(`#DiaLst_${t.IdLimite}`).value = moment(t.Fecha_Last).format('YYYY-MM-DD');

		if (document.querySelector(`#HrsTso_${t.IdLimite}`))
			document.querySelector(`#HrsTso_${t.IdLimite}`).value = t.Horas_TSO_Instalacion;
		if (document.querySelector(`#CicTso_${t.IdLimite}`))
			document.querySelector(`#CicTso_${t.IdLimite}`).value = t.Ciclos_TSO_Instalacion;
		if (document.querySelector(`#DiaTso_${t.IdLimite}`))
			document.querySelector(`#DiaTso_${t.IdLimite}`).value = t.Dias_TSO_Instalacion;

		if (document.querySelector(`#HrsElp_${t.IdLimite}`))
			document.querySelector(`#HrsElp_${t.IdLimite}`).value = t.Horas_Elapsed;
		if (document.querySelector(`#CicElp_${t.IdLimite}`))
			document.querySelector(`#CicElp_${t.IdLimite}`).value = t.Ciclos_Elapsed;
		if (document.querySelector(`#DiaElp_${t.IdLimite}`))
			document.querySelector(`#DiaElp_${t.IdLimite}`).value = t.Dias_Elapsed;

		if (document.querySelector(`#HrsNxt_${t.IdLimite}`))
			document.querySelector(`#HrsNxt_${t.IdLimite}`).value = t.Horas_Next;
		if (document.querySelector(`#CicNxt_${t.IdLimite}`))
			document.querySelector(`#CicNxt_${t.IdLimite}`).value = t.Ciclos_Next;
		if (document.querySelector(`#DiaNxt_${t.IdLimite}`))
			document.querySelector(`#DiaNxt_${t.IdLimite}`).value = moment(t.Fecha_Next).format('YYYY-MM-DD');

		if (document.querySelector(`#HrsRem_${t.IdLimite}`))
			document.querySelector(`#HrsRem_${t.IdLimite}`).value = t.Horas_Remain;
		if (document.querySelector(`#CicRem_${t.IdLimite}`))
			document.querySelector(`#CicRem_${t.IdLimite}`).value = t.Ciclos_Remain;
		if (document.querySelector(`#DiaRem_${t.IdLimite}`))
			document.querySelector(`#DiaRem_${t.IdLimite}`).value = t.Dias_Remain;

		if ((t.Limite_Individual_Horas + t.Limite_Individual_Ciclos + t.Limite_Individual_Dias) > 0) {
			document.getElementById(`LimInd_${t.IdLimite}`).checked = true;
			document.getElementById(`LimInd_${t.IdLimite}`).dispatchEvent(new Event('change'));
		}
	})
}
const tablas = () => {
	tabla = $('#tabla').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{ data: 'Id' },
			{ data: 'Estado.Nombre' },
			{ data: 'Modelo.Nombre' },
			{ data: 'Serie' },
			{ data: 'Aircraft.Matricula' },
			{ data: 'Posicion.Codigo' },
			{ data: 'Capacity' },
			{ data: 'TSN' },
			{ data: 'CSN' },
			{
				data: null,
				orderable: false,
				defaultContent: `<button data-type="edit" type="button" class="btn btn-primary btn-primary py-0"><i class="fas fa-pencil-alt"></i></button>`
			}
		],
		columnDefs: [
			{ targets: [7, 8], className: 'text-end' },
			{ targets: '_all', className: 'text-center' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} button[data-type="edit"]`).click(async (e) => {
				procc()
				let data = tabla.rows(e.target.closest('tr')).data()[0];
				let modelo = new Modelo(data.IdModelo);
				document.querySelectorAll(`div[data-tipo="Tiempos"]`).forEach(d => { d.classList.add('d-none') })
				modelo.Limites.forEach(lim => {
					if (lim.Activo)
						document.querySelector(`div[data-idlimite="${lim.IdLimite}"]`).classList.remove('d-none')
				})
				itemMayor.clear();
				itemMayor.setValores(data);
				Tiempos.getTiempos(data.Id, 1).then(tiempos => {
					itemMayor.Tiempos = clonObject( tiempos.Data)
					itemMayor.write()
					document.querySelectorAll('div[data-tipo="Tiempos"] input').forEach(i => { i.value = '' })
					itemMayor.Tiempos.forEach(t => {
						if (document.querySelector(`#HrsInd_${t.Limite.IdLimite}`))
							document.querySelector(`#HrsInd_${t.Limite.IdLimite}`).value = t.Limite_Individual_Horas;
						if (document.querySelector(`#CicInd_${t.Limite.IdLimite}`))
							document.querySelector(`#CicInd_${t.Limite.IdLimite}`).value = t.Limite_Individual_Ciclos;
						if (document.querySelector(`#DiaInd_${t.Limite.IdLimite}`))
							document.querySelector(`#DiaInd_${t.Limite.IdLimite}`).value = t.Limite_Individual_Dias;

						if (document.querySelector(`#HrsLst_${t.Limite.IdLimite}`))
							document.querySelector(`#HrsLst_${t.Limite.IdLimite}`).value = t.Horas_Last;
						if (document.querySelector(`#CicLst_${t.Limite.IdLimite}`))
							document.querySelector(`#CicLst_${t.Limite.IdLimite}`).value = t.Ciclos_Last;
						if (document.querySelector(`#DiaLst_${t.Limite.IdLimite}`))
							document.querySelector(`#DiaLst_${t.Limite.IdLimite}`).value = moment(t.Fecha_Last).format('YYYY-MM-DD');

						if (document.querySelector(`#HrsTso_${t.Limite.IdLimite}`))
							document.querySelector(`#HrsTso_${t.Limite.IdLimite}`).value = t.Horas_TSO_Instalacion;
						if (document.querySelector(`#CicTso_${t.Limite.IdLimite}`))
							document.querySelector(`#CicTso_${t.Limite.IdLimite}`).value = t.Ciclos_TSO_Instalacion;
						if (document.querySelector(`#DiaTso_${t.Limite.IdLimite}`))
							document.querySelector(`#DiaTso_${t.Limite.IdLimite}`).value = t.Dias_TSO_Instalacion;

						if (document.querySelector(`#HrsElp_${t.Limite.IdLimite}`))
							document.querySelector(`#HrsElp_${t.Limite.IdLimite}`).value = t.Horas_Elapsed;
						if (document.querySelector(`#CicElp_${t.Limite.IdLimite}`))
							document.querySelector(`#CicElp_${t.Limite.IdLimite}`).value = t.Ciclos_Elapsed;
						if (document.querySelector(`#DiaElp_${t.Limite.IdLimite}`))
							document.querySelector(`#DiaElp_${t.Limite.IdLimite}`).value = t.Dias_Elapsed;

						if (document.querySelector(`#HrsNxt_${t.Limite.IdLimite}`))
							document.querySelector(`#HrsNxt_${t.Limite.IdLimite}`).value = t.Horas_Next;
						if (document.querySelector(`#CicNxt_${t.Limite.IdLimite}`))
							document.querySelector(`#CicNxt_${t.Limite.IdLimite}`).value = t.Ciclos_Next;
						if (document.querySelector(`#DiaNxt_${t.Limite.IdLimite}`))
							document.querySelector(`#DiaNxt_${t.Limite.IdLimite}`).value = moment(t.Fecha_Next).format('YYYY-MM-DD');

						if (document.querySelector(`#HrsRem_${t.Limite.IdLimite}`))
							document.querySelector(`#HrsRem_${t.Limite.IdLimite}`).value = t.Horas_Remain;
						if (document.querySelector(`#CicRem_${t.Limite.IdLimite}`))
							document.querySelector(`#CicRem_${t.Limite.IdLimite}`).value = t.Ciclos_Remain;
						if (document.querySelector(`#DiaRem_${t.Limite.IdLimite}`))
							document.querySelector(`#DiaRem_${t.Limite.IdLimite}`).value = t.Dias_Remain;

						if ((t.Limite_Individual_Horas + t.Limite_Individual_Ciclos + t.Limite_Individual_Dias) > 0) {
							document.getElementById(`LimInd_${t.Limite.IdLimite}`).checked = true;
							document.getElementById(`LimInd_${t.Limite.IdLimite}`).dispatchEvent(new Event('change'));
						}
					})
					endPro()
				})
			});
		}
	});
}