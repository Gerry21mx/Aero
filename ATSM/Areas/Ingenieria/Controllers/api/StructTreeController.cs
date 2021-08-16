using ATSM.Ingenieria;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ATSM.Areas.Ingenieria.Controllers.api
{
    public class StructTreeController : ApiController
    {
        // GET api/<controller>/ByCadena
        public List<JsTree3Node> Get(int idComponenteMayor, int idModelo, int idItemMayor) {
            var familias = Family.GetFamilys();
            List<JsTree3Node> Lista = new List<JsTree3Node>();
			if (idComponenteMayor > 0 && idModelo == 0 && idItemMayor == 0) {
                ComponenteMayor mayor = new ComponenteMayor(idComponenteMayor);
                foreach (var cap in mayor.Capacidades) {
                    if (cap.Capacidad == null)
                        cap.SetCapacidad();

                    JsTree3Node nCap = new JsTree3Node();
                    nCap.id = $"n_{cap.IdCapacidad}";
                    nCap.text = cap.Capacidad.Nombre;
                    nCap.icon = "fas fa-plane";
                    nCap.state = new State(false, false, false);
                    nCap.children = new List<JsTree3Node>();
                    nCap.li_attr = new { data_idcapacidad = cap.IdCapacidad, data_idmodelo = 0, data_idtipo = 0, data_idfamilia = 0, data_idcomponentemenor = 0, data_iditemmenor = 0 };
                    cap.Capacidad.GetModelos();
                    foreach (var mod in cap.Capacidad.Modelos) {
                        if (mod.IdComponenteMayor != mayor.Id)
                            continue;
                        List<ComponenteMenor> menores = ComponenteMenor.GetComponentes(mayor.Id, mod.Id);
                        JsTree3Node nMod = new JsTree3Node();
                        nMod.id = $"n_{cap.IdCapacidad}_{mod.Id}";
                        nMod.text = mod.Nombre;
                        nMod.icon = "fab fa-telegram-plane";
                        nMod.state = new State(false, false, false);
                        nMod.children = new List<JsTree3Node>();
                        nMod.li_attr = new { data_idcapacidad = cap.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = 0, data_idfamilia = 0, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                        for (int i = 1; i <= 3; i++) {
                            TipoMenor tipo = new TipoMenor(i);
                            JsTree3Node nTipMen = new JsTree3Node();
                            nTipMen.id = $"n_{cap.IdCapacidad}_{mod.Id}_{tipo.Id}";
                            nTipMen.text = tipo.Nombre;
                            nTipMen.icon = i == 1 ? "fas fa-cogs" : i == 2 ? "fab fa-adversal" : "fas fa-tools";
                            nTipMen.state = new State(false, false, false);
                            nTipMen.children = new List<JsTree3Node>();
                            nTipMen.li_attr = new { data_idcapacidad = cap.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = i, data_idfamilia = 0, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                            foreach (var fam in familias) {
                                if (i == 1 && fam.TM01 != true)
                                    continue;
                                if (i == 2 && fam.TM02 != true)
                                    continue;
                                if (i == 3 && fam.TM03 != true)
                                    continue;
                                JsTree3Node nfam = new JsTree3Node();
                                nfam.id = $"n_{cap.IdCapacidad}_{mod.Id}_{tipo.Id}_{fam.Id}";
                                nfam.text = fam.Nombre;
                                nfam.icon = "fas fa-users";
                                nfam.state = new State(false, false, false);
                                nfam.children = new List<JsTree3Node>();
                                nfam.li_attr = new { data_idcapacidad = cap.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = i, data_idfamilia = fam.Id, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                                foreach (var componente in menores) {
                                    if (componente.IdFamily != fam.Id) { continue; }
                                    var hijos = ComponenteMenor.GetVinculados(componente.Id);
                                    if (componente.NoPadres > 0) { continue; }
                                    JsTree3Node nComMen = new JsTree3Node();
                                    nComMen.id = $"n_{cap.IdCapacidad}_{mod.Id}_{tipo.Id}_{fam.Id}_{componente.Id}";
                                    nComMen.text = $"<b>{componente.Part}</b> <small>{(componente.Description.Length > 75 ? componente.Description.Substring(0, 70) + "..." : componente.Description)}</small>";
                                    nComMen.icon = "fas fa-puzzle-piece";
                                    nComMen.state = new State(false, false, false);
                                    nComMen.li_attr = new { data_idcapacidad = cap.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = i, data_idfamilia = fam.Id, data_idcomponentemenor = componente.Id, data_iditemmenor = 0 };
                                    //  OTROS POSIBLES HIJOS
                                    nfam.children.Add(nComMen);
                                }
                                nTipMen.children.Add(nfam);
                            }
                            nMod.children.Add(nTipMen);
                        }
                        nCap.children.Add(nMod);
                    }
                    Lista.Add(nCap);
                }
			}
			if (idModelo > 0 && idItemMayor == 0) {
                Modelo mod = new Modelo(idModelo);

                List<ComponenteMenor> menores = ComponenteMenor.GetComponentesByModelo(mod.Id);
                JsTree3Node nMod = new JsTree3Node();
                nMod.id = $"n_{mod.Id}";
                nMod.text = mod.Nombre;
                nMod.icon = "fab fa-telegram-plane";
                nMod.state = new State(false, false, false);
                nMod.children = new List<JsTree3Node>();
                nMod.li_attr = new { data_idcapacidad = mod.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = 0, data_idfamilia = 0, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                for (int i = 1; i <= 3; i++) {
                    TipoMenor tipo = new TipoMenor(i);
                    JsTree3Node nTipMen = new JsTree3Node();
                    nTipMen.id = $"n_{mod.Id}_{tipo.Id}";
                    nTipMen.text = tipo.Nombre;
                    nTipMen.icon = i == 1 ? "fas fa-cogs" : i == 2 ? "fab fa-adversal" : "fas fa-tools";
                    nTipMen.state = new State(false, false, false);
                    nTipMen.children = new List<JsTree3Node>();
                    nTipMen.li_attr = new { data_idcapacidad = mod.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = i, data_idfamilia = 0, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                    foreach (var fam in familias) {
                        if (i == 1 && fam.TM01 != true)
                            continue;
                        if (i == 2 && fam.TM02 != true)
                            continue;
                        if (i == 3 && fam.TM03 != true)
                            continue;
                        JsTree3Node nfam = new JsTree3Node();
                        nfam.id = $"n_{mod.Id}_{tipo.Id}_{fam.Id}";
                        nfam.text = fam.Nombre;
                        nfam.icon = "fas fa-users";
                        nfam.state = new State(false, false, false);
                        nfam.children = new List<JsTree3Node>();
                        nfam.li_attr = new { data_idcapacidad = mod.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = i, data_idfamilia = fam.Id, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                        foreach (var componente in menores) {
                            if (componente.IdFamily != fam.Id) { continue; }
                            var hijos = ComponenteMenor.GetVinculados(componente.Id);
                            if (componente.NoPadres > 0) { continue; }
                            JsTree3Node nComMen = new JsTree3Node();
                            nComMen.id = $"n_{mod.Id}_{tipo.Id}_{fam.Id}_{componente.Id}";
                            nComMen.text = $"<b>{componente.Part}</b> <small>{(componente.Description.Length > 75 ? componente.Description.Substring(0, 70) + "..." : componente.Description)}</small>";
                            nComMen.icon = "fas fa-puzzle-piece";
                            nComMen.state = new State(false, false, false);
                            nComMen.li_attr = new { data_idcapacidad = mod.IdCapacidad, data_idmodelo = mod.Id, data_idtipo = i, data_idfamilia = fam.Id, data_idcomponentemenor = componente.Id, data_iditemmenor = 0 };
                            //  OTROS POSIBLES HIJOS
                            nfam.children.Add(nComMen);
                        }
                        nTipMen.children.Add(nfam);
                    }
                    nMod.children.Add(nTipMen);
                }
                Lista.Add(nMod);
            }
			if (idItemMayor > 0) {
                ItemMayor itemMayor = new ItemMayor(idItemMayor);
                var menores = DataBase.Query($"SELECT CM.Part, CM.Description, CM.ATA1, CM.ATA2, CM.ATA3, CM.Directive, CM.Amendment, CM.ServiceBulletin, CM.IdFamily, CM.IdTipoMenor, (SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdComponenteMenor = CM.Id) AS Hijos, (SELECT COUNT(*) FROM ComponenteMenorVinculado WHERE IdVinculado = CM.Id) AS Padres, IM.* FROM ItemMenor IM LEFT JOIN ComponenteMenor CM ON IM.IdComponenteMenor = CM.Id WHERE IM.IdItemMayor = {itemMayor.Id}");


                JsTree3Node nIMay = new JsTree3Node();
                nIMay.id = $"n_{itemMayor.Id}";
                nIMay.text = $"{itemMayor.Aircraft.Matricula} {itemMayor.Componente.Codigo} {itemMayor.Modelo.Nombre} {itemMayor.Serie}";
                nIMay.icon = "fab fa-telegram-plane";
                nIMay.state = new State(true, false, false);
                nIMay.children = new List<JsTree3Node>();
                nIMay.li_attr = new { data_idcapacidad = itemMayor.Modelo.IdCapacidad, data_idmodelo = itemMayor.IdModelo, data_idtipo = 0, data_idfamilia = 0, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                for (int i = 1; i <= 3; i++) {
                    TipoMenor tipo = new TipoMenor(i);
                    JsTree3Node nTipMen = new JsTree3Node();
                    nTipMen.id = $"n_{itemMayor.Id}_{tipo.Id}";
                    nTipMen.text = tipo.Nombre;
                    nTipMen.icon = i == 1 ? "fas fa-cogs" : i == 2 ? "fab fa-adversal" : "fas fa-tools";
                    nTipMen.state = new State(false, false, false);
                    nTipMen.children = new List<JsTree3Node>();
                    nTipMen.li_attr = new { data_idcapacidad = itemMayor.Modelo.IdCapacidad, data_idmodelo = itemMayor.IdModelo, data_idtipo = i, data_idfamilia = 0, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                    foreach (var fam in familias) {
                        if (i == 1 && fam.TM01 != true)
                            continue;
                        if (i == 2 && fam.TM02 != true)
                            continue;
                        if (i == 3 && fam.TM03 != true)
                            continue;
                        JsTree3Node nfam = new JsTree3Node();
                        nfam.id = $"n_{itemMayor.Id}_{tipo.Id}_{fam.Id}";
                        nfam.text = fam.Nombre;
                        nfam.icon = "fas fa-users";
                        nfam.state = new State(false, false, false);
                        nfam.children = new List<JsTree3Node>();
                        nfam.li_attr = new { data_idcapacidad = itemMayor.Modelo.IdCapacidad, data_idmodelo = itemMayor.IdModelo, data_idtipo = i, data_idfamilia = fam.Id, data_idcomponentemenor = 0, data_iditemmenor = 0 };

                        foreach (var item in menores.Rows) {
                            if (item.IdFamily != fam.Id) { continue; }
                            var hijos = ComponenteMenor.GetVinculados(item.Id);
                            if (item.Padres > 0) { continue; }
                            JsTree3Node nComMen = new JsTree3Node();
                            nComMen.id = $"n_{itemMayor.Id}_{tipo.Id}_{fam.Id}_{item.Id}";
                            nComMen.text = $"<b>{(i==1? item.Part: i==2?(!string.IsNullOrEmpty(item.Directive)?item.Directive:item.ServiceBulletin): "")}</b> <small>{(item.Description.Length > 75 ? item.Description.Substring(0, 70) + "..." : item.Description)}</small>";
                            nComMen.icon = "fas fa-puzzle-piece";
                            nComMen.state = new State(false, false, false);
                            nComMen.li_attr = new { data_idcapacidad = itemMayor.Modelo.IdCapacidad, data_idmodelo = itemMayor.IdModelo, data_idtipo = i, data_idfamilia = fam.Id, data_idcomponentemenor = item.IdComponenteMenor, data_iditemmenor = item.Id };
                            //  OTROS POSIBLES HIJOS
                            nfam.children.Add(nComMen);
                        }
                        nTipMen.children.Add(nfam);
                    }
                    nIMay.children.Add(nTipMen);
                }
                Lista.Add(nIMay);
            }
            return Lista;
        }
    }
}
