class Articulo extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async save(formData) {
        let r = { Valid: false, Mensaje: '', Error: this.constructor.name + '.Save - Sin Procesar (Base.JS)' };
        let res = await fetch(`${(this._apiElement)}`, {
            method: 'POST',
            body: formData
        });
        if (res.ok) {
            let post = await res.json();
            if (post.Data) {
                let data = post.Data;
                if (data.Valid) {
                    this.setValores(data.Elemento);
                    if (this.onSave !== undefined && this.onSave instanceof Function) {
                        this.onSave();
                    }
                } else {
                    console.log(`Save: ${data.Error}`);
                }
                return data;
            }
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
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
    static async getArticulos() {
        let res = await fetch(`${(this.Api + this.name)}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getVArticulos() {
        let res = await fetch(`${(this.Api + this.name)}/vart`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}