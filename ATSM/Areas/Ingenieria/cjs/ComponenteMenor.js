class ComponenteMenor extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async byCadena(cadena) {
        let res = await fetch(`${(this._apiElement)}/ByCadena?cadena=${cadena}`);
        if (res.ok) {
            let post = await res.json();
            if (post.Data) {
                this.setValores(clonObject(post.Data));
                if (this.onGetInstance !== undefined && this.onGetInstance instanceof Function)
                    this.onGetInstance();
            }
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    async compatibles() {
        let res = await fetch(`${(this._apiElement)}/Compatibles?idMenor=${this.Id}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getComponentesMenores(idMayor, idModelo) {
        let res = await fetch(`${(this.Api + this.name)}?idMayor=${idMayor}&idModelo=${idModelo}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getByModelo(idModelo) {
        let res = await fetch(`${(this.Api + this.name)}/ByModelo?idModelo=${idModelo}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async generaraPN(cadena) {
        let res = await fetch(`${(this.Api + this.name)}/GenerarPN?cadena=${cadena}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async query(idMayor, idFamilia, idModelo, idTipo, ata1) {
        let res = await fetch(`${(this.Api + this.name)}/query?idMayor=${idMayor}&idFamilia=${idFamilia}&idModelo=${idModelo}&idTipo=${idTipo}&ata1=${ata1}&`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async vinculados(idComponenteMenor) {
        let res = await fetch(`${(this.Api + this.name)}/vinculados?idComponenteMenor=${idComponenteMenor}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}