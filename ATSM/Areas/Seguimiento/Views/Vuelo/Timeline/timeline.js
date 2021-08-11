var datos, timeline, vueloHub;
var cnxHub = false;
var inicio = moment().startOf('day').add(-3, 'days').toDate();
var fin = moment().add(1,'days').endOf('day').toDate();
var options = {
    width: '100%',
    height: 'auto',
    editable: true,
    animate: true,
    style: 'box',
    axisOnTop: true,
    showCurrentTime: true,
    showNavigation: true,
    groupMinHeight: 25,
    moveable: true,
    min: new Date(2014, 05, 01),
    max: moment().startOf('day').add(2, 'days').toDate(),
    timeChangeable: true,
    zoomMax: 2592000000,     //  30 Dias
    zoomMin: 21600000,              //  6 Horas
    groupsChangeable: true,
    groupsOnRight: false,
    stackEvents: true
};
$(document).scroll(function (e) {
    var y = $('.timeline-groups-axis div').eq(0).offset().top; // + $('.timeline-groups-axis div').eq(0).height();
    var ps = $(this).scrollTop();
    if (ps > y) {
        $('.timeline-content div').eq(0).css({ 'position': 'fixed' });
        $('.timeline-content div').eq(0).css({ 'left': $('.timeline-groups-axis div').eq(0).width() });
        $('.timeline-content div').eq(0).css({ 'z-index': '999' });
    } else {
        $('.timeline-content div').eq(0).css({ 'position': 'absolute' });
        $('.timeline-content div').eq(0).css({ 'left': '0px' });
        $('.timeline-content div').eq(0).css({ 'z-index': 'auto' });
    }
});
$(document).ready(() => {
    $('#main_navbar').addClass('position-absolute');
    datos = [];
    options.start = inicio;
    options.end = fin;
    timeline = new links.Timeline(document.getElementById('vuelos'), options); // Instantiate our timeline object.
    timeline.setScale(5, 1);
    timeline.setVisibleChartRange(inicio, fin);

    links.events.addListener(timeline, 'rangechange', onrangechange);
    links.events.addListener(timeline, 'select', infoTramo);
    
    consulta();
    hubs();
});
function hubs() {
    vueloHub = $.connection.vueloHub;
    vueloHub.client.addVuelo = (datosvlo) => {
        if (datosvlo.Valid) {
            let vuelo = new Vuelo();
            vuelo.setValores(datosvlo);
            vuelo.Tramos.forEach((tramo, idx) => {
                //  Itinerario
                if (tramo.Salida === null || tramo.ItinerarioDespegue === null || tramo.ItinerarioAterrizaje === null) return null;
                tramo.Llegada = tramo.Llegada === null ? tramo.Salida : tramo.Llegada;
                let sal = new Date(tramo.Salida.substring(0, 11) + tramo.ItinerarioDespegue);
                let lle = new Date(tramo.Llegada.substring(0, 11) + tramo.ItinerarioAterrizaje);
                if (sal > lle)
                    lle = moment(lle).add(1, 'days').toDate();
                let pa = datos.findIndex(t => t.tipo === 1 && t.pierna.IdTramo === tramo.IdTramo)
                let dtl = { start: sal, end: lle, className: `text-center sz-font-725 bg-secondary`, content: `${vuelo.Trip} - ${tramo.Pierna}`, group: vuelo.Aeronave.Matricula, editable: false, type: "range", idC: pa === -1 ? datos.length : pa, tipo: 1, pierna: tramo };
                if (pa === -1)
                    datos.push(dtl);
                else
                    datos[pa] = dtl;
                //  Vuelo
                if (tramo.Despegue === null || tramo.Aterrizaje === null) return null;
                tramo.Llegada = tramo.Llegada === null ? tramo.Salida : tramo.Llegada;
                sal = new Date(tramo.Salida.substring(0, 11) + tramo.Despegue);
                lle = new Date(tramo.Llegada.substring(0, 11) + tramo.Aterrizaje);
                if (sal > lle)
                    lle = moment(lle).add(1, 'days').toDate();
                pa = datos.findIndex(t => t.tipo === 0 && t.pierna.IdTramo === tramo.IdTramo)
                dtl = { start: sal, end: lle, className: "text-center sz-font-725 bg-success", content: `${vuelo.Trip} - ${tramo.Pierna}`, group: vuelo.Aeronave.Matricula, editable: false, type: "range", idC: pa === -1 ? datos.length : pa, tipo: 0, pierna: tramo };
                if (pa === -1)
                    datos.push(dtl);
                else
                    datos[pa] = dtl;
            });
            timeline.draw(datos);
        }
    }
    vueloHub.client.delVuelo = (idvuelo) => {
        if (idvuelo > 0) {
            var idx = datos.findIndex(i => i.pierna.IdVuelo === idvuelo);
            do {
                if(idx>=0)
                    datos.splice(idx, 1);
                idx = datos.findIndex(i => i.pierna.IdVuelo === idvuelo);
            } while (idx >= 0);
            timeline.draw(datos);
        }
    }
    vueloHub.client.addTramo = (datosAct) => {
        if (datosAct.Valid) {
            var tramo = new VueloTramo();
            tramo.setValores(datosAct);
            var range = timeline.getVisibleChartRange();
            if (tramo.ItinerarioDespegue !== null && tramo.ItinerarioAterrizaje !== null) {
                let tid = tramo.ItinerarioDespegue.split(':');
                let tia = tramo.ItinerarioAterrizaje.split(':');
                let ides = moment(tramo.Salida);
                let iate = moment(tramo.Llegada);
                ides.hour(parseInt(tid[0]) || 0).minute(parseInt(tid[1]) || 0);
                iate.hour(parseInt(tia[0]) || 0).minute(parseInt(tia[1]) || 0);
                if ((ides > range.start && ides <= range.end) || (iate > range.start && iate <= range.end)) {
                    contra(tramo.IdTramo);
                    let noti = new Notification(tramo, {
                        icon: `${window.location.origin}/img/icons/icon-72x72.png`,
                        body: `Actualizacion de Vuelo<br>Pierna: ${tramo.Pierna}`
                    });

                    return true;
                }
            }
            if (tramo.Despegue !== null && tramo.Aterrizaje !== null) {
                let tid = tramo.Despegue.split(':');
                let tia = tramo.Aterrizaje.split(':');
                let des = moment(tramo.Salida);
                let ate = moment(tramo.Llegada);
                des.hour(parseInt(tid[0]) || 0).minute(parseInt(tid[1]) || 0);
                ate.hour(parseInt(tia[0]) || 0).minute(parseInt(tia[1]) || 0);
                if ((des > range.start && des <= range.end) || (ate > range.start && ate <= range.end)) {
                    contra(tramo.IdTramo);
				}
            }
        }
    }
    $.connection.hub.start().done(() => {
        cnxHub = true;
    });
    $.connection.hub.error(function (error) {
        console.log('SignalR error: ' + error)
    });
}
function infoTramo(e) {
    if ($('#inf').length > 0) { $('#inf').remove(); }
    var sel = timeline.getSelection();
    if (sel.length > 0) {
        if (sel[0].row != undefined) {
            procc();
            $.get(`${window.location.origin}/moldes/itramo?idtramo=${datos[sel[0].row].pierna.IdTramo}`, (modal) => {
                $('body').append($(modal));
                $('#InfoTramo').modal('show').on('hidden.bs.modal', function (e) {
                    $('#InfoTramo').remove();
                });
                $('#modTra').click(() => {
                    $('#InfoTramo').modal('hide');
                });
                endPro();
            });
        }
    }
}
function consulta() {
    procc();
    let range = timeline.getVisibleChartRange();
    timeline.deleteAllItems();
    VueloTramo.timeline({ desde: moment(inicio).format('YYYY-MM-DD'), hasta: moment(fin).format('YYYY-MM-DD') }).then((data) => {
        if (data === null) return;
        if (data.Data === null) return;
        var f = new Date();
        datos = [];
        data.Data.forEach((tramo, idx) => {
            if (tramo.start !== null && tramo.start !== undefined)
                tramo.start = new Date(tramo.start);
            else
                return;
            if (tramo.end !== null && tramo.end !== undefined)
                tramo.end = new Date(tramo.end);
            else
                return;
            datos.push(tramo);
        });
        timeline.draw(datos);
        endPro();
    });
    timeline.setVisibleChartRange(range.start, range.end);
}
function contra(idtramo) {
    VueloTramo.timeline({ desde: moment(inicio).format('YYYY-MM-DD'), hasta: moment(fin).format('YYYY-MM-DD'), idtramo: idtramo }).then((data) => {
        if (data === null) return;
        if (data.Data === null) return;
        data.Data.forEach((tramo, idx) => {
            if (tramo.start !== null && tramo.start !== undefined) {
                tramo.start = new Date(tramo.start);
            }
            if (tramo.end !== null && tramo.end !== undefined) {
                tramo.end = new Date(tramo.end);
            }
            tramo.className = `text-center sz-font-725 ${(tramo.tipo === 1 ? 'bg-secondary' : 'bg-success')}`;
            let i = datos.findIndex(e => e.tipo === tramo.tipo && e.pierna.IdTramo === tramo.pierna.IdTramo);
            if (i > -1) {
                datos[i] = tramo;
            } else {
                datos.push(tramo);
			}
        });
        timeline.draw(datos);
    });
}
function ctd(fec) {
    if (fec === null) return null;
    let tz = (new Date()).getTimezoneOffset() * -1;
    let dm = moment(fec).add(tz, 'minutes');
    let res = new Date(dm.year(), dm.month(), dm.date(), dm.hour(), dm.minute());
    return res;
}
function onrangechange(range) {
    if (range === undefined) {
        return false
    };
    f1 = range.start;
    f2 = range.end;
    if (f2 > timeline.options.end) {
        f2 = timeline.options.end;
	}

    let horas = (f2 - f1) / 1000 / 60 / 60;
    if (horas < 24)
        timeline.setScale(links.Timeline.StepDate.SCALE.HOUR, 1);
    if (horas < 8)
        timeline.setScale(links.Timeline.StepDate.SCALE.MINUTE, 30);
    if (horas > 24)
        timeline.setScale(links.Timeline.StepDate.SCALE.DAY, 1);

    f1 = moment(f1).startOf('day').toDate();
    f2 = moment(f2).endOf('day').toDate();
    if (inicio > f1) {
        f1 = moment(f1).startOf().toDate();
    }
    if (fin < f2) {
        f2 = moment(f2).endOf().toDate();
    }
    if (inicio > f1 || fin < f2) {
        inicio = f1;
        fin = f2;
        consulta();
	}
}