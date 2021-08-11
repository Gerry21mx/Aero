class Rol extends Base {
	constructor(roleId = 0) {
		super('RoleId');
		this.RoleId = roleId;
		this.Valid = false;
		this.getInstance();
	}
	byNombre(nombre) {
		this.getInstance({ action: 'ByNombre', value: nombre });
	}
	static getRoles(area) {
		area = area === undefined ? null : area === '' ? null : area;
		var res = [];
		let url = `${this.Api + this.name}?area=${(area ?? '')}`;
		$.ajax({
			async: false,
			url: url,
			dataType: 'json',
			method: 'GET',
			success: function (data) {
				if (data.Data !== null) {
					res = data.Data;
				}
			},
			error: function (x, y, z) {
				console.error(`${_this.name}.getRoles()`);
			}
		});
		return res;
	}
}