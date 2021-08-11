class button{
	constructor(id = 'button1', icon = '', content = '', css = '', callback = '', column_width = 3, dismiss_modal = false){
		this.id = id;
		this.icon = icon;
		this.content = content;
		this.css = css;
		this.callback = callback;
		this.column_width = column_width;
		this.dismiss_modal = dismiss_modal;
	}
}
class content {
	constructor(id = '', content='',css='',icon='', buttons) {
		this.id = id;
		this.content = content;
		this.css = css;
		this.icon = icon;
		this.buttons = buttons;
	}
}
class modal {
	constructor(id = 'button1', size = '', close_button = '', title, body, footer) {
		this.id = id;
		this.size = size;
		this.close_button = close_button;
		this.title = title;
		this.body = body;
		this.footer = footer;
	}
}

function mmodal(titulo, mensaje, botones, fBtn1, fBtn2, fCancel) {
	let btnOk = new button('btnOk', '', 'Ok', 'btn-sm btn-block btn-success', (fBtn1 !== undefined ? 'fBtn1' : ''), 4, true)
};
	let botones = botones === undefined ? [new button()]
	var configMsj = {
		id: 'MensajeModal',
		size: '',
		close_button: true,
		title: {
			id: 'mensajeModalTitle',
			content: 'Titulo',
			css: '',
			icon: ''
		},
		body: {
			id: 'mensajeModalBody',
			content: '',
			css: ''
		},
		footer: {
			id: 'mensajeModalFooter',
			content: '',
			css: '',
			buttons: [
				{
					id: '',
					icon: '',
					content: '',
					css: '',
					callback: '',
					column_width: 1 - 12,
					dismiss_modal: false
				}
			]
		}
	};
	var a = '';
	var f = botones === undefined ? `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-success" ${(fBtn1 !== undefined ? `onclick="fBtn1()"` : '')} data-dismiss="modal">Ok</button></div>` : botones;
	var tc = '';
	var size = '';
if (titulo === undefined && mensaje === undefined) {
	var cTitle = new content('mensajeModalTitle', 'Iniciar Sesion', '', 'fas fa-key');
	var cBody = new content('mensajeModalBody');
	cBody.content = `<div class="card-body">
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
	var cFooter = new content('mensajeModalFooter',)
		f = `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-success" onclick="login()">Iniciar</button></div>`;
		f += `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-danger" onclick="location.href = location.origin;" data-dismiss="modal">Cancelar</button></div>`;
		tc = 'bg-dark w-100 pl-3 rounded text-white font-weight-bold';
		//size = 'lg';
		botones = null;
	}
	if (typeof (botones) === 'number') {
		switch (botones) {
			case 1:     //  OK, CANCELAR
				f = `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-success" ${(fBtn1 !== undefined ? `onclick="fBtn1()"` : '')}>Ok</button></div>`;
				f += `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-danger" ${(fBtn2 !== undefined ? `onclick="fBtn2()"` : '')} data-dismiss="modal">Cancelar</button></div>`;
				tc = 'bg-secondary w-100 pl-3 rounded text-white font-weight-bold';
				break;
			case 2:     //  SI, NO
				f = `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-primary" ${(fBtn1 !== undefined ? `onclick="fBtn1()"` : '')}>Si</button></div>`;
				f += `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-secondary" ${(fBtn2 !== undefined ? `onclick="fBtn2()"` : '')} data-dismiss="modal">No</button></div>`;
				tc = 'bg-secondary w-100 pl-3 rounded text-white font-weight-bold';
				break;
			case 3:     //  Enviar Error a Desarrollador, Cancelar
				titulo = `<i class="fas fa-exclamation-triangle"></i> ${titulo}`;
				f = `<div class="col-5"><button type="button" class="btn btn-sm btn-block btn-warning" onclick="EnviarError("${mensaje}")>Enviar Error a Desarrollador</button></div>`;
				f += `<div class="col-4"><button type="button" class="btn btn-sm btn-block btn-dark" data-dismiss="modal">Cancelar</button></div>`;
				tc = 'bg-danger w-100 pl-3 rounded text-white font-weight-bold';
				size = 'xl';
				break;
		}
	}
	if (document.getElementById('MensajeModal') === null) {
		var datos = JSON.stringify({ o: 1, t: titulo, tcs: tc, b: '', f: f, s: size });
		$.ajax({
			async: false,
			url: `${window.location.origin}/appRest/asHtml?j=${datos.replaceAll('<', "[|")}`,
			success: (x) => {
				a = x;
			},
			error: (w, x, y, z) => {
				console.log(w);
			}
		});
		$('body').append($(a));
		document.getElementById('MensajeModalBody').innerHTML = mensaje;
	} else {
		document.getElementById('MensajeModalTitle').innerHTML = titulo;
		document.getElementById('MensajeModalBody').innerHTML = mensaje;
		document.getElementById('MensajeModalFooter').innerHTML = f;
		document.getElementById('MensajeModalTitle').classList.value = `modal-title ${tc}`;
		document.getElementsByClassName('modal-dialog modal-dialog-centered')[0].classList.value = `modal-dialog modal-dialog-centered ${(size !== '' ? `modal-${size}` : '')}`;
	}
	if (fCancel !== undefined) {
		$('#MensajeModal').on('hidden.bs.modal', fCancel);
	}
	$('#MensajeModal').modal('show');
}