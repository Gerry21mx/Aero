var item, titulo, tLim, tMod, itemTemporal;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	item = new ItemMenor(id);
	if (item.Valid)
		fill(true);
	else {
		clear();
		item.Menor = new ComponenteMenor();
	}
	titulo = document.getElementById('tit').innerText;

	if (document.getElementById('reg'))
		document.getElementById('reg').addEventListener('click', registrar)
	if (document.getElementById('del'))
		document.getElementById('del').addEventListener('click', eliminar)
	if (document.getElementById('cle'))
		document.getElementById('cle').addEventListener('click', clear)

	document.getElementById('IdMayor').addEventListener('change', moMods)
	document.getElementById('IdModelo').addEventListener('change', conCoMen);
	document.getElementById('NP').addEventListener('blur', conPics)
	document.querySelector('span.btn[data-id="iNP"]').addEventListener('click', (e) => { infoComMenor(item.Menor); });
	document.getElementById('IdItemMayor').addEventListener('change', (e) => {
		item.Mayor = new ItemMayor(parseInt(e.target.value) || 0);
	});
	document.getElementById('iIdItemMayor').addEventListener('click', (e) => {
		infoItmMayor(item.Mayor);
	});
	document.getElementById('Id').addEventListener('blur', consulta)
	document.getElementById('IdPIC').addEventListener('change', consulta)
	document.getElementById('NP').addEventListener('blur', consulta)
	document.querySelectorAll('#cTimes input').forEach(inp => { inp.addEventListener('blur', ct1); });
});
//TCOM
const moMods = (e) => {
	citMy();
	let idc = parseInt(e.target.value) || 0;
	document.querySelectorAll('#IdModelo option').forEach(om => {
		let mcm = parseInt(om.dataset.idcomponentemayor) || 0;
		if (idc === mcm || om.value === '0')
			om.classList.remove('d-none')
		else
			om.classList.add('d-none');
	});
}
const conCoMen = (e) => {
	citMy();
	let idm = parseInt(document.getElementById('IdModelo').value) || 0;
	if (idm > 0)
		ComponenteMenor.getByModelo(idm).then(data => {
			document.getElementById('listNP').innerHTML = '';
			data.Data.forEach(cm => {
				if (cm.IdTipoMenor !== TCOM)
					return false;
				let ola = document.createElement('option');
				ola.value = cm.Part;
				ola.dataset.id = cm.Id;
				ola.dataset.description = cm.Description;
				document.getElementById('listNP').append(ola);
			});
		});
}
const conPics = (e) => {
	let op = document.querySelector(`#list${e.target.id} option[value="${e.target.value}"]`);
	document.getElementById('tielim').innerHTML = '';
	document.getElementById('IdPIC').innerHTML = '';
	if (op) {
		item.IdComponenteMenor = parseInt(op.dataset.id) || 0;
		item.Menor = new ComponenteMenor(item.IdComponenteMenor);
		if (item.Menor.Valid) {
			if (!item.Menor.Serie)
				document.getElementById('Serie').parentElement.parentElement.classList.add('d-none')
			else
				document.getElementById('Serie').parentElement.parentElement.classList.remove('d-none')
			document.getElementById('IdPIC').innerHTML = '<option value="0">...</option>';
			let ml = parseInt(document.getElementById('IdModelo').value) || 0;
			PIC.getByMenor(item.IdComponenteMenor).then(data => {
				if (data.Data)
					data.Data.forEach((item, index) => {
						if (item.IdModelo !== ml) return;
						let os = document.createElement('option');
						os.value = item.Id;
						os.innerText = `${item.Descripcion} (${item.IdComponente}-${item.IdModelo}-${item.ATA1}-${item.Consecutivo})`
						document.getElementById('IdPIC').appendChild(os);
					})
			})
			item.Menor.Limites.forEach(addLT);
		}
		else
			e.target.value = '';
	}
	else
		e.target.value = '';
}
const ct1 = (e) => {
	let tMAY = document.querySelectorAll('#IdItemMayor option')[document.getElementById('IdItemMayor').selectedIndex].dataset;
	let TiemposMayor = {
		TSN: parseFloat(tMAY.tsn) || 0,
		CSN: parseInt(tMAY.csn) || 0
	}
	let datos = {
		TSN: parseFloat(document.getElementById('TSN').value) || 0,
		CSN: parseInt(document.getElementById('CSN').value) || 0,
		TSN_C: parseFloat(document.getElementById('TSN_Componente_Instalacion').value)||0,
		CSN_C: parseInt(document.getElementById('CSN_Componente_Instalacion').value)||0,
		TSN_A: parseFloat(document.getElementById('TSN_Airframe_Instalacion').value)||0,
		CSN_A: parseInt(document.getElementById('CSN_Airframe_Instalacion').value)||0,
	}
	let tipo = e.target.placeholder === 'Horas' ? 1 : e.target.placeholder === 'Ciclos' ? 2 : 3;
	let origen = e.target.id.lastIndexOf('Airframe') > -1 ? 3 : e.target.id.lastIndexOf('Componente') > -1 ? 2 : 1;
	if (origen === 3) {
		if (tipo === 1)
			document.getElementById('TSN').value = TiemposMayor.TSN - datos.TSN_A + datos.TSN_C;
		else
			document.getElementById('CSN').value = TiemposMayor.CSN - datos.CSN_A + datos.CSN_C;
	}
}
const citMy = (e) => {
	let im = parseInt(document.getElementById('IdMayor').value) || 0;
	let ml = parseInt(document.getElementById('IdModelo').value) || 0;
	document.getElementById('IdItemMayor').innerHTML = '<option value="0" data-tsn="0" data-csn="0">...</option>';
	ItemMayor.getItemsByModelo(im, ml).then(data => {
		if (data.Data)
			data.Data.forEach((itm, index) => {
				let os = document.createElement('option');
				os.value = itm.Id;
				os.dataset.tsn = itm.TSN;
				os.dataset.csn = itm.CSN;
				os.innerText = itm.IdComponente === 1 ? itm.Aircraft.Matricula : `${itm.Serie} ${(itm.Aircraft.Valid ? `(${itm.Aircraft.Matricula}) ${itm.Posicion.Codigo}`:'')}`;
				document.getElementById('IdItemMayor').appendChild(os);
			})
	})
}

