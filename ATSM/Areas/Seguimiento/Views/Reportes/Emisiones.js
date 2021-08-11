$(function () {
	$('#rep').click(reporte);
});
function reporte() {
	procc();
	let datos = {
		f1: $('#Desde').val(),
		f2: $('#Hasta').val(),
		t: $('#Operacion').val(),
	}
	$('#tabContainer').html('');
	if (datos.f1 !== '' && datos.f2 !== '' && datos.t !== '') {
		$.get(`${window.location.origin}/Seguimiento/Reportes/tERep`, datos, (html) => {
			$('#tabContainer').html(html);
			$('tr[id^="res_"]').click(mosOcuDes);
			endPro();
		});
	} else {
		endPro();
	}
}
function mosOcuDes() {

	var nf = parseInt(this.id.replace('res_', '')) || 0;
	if (nf > 0) {
		if ($('#des_' + nf).hasClass('d-block')) {
			$('#des_' + nf).removeClass('d-block');
			$('#des_' + nf).addClass('d-none');
			$(this).removeClass('bg-warning');
		} else {
			$('#des_' + nf).removeClass('d-none');
			$('#des_' + nf).addClass('d-block');
			$(this).addClass('bg-warning');
		}
	}
}