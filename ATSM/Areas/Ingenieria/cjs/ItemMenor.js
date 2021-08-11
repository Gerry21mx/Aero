class ItemMenor extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async byMayorMenorPic(idMayor, idMenor, idPic) {
        let res = await fetch(`${this._apiElement}/ByMayorMenorPic?idMayor=${idMayor}&idMenor=${idMenor}&idPic=${idPic}`);
        if (res.ok) {
            let post = await res.json();
            this.setValores(post.Data);
            if (this.onGetInstance !== undefined && this.onGetInstance instanceof Function) {
                this.onGetInstance();
            }
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async filtro(idAircraft, idComponenteMayor, idMayor, idFamilia) {
        let res = await fetch(`${this.Api + this.name}?idAircraft=${idAircraft}&idComponenteMayor=${idComponenteMayor}&idMayor=${idMayor}&idFamilia=${idFamilia}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}