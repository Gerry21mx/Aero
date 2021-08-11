class Base {
	static get Api() { return `${window.location.origin}/api/`; }
	constructor(idClass) {
		Object.defineProperty(this, '_api', { configurable: false, get: () => { return `${window.location.origin}/api/`; }, set: () => { return false; } });
		Object.defineProperty(this, '_apiElement', { configurable: false, get: () => { return `${this._api + this.constructor.name}`; }, set: () => { return false; } });
		Object.defineProperty(this, '_IdClass', { get: () => { return idClass; }, set: () => { return false; } });
		this.clear();
	}
	getInstance(iApi) {
		procc();
		var url = `${this._apiElement}/${this[this._IdClass]}`;
		if (iApi !== undefined) {
			url = `${this._apiElement}/${(iApi.value + (iApi.action === '' ? '' : `/${iApi.action}`))}`;

		}
		var _this = this;
		$.ajax({
			async: false,
			url: url,
			dataType: 'json',
			method: 'GET',
			success: function (data) {
				_this.setValores(data.Data);
				if (_this.onGetInstance !== undefined && _this.onGetInstance instanceof Function) {
					_this.onGetInstance();
				}
			},
			error: (x, y, z) => { console.error(`${_this.constructor.name}.GetInstance()`); }
		});
		endPro();
	}
	save() {
		procc();
		var res = { Valid: false, Mensaje: '', Error: this.constructor.name + '.Save - Sin Procesar (Base.JS)' };
		var _this = this;
		$.ajax({
			async: false,
			url: this._apiElement,
			dataType: 'json',
			method: 'POST',
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify(this),
			success: function (data) {
				if (data !== null) {
					res = data;
					if (data.Valid) {
						_this.setValores(data.Elemento);
						if (_this.onSave !== undefined && _this.onSave instanceof Function) {
							_this.onSave();
						}
					} else {
						console.log(`Save: ${data.Error}`);
					}
				}
			},
			error: (jqXHR, textStatus, errorThrown) => {
				console.error(`${_this.constructor.name}.Save()`);
				res.Error = `<b>${_this.constructor.name}:</b><br>${jqXHR.responseText}`;
			}
		});
		endPro();
		return res;
	}
	delete() {
		procc();
		var res = { Valid: false, Mensaje: '', Error: this.constructor.name + '.Delete - Sin Procesar (Base.JS)' };
		var _this = this;
		let url = `${this._apiElement}/${this[this._IdClass]}`;
		$.ajax({
			async: false,
			url: url,
			dataType: 'json',
			method: 'DELETE',
			contentType: "application/json; charset=utf-8",
			//data: JSON.stringify(this[this._IdClass]),
			data: JSON.stringify(this),
			success: function (data) {
				if (data !== null) {
					res = data;
					if (res.Valid) {
						_this.clear();
						if (_this.onDelete !== undefined && _this.onDelete instanceof Function) {
							_this.onDelete();
						}
					} else {
						throw Error(`Delete: ${res.Error}`);
					}
				}
			},
			error: function (x, y, z) {
				console.error(`${_this.constructor.name}.Delete()`);
			}
		});
		endPro();
		return res;
	}

	async getAction(action, value) {
		var res = null;
		try {
			res = await response(await fetch(`${this._apiElement}/${((value !== '' && value !== undefined && value !== null) ? `${value}/` : '')}${action}`));
		} catch (e) {
			console.log(`Error Get Fetch: ${e}`);
		}
		return res;
	}
	async postAction(action, data) {
		var res = null;
		try {
			res = await response(await fetch(`${this._apiElement}/${action}`, { method: 'POST', body: JSON.stringify(data), headers: { 'Content-Type': 'application/json' } }));
		} catch (e) {
			console.log(`Error Post Fetch: ${e}`);
		}
		return res;
	}

	clon() {
		return clonObject(this);
		if (this.onClon !== undefined && this.onClon instanceof Function) {
			this.onClon();
		}
	}
	setValores(objeto) {
		if (objeto === null || objeto === undefined) {
			this.clear();
			return false;
		}
		Object.keys(objeto).forEach(key => {
			this[key] = objeto[key];
		});
		if (this.onSetValores !== undefined && this.onSetValores instanceof Function) {
			this.onSetValores();
		}
	}
	clear() {
		clearData(this);
		this.Consecutivo = -1;
		if (this.onClear !== undefined && this.onClear instanceof Function) {
			this.onClear();
		}
	}
	read(prefijo = '', contenedor = null) {
		Object.keys(this).forEach((key, indice) => {
			let ss = `#${key}`;
			let sp = prefijo + key;
			sp = contenedor !== null ? `#${contenedor} [id="${sp}"]` : sp;
			let selector = document.querySelector(sp) ? sp : ss;
			var campo = document.querySelector(selector);
			if (campo) {
				if (campo.type === undefined && campo.tagName !== 'INPUT') {
					this[key] = campo.textContent.trim();
				}
				else {
					if (campo.type === 'checkbox') {
						this[key] = (typeof (this[key]) === 'boolean' || this[key] === null) ? campo.checked : this[key];
					}
					else {
						if (campo.type === 'radio') {
							if (document.querySelector(`input[name="${key}"]:checked`)) {
								let valRS = document.querySelector(`input[name="${key}"]:checked`).value
								if (campo.classList.contains('num'))
									valRS = parseInt(valRS) || 0;
								this[key] = (typeof (this[key]) === 'number' || this[key] === null) ? valRS : this[key];
							}
						} else {
							if (campo.classList.contains('num')) {
								this[key] = (typeof (this[key]) === 'number' || this[key] === null) ? (parseInt(accounting.unformat(campo.value)) || 0) : (campo.type === 'text' ? campo.value : this[key]);
							}
							else {
								if (campo.classList.contains('dec')) {
									this[key] = (typeof (this[key]) === 'number' || this[key] === null) ? (parseFloat(accounting.unformat(campo.value)) || 0) : this[key];
								}
								else {
									if (campo.classList.contains('mon')) {
										this[key] = (typeof (this[key]) === 'number' || this[key] === null) ? (parseFloat(accounting.unformat(campo.value)) || 0) : this[key];
									}
									else {
										this[key] = campo.value.trim();
									}
								}
							}
						}
					}
				}
			}
		});
		if (this.onRead !== undefined && this.onRead instanceof Function) {
			this.onRead();
		}
	}
	write(prefijo = '', contenedor = null) {
		Object.keys(this).forEach((key, indice) => {
			if (this[key]===null) { return; }
			let ss = `#${key}`;
			let sp = prefijo + key;
			sp = contenedor !== null ? `#${contenedor} [id="${sp}"]` : sp;
			let selector = document.querySelector(sp) ? sp : ss;
			var campo = document.querySelector(selector);
			if (campo) {
				switch (campo.type) {
					case 'checkbox':
						campo.checked = this[key];
						campo.dispatchEvent(new Event('change'));
						break;
					case 'radio':
						document.querySelectorAll(`input[name="${key}"]`).forEach(r => { r.checked = false })
						if (document.querySelector(`input[name="${key}"][value="${this[key]}"]`))
							document.querySelector(`input[name="${key}"][value="${this[key]}"]`).checked = true
						break;
					case 'date':
						campo.value = this[key] !== null ? moment(this[key]).format('YYYY-MM-DD') : '';
						break;
					default:
						if (campo.type === undefined && campo.tagName !== 'INPUT') {
							campo.textContent = this[key];
						} else {
							campo.value = this[key];
						}
						break;
				}
			}
		});
		if (this.onWrite !== undefined && this.onWrite instanceof Function) {
			this.onWrite();
		}
	}
	json() {
		return JSON.stringify(this);
	}
	async rAnswer(postResp) {
		if (postResp.ok) {
			let post = await postResp.json();
			return post.Data;
		} else {
			console.log(`Error: ${postResp.status} - ${postResp.statusText}`);
			return null;
		}
	}
}
function clearData(objeto) {
	Object.keys(objeto).forEach(key => {
		switch (typeof objeto[key]) {
			case "boolean":
				objeto[key] = false;
				break;
			case "number":
				objeto[key] = 0;
				break;
			case "string":
				objeto[key] = '';
				break;
			case "object":
				if (objeto[key] instanceof Array)
					objeto[key] = [];
				else if (objeto[key] instanceof Date)
					objeto[key] = null;
				else if (objeto[key] !== null)
					objeto[key] = clearData(objeto[key]);
				break;
		}
	}
	);
	return objeto;
}
async function getAction(url) {
	var res = null;
	try {
		res = await response(await fetch(url));
	} catch (e) {
		console.log(`Error Get Fetch: ${e}`);
	}
	return res;
}
async function postAction(url, data) {
	var res = null;
	try {
		res = await response(await fetch(url, { method: 'POST', body: JSON.stringify(data), headers: { 'Content-Type': 'application/json' } }));
	} catch (e) {
		console.log(`Error Post Fetch: ${e}`);
	}
	return res;
}
async function response(fetchResp) {
	if (fetchResp.ok) {
		let post = await fetchResp.json();
		return post;
	} else {
		console.log(`Error Fetched: ${fetchResp.status} - ${fetchResp.statusText}`);
		return { Valid: false, Mensaje: '', Error: `Error Fetched: ${fetchResp.status} - ${fetchResp.statusText}`, Data: null };
	}
}

