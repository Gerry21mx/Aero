document.addEventListener('DOMContentLoaded', () => {
	document.getElementById('login').addEventListener('click', login);
	document.getElementById('recovery').addEventListener('click', recovery);
	//document.querySelectorAll('input[type="text"]').forEach(elemento => {
	//	elemento.addEventListener('keypress', (key) => {
	//		if (key.originalEvent.charCode === 13 || key.originalEvent.key === 'Enter')
	//			login();
	//	});
	//});
});
document.body.addEventListener('focus', () => {
	usuario.isAuthenticated().then(logged => {
		if (logged) window.location.href = 'Home';
	});
});
function login() {
	procc();
	usuario.read();
	if (usuario.Nickname !== '' && usuario.Password !== '')
		usuario.logMeIn().then(res => {
			if (res !== null)
				if (res.logged) {
					let lurl = localStorage.getItem('lurl');
					location.href = lurl ?? 'Home';
				}
				else
					MsjBox('Iniciar Sesion', res.Message);
		});
	endPro();
}
function recovery() {
	Usuario.recovery($('#Nickname').val().trim()).then(data => {
		MsjBox('Recuperar Contraseña', data.Error === '' ? data.Mensaje : data.Error);
	});
}