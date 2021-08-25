

var app = new Vue({
    el: '#app',
    data: {
        cotizacion: new CotizacionClass(),
        showTable: true,
    },
    methods: {
        postCotizacion() {
            this.cotizacion.postCotizacion(this.cotizacion);
        }
    }
})