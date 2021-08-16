var arbol;
document.addEventListener('DOMContentLoaded', (e) => {
	document.querySelector('#IdComponenteMayor').addEventListener('change', vComMay)
	document.querySelector('#IdModelo').addEventListener('change', vMod)
	document.getElementById('pro').addEventListener('click', consulta)
})
const vComMay = (e) => {
	let so = e.target.options[e.target.options.selectedIndex]
	let idcm = parseInt(so.value) || 0;
	document.querySelector('#IdModelo').value='0'
	document.querySelector('#IdItemMayor').value='0'
	if (idcm === 0) {
		document.querySelectorAll(`#IdModelo option`).forEach(o => { o.classList.remove('d-none') })
		document.querySelectorAll(`#IdItemMayor option`).forEach(o => { o.classList.remove('d-none') })
		return
	}
	document.querySelectorAll(`#IdModelo option`).forEach(o => { o.classList.add('d-none') })
	document.querySelectorAll(`#IdModelo option[data-idmayor="${idcm}"]`).forEach(o => { o.classList.remove('d-none') })
	document.querySelectorAll(`#IdItemMayor option`).forEach(o => { o.classList.add('d-none') })
	document.querySelectorAll(`#IdItemMayor option[data-idcomponente="${idcm}"]`).forEach(o => { o.classList.remove('d-none') })
}
const vMod = (e) => {
	let so = e.target.options[e.target.options.selectedIndex]
	let idcm = parseInt(so.value) || 0;
	document.querySelector('#IdItemMayor').value='0'
	if (idcm === 0) {
		document.querySelectorAll(`#IdItemMayor option`).forEach(o => { o.classList.remove('d-none') })
		document.querySelector('#IdComponenteMayor').dispatchEvent(new Event('change'))
		return
	}
	document.querySelectorAll(`#IdItemMayor option`).forEach(o => { o.classList.add('d-none') })
	document.querySelectorAll(`#IdItemMayor option[data-idmodelo="${idcm}"]`).forEach(o => { o.classList.remove('d-none') })
}
const consulta = async (e) => {
	procc()
	let idcm = parseInt(document.getElementById('IdComponenteMayor').value) || 0
	let idm = parseInt(document.getElementById('IdModelo').value) || 0
	let idim = parseInt(document.getElementById('IdItemMayor').value) || 0
	let res = await fetch(`${window.location.origin}/api/StructTree?idComponenteMayor=${idcm}&idModelo=${idm}&idItemMayor=${idim}`);
	if (res.ok) {
		const pds = (objeto) => {
			if (objeto != null) {
				let nuevo = {}
				Object.keys(objeto).forEach(key => { nuevo[key.replace('_', '-')] = objeto[key] })
				objeto = nuevo
			}
			return objeto
		}
		const valNodo = (nodo) => {
			nodo.li_attr = pds(nodo.li_attr)
			if (nodo.children)
				nodo.children.forEach(valNodo)
		}
		let data = await res.json();
		data.forEach(valNodo)

		if (arbol !== undefined)
			$('#arbol').jstree('destroy');
		arbol = $('#arbol').jstree({
			core: {
				themes: {
					stripes: 'true'
				},
				multiple: false,
				animation: 0,
				data: data
			},
			plugins: ['wholerow', 'types', 'themes', 'search']
		})
		$('#arbol').on('select_node.jstree', select)
		endPro()
	} else
		console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
}
const select = (e, datos) => {
	var data = datos.node.li_attr;
	let idCapacidad = data['data-idcapacidad']
	let idComponenteMenor = data['data-idcomponentemenor']
	let idFamilia = data['data-idfamilia']
	let idItemMenor = data['data-iditemmenor']
	let idModelo = data['data-idmodelo']
	let idTipo = data['data-idtipo']
	document.getElementById('information').innerText=''
	if (idCapacidad > 0) {
		let boton = document.createElement('a')
		boton.target = '_blank'
		boton.classList.add('btn', 'btn-outline-primary', 'btn-sm', 'mx-1')
		boton.href = `${window.location.origin}/Ingenieria/Catalogos/Capacidades?id=${idCapacidad}`
		boton.innerHTML = `<i class="fas fa-link"></i> Capacidad`
		document.getElementById('information').appendChild(boton);
	}
	if (idModelo > 0) {
		let boton = document.createElement('a')
		boton.target = '_blank'
		boton.classList.add('btn', 'btn-outline-primary', 'btn-sm', 'mx-1')
		boton.href = `${window.location.origin}/Ingenieria/Catalogos/Modelos?id=${idModelo}`
		boton.innerHTML = `<i class="fas fa-link"></i> Modelo`
		document.getElementById('information').appendChild(boton);
	}
	if (idFamilia > 0) {
		let boton = document.createElement('a')
		boton.target = '_blank'
		boton.classList.add('btn', 'btn-outline-primary', 'btn-sm', 'mx-1')
		boton.href = `${window.location.origin}/Ingenieria/Catalogos/Familias?id=${idFamilia}`
		boton.innerHTML = `<i class="fas fa-link"></i> Familia`
		document.getElementById('information').appendChild(boton);
	}
	if (idComponenteMenor > 0) {
		let boton = document.createElement('a')
		boton.target = '_blank'
		boton.classList.add('btn', 'btn-outline-success', 'btn-sm', 'mx-1')
		boton.href = `${window.location.origin}/Ingenieria/ComponenteMenor/${(idTipo === 1 ? 'Componente' : idTipo === 2 ? 'ADSB' : 'Servicio')}?id=${idComponenteMenor}`
		boton.innerHTML = `<i class="fas fa-link"></i> Modificar`
		document.getElementById('information').appendChild(boton);
	}
	if (idItemMenor > 0) {
		let boton = document.createElement('a')
		boton.target = '_blank'
		boton.classList.add('btn', 'btn-outline-info', 'btn-sm', 'mx-1')
		boton.href = `${window.location.origin}/Ingenieria/Items/${(idTipo === 1 ? 'Componente' : idTipo === 2 ? 'ADSB' : 'Servicio')}?id=${idItemMenor}`
		boton.innerHTML = `<i class="fas fa-link"></i> Item`
		document.getElementById('information').appendChild(boton);
	}
}