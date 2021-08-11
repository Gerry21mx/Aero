class PIC extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async getComponentes() {
        let res = await fetch(`${(this._apiElement)}/Componentes?idpic=${this.Id}`);
        if (res.ok) {
            let post = await res.clone().json();
            if (post.Data)
                this.Componentes = post.Data;
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getByMayor(idComponente) {
        let res = await fetch(`${(this.Api + this.name)}/ByComponente?idComponente=${idComponente}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getByMenor(idComponenteMenor) {
        let res = await fetch(`${(this.Api + this.name)}/ByComponenteMenor?idMenor=${idComponenteMenor}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getByModelo(idmodelo) {
        let res = await fetch(`${(this.Api + this.name)}/ByModelo?idmodelo=${idmodelo}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getByMMA(idmayor, idmodelo, ata1) {
        let res = await fetch(`${(this.Api + this.name)}/ByMMA?idmayor=${idmayor}&idmodelo=${idmodelo}&ata1=${ata1}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async MenorEnPICMayorModelo(idmayor, idmodelo, idmenor) {
        let res = await fetch(`${(this.Api + this.name)}/MenorEnPICMayorModelo?idmayor=${idmayor}&idmodelo=${idmodelo}&idmenor=${idmenor}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}