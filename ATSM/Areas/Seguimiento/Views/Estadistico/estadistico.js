var resumen, desglose, chartVuelos, chartHoras;
$(function () {
	$('#Desde').val('2020-01-01');
	$('#Hasta').val('2020-12-31');
	$('#vls').click(() => {
		let url = `${window.location.origin}/Seguimiento/Estadistico/Vuelos?a=${$('#IdAeronave').val()}&d=${$('#Desde').val()}&h=${$('#Hasta').val()}&c=${$('#IdCapacidad').val()}`;
		window.open(url);
	});
	$('#hrs').click(() => {
		let url = `${window.location.origin}/Seguimiento/Estadistico/Horas?a=${$('#IdAeronave').val()}&d=${$('#Desde').val()}&h=${$('#Hasta').val()}&c=${$('#IdCapacidad').val()}`;
		window.open(url);
	});
});
function gHoras() {
	var hm = [['Mes', 'Horas', { role: 'style' }, { role: 'annotation' }]];
	desglose.forEach((vlo, idx) => {
		let fec = moment(vlo.Salida);
		let m = fec.format('MMMM');
		let mv = moment(vlo.fdAterrizaje).diff(moment(vlo.fdDespegue), 'minutes');

		let ind = hm.findIndex(e => e[0] === m);
		if (ind > -1) {
			hm[ind][1] += mv / 60;
			hm[ind][3] = `${hm[ind][1]}`;
		} else {
			hm.push([m, (mv / 60), `color: ${gColor()}`, `${(mv / 60)}`]);
		}

	});
	$('#horas').tab('show');
	var data = google.visualization.arrayToDataTable(hm);
	var view = new google.visualization.DataView(data);
	view.setColumns([0, 1,
		{
			calc: "stringify",
			sourceColumn: 1,
			type: "string",
			role: "annotation"
		},
		2]);
	var options = {
		title: 'Horas Por Mes',
		is3D: true,
		bar: { groupWidth: "85%" },
		legend: { position: "none" }
	};
	chartHoras = new google.visualization.ColumnChart(document.getElementById('vMes'));
	chartHoras.draw(view, options);
	$('#slHrs').addClass('d-none');

}
function gColor() {
	var simbolos, color;
	simbolos = "0123456789ABCDEF";
	color = "#";
	for (var i = 0; i < 6; i++) {
		color = color + simbolos[Math.floor(Math.random() * 16)];
	}
	return color;
}