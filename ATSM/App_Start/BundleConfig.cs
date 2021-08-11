using System.Web;
using System.Web.Optimization;

namespace ATSM {
	public class BundleConfig {
		// Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
				"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
				"~/Scripts/jquery.signalR-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/timeline").Include(
				"~/Ext/timeline/timeline.js"));

			bundles.Add(new StyleBundle("~/Content/timeline").Include(
				"~/Ext/timeline/timeline.css"));

			bundles.Add(new ScriptBundle("~/bundles/d3").Include(
				"~/Scripts/d3/d3.js"));

			bundles.Add(new StyleBundle("~/Content/dashboard").Include(
				"~/Content/sb-admin-2.css"));

			bundles.Add(bundle: new Bundle("~/bundles/dashboard").Include(
				"~/Scripts/jquery-easing/jquery.easing.js",
				"~/Scripts/sb-admin-2.js",
				"~/Scripts/chart.js/Chart.js"));

			// Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
			// para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(bundle: new Bundle("~/bundles/bootstrap").Include(
				"~/Scripts/bootstrap.bundle.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css",
				"~/Content/jsTree/themes/default/style.css",
				"~/Ext/supercontextmenu-master/dist/context-menu.min.css",
				"~/Ext/bootnavbar/bootnavbar.css",
				"~/Content/site.css"));

			bundles.Add(new StyleBundle("~/Content/datatables").Include(
				"~/Ext/DataTables/datatables.css",
				"~/Ext/DataTables/Buttons-1.7.0/css/buttons.dataTables.css",
				"~/Ext/DataTables/RowGroup-1.1.2/css/rowGroup.dataTables.css",
				"~/Ext/DataTables/Select-1.3.3/css/select.dataTables.css"));

			bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
				"~/Ext/DataTables/datatables.js",
				"~/Ext/DataTables/Buttons-1.7.0/js/dataTables.buttons.js",
				"~/Ext/DataTables/Buttons-1.7.0/js/buttons.html5.js",
				"~/Ext/DataTables/Buttons-1.7.0/js/buttons.print.js",
				"~/Ext/DataTables/JSZip-2.5.0/jszip.js",
				"~/Ext/DataTables/pdfmake-0.1.36/pdfmake.js",
				"~/Ext/DataTables/pdfmake-0.1.36/vfs_fonts.js",
				"~/Ext/DataTables/RowGroup-1.1.2/js/dataTables.rowGroup.js",
				"~/Ext/DataTables/Select-1.3.3/js/dataTables.select.js"));

			bundles.Add(bundle: new Bundle("~/bundles/plugins").Include(
				"~/Ext/accounting/accounting.js",
				"~/Ext/supercontextmenu-master/dist/context-menu.js",
				"~/Scripts/jsTree3/jstree.js",
				"~/Scripts/moment.js",
				"~/Scripts/moment-with-locales.js",
				"~/Ext/pouchdb/dist/pouchdb.js"
				));

			bundles.Add(new ScriptBundle("~/bundles/bootnavbar").Include(
				"~/Ext/bootnavbar/bootnavbar.js"));

			bundles.Add(bundle: new Bundle("~/bundles/base").Include(
				"~/cjs/gen/Base.js",
				"~/cjs/gen/Usuario.js",
				"~/cjs/gen/modulos.js",
				"~/cjs/gen/funciones.js",
				"~/cjs/gen/Perfil.js",
				"~/cjs/gen/Rol.js",
				"~/cjs/app.js"
				));

			bundles.Add(new ScriptBundle("~/bundles/ClasesJs").Include(
				"~/Areas/Cuentas/cjs/Account.js",
				"~/Areas/Cuentas/cjs/Moneda.js",
				"~/Areas/Cuentas/cjs/Saldo.js",
				"~/Areas/Cuentas/cjs/Transaccion.js",

				"~/Areas/Gastos/cjs/Comprobacion.js",
				"~/Areas/Gastos/cjs/Gasto.js",

				"~/Areas/Seguimiento/cjs/Aeropuerto.js",
				"~/Areas/Seguimiento/cjs/Demora.js",
				"~/Areas/Seguimiento/cjs/JumpSeat.js",
				"~/Areas/Seguimiento/cjs/Ruta.js",
				"~/Areas/Seguimiento/cjs/TipoVuelo.js",
				"~/Areas/Seguimiento/cjs/Vuelo.js"
				));

			bundles.Add(new ScriptBundle("~/bundles/cJsCuentas").Include(
				"~/Areas/Cuentas/cjs/Account.js",
				"~/Areas/Cuentas/cjs/Moneda.js",
				"~/Areas/Cuentas/cjs/Saldo.js",
				"~/Areas/Cuentas/cjs/Transaccion.js"
				));

			bundles.Add(new ScriptBundle("~/bundles/cJsGastos").Include(
				"~/Areas/Gastos/cjs/Comprobacion.js",
				"~/Areas/Gastos/cjs/Gasto.js"
				));

			bundles.Add(new ScriptBundle("~/bundles/cJsSeguimiento").Include(
				"~/Areas/Seguimiento/cjs/Aeropuerto.js",
				"~/Areas/Seguimiento/cjs/Demora.js",
				"~/Areas/Seguimiento/cjs/JumpSeat.js",
				"~/Areas/Seguimiento/cjs/Ruta.js",
				"~/Areas/Seguimiento/cjs/TipoVuelo.js",
				"~/Areas/Seguimiento/cjs/Vuelo.js"
				));

			BundleTable.EnableOptimizations = false;
		}
	}
}
