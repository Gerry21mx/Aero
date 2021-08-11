class ComponenteMenorModelo extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async getByMayorMenorModelo(idMayor, idMenor, idModelo) {
        let res = await fetch(`${this._apiElement}/ByMayorMenorModelo?idMayor=${idMayor}&idMenor=${idMenor}&idModelo=${idModelo}`);
        if (res.ok) {
            let post = await res.json();
            this.setValores(data.Data);
            if (this.onGetInstance !== undefined && this.onGetInstance instanceof Function) {
                this.onGetInstance();
            }
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getComponenteMenorModelos(idMayor, idModelo) {
        let res = await fetch(`${(this.Api + this.name)}/byMayorModelo?idMayor=${idMayor}&idModelo=${idModelo}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}