const consulta = (e) => {
	let id = parseInt(document.getElementById('Id').value) || 0;
	itemTemporal = item.clon();
	if (id > 0) {
		item = new ItemMenor(id);
		fill(true);
	}
	else {
		let idMayor = parseInt(document.getElementById('IdItemMayor').value) || 0;
		let idPic = TCOM === 1 ? parseInt(document.getElementById('IdPIC').value) || 0 : null;
		item.byMayorMenorPic(idMayor, item.IdComponenteMenor, idPic).then(r => { fill(false); });
	}
}
const fill = (sw) => {
	if (item.Valid) {
		item.write();
		if (sw) {
			document.getElementById('tielim').innerHTML = '';
			document.getElementById('IdMayor').value = item.Mayor.IdComponente;
			if (item.Menor.Modelos.length > 0) {
				document.getElementById('IdModelo').value = item.Menor.Modelos[0].Modelo.Id;
				if (document.querySelectorAll('#IdModelo option[value="18"]').length > 0)
					document.querySelectorAll('#IdModelo option[value="18"]')[0].classList.remove('d-none');
			}
			document.getElementById('IdItemMayor').innerHTML = `<option selected value="${item.Mayor.Id}" data-tsn="${item.Mayor.TSN}" data-csn="${item.Mayor.CSN}">${(item.Mayor.IdComponente === 1 ? item.Mayor.Aircraft.Matricula : `${item.Mayor.Serie} ${(item.Mayor.Aircraft.Valid ? `(${item.Mayor.Aircraft.Matricula}) ${item.Mayor.Posicion.Codigo}` : '')}`)}</option>`;
			document.getElementById('NP').value = item.Menor.Part;
			document.getElementById('IdPIC').innerHTML = `<option selected value="${item.PIC.Id}">${item.PIC.Descripcion} (${item.PIC.IdComponente}-${item.PIC.IdModelo}-${item.PIC.ATA1}-${item.PIC.Consecutivo})</option>`;
			item.Menor.Limites.forEach(addLT);
		}
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
	else {
		item.setValores(itemTemporal);
	}
}
const eliminar = () => {
	if (item.Valid) {
		let res = item.delete();
		MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
		if (res.Valid)
			clear();
	}
}
const registrar = () => {
	item.read();
	let oTime = new Tiempos();
	document.querySelectorAll('div[data-tipo="Tiempos"]').forEach(tl => {
		let idlimite = parseInt(tl.dataset.idlimite) || 0;
		oTime.clear();
		oTime.IdLimite = idlimite;
		oTime.IdItemMenor = item.Id;
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
		item.Tiempos = item.Tiempos === null ? [] : item.Tiempos;
		let idt = item.Tiempos.findIndex(idl => { return idl.IdLimite === oTime.IdLimite; });
		if (idt > -1)
			item.Tiempos[idt] = clonObject(oTime);
		else
			item.Tiempos.push(clonObject(oTime));
	});
	let res = item.save();
	MsjBox(titulo, res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid)
		clear();
}
const clear = () => {
	item.clear();
	item.write();
	document.querySelectorAll('#IdModelo option').forEach(om => { if (om.value !== '0') om.classList.add('d-none') });
	document.querySelectorAll('select, input').forEach(om => { om.value = ''; });
	document.getElementById('tielim').innerHTML = '';
}
const addLT = (limite) => {
	let mosHrs = TCOM === 2 ? true : limite.Horas > 0;
	let mosCic = TCOM === 2 ? true : limite.Ciclos > 0;
	let col = document.createElement('div');
	col.id = `tiem_lim_${limite.Id}`;
	col.classList.add('col-12', 'col-md-6');
	col.dataset.tipo = 'Tiempos';
	col.dataset.idlimite = limite.Id;
	col.innerHTML = `<div class="row justify-content-center">
				<div class="col-12 col-md-10">
					<div class="alert alert-warning py-0 text-center sz-font-75">
						${limite.Limit.Definicion}
						<span class="float-end">
							<div class="form-check form-switch">
								<input type="checkbox" class="form-check-input" id="LimInd_${limite.Id}" data-activo="LimInd_${limite.Id}" data-inactivo="" data-fija="true">
								<label class="form-check-label" for="LimInd_${limite.Id}">Limites Individuales</label>
							</div>
						</span>
					</div>
				</div>
			</div>
			<div class="row alert alert-danger py-0 text-center sz-font-725 g-1 my-0 d-none" data-tiempo="0" data-limit="individuales">
				<div class="col-2 mt-1 mt-md-2">
					Limites Ind.
				</div>
${(mosHrs ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Horas</span>
						<input type="number" id="HrsInd_${limite.Id}" list="listHoras" class="form-control form-control-sm dec text-end" placeholder="Horas" maxlength="6" value="0">
					</div>
				</div>` : '')}