function clonObject(objeto) {
	var temp = {};
	if (objeto instanceof Array) {
		temp = [];
		objeto.forEach(key => {
			temp.push(clonObject(key));
		});
	} else {
		if (objeto !== null && objeto !== undefined) {
			Object.keys(objeto).forEach(key => {
				if (typeof (objeto[key]) === 'object') {
					temp[key] = clonObject(objeto[key]);
				} else {
					temp[key] = objeto[key];
				}
			});
		} else {
			temp = null;
		}
	}
	return temp;
}
function assignValores(objetos) {
	if (!(objetos instanceof Array)) {
		objetos = [objetos];
	}
	var oResultado = {};
	objetos.forEach((objeto, indice, arreglo) => {
		Object.keys(objeto).forEach(key => {
			if (objeto[key] instanceof Array || typeof (objeto[key]) === 'object') {
				return false;
			}
			oResultado[key] = objeto[key];
		});
	});
	return oResultado;
}
function fileExist(ruta, archivo) {
	var res = { Status: false, Message: 'Consulta no procesada.', Data: null };
	$.ajax({
		async: false,
		url: `${window.location.origin}/api/System/FileExist`,
		dataType: 'json',
		data: { ruta: ruta, archivo: archivo },
		method: 'GET',
		success: function (data) {
			res = data;
		},
		error: function (x, y, z) {
			console.error(x.responseText);
		}
	});
	return res;
}