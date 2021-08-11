class Tiempos extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async byLimiteItem(idLimite, idItem, tipo) {
        let res = await fetch(`${(this._apiElement)}?idLimite=${idLimite}&idItem=${idItem}&tipo=${tipo}`);
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
    static async getTiempos(idItem, tipo) {
        let res = await fetch(`${(this.Api + this.name)}?idItem=${idItem}&tipo=${tipo}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}