class Usuario extends Base {
	constructor(idusuario = 0) {
		super('IdUsuario');
		this.IdUsuario = idusuario;
		this.Valid = false;
		this.getInstance();
	}
	byNickName(nickname) {
		this.getInstance({ action: 'byNickName', value: nickname });
	}
	air(rolName) {
		var res = false;
		$.ajax({
			async: false,
			url: `${this._apiElement}/Air`,
			dataType: 'json',
			method: 'POST',
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify(rolName),
			success: (response) => {
				res = response;
			},
			error: (jqXHR, textStatus, errorThrown) => {
				console.error(`Usuario air Error: ${jqXHR.responseText}`);
			}
		});
		return res;
	}
	async inRol(rolName) {
		let datos = { IdUsuario: this.IdUsuario, NickName: this.NickName, RolName: rolName };
		let res = await this.postAction('InRole', datos);
		return res;
	}
	async isAuthenticated() {
		let res = false;
		await this.getAction('IsAuthenticated').then(data => {
			if (data)
				res = data.Data;
		});
		return res;
	}
	async logMeIn() {
		var res = { Valid: false, Mensaje: 'No se Ha podido Iniciar Sesion' };
		if (this.Nickname !== '' && this.Password !== '') {
			let data = await this.postAction('LogMeIn', { NickName: this.Nickname, Password: this.Password });
			if (data !== null) {
				res = data.Data;
				if (res.logged) {
					this.setValores(res.user);
				}
			}
		}
		return res;
	}
	async logOff() {
		let data = await this.getAction('LogOff');
		if (data.Valid) {
			location.href = location.origin;
		} else {
			MsjBox('Cerrar Sesion', data.Error);
		}
	}
	async getFirmado() {
		let data = await this.getAction('ufir');
		let res = false;
		if (data !== null) {
			if (data.Data !== null) {
				this.setValores(data.Data);
				res = true;
			}
		}
		return res;
	}
	static async getUsuarios(area) {
		area = area === undefined ? null : area === '' ? null : area;
		var res = [];
		let data = await getAction(`${this.Api + this.name}?area=${(area === null ? '' : area)}`);
		if (data.Data !== null) {
			res = data.Data;
		}
		return res;
	}
	static async recovery(nickName) {
		let res = await fetch(`${this.Api + this.name}/${nickName}/Recovery`);
		if (res.ok) {
			let post = await res.json();
			return post;
		} else {
			console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
			return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
		}
	}
	static async reset(nickName = '', token = '', password = '') {
		var res = { Valid: false, Error: 'No ha podido procesarse la recuperacion de Contraseña.' };
		let data = await postAction(`${this.Api + this.name}/ResetPsw`, { usu: nickName, token: token, psw: password });
		if (data !== null) {
			res = data;
		}
		return res;
	}
}