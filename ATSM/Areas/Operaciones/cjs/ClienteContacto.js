class ClienteContacto extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async byNombre(idcliente, nombre) {
        let res = await fetch(`${(this._apiElement)}/ByNombre?idcliente=${idcliente}&nombre=${nombre}`);
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
    static async getClienteContactos(idcliente) {
        let res = await fetch(`${(this.Api + this.name)}/ContactosCLiente?idcliente=${idcliente}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}