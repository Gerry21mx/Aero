document.addEventListener('DOMContentLoaded', () => {
	if (document.querySelector('button[tipo="infoAeropuerto"]'))
		document.querySelector('button[tipo="infoAeropuerto"]').addEventListener('click', (e) => {
			procc();
			let id = '';
			switch (e.target.tagName) {
				case 'I':
					id = e.target.parentElement.id.substring(1);
					break;
				case 'LABEL':
					id = e.target.id.substring(1);
					break;
			}
			if (id === '') {
				endPro();
				return;
			}
			let val = parseInt(document.getElementById(id).value) || 0;
			if (val > 0 && val !== undefined) {
				$.get(`${window.location.origin}/moldes/iaeropuerto?id=${val}`, (modal) => {
					$('body').append($(modal));
					$('#imAeropuerto').modal('show').on('hidden.bs.modal', function (e) {
						$('#imAeropuerto').remove();
					});
					endPro();
				});
			} else {
				endPro();
			}
		});
})

const infoComMenor = (componente) => {
	if (!componente.Valid) return;
	let limites = '';
	componente.Limites.forEach(limit => {
		limites += `<tr>
			<th>${limit.Limit.Codigo}</th>
			<th>${limit.Horas}</th>
			<th>${limit.Ciclos}</th>
			<th>${limit.Dias}</th>
		</tr>`;
	});
	let modelos = '';
	componente.Modelos.forEach(modelo => {
		modelos += `<tr>
			<th>${modelo.Mayor.Descripcion}</th>
			<th>${modelo.Modelo.Nombre}</th>
			<th>${modelo.Cantidad}</th>
		</tr>`;
	});
	let modal = document.createElement('div');
	modal.classList.add('modal', 'fade');
	modal.tabIndex = '-1';
	modal.id = 'icMenor';
	modal.innerHTML = `<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
    <div class="modal-content">
      <div class="modal-header bg-warning">
        <h6 class="modal-title">
			NP <span class="text-primary">${componente.Part}</span><br />
			<small>
				<span class="text-muted">${componente.Description})</span>
			</small>
		</h6>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body">
		  <div class="row justify-content-around">
			  <div class="col-6">
				  <b>Family: </b> <span class="text-primary">${componente.Familia.Nombre}</span>
              </div>
			  <div class="col-6">
				  <b>ATA: </b> <span class="text-primary">${componente.ATA1}-${componente.ATA2}-${componente.ATA3}</span>
              </div>
          </div>

		  <div class="table-responsive sz-font-7 mt-3">
			  <table id="tLim" class="table table-sm compact table-striped table-bordered">
				<thead class="bg-primary">
					<tr>
						<th>Limite</th>
						<th>Horas</th>
						<th>Ciclos</th>
						<th>Dias</th>
					</tr>
				</thead>
				<tbody>${limites}</tbody>
			</table>
          </div>

		  <div class="table-responsive sz-font-7 mt-0">
              <table id="tMod" class="table table-sm compact table-striped table-bordered">
				<thead class="bg-primary">
					<tr>
						<th>Mayor</th>
						<th>Modelo</th>
						<th>Cantidad</th>
					</tr>
				</thead>
				<tbody>${modelos}</tbody>
			</table>
          </div>
      </div>
    </div>
  </div>`;
	let om = document.body.appendChild(modal);
	let modalInfo = new bootstrap.Modal(om, { backdrop: 'static' });
	document.getElementById('icMenor').addEventListener('hidden.bs.modal', (e) => { document.getElementById('icMenor').remove(); });
	modalInfo.show();
}
const infoItmMayor = (item) => {
	if (!item.Valid) return;
	let tiempos = '';
	if (item.Limites.length > 0) {
		let filas = '';
		item.Tiempos.forEach(tiempo => {
			let idl = item.Limites.findIndex(l => { return l.IdLimite === tiempo.IdLimite; });
			if (idl > -1) {
				let lim = item.Limites[idl]
				if ((tiempo.Limite_Individual_Horas + tiempo.Limite_Individual_Ciclos + tiempo.Limite_Individual_Dias) > 0) {
					filas += `<tr class="table-danger">
						<th>Limite Individual ${lim.Limit.Codigo}</th>
						<th>${tiempo.Limite_Individual_Horas}</th>
						<th>${tiempo.Limite_Individual_Ciclos}</th>
						<th>${tiempo.Limite_Individual_Dias}</th>
					</tr>`;
				}
				else {
					filas += `<tr class="table-info">
						<th>Limite ${lim.Limit.Codigo}</th>
						<th>${lim.Horas}</th>
						<th>${lim.Ciclos}</th>
						<th>${lim.Dias}</th>
					</tr>`;
				}
			}
			filas += `<tr class="table-primary">
				<th>Ultimo Cumplimiento</th>
				<th>${tiempo.Horas_Last}</th>
				<th>${tiempo.Ciclos_Last}</th>
				<th>${tiempo.Fecha_Last}</th>
			</tr>`;
			filas += `<tr class="table-success">
				<th>Tiempo Transcurrido</th>
				<th>${tiempo.Horas_Elapsed}</th>
				<th>${tiempo.Ciclos_Elapsed}</th>
				<th>${tiempo.Dias_Elapsed}</th>
			</tr>`;
		});
		tiempos = `<div class="table-responsive sz-font-7 mt-3">
				  <table id="tTie" class="table table-sm compact table-striped table-bordered text-center">
					<thead>
						<tr class="table-dark">
							<th></th>
							<th class="text-center">Horas</th>
							<th class="text-center">Ciclos</th>
							<th class="text-center">Dias</th>
						</tr>
					</thead>
					<tbody>${filas}</tbody>
				</table>
			  </div>`;
	}
	let modal = document.createElement('div');
	modal.classList.add('modal', 'fade');
	modal.tabIndex = '-1';
	modal.id = 'iItmMayor';
	modal.innerHTML = `<div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
    <div class="modal-content">
      <div class="modal-header bg-warning">
        <h6 class="modal-title">
			<span class="text-primary">${item.Componente.Descripcion}</span><br />
			<small>
				<span class="text-muted">${item.Estado.Nombre}</span>
			</small>
		</h6>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>
      <div class="modal-body">
		  <div class="row justify-content-around">
			  <div class="col-6 col-md-4">
				  <b>Matricula: </b> <span class="text-primary">${item.Aircraft.Matricula}</span>
              </div>
			  <div class="col-6 col-md-4">
				  <b>Serie: </b> <span class="text-primary">${item.Serie}</span>
              </div>
			  <div class="col-6 col-md-4">
				  <b>Empresa: </b> <span class="text-primary">${item.Aircraft.Empresa.Nombre}</span>
              </div>
          </div>
		  <div class="row justify-content-around">
			  <div class="col-6 col-md-4">
				  <b>TSN: </b> <span class="text-primary">${accounting.formatNumber(item.TSN,2,',','.')}</span>
              </div>
			  <div class="col-6 col-md-4">
				  <b>CSN: </b> <span class="text-primary">${accounting.formatNumber(item.CSN,0,',','.')}</span>
              </div>
			  <div class="col-6 col-md-4">
				  <b>Año: </b> <span class="text-primary">${item.Anio}</span>
              </div>
          </div>
		  <div class="row justify-content-around">
			  <div class="col-6 col-md-4">
				  <b>Modelo: </b> <span class="text-primary">${item.Modelo.Nombre}</span>
              </div>
			  <div class="col-6 col-md-4">
				  <b>Fabricante: </b> <span class="text-primary">${item.Modelo.Fabricante}</span>
              </div>
			  <div class="col-6 col-md-4">
				  <b>Capacidad: </b> <span class="text-primary">${item.Modelo.Capacidad.Nombre}</span>
              </div>
          </div>
		  <div class="row justify-content-around">
			  <div class="col-6 col-md-4">
				  <b>Posicion: </b> <span class="text-primary">${item.Posicion.Nombre}</span>
              </div>
          </div>
		  ${tiempos}
      </div>
      <div class="modal-footer">
        <a type="button" href="${window.location.origin}/Ingenieria/ComponenteMayor/Mayores?idc=${item.IdComponente}&id=${item.Id}" target="_blank" class="btn btn-sm btn-outline-dark" data-bs-dismiss="modal">Revisar</a>
      </div>
    </div>
  </div>`;
	let om = document.body.appendChild(modal);
	let modalInfo = new bootstrap.Modal(om, { backdrop: 'static' });
	document.getElementById('iItmMayor').addEventListener('hidden.bs.modal', (e) => { document.getElementById('iItmMayor').remove(); });
	modalInfo.show();
}