${(mosCic ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Ciclos</span>
						<input type="number" id="CicInd_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Ciclos" maxlength="6" value="0">
					</div>
				</div>` : '')}
				<div class="col-4 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Dias</span>
						<input type="number" id="DiaInd_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Dias" maxlength="9" value="0">
					</div>
				</div>
			</div>
			<div class="row alert alert-primary py-0 text-center sz-font-725 g-1 my-0" data-tiempo="0">
				<div class="col-2 mt-1 mt-md-2">
					Ultimo Cump.
				</div>
${(mosHrs ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Horas</span>
						<input type="number" id="HrsLst_${limite.Id}" list="listHoras" class="form-control form-control-sm dec text-end" placeholder="Horas" maxlength="6" value="0">
					</div>
				</div>` : '')}
${(mosCic ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Ciclos</span>
						<input type="number" id="CicLst_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Ciclos" maxlength="6" value="0">
					</div>
				</div>` : '')}
				<div class="col-4 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Fecha</span>
						<input type="date" id="DiaLst_${limite.Id}" class="form-control form-control-sm fec text-center" placeholder="Fecha" maxlength="10">
					</div>
				</div>
			</div>

			<div class="row alert alert-secondary py-0 text-center sz-font-725 g-1 my-0 ${(TCOM === 1 ? '':'d-none')}" data-tiempo="1">
				<div class="col-2 mt-1 mt-md-2">
					TSO Instalacion
				</div>
${(mosHrs ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Horas</span>
						<input type="number" id="HrsTso_${limite.Id}" list="listHoras" class="form-control form-control-sm dec text-end" placeholder="Horas" maxlength="6" value="0">
					</div>
				</div>` : '')}
${(mosCic ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Ciclos</span>
						<input type="number" id="CicTso_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Ciclos" maxlength="6" value="0">
					</div>
				</div>` : '')}
				<div class="col-4 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Dias</span>
						<input type="number" id="DiaTso_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Dias" maxlength="9" value="0">
					</div>
				</div>
			</div>

			<div class="row alert alert-success py-0 text-center sz-font-725 g-1 my-0" data-tiempo="1">
				<div class="col-2 mt-1 mt-md-2">
					 Transcurrido
				</div>
${(mosHrs ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Horas</span>
						<input type="number" id="HrsElp_${limite.Id}" list="listHoras" class="form-control form-control-sm dec text-end" placeholder="Horas" maxlength="6" value="0" disabled="true">
					</div>
				</div>` : '')}
${(mosCic ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Ciclos</span>
						<input type="number" id="CicElp_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Ciclos" maxlength="6" value="0" disabled="true">
					</div>
				</div>` : '')}
				<div class="col-4 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Dias</span>
						<input type="number" id="DiaElp_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Dias" maxlength="9" value="0" disabled="true">
					</div>
				</div>
			</div>

			<div class="row alert alert-info py-0 text-center sz-font-725 g-1 my-0" data-tiempo="1">
				<div class="col-2 mt-1 mt-md-2">
					Siguiente
				</div>
${(mosHrs ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Horas</span>
						<input type="number" id="HrsNxt_${limite.Id}" list="listHoras" class="form-control form-control-sm dec text-end" placeholder="Horas" maxlength="6" value="0" disabled="true">
					</div>
				</div>` : '')}
${(mosCic ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Ciclos</span>
						<input type="number" id="CicNxt_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Ciclos" maxlength="6" value="0" disabled="true">
					</div>
				</div>` : '')}
				<div class="col-4 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Dias</span>
						<input type="date" id="DiaNxt_${limite.Id}" class="form-control form-control-sm text-end" placeholder="Dias" maxlength="10" value="0" disabled="true">
					</div>
				</div>
			</div>

			<div class="row alert alert-warning py-0 text-center sz-font-725 g-1 my-0" data-tiempo="1">
				<div class="col-2 mt-1 mt-md-2">
					Remanentes
				</div>
${(mosHrs ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Horas</span>
						<input type="number" id="HrsRem_${limite.Id}" list="listHoras" class="form-control form-control-sm dec text-end" placeholder="Horas" maxlength="6" value="0" disabled="true">
					</div>
				</div>` : '')}
${(mosCic ? `
				<div class="col-3 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Ciclos</span>
						<input type="number" id="CicRem_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Ciclos" maxlength="6" value="0" disabled="true">
					</div>
				</div>` : '')}
				<div class="col-4 mt-1">
					<div class="input-group input-group-sm">
						<span class="input-group-text">Dias</span>
						<input type="number" id="DiaRem_${limite.Id}" class="form-control form-control-sm num text-end" placeholder="Dias" maxlength="9" value="0" disabled="true">
					</div>
				</div>
			</div>
			<div class="row justify-content-center mt-1 mt-md-2">
				<div class="col-6 col-md-3">
					<button class="btn btn-sm btn-outline-primary" id="cle${limite.Id}"><i class="fas fa-trash-restore"></i> Limpiar</button>
				</div>
			</div>`;
	document.getElementById('tielim').appendChild(col);
	changeSwitch();
	tooltips();
	document.querySelectorAll('input[type="checkbox"][id^="LimInd"]').forEach(chk => {
		chk.addEventListener('change', (e) => {
			let cnt = e.target.closest('div[data-tipo="Tiempos"]');
			if (e.target.checked)
				cnt.querySelector('.row[data-limit="individuales"]').classList.remove('d-none');
			else
				cnt.querySelector('.row[data-limit="individuales"]').classList.add('d-none');
		});
	});
	//		EVENTOS
	document.getElementById(`cle${limite.Id}`).addEventListener('click', (e) => {
		document.querySelectorAll(`#tiem_lim_${limite.Id} input`).forEach(i => { i.value=''; });
	})
	document.getElementById('TSN').addEventListener('blur', (e) => {
		document.querySelectorAll(`input[id*="Lst_"]`).forEach(i => { i.dispatchEvent(new Event('blur')); });
	})
	document.querySelectorAll('div[data-tipo="Tiempos"]').forEach(tmp => {
		let idl = parseInt(tmp.dataset.idlimite) || 0;
		tmp.querySelectorAll(`input[id*="Lst_"],input[id*="Tso_"]`).forEach(e => {
			e.addEventListener('blur', (e) => {
				let campo = e.target.id.substring(0, 3);
				let ixl = item.Menor.Limites.findIndex(l => { return l.Id === idl });
				let lim = 0;
				if (document.getElementById(`LimInd_${idl}`).checked)
					lim = parseFloat(document.getElementById(`${campo}Ind_${idl}`).value) || 0;
				else if (ixl > -1)
					lim = item.Menor.Limites[ixl][(campo === 'Hrs' ? 'Horas' : campo === 'Dia' ? 'Dias' : 'Ciclos')];
				let tpo = campo === 'Hrs' ? 'TSN' : 'CSN';
				let ta = item.Mayor[tpo];
				let sn = document.getElementById(campo + 'Elp_' + idl);
				if (campo !== 'Dia') {
					let ult = parseFloat(document.getElementById(`${campo}Lst_${idl}`).value) || 0;
					let tso = parseFloat(document.getElementById(`${campo}Tso_${idl}`).value) || 0;
					let elp = ta - ult + tso;
					let nxt = ult - tso + lim;
					let rem = nxt - ta;
					document.getElementById(campo + 'Elp_' + idl).value = elp;		//	Transcurrido
					document.getElementById(campo + 'Nxt_' + idl).value = nxt;		//	Siguiente
					document.getElementById(campo + 'Rem_' + idl).value = rem;		//	Remanente
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
}