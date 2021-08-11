class Tarea extends Base {
	constructor(taskId = 0) {
		super('TaskId');
		this.TaskId = taskId;
		this.Valid = false;
		this.getInstance();
		delete this.Consecutivo;
	}
	static consComponenteModelo(idComponente, idModelo) {
		let tsk = new Tarea();
		if (idComponente > 0 && idModelo !== '') {
			tsk.idComponente = idComponente;
			tsk.idModelo = idModelo;
			tsk.GetInstance();
		}
		return tsk;
	}
}

class MaterialTool extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
		delete this.Consecutivo;
	}
}

class Instruction extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
		delete this.Consecutivo;
	}
}

class Reference extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
		delete this.Consecutivo;
	}
}

class TImage extends Base {
	constructor(id = 0) {
		super('Id');
		this.Id = id;
		this.Valid = false;
		this.getInstance();
		delete this.Consecutivo;
	}
}