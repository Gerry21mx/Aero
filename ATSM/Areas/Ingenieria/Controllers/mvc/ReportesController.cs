using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using WebMatrix.WebData;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace ATSM.Areas.Ingenieria.Controllers
{
    public class ReportesController : Controller
    {
        // GET: Ingenieria/Reportes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ingenieria/ComponentesMenores
        public ActionResult ComponentesMenores()
        {
            return View("ComponentesMenores/Index");
        }

        // GET: Ingenieria/ItemsMayores
        public ActionResult ItemsMayores()
        {
            return View("ItemsMayores/Index");
        }

        // GET: Ingenieria/ResumenAeronave
        public ActionResult ResumenAeronave()
        {
            return View("ResumenAeronave/Index");
        }

        // GET: Ingenieria/PDF_AFAC
        public ActionResult PDF_AFAC() {
            var usuario = new Usuario(WebSecurity.CurrentUserId);
            string idComponenteMayor = Request["idComponenteMayor"];  //      Airframe, Motores, Helices, Etc...
            int idCMy = 0;
            Int32.TryParse(idComponenteMayor, out idCMy);
            ComponenteMayor componenteMayor = new ComponenteMayor(idCMy);

            var idTipoMenor = Request["idTipoMenor"];        //      Componentes, AD's / SB's, Servicios
            int idTMn = 0;
            Int32.TryParse(idTipoMenor, out idTMn);
            TipoMenor tipoMenor = new TipoMenor(idTMn);

            var idFamilia = Request["idFamilia"];
            int idFM = 0;
            Int32.TryParse(idFamilia, out idFM);
            Family familia = new Family(idFM);

            var idAircraft = Request["idAircraft"];
            int idAC = 0;
            Int32.TryParse(idAircraft, out idAC);
            Aircraft avion = new Aircraft(idAC);

            var idItemMayor = Request["idItemMayor"];
            int idIMy = 0;
            Int32.TryParse(idItemMayor, out idIMy);
            ItemMayor itemMayor = new ItemMayor(idIMy);

            string reporte = "";
			switch (idTMn) {
                case 1: //  Componente
                    reporte = "COM";
                break;
                case 2: //  Directiva/Service Bulletin
				    if (familia.Nombre.ToUpper().Contains("DIRECTIVES")) {
                        reporte = "ADS";
				    } else {
                        reporte = "SBS";
				    }
                break;
                case 3: //  Servicio
                    reporte = "SRV";
                break;
			}
            var PDF_File = Server.MapPath("~/Files/tmp/") + $"{reporte}_{WebSecurity.CurrentUserId}-1.pdf";
            var PDF_File2 = Server.MapPath("~/Files/tmp/") + $"{reporte}_{WebSecurity.CurrentUserId}-2.pdf";
            if (System.IO.File.Exists(PDF_File)) {
                try {
                    System.IO.File.Delete(PDF_File);
                }
                catch (Exception) {
                    throw;
                }
            }

			ReportDocument Reporte = new ReportDocument();
            Reporte.Load(Server.MapPath($"~/Areas/Ingenieria/Views/Reportes/Reportes/{reporte}.rpt"));
            //Reporte.Load(Server.MapPath($"~/Areas/Ingenieria/Views/Reportes/Reportes/ADS.rpt"));
            Reporte.SetDatabaseLogon("sa", "IT266.48", "192.168.1.40", "ATSM");
            Reporte.SetParameterValue("@idAircraft", idAircraft);
            Reporte.SetParameterValue("@idComponenteMayor", idComponenteMayor);
            Reporte.SetParameterValue("@idItemMayor", idItemMayor);
            Reporte.SetParameterValue("@idFamilia", idFamilia);
            Reporte.SetParameterValue("@imprime", usuario.Nombre);
            if(reporte == "COM" || reporte == "SRV") {
                Reporte.SetParameterValue("@nombreFamilia", familia.Nombre);
                Reporte.SetParameterValue("@orden", 1);
			} else {
                Reporte.SetParameterValue("@orden", 2);
			}
            Reporte.SetParameterValue("@soloPadres", reporte == "SRV" ? 1 : 0);
            Reporte.SetParameterValue("@titulo", $"{componenteMayor.Descripcion} {(itemMayor.Posicion.Id == 1 ? "" : itemMayor.Posicion.Codigo)}");
            Reporte.SetParameterValue("@ruta", Server.MapPath("~/img/empresas/"));

            ExportOptions exportOpts = new ExportOptions();
            DiskFileDestinationOptions diskOpts = ExportOptions.CreateDiskFileDestinationOptions();
            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat;
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
            diskOpts.DiskFileName = PDF_File;
            exportOpts.ExportDestinationOptions = diskOpts;
            Reporte.Export(exportOpts);
            Reporte.Close();
            Reporte.Dispose();

			//  Encriptar Documento

			PdfReader leerOriginal = new PdfReader(PDF_File);
			Rectangle tamaño = leerOriginal.GetPageSizeWithRotation(1);
			Document document = new Document(tamaño);
			var paginas = leerOriginal.NumberOfPages;
			FileStream oFS = new FileStream(PDF_File2, FileMode.Create, FileAccess.Write);
			PdfWriter writer = PdfWriter.GetInstance(document, oFS);

			//writer.SetEncryption(PdfWriter.STRENGTH40BITS, null, "$!$76w#1973&2846_524736?", PdfWriter.AllowPrinting);
			document.AddTitle($"{familia.Nombre.ToUpper()} MANDATORY");
			document.AddAuthor(usuario.Nombre);
			document.AddSubject($"{familia.Nombre.ToUpper()} MANDATORY REPORT");
			document.AddKeywords($"{componenteMayor.Codigo},{tipoMenor.Nombre},{familia.Codigo}");
			document.AddCreator("CICMA (EM)");
			document.AddProducer();
			document.AddHeader($"Componente Mayor", componenteMayor.Descripcion);
			document.AddHeader($"Tipo Menor", tipoMenor.Nombre);
			document.AddHeader($"Familia", familia.Nombre);
			document.AddCreationDate();
			document.Open();
			PdfContentByte cb = writer.DirectContent;
			for (var page_number = 1; page_number <= leerOriginal.NumberOfPages; page_number++) {
				document.SetPageSize(leerOriginal.GetPageSizeWithRotation(page_number));
				document.NewPage();
				PdfImportedPage page = writer.GetImportedPage(leerOriginal, page_number);
				int rotation = leerOriginal.GetPageRotation(page_number);
				if (rotation == 90 || rotation == 270) {
					cb.AddTemplate(page, 0, -1f, 1f, 0, 0, leerOriginal.GetPageSizeWithRotation(page_number).Height);
				}
				else {
					cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
				}
			}
			document.Close();
			oFS.Close();
			writer.Close();
			leerOriginal.Close();
            if (System.IO.File.Exists(PDF_File)) {
                try {
                    System.IO.File.Delete(PDF_File);
                }
                catch (Exception) {
                    throw;
                }
            }

            byte[] byteStream = System.IO.File.ReadAllBytes(PDF_File2);
            MemoryStream ms = new MemoryStream(byteStream);
            //ms.Write(byteStream, 0, byteStream.Length);
            ms.Position = 0;
            FileStreamResult fsr = new FileStreamResult(ms, "application/pdf");
            fsr.FileDownloadName = $"{familia.Nombre} ({avion.Matricula} {componenteMayor.Descripcion} {(itemMayor.Posicion.Id == 1 ? "" : itemMayor.Posicion.Codigo)})";

            if (System.IO.File.Exists(PDF_File2)) {
                try {
                    System.IO.File.Delete(PDF_File2);
                }
                catch (Exception) {
                    throw;
                }
            }
            return fsr;
            //return File(PDF_File2, "application/pdf");
        }

        // GET: Ingenieria/PDF_AFAC
        public ActionResult PDF_General() {
            var usuario = new Usuario(WebSecurity.CurrentUserId);
            var idComponenteMayor = Request["idComponenteMayor"];  //      Airframe, Motores, Helices, Etc...
            var idTipoMenor = Request["idTipoMenor"];        //      Componentes, AD's / SB's, Servicios
            var idFamilia = Request["idFamilia"];
            var idAircraft = Request["idAircraft"];
            var idItemMayor = Request["idItemMayor"];
            var PDF_File = Server.MapPath("~/Files/tmp/") + $"ADS_{WebSecurity.CurrentUserId}.pdf";
            if (System.IO.File.Exists(PDF_File)) {
                System.IO.File.Delete(PDF_File);
            }
            ReportDocument Reporte = new ReportDocument();
            Reporte.Load(Server.MapPath("~/Areas/Ingenieria/Views/Reportes/ResumenAeronave/ADS.rpt"));
            Reporte.SetDatabaseLogon("sa", "IT266.48", "192.168.1.40", "ATSM");
            Reporte.SetParameterValue("@idAircraft", idAircraft);
            Reporte.SetParameterValue("@idComponenteMayor", idComponenteMayor);
            Reporte.SetParameterValue("@idItemMayor", idItemMayor);
            Reporte.SetParameterValue("@idFamilia", idFamilia);
            Reporte.SetParameterValue("@imprime", usuario.Nombre);
            Reporte.SetParameterValue("@ruta", Server.MapPath("~/img/empresas/"));

            ExportOptions exportOpts = new ExportOptions();
            DiskFileDestinationOptions diskOpts = ExportOptions.CreateDiskFileDestinationOptions();
            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat;
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
            diskOpts.DiskFileName = PDF_File;
            exportOpts.ExportDestinationOptions = diskOpts;
            Reporte.Export(exportOpts);

            Reporte.Close();
            Reporte.Dispose();
            return File(PDF_File, "application/pdf");
        }
    }
}