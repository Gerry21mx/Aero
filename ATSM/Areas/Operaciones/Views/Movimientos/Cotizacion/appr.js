var cotizacion, tabla;
document.addEventListener('DOMContentLoaded', () => {
    let id = parseInt(getPar('id')) || 0;
    cotizacion = new Cotizacion(id);
    if (cotizacion.Valid)
        cotizacion.write();
    document.querySelector('#reg').addEventListener('click', registrar);
    document.querySelector('#cle').addEventListener('click', clear);
    document.querySelector('#del').addEventListener('click', eliminar);
});

const registrar = () => {
    cotizacion = {
        Alto: 125
        Ancho: 365
        Bultos: "1"
        Cliente: "1"
        Consecutivo: -1
        Contacto: null
        Despaletizable: true
        Destino: "MTY"
        FechaAceptacion: "0001-01-01T00:00:00"
        FechaRechazo: "0001-01-01T00:00:00"
        FechaSolicitud: "0001-01-01T00:00:00"
        Hazmat: "01"
        Id: 0
        IdCliente: 0
        IdContactoCliente: 0
        IdDestino: 1
        IdOrigen: 1
        Largo: 123
        Observacion: "jhgajhgajshg"
        Origen: "SLW"
        Peso: "1265"
        TipoServicio: 1
        UMEDimensiones: "PZA"
        UMEPeso: "KG"
        Usuario: 1
        Valid: true
    }
    
    let res = cotizacion.save();
    console.log(res);
    MsjBox('Cotización', res.Error !== '' ? res.Error : res.Mensaje);
    if (res.Valid) {
        updDT(tabla, cotizacion, 'Id');
        clear();
    }
}

const clear = () => {
    cotizacion.clear();
    cotizacion.write();
}

const eliminar = () => {
    if (cotizacion.Valid) {
        let id = cotizacion.Id;
        let res = cotizacion.delete();
        MsjBox('Cotizacion', res.Error !== '' ? res.Error : res.Mensaje);
    }
}