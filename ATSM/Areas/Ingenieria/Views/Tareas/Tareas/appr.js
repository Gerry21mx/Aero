var task, edI = -1, edIG = -1, edMT, tab1, tab2, tab3, tab4, tab5;
$(document).ready(function () {
	CKEDITOR.replace('ckeIns', {
		language: 'es',
		uiColor: '#9AB8F3'
	});
	tablas();
	var xidt = parseInt(getPar('tid')) || 0;
	task = new Tarea(xidt);
	if (!task.Valid) {
		var xidc = parseInt(getPar('idc')) || 0;
		let compT = new ComponenteMenor(xidc);
		task.ComponenteMenor = compT;
		if (task.ComponenteMenor.Valid) {
			task.idComponente = task.ComponenteMenor.idComponente;
		}
		task.idModelo = getPar('mod');
		if (task.idComponente > 0 && task.idModelo !== '') {
			task.GetInstance();
			if (!task.Valid) {
				task.ComponenteMenor = compT;
			}
			conTask();
		}
	} else {
		conTask();
	}

	$('#addI').click(addI);
	$('#add1, #add2').click(addMatrialTool);
	$('#addR').click(addR);

	$('#np').blur(mosCom);
	$('#descripcion').blur(mosCom);
	$('#np1, #np2').blur(InvNP);
	$('#btnSav').click(save);
	$('#btnDel').click(del);
	$('#idModelo').change(function () {
		let idM = $('#idModelo').val().trim();
		if (idM !== '') {
			let tsk = Tarea.consComponenteModelo(task.idComponente, $('#idModelo').val().trim());
			task = tsk.Valid ? tsk : task;
			conTask();
		}
	});
	$('#btnImg').click(() => { $('#imageTU').click(); });
	$('#addIMG').click(addIMG);

	document.getElementById("imageTU").onchange = (e) => {
		let reader = new FileReader();
		let tit = document.getElementById("Titulo").value, pre = document.getElementById('preview');
		reader.readAsDataURL(e.target.files[0]);
		reader.onload = () => {
			pre.src = reader.result;
			pre.alt = tit;
			pre.title = tit;
		};
		if (e.target.files.length > 0) {
			document.getElementById('preview').classList.remove('d-none');
		} else {
			document.getElementById('preview').classList.add('d-none');
		}
	}
	//$('#btnPrev').popupWindow({
	//	windowName: 'Preview Task',
	//	height: 900,
	//	width: 1200,
	//	centerScreen: 1,
	//	location: 0,
	//	menubar: 0,
	//	rezisable: 0,
	//	scrollbars: 1,
	//	windowURL: `${window.location.origin}/appRest/appReports.cshtml?rep=3&tid=${task.TaskId}&idC=${task.ComponenteMenor.idComponente}`,
	//	toolbar: 0
	//});
	document.getElementById('IdMayor').addEventListener('change', moMods);
	document.getElementById('IdModelo').addEventListener('change', conCoMen);
	document.getElementById('np').addEventListener('blur', conPics);
});
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
			PIC.getByMenor(item.IdComponenteMenor).then(data => {
				if (data.Data)
					data.Data.forEach((item, index) => {
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

function save() {
	procesando(true);
	task.Read();
	task.TypeId = parseInt(task.TypeId) || 0;
	if (task.Instructions.length === 0) {
		Mensaje('Task Definition', '<i class="fas fa-exclamation-triangle"></i> Falta Informacion<br>Debe agregar por lo menos una Instruccioin a la Tarea');
		return false;
	}
	if (task.idModelo === '' || task.idModelo === null || !task.ComponenteMenor.Valid) {
		Mensaje('Task Definition', '<i class="fas fa-exclamation-triangle"></i> Falta Informacion<br>Debe indicar el Componente y el Modelo al cual aplica la Tarea');
		return false;
	}
	var dataString = new FormData();
	task.Images.forEach(imagen => {
		dataString.append(imagen.FileName, imagen.File);
	});
	task.idComponente = task.ComponenteMenor.idComponente;
	task.idModelo = $('#idModelo').val();
	var res = task.Save();
	Mensaje('Error al Guardar', res.Error !== '' ? res.Error : res.Mensaje);
	if (res.Valid) {
		dataString.append("json", JSON.stringify({ op: 1, Images: task.Images }));
		if (task.Images.length > 0) {
			$.ajax({
				async: false,
				url: `${window.location.origin}/appRest/appFiles.cshtml`,
				type: "POST",
				dataType: "HTML",
				data: dataString,
				cache: false,
				contentType: false,
				processData: false,
				success: function (res) {
					respvar = JSON.parse(res);
					if (!respvar.Valid) {
						res.Valid = false;
						res.Mensaje = respvar.Error;
					}
					console.log(respvar.Valid ? respvar.Mensaje : respvar.Error);
				},
				error: function (x, y, z) {
					mostrarError(x, y, z, 'Upload Imagenes Tarea');
				}
			});
		}
		Mensaje('Tarea Guardada', res.Mensaje);
		Clear();
	}
	terminaProcesando();
}
function del() {
	let res = task.Delete();
	Mensaje('Eliminar Tarea', res.Valid ? res.Mensaje : res.Error);
}
function conTask() {
	let idM = $('#idModelo').val();
	$('#LblTaskId').text('');
	Clear();
	task.Write();
	if (task.Valid) {
		$('#LblTaskId').text(task.TaskId);
		$('#btnPrev').removeAttr('disabled');
		idM = task.idModelo;
		let comp = new ComponenteMenor();
		comp.SetValores(task.ComponenteMenor);
		setModelos(comp.GetModelos());
		setData(task.ComponenteMenor);
		tab1.rows.add(task.MaterialTools.filter(item => item.Type === 1));
		tab1.columns.adjust().draw();
		tab2.rows.add(task.MaterialTools.filter(item => item.Type === 2));
		tab2.columns.adjust().draw();
		tab3.rows.add(task.Instructions);
		tab3.columns.adjust().draw();
		tab4.rows.add(task.ReferenceS);
		tab4.columns.adjust().draw();
		tab5.rows.add(task.Images);
		tab5.columns.adjust().draw();
		$('#Repeat').focus();
	} else {
		task.ComponenteMenor.Write();
		setModelos(task.ComponenteMenor.GetModelos());
	}
	$('#idModelo').val(idM);
}
function addMatrialTool() {
	let refe = new MaterialTool();
	refe.Type = this.id === 'add1' ? 1 : 2;
	refe.NP = $(`#np${refe.Type}`).val().trim();
	refe.MAR = refe.Type === 1 ? $("#MAR").prop("checked") : false;
	refe.Descripcion = $(`#des${refe.Type}`).text().trim();
	refe.Cantidad = parseFloat($(`#qty${refe.Type}`).val()) || 0;
	if (refe.NP !== '') {
		let pos = task.MaterialTools.findIndex(elem => elem.NP == refe.NP);
		if (pos > -1) {
			if (refe.Cantidad > 0 || refe.MAR) {
				if (refe.Type === 1) {
					let rw = tab1.data().toArray().findIndex((elem) => { return elem.NP === refe.NP; });
					if (rw === -1) {
						Mensaje('Articulo Repetido', 'El articulo ya ha sido agregado a la lista de Herramientas y no puede ser duplicado.')
						return false;
					}
					tab1.row(rw).data(refe);
				} else {
					let rw = tab2.data().toArray().findIndex((elem) => { return elem.NP === refe.NP; });
					if (rw === -1) {
						Mensaje('Articulo Repetido', 'El articulo ya ha sido agregado a la lista de Materiales y no puede ser duplicado.')
						return false;
					}
					tab2.row(rw).data(refe);
				}
				task.MaterialTools[pos] = ClonObject(refe);
			} else {
				if (refe.Type === 1) {
					let rw = tab1.data().toArray().findIndex((elem) => { return elem.NP === refe.NP; });
					if (rw > -1) {
						tab1.row(rw).remove();
					}
				} else {
					let rw = tab2.data().toArray().findIndex((elem) => { return elem.NP === refe.NP; });
					if (rw > -1) {
						tab2.row(rw).remove();
					}
				}
				task.MaterialTools[pos].delete = true;
			}
		} else {
			if (refe.Cantidad > 0 || refe.MAR) {
				task.MaterialTools.push(refe);
				if (refe.Type === 1) {
					tab1.row.add(refe);
				} else {
					tab2.row.add(refe);
				}
			}
		}
	}
	if (refe.Type === 1) {
		tab1.columns.adjust().draw();
	} else {
		tab2.columns.adjust().draw();
	}
	$(`#qty${refe.Type}, #np${refe.Type}`).val('');
	$(`#des${refe.Type}`).text('');
	$(`#qty${refe.Type}`).focus();
}
function addI() {
	let instruccion = new Instruction();
	instruccion.No = parseInt(document.getElementById('No').value) || 0;
	if (instruccion.No <= 0) {
		return false;
		MsjBox('Instruccion Incompleta', 'Falta el Numero de Instruccion');
	}
	instruccion.Title = document.getElementById('tit').value.trim();
	instruccion.Contenido = CKEDITOR.instances['ckeIns'].getData();
	instruccion.Tecnico = $('#tec').prop('checked');
	instruccion.Inspector = $('#ins').prop('checked');

	CKEDITOR.instances['ckeIns'].setData('');
	document.getElementById('No').value = '0';
	document.getElementById('tit').value = '';
	$('#tec, #ins').prop('checked', false);

	if (instruccion.Contenido !== '') {
		let pos = task.Instructions.findIndex(elem => elem.No === instruccion.No);
		if (pos > -1) {
			let rw = tab3.data().toArray().findIndex((elem) => { return elem.No === instruccion.No; });
			if (rw === -1) {
				Mensaje('Instruccion Perdida', 'No se Encuentra la Instruccino, Recargue la pagina.')
				return false;
			}
			tab3.row(rw).data(instruccion);
			task.Instructions[pos] = ClonObject(instruccion);
		} else {
			task.Instructions.push(instruccion);
			tab3.row.add(instruccion);
		}
	}
	tab3.columns.adjust().draw();
	$(`#tit`).focus();
	edI = -1;
}
function addR() {
	let refe = new Reference();
	refe.Read();
	if (refe.Reference !== '') {
		let pos = task.ReferenceS.findIndex(elem => elem.Reference === refe.Reference);
		if (pos > -1) {
			let rw = tab4.data().toArray().findIndex((elem) => { return elem.Reference === refe.Reference; });
			if (rw === -1) {
				Mensaje('Referencia Repetida', 'La Referencia ya ha sido Agregada.')
				return false;
			}
			tab4.row(rw).data(refe);
			task.ReferenceS[pos] = ClonObject(refe);
		} else {
			task.ReferenceS.push(refe);
			tab4.row.add(refe);
		}
	}
	tab4.columns.adjust().draw();
	$(`#Reference, #Designation`).val('');
	$(`#Reference`).focus();
}
function addIMG() {
	let tImg = new TImage();
	tImg.No = edIG === -1 ? task.Images.length : edIG;
	tImg.FileName = $('#imageTU')[0].files[0] !== undefined ? $('#imageTU')[0].files[0].name : '';
	tImg.Titulo = document.getElementById('Titulo').value;
	tImg.File = document.getElementById("imageTU").files.length > 0 ? document.getElementById("imageTU").files[0] : null;
	tImg.src = document.getElementById("preview").src;
	if (tImg.FileName !== '' && tImg.Titulo !== '') {
		let pos = task.Images.findIndex(elem => elem.No === tImg.No);
		if (pos > -1) {
			let rw = tab5.data().toArray().findIndex((elem) => { return elem.No === tImg.No; });
			if (rw === -1) {
				Mensaje('Imagen Perdida', 'No se Encuentra la Imagen, Recargue la pagina.');
				return false;
			}
			tab5.row(rw).data(tImg);
			task.Images[pos].FileName = tImg.FileName;
			task.Images[pos].Titulo = tImg.Titulo;
			task.Images[pos].src = tImg.src;
			if (tImg.File !== null) {
				if (tImg.File.lastModified !== task.Images[pos].File.lastModified || tImg.File.name !== task.Images[pos].File.name || tImg.File.size !== task.Images[pos].File.size || tImg.File.type !== task.Images[pos].File.type) {
					task.Images[pos].File = tImg.File;
				}
			}
		} else {
			task.Images.push(tImg);
			tab5.row.add(tImg);
		}
	}
	tab5.columns.adjust().draw();
	document.getElementById('Titulo').value = '';
	document.getElementById("imageTU").value = '';
	document.getElementById("preview").src = '';
	document.getElementById("preview").alt = '';
	document.getElementById("preview").title = '';
	edIG = -1;
}
function mosCom() {
	let comp = ComponenteMenor.InstanceCadena(this.value, this.id === 'np' ? 1 : 2);
	if (comp.Valid) {
		task.ComponenteMenor.SetValores(comp);
		task.idComponente = comp.idComponente;
		$('#np').val(comp.np);
		$('#descripcion').val(comp.descripcion);
		$('#ata').text(comp.ata);
		$('#idModelo').text('');
		$('#idModelo').append($('<option value="" selected>Seleccionar...</option>'));
		setModelos(comp.GetModelos())
	}
}
function setModelos(models) {
	var mp = getPar('mod');
	models.forEach((mod) => {
		let m = mod.Modelo.Nombre;
		let sel = '';
		if (mp !== null && mp !== undefined) {
			if (m === mp) {
				sel = 'selected';
				task.idModelo = mp;
			}
		}
		$('#idModelo').append($(`<option value="${m}" ${sel}>${m}</option>`));
	});
}
function Clear() {
	//task.Clear();
	$('#btnPrev').prop('disabled', true);

	$('#idModelo').text('');
	$('input').val('');
	$('#instCont').text('');
	CKEDITOR.instances['ckeIns'].setData('');
	edI = -1;
	edIG = -1;
	edMT = null;
	tab1.clear();
	tab2.clear();
	tab3.clear();
	tab4.clear();
	tab5.clear();
	tab1.columns.adjust().draw();
	tab2.columns.adjust().draw();
	tab3.columns.adjust().draw();
	tab4.columns.adjust().draw();
	tab5.columns.adjust().draw();
	$('input[type="checkbox"]').prop('checked', false);
	document.getElementById('preview').classList.add('d-none');
	document.getElementById('preview').src = '';
	document.getElementById('preview').alt = '';
	document.getElementById('preview').title = '';
}
function InvNP() {
	$(`#des${ori}`).text('');
	var datos = { op: 1, par: this.value };
	var ori = this.id === 'np1' ? 1 : 2;
	let des = `http://192.168.1.26/@consultas/alm.cshtml`;
	if (this.value !== '') {
		$(`<div class="progress"><div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div></div>`).appendTo(`#des${ori}`);
		$.ajax({
			url: des,
			async: false,
			data: datos,
			type: "GET",
			dataType: "jsonp",
			success: function (dat) {
				if (dat.articulo.length > 0) {
					$(`#np${ori}`).val(dat.articulo[0].parte);
					$(`#des${ori}`).text(dat.articulo[0].descrip);
				} else {
					$(`#qty${ori}`).val('');
					$(`#np${ori}`).val('');
					$(`#des${ori}`).html('<span class="text-danger">El Numero de Parte no esta dado de alta en el Almacen</span>');
					//Mensaje('No Record', '<b>El Numero de Parte no esta dado de alta en el Almacen<br>Favor de verificarlo.</b>');
				}
			},
			error: mostrarError
		});
	}
}

const tablas = (e) => {
	tab1 = $('#tab1').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{
				data: null,
				render: (data, type, row) => {
					return data.MAR ? 'AR' : data.Cantidad > 0 ? data.Cantidad : 0;
				}
			},
			{ data: 'NP' },
			{ data: 'Descripcion' },
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-right' },
			{ targets: [1, 2], className: 'text-left' },
			{
				targets: [3],
				orderable: false,
				className: 'text-center',
				defaultContent: `<div class="btn-group btn-group-sm w-100" role="group" aria-label="Opciones">
  <button tipo="editar" type="button" class="btn btn btn-sm btn-success"><i class="fas fa-pen-square"></i></button>
  <button tipo="eliminar" type="button" class="btn btn-sm btn-danger"><i class="fas fa-trash-alt"></i></button>
</div>`
			}
		],
		drawCallback: function (settings) {
			$(`#tab1 button[tipo="editar"]`).click((btn) => {
				let row = tab1.row($(btn.target).parents('tr'));
				var data = row.data();
				$(`#np${data.Type}`).val(data.NP);
				$(`#des${data.Type}`).text(data.Descripcion);
				$(`#qty${data.Type}`).val(data.Cantidad);
				$('#MAR').prop('checked', data.MAR);
			});
			$(`#tab1 button[tipo="eliminar"]`).click((btn) => {
				let row = tab1.row($(btn.target).parents('tr'));
				let data = row.data();
				let pos = task.MaterialTools.findIndex(elem => elem.NP === data.NP);
				if (pos > -1) {
					task.MaterialTools[pos].delete = true;
					tab1.row(row).remove();
					tab1.columns.adjust().draw();
				}
			});
		}
	});
	tab2 = $('#tab2').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{ data: 'Cantidad' },
			{ data: 'NP' },
			{ data: 'Descripcion' },
			{ data: null }
		],
		columnDefs: [
			{ targets: [0], className: 'text-right' },
			{ targets: [1, 2], className: 'text-left' },
			{
				targets: [3],
				orderable: false,
				className: 'text-center',
				defaultContent: `<div class="btn-group btn-group-sm w-100" role="group" aria-label="Opciones">
  <button tipo="editar" type="button" class="btn btn btn-sm btn-success"><i class="fas fa-pen-square"></i></button>
  <button tipo="eliminar" type="button" class="btn btn-sm btn-danger"><i class="fas fa-trash-alt"></i></button>
</div>`
			}
		],
		drawCallback: function (settings) {
			$('#tab2 button[tipo="editar"]').click((btn) => {
				let row = tab2.row($(btn.target).parents('tr'));
				var data = row.data();
				$(`#np${data.Type}`).val(data.NP);
				$(`#des${data.Type}`).text(data.Descripcion);
				$(`#qty${data.Type}`).val(data.Cantidad);
			});
			$(`#tab2 button[tipo="eliminar"]`).click((btn) => {
				let row = tab2.row($(btn.target).parents('tr'));
				let data = row.data();
				let pos = task.MaterialTools.findIndex(elem => elem.NP === data.NP);
				if (pos > -1) {
					task.MaterialTools[pos].delete = true;
					tab2.row(row).remove();
					tab2.columns.adjust().draw();
				}
			});
		}
	});
	tab3 = $('#tab3').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{ data: 'No', visible: true, orderData: [0] },
			{ data: 'Contenido', width: '90%', orderable: false },
			{
				data: null,
				orderable: false,
				render: (data, type, row) => {
					let des = `<p>Tecnico: ${(data.Tecnico ? '<i class="fas fa-check-circle"></i>' : '<i class="fas fa-times-circle"></i>')}</p>
<p>Inspector: ${(data.Inspector ? '<i class="fas fa-check-circle"></i>' : '<i class="fas fa-times-circle"></i>')}</p>
<div class="btn-group btn-group-sm w-100" role="group" aria-label="Opciones">
  <button tipo="editar" type="button" class="btn btn btn-sm btn-success"><i class="fas fa-pen-square"></i></button>
  <button tipo="eliminar" type="button" class="btn btn-sm btn-danger"><i class="fas fa-trash-alt"></i></button>
</div>`;
					return des;
				}
			}
		],
		columnDefs: [
			{ target: [0] },
			{ targets: [1], className: 'text-left' },
			{ targets: [2], className: 'text-center' }
		],
		drawCallback: function (settings) {
			$('#tab3 button[tipo="editar"]').click((btn) => {
				let rw = tab3.row($(btn.target).parents('tr'));
				var data = rw.data();
				$('#No').val(data.No);
				document.getElementById('tit').value = data.Title;
				CKEDITOR.instances['ckeIns'].setData(data.Contenido);
				$('#tec').prop('checked', data.Tecnico);
				$('#ins').prop('checked', data.Inspector);
				edI = data.No;
			});
			$(`#tab3 button[tipo="eliminar"]`).click((btn) => {
				let row = tab3.row($(btn.target).parents('tr'));
				let data = row.data();
				let pos = task.Instructions.findIndex(elem => elem.No == data.No);
				if (pos > -1) {
					task.Instructions[pos].delete = true;
					tab3.row(row).remove();
					tab3.columns.adjust().draw();
				}
			});
		}
	});
	tab4 = $('#tab4').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{ data: 'Reference' },
			{ data: 'Designation' },
			{ data: null }
		],
		columnDefs: [
			{ targets: '_all', className: 'text-left' },
			{
				targets: [2],
				className: 'text-center',
				orderable: false,
				defaultContent: `<div class="btn-group btn-group-sm w-100" role="group" aria-label="Opciones">
  <button tipo="editar" type="button" class="btn btn btn-sm btn-success"><i class="fas fa-pen-square"></i></button>
  <button tipo="eliminar" type="button" class="btn btn-sm btn-danger"><i class="fas fa-trash-alt"></i></button>
</div>`
			}
		],
		drawCallback: function (settings) {
			$('#tab4 button[tipo="editar"]').click((btn) => {
				let rw = tab4.row($(btn.target).parents('tr'));
				var data = rw.data();
				$(`#Reference`).val(data.Reference);
				$(`#Designation`).val(data.Designation);
			});
			$(`#tab4 button[tipo="eliminar"]`).click((btn) => {
				let row = tab4.row($(btn.target).parents('tr'));
				let data = row.data();
				let pos = task.ReferenceS.findIndex(elem => elem.Reference === data.Reference);
				if (pos > -1) {
					task.ReferenceS[pos].delete = true;
					tab4.row(row).remove();
					tab4.columns.adjust().draw();
				}
			});
		}
	});
	tab5 = $('#tab5').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/Spanish.json` },
		dom: "<'row'<'col-12 col-md-4'l><'col-12 col-md-8'f>><'row'<'col'tr>><'row'<'col'p>>",
		data: [],
		columns: [
			{ data: 'No', visible: false, orderData: [0] },
			{ data: 'Titulo' },
			{
				data: null,
				render: (data, type, row) => {
					let src = data.src === undefined ? `${window.location.origin}/Archivo/task/imagenes/${data.FileName}` : data.src;
					let res = `<img src="${src}" alt="${data.Titulo}" class="img-thumbnail" title="${data.Titulo}" style="height:200px;width:200px;">`;
					return res;
				}
			},
			{ data: null }
		],
		columnDefs: [
			{ targets: '_all', className: 'text-center' },
			{ targets: [2, 3], className: 'text-center', orderable: false },
			{
				targets: [3],
				defaultContent: `<div class="btn-group btn-group-sm w-100" role="group" aria-label="Opciones">
  <button tipo="editar" type="button" class="btn btn btn-sm btn-success"><i class="fas fa-pen-square"></i></button>
  <button tipo="eliminar" type="button" class="btn btn-sm btn-danger"><i class="fas fa-trash-alt"></i></button>
</div>`
			}
		],
		drawCallback: function (settings) {
			$('#tab5 button[tipo="editar"]').click((btn) => {
				let rw = tab5.row($(btn.target).parents('tr'));
				var data = rw.data();
				$(`#Titulo`).val(data.Titulo);
				edIG = data.No;
				let pos = task.Images.findIndex(elem => elem.No === edIG);
				document.getElementById('preview').src = data.src;
				document.getElementById('preview').classList.remove('d-none');
			});
			$(`#tab5 button[tipo="eliminar"]`).click((btn) => {
				let row = tab5.row($(btn.target).parents('tr'));
				let data = row.data();
				let pos = task.Images.findIndex(elem => elem.No === data.No);
				if (pos > -1) {
					task.Images[pos].delete = true;
					tab5.row(row).remove();
					tab5.columns.adjust().draw();
				}
			});
		}
	});
}