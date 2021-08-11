const usuario = new Usuario();
var cntIS = 3, conErr = 3;
document.addEventListener('DOMContentLoaded', () => {
	moment.locale('es');
	usuario.getFirmado().then(f => {
		if (!f && window.location.pathname !== '/Login' && window.location.pathname !== '/Login/Restablecer/Index')
			login();
	});
	document.querySelectorAll('input,textarea,select,button').forEach(element => {
		element.addEventListener('keyup', (e) => {
			if (e.key === 'Enter' || e.keyCode === 13) {
				e.target.blur();
				let swfs = false;
				for (var de of document.querySelectorAll('input,textarea,select,button')) {
					if (swfs) {
						de.focus();
						return;
					}
					if (de === e.target)
						swfs = true;
				}
			}
		});
	});
	document.querySelectorAll('.dec, .mon, input[type="number"]').forEach(elemento => {
		elemento.addEventListener('keypress', (e) => {
			if ('-0123456789.'.indexOf(e.key) === -1) {
				e.preventDefault();
				return false;
			}
		});
		elemento.addEventListener('blur', (e) => {
			e.target.value = e.target.value === '' ? 0 : e.target.value;
		});
		elemento.value = 0;
	});
	document.querySelectorAll('input,textarea').forEach(elem => {
		elem.addEventListener('click', (e) => { e.target.select(); });
		elem.addEventListener('focus', (e) => { e.target.select(); });
	});
	changeSwitch();
	if (document.querySelector('#logout'))
		document.querySelector('#logout').addEventListener('click', () => { usuario.logOff(); });
	//		MENU
	if (document.getElementById('main_navbar'))
		$('#main_navbar').bootnavbar();
	tooltips();
});
const tooltips = (e) => {
	let tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
	let tooltipList = tooltipTriggerList.map((tooltipTriggerEl) => {
		return new bootstrap.Tooltip(tooltipTriggerEl);
	});
}
const changeSwitch = (e) => {
	document.querySelectorAll('input.form-check-input[type="checkbox"]').forEach(elem => {
		elem.addEventListener('change', (e) => {
			if (e.target.dataset.fija !== "true")
				e.target.parentElement.querySelector('label').innerText = e.target.checked ? e.target.dataset.activo : e.target.dataset.inactivo;
		});
	});
}
document.addEventListener('focus', () => {
	if (window.location.pathname !== '/Login' && window.location.pathname !== '/Login/Restablecer/Index')
		usuario.isAuthenticated().then((aut) => {
			if (aut && document.getElementById('oRecUl')) {
				let modal = bootstrap.Modal.getInstance(document.getElementById('oRecUl'));
				modal.hide();
				document.getElementById('oRecUl').remove();
			}
			if (!aut && document.getElementById('oRecUl') === null) {
				login();
			}
		});
});
///		Usuario
function login(idUsP) {
	let modalLogin;
	localStorage.setItem('lurl', window.location);
	if (document.getElementById('oRecUl'))
		modalLogin = bootstrap.Modal.getInstance(document.getElementById('oRecUl'));
	else {
		let dm = document.createElement('div');
		dm.classList.add('modal', 'fade');
		dm.id = 'oRecUl';
		dm.tabIndex = '-1';
		dm.setAttribute('role', 'dialog');
		dm.setAttribute('aria-labelledby', 'Iniciar Sesion');
		dm.setAttribute('aria-hidden', 'true');
		dm.innerHTML = `<div class="modal-dialog" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<h5 class="modal-title" id="oRecUlLabel">Iniciar Sesion.</h5>
					</div>
					<div class="modal-body text-center">
						<p>La sesion ha finalizado por tiempo de Inactividad, Por favor vuelva a iniciar sesion para continuar.</p>
						<div class="row justify-content-center mt-4">
							<div class="col-12 col-md-7">
								<div class="input-group input-group-sm mb-3">
									<div class="input-group-prepend">
										<span class="input-group-text" id="baUsu"><i class="fas fa-user"></i></span>
									</div>
									<input id="fisUsu" name="fisUsu" type="text" class="form-control form-control-sm" placeholder="Username" aria-label="Username" aria-describedby="baUsu" autofocus>
								</div>
							</div>
						</div>
						<div class="row justify-content-center">
							<div class="col-12 col-md-7">
								<div class="input-group input-group-sm mb-3">
									<div class="input-group-prepend">
										<span class="input-group-text" id="baPsw"><i class="fas fa-key"></i></span>
									</div>
									<input id="fisPsw" name="fisPsw" type="password" class="form-control form-control-sm" placeholder="Password" aria-label="Password" aria-describedby="baPsw" autofocus>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer align-content-center">
						<div class="d-grid col-4 gap-2 d-md-block">
							<button class="btn btn-sm btn-primary" id="iniSU">Iniciar</button>
							<button class="btn btn-sm btn-danger" id="canSU" data-bs-dismiss="modal">Cancelar</button>
						</div>
					</div>
				</div>
			</div>`;
		odm = document.body.appendChild(dm);
		modalLogin = new bootstrap.Modal(odm, { backdrop: 'static', keyboard: false });
	}
	document.getElementById('oRecUl').addEventListener('hidden.bs.modal', (e) => {
		usuario.isAuthenticated().then(autenticado => {
			if (!autenticado)
				window.location.replace(`${window.location.origin}/Login`);
		});
	});
	document.querySelector('#iniSU').addEventListener('click', ()=> {
		conErr--;
		usuario.Nickname = document.getElementById('fisUsu').value;
		usuario.Password = document.getElementById('fisPsw').value;
		usuario.logMeIn().then(data => {
			if (data != null) {
				if (data.logged) {
					modalLogin.hide();
					conErr = 3;
				} else {
					MsjBox('Acceso Denegado', `Usuario o Password erroneo.!<br>Quedan ${conErr} Intentos.`);
					if (conErr == 0)
						window.location.replace(`${window.location.origin}/Login`);
				}
			}
		});
		$('#canSU').click(function () {
			window.location.replace(`${window.location.origin}/Login`);
		});
	});
	modalLogin.show();
}
//		Mensajes
function MsjBox(titulo, mensaje, botones, fBtn1, fBtn2, fCancel) {
	var a = '';
	var f = botones === undefined ? `<div class="col-4 d-grid"><button id="fBtn1" type="button" class="btn btn-sm btn-success" data-bs-dismiss="modal">Ok</button></div>` : botones;
	var tc = '';
	var size = '';
	if (titulo === undefined && mensaje === undefined) {
		titulo = 'Iniciar Sesion';
		mensaje = `<div class="card-body">
						<div class="input-group form-group">
							<div class="input-group-prepend">
								<span class="input-group-text"><i class="fas fa-user"></i></span>
							</div>
							<input type="text" class="form-control" placeholder="Usuario" id="usuVe" name="usuVe">
						</div>
						<div class="input-group form-group">
							<div class="input-group-prepend">
								<span class="input-group-text"><i class="fas fa-key"></i></span>
							</div>
							<input type="password" class="form-control" placeholder="Contraseña" id="pswVe" name="pswVe" onkeypress="function (this) { 
	let evt = obj.onkeypress.arguments[0];
	if (evt.charCode === 13 || evt.key === 'Enter') {
		login();
	}
}">
						</div>
					</div>`;
		f = `<div class="col-4 d-grid"><button type="button" class="btn btn-sm btn-success" onclick="login()">Iniciar</button></div>
			<div class="col-4 d-grid"><button type="button" class="btn btn-sm btn-danger" onclick="location.href = location.origin;" data-bs-dismiss="modal">Cancelar</button></div>`;
		tc = 'bg-dark w-100 pl-3 rounded text-white font-weight-bold';
		botones = null;
	}
	if (typeof (botones) === 'number') {
		switch (botones) {
			case 1:     //  OK, CANCELAR
				f = `<div class="col-4 d-grid"><button id="fBtn1" type="button" class="btn btn-sm btn-success" data-bs-dismiss="modal">Ok</button></div>
					<div class="col-4 d-grid"><button id="fBtn2" type="button" class="btn btn-sm btn-danger" data-bs-dismiss="modal">Cancelar</button></div>`;
				tc = 'bg-secondary w-100 pl-3 rounded text-white font-weight-bold';
				break;
			case 2:     //  SI, NO
				f = `<div class="col-4 d-grid"><button id="fBtn1" type="button" class="btn btn-sm btn-success" data-bs-dismiss="modal">Si</button></div>
					<div class="col-4 d-grid"><button id="fBtn2" type="button" class="btn btn-sm btn-danger" data-bs-dismiss="modal">No</button></div>`;
				tc = 'bg-secondary w-100 pl-3 rounded text-white font-weight-bold';
				break;
			case 3:     //  Enviar Error a Desarrollador, Cancelar
				titulo = `<i class="fas fa-exclamation-triangle"></i> ${titulo}`;
				f = `<div class="col-5 d-grid"><button type="button" class="btn btn-sm btn-warning" onclick="EnviarError("${mensaje}") data-bs-dismiss="modal">Enviar Error a Desarrollador</button></div>
					<div class="col-4 d-grid"><button type="button" class="btn btn-sm btn-dark" data-bs-dismiss="modal">Cancelar</button></div>`;
				tc = 'bg-danger w-100 pl-3 rounded text-white font-weight-bold';
				size = 'modal-lg';
				break;
		}
	}
	let eModal = document.getElementById('MensajeModal');
	let modalMensaje;

	if (eModal) {
		document.getElementById('MensajeModalTitle').innerHTML = titulo;
		document.getElementById('MensajeModalBody').innerHTML = mensaje;
		document.getElementById('MensajeModalFooter').innerHTML = f;
		document.getElementById('MensajeModalTitle').classList.value = `modal-title ${tc}`;
		document.getElementsByClassName('modal-dialog modal-dialog-centered')[0].classList.value = `modal-dialog modal-dialog-centered ${(size !== '' ? `modal-${size}` : '')}`;
		modalMensaje = bootstrap.Modal.getInstance(eModal);
	}
	else {
		let dm = document.createElement('div');
		dm.classList.add('modal', 'fade');
		dm.id = 'MensajeModal';
		dm.tabIndex = '-1';
		dm.setAttribute('role', 'dialog');
		dm.setAttribute('aria-labelledby', 'Mensaje');
		dm.setAttribute('aria-hidden', 'true');
		dm.innerHTML = `<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable ${size}">
		<div class="modal-content">
			<div class="modal-header">
				<h5 class="modal-title ${tc}" id="MensajeModalTitle">${titulo}</h5>
				<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
			</div>
			<div class="modal-body" id="MensajeModalBody">${mensaje}</div>
			<div class="modal-footer" id="MensajeModalFooter">${f}</div>
		</div>
	</div>`;
		eModal = document.body.appendChild(dm);
		modalMensaje = new bootstrap.Modal(eModal, { backdrop: false });
	}

	if (fBtn1 !== undefined)
		if (document.getElementById('fBtn1'))
			document.getElementById('fBtn1').addEventListener('click', fBtn1);
	if (fBtn2 !== undefined)
		if (document.getElementById('fBtn2'))
			document.getElementById('fBtn2').addEventListener('click', fBtn2);
	if (fCancel !== undefined)
		modalMensaje.addEventListener('hidden.bs.modal', fCancel);
	modalMensaje.show();
}
function EnviarError(error) {
	var datos = {
		tit: document.title,
		pag: document.location.toString(),
		err: error
	};
	$.ajax({
		url: `${window.location.origin}/includes/mail/error`,
		dataType: 'json',
		type: 'POST',
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify(datos),
		success: function (data) {
			if (data !== null) {
				console.log(`Error del Sistema al Enviar el Error: ${data.mensaje}`);
			}
		},
		error: MsjError
	});
}
function MsjError(jqXHR, status, error, origen) {
	if (error === 'NetworkError: A network error occurred.') {
		console.log('' + error);
		console.log('' + JSON.stringify(jqXHR));
		console.log('' + status);
		console.log('' + origen);
		return false;
	}
	MsjBox('Error en el Servidor', jqXHR.responseText + (origen !== undefined ? '<hr>' + origen : ''), 3);
	endPro();
}
///		Tablas
function loadTable(rfTabla, datos) {
	rfTabla.clear();
	rfTabla.rows.add(datos);
	//rfTabla.columns.adjust().draw();
	rfTabla.draw();
}
function updDT(rfTabla, item, key = null) {
	if (key === null) {
		key = 'Consecutivo';
		if (item.Consecutivo === -1 || item.Consecutivo === null || item.Consecutivo === undefined)
			item.Consecutivo = rfTabla.data().length;
	}
	let i = rfTabla.data().toArray().findIndex(i => i[key] === item[key]);
	if (i > -1) {
		rfTabla.row(i).data(clonObject(item));
	} else {
		rfTabla.row.add(clonObject(item));
	}
	//rfTabla.columns.adjust().draw();
	rfTabla.draw();
}
function delDT(rfTabla, key, value) {
	let i = rfTabla.data().toArray().findIndex(i => i[key] === value);
	if (i > -1) {
		rfTabla.row(i).remove();
		//rfTabla.columns.adjust().draw();
		rfTabla.draw();
	}
}
//		Funciones
function procc() {
	let mdlPro;
	if (document.getElementById('modpro'))
		mdlPro = bootstrap.Modal.getInstance(document.getElementById('modpro'));
	else {
		let dm = document.createElement('div');
		dm.classList.add('modal', 'text-center');
		dm.id = 'modpro';
		dm.tabIndex = '-1';
		dm.innerHTML = '<div class="modal-dialog modal-dialog-centered text-center" role="document"><div class="spinner-border text-success" role="status"><span class="sr-only"> Cargando...</span></div>&nbsp;&nbsp;&nbsp;Cargando...</div>';
		document.body.appendChild(dm);
		mdlPro = new bootstrap.Modal(document.getElementById('modpro'), {
			backdrop: 'static',
			keyboard: false
		});
	}
	mdlPro.show();
}
function endPro() {
	if (document.getElementById('modpro')) {
		let mdlPro = bootstrap.Modal.getInstance(document.getElementById('modpro'));
		if (mdlPro)
			mdlPro.hide();
		//else {
		//	document.getElementById('modpro').remove();
		//	document.querySelectorAll('.modal-backdrop').forEach(e => { e.remove(); });
		//}
	}
}
function getPar(key) {
	key = key.replace(/[\[]/, '\\[');
	key = key.replace(/[\]]/, '\\]');
	let pattern = "[\\?&]" + key + "=([^&#]*)";
	let regex = new RegExp(pattern);
	let url = unescape(window.location.href);
	let results = regex.exec(url);
	if (results === null) {
		return null;
	} else {
		return results[1];
	}
}