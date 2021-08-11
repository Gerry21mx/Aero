class Limite extends Base {
    constructor(id = 0) {
        super('Id');
        this.Id = id;
        this.Valid = false;
        this.getInstance();
    }
    static async byIdComponenteMenor(idComponenteMenor) {
        let res = await fetch(`${(this.Api + this.name)}/ByIdComponenteMenor?idComponenteMenor=${idComponenteMenor}`);
        if (res.ok) {
            let post = await res.json();
            return post;
        } else {
            console.log(`Error Fetched: ${res.status} - ${res.statusText}`);
            return { Valid: false, Mensaje: '', Error: `Error Fetched: ${res.status} - ${res.statusText}`, Data: null };
        }
    }
}