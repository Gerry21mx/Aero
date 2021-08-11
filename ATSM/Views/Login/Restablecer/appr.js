document.addEventListener('DOMContentLoaded', (e) => {
	document.getElementById('res').addEventListener('click', (e) => {
		//let nk = document.getElementById('Nickname').value;
		let nk = getPar('usuario');
		let p1 = document.getElementById('Password').value;
		let p2 = document.getElementById('Password2').value;
		let tk = getPar('token');
		if (p1 === p2)
			Usuario.reset(nk, tk, p1).then(data => {
				MsjBox('Restablecer Contraseña', data.Error !== '' ? data.Mensaje : data.Error);
				window.location.href = window.location.origin;
			});
		else {
			MsjBox('Restablecer Contraseña', 'El Password No Coincide.');
		}
	})
})