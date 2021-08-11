class Bitacora extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async byFolioAircraft(idFolio, idAircraft) {
        let res = await fetch(`${(this._apiElement)}/byAircraftFolio?idAircraft=${idAircraft}&idFolio=${idFolio}`);
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
    static async inicial(idAircraft) {
        let res = await fetch(`${(this.Api + this.name)}/inicial?idAircraft=${idAircraft}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async final(idAircraft) {
        let res = await fetch(`${(this.Api + this.name)}/final?idAircraft=${idAircraft}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}
class BitacoraTramo extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async byBitacoraPierna(idBitacora, idPierna) {
        let res = await fetch(`${(this._apiElement)}?idBitacora=${idBitacora}&idPierna=${idPierna}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
    static async getBitacoraTramos(idBitacora) {
        let res = await fetch(`${(this.Api + this.name)}/Tramos?idBitacora=${idBitacora}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}
class BitacoraParametrosMotor extends Base {
    constructor(idBitacora = 0) {
        super('IdBitacora');
        this.IdBitacora = idBitacora;
        this.Valid = false;
        this.getInstance();
    }
}
class BitacoraRCCA extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    async getByBitacoraNo(idBitacora, no) {
        let res = await fetch(`${(this._apiElement)}?idBitacora=${idBitacora}&no=${no}`);
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
    static async getBitacoraRCCAs(idBitacora) {
        let res = await fetch(`${(this.Api + this.name)}/RCCAs?idBitacora=${idBitacora}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}