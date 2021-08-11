class ComponenteMenorVinculado extends Base {
    constructor(idComponenteMenor = 0, idVinculado = 0) {
        super('IdComponenteMenor');
        this.IdComponenteMenor = 0;
        this.IdVinculado = 0;
        this.getInstance(idComponenteMenor, idVinculado);
    }
    async getInstance(idComponenteMenor, idVinculado) {
        let res = await fetch(`${(this._apiElement)}?idComponente=${idComponenteMenor}&idVinculado=${idVinculado}`);
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
    static async getComponenteMenorVinculados(idComponente) {
        let res = await fetch(`${(this.Api + this.name)}/Vinculados?idComponente=${idComponente}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}