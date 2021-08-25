class CotizacionClass {
    constructor(url = 'http://localhost:44308/api/cotizacion', cliente = 'test', origen = 'origen test') {
        this.url = url;
        this.cliente = cliente;
        this.origen = origen;
    }

    // get all cotizaciones
    getCotizaciones() {

    }

    // get only one cotizacion
    getCotizacion() {

    }

    // create cotizacion
    postCotizacion(payload) {
        console.log(payload);
        axios
            .post(this.url, payload)
            .then(response => console.log(response.data))
            .catch(error => console.log(error));
    }

    // delete cotizacion
    deleteCotizacion() {

    }

    // update cotizacion
    updateCotizacion() {

    }
}
