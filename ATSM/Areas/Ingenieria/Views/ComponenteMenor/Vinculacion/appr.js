var vinculado, componente, cVinculado, titulo, tLim, tMod, tabla, comCompa;
document.addEventListener('DOMContentLoaded', () => {
	let id = parseInt(getPar('id')) || 0;
	componente = new ComponenteMenor(id);
	componente.onGetInstance = compatibles;
	cVinculado = new ComponenteMenor();
	vinculado = new ComponenteMenorVinculado();
	comCompa = [];
	if (componente.Valid) {
		document.getElementById('IdComponenteMenor').value = componente.Part;
		document.getElementById('desCom').innerText = componente.Description;
	}
	document.getElementById('IdComponenteMenor').addEventListener('blur', consulta)
	tablas()
});
const compatibles = () => {
	if (componente.Valid) {
		componente.compatibles().then(data => {
			comCompa = data.Data;
			loadTable(tabla, comCompa)
		});
	}
}
const consulta = (e) => {
	let part = e.target.value.trim();
	if (part === '') return;
	procc();
	componente.byCadena(part).then(r => {
		document.getElementById('desCom').innerText = componente.Description;
		endPro();
	});
}
const clear = () => {

}
const tablas = (e) => {
	tabla = $('#tVinculados').DataTable({
		processing: true,
		language: { url: `${window.location.origin}/Scripts/DataTables/Spanish.json` },
		dom: "<'row'<'col-4'l><'col-8'f>><'row'<'col'tr>><'row'<'col-12'p>>",
		data: [],
		columns: [
			{
				data: 'Part',
				width: '30%'
			},
			{
				data: 'Description',
				width: '60%'
			},
			{
				data: null,
				className: 'text-center',
				width: '10%',
				render: (data, type) => {
					let idx = componente.Vinculados.findIndex(v => { return v.IdVinculado === data.Id });
					return type === 'display' ? `<div class="form-check form-switch">
	<input data-type="vincular" class="form-check-input" type="checkbox" ${(idx > -1 ? 'checked' : '')}>
</div>`: idx > -1;
				//<label class="form-check-label" for="vinculado">Vinculado</label>
				}
			}
		],
		columnDefs: [
			{ targets: [2], className: 'text-end' }
		],
		drawCallback: function (settings) {
			$(`#${settings.sTableId} input[data-type="vincular"]`).change((e) => {
				let data = tabla.rows(e.target.closest('tr')).data()[0];
				vinculado.clear();
				vinculado.IdComponenteMenor = componente.Id;
				vinculado.IdVinculado = data.Id;
				if (e.target.checked) {
					var res = vinculado.save();
					if (res.Valid && res.Error === '') {
						componente.Vinculados.push(vinculado.clon())
						//addVin(cVinculado)
					}
					else {
						e.target.checked = false
						MsjBox('Vinculacion', res.Error)
					}
				}
				else {
					var res = vinculado.delete();
					if (res.Valid) {
						let idx = componente.Vinculados.findIndex(v => { return v.IdVinculado === data.Id });
						if (idx > -1)
							componente.Vinculados.splice(idx, 1)
						//addVin(cVinculado)
					}
					else {
						MsjBox('Vinculacion', res.Error)
					}
				}
				tabla.draw();
			});
		}
	});
}