using Aldebaran.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace Aldebaran.Web.Controllers
{
    public partial class ExportAldebaranContextController : ExportController
    {
        private readonly AldebaranContext context;
        private readonly AldebaranContextService service;

        public ExportAldebaranContextController(AldebaranContext context, AldebaranContextService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/AldebaranContext/actordens/csv")]
        [HttpGet("/export/AldebaranContext/actordens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActordensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetActordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/actordens/excel")]
        [HttpGet("/export/AldebaranContext/actordens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActordensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetActordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/actpedidos/csv")]
        [HttpGet("/export/AldebaranContext/actpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetActpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/actpedidos/excel")]
        [HttpGet("/export/AldebaranContext/actpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetActpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/actxactpedidos/csv")]
        [HttpGet("/export/AldebaranContext/actxactpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActxactpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetActxactpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/actxactpedidos/excel")]
        [HttpGet("/export/AldebaranContext/actxactpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportActxactpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetActxactpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/agentesforwarders/csv")]
        [HttpGet("/export/AldebaranContext/agentesforwarders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAgentesforwardersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAgentesforwarders(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/agentesforwarders/excel")]
        [HttpGet("/export/AldebaranContext/agentesforwarders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAgentesforwardersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAgentesforwarders(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ajustes/csv")]
        [HttpGet("/export/AldebaranContext/ajustes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAjustesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAjustes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ajustes/excel")]
        [HttpGet("/export/AldebaranContext/ajustes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAjustesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAjustes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ajustesinvs/csv")]
        [HttpGet("/export/AldebaranContext/ajustesinvs/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAjustesinvsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAjustesinvs(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ajustesinvs/excel")]
        [HttpGet("/export/AldebaranContext/ajustesinvs/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAjustesinvsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAjustesinvs(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ajustesxitems/csv")]
        [HttpGet("/export/AldebaranContext/ajustesxitems/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAjustesxitemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAjustesxitems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ajustesxitems/excel")]
        [HttpGet("/export/AldebaranContext/ajustesxitems/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAjustesxitemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAjustesxitems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/alarmas/csv")]
        [HttpGet("/export/AldebaranContext/alarmas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAlarmasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAlarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/alarmas/excel")]
        [HttpGet("/export/AldebaranContext/alarmas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAlarmasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAlarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/alarmascantidadesminimas/csv")]
        [HttpGet("/export/AldebaranContext/alarmascantidadesminimas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAlarmascantidadesminimasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAlarmascantidadesminimas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/alarmascantidadesminimas/excel")]
        [HttpGet("/export/AldebaranContext/alarmascantidadesminimas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAlarmascantidadesminimasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAlarmascantidadesminimas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anulacionreservas/csv")]
        [HttpGet("/export/AldebaranContext/anulacionreservas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnulacionreservasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAnulacionreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anulacionreservas/excel")]
        [HttpGet("/export/AldebaranContext/anulacionreservas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnulacionreservasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAnulacionreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anuladetcantprocesos/csv")]
        [HttpGet("/export/AldebaranContext/anuladetcantprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnuladetcantprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAnuladetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anuladetcantprocesos/excel")]
        [HttpGet("/export/AldebaranContext/anuladetcantprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnuladetcantprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAnuladetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anulaprocesos/csv")]
        [HttpGet("/export/AldebaranContext/anulaprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnulaprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAnulaprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anulaprocesos/excel")]
        [HttpGet("/export/AldebaranContext/anulaprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnulaprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAnulaprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anulasubitemdetprocesos/csv")]
        [HttpGet("/export/AldebaranContext/anulasubitemdetprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnulasubitemdetprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAnulasubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/anulasubitemdetprocesos/excel")]
        [HttpGet("/export/AldebaranContext/anulasubitemdetprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAnulasubitemdetprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAnulasubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/areas/csv")]
        [HttpGet("/export/AldebaranContext/areas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAreasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAreas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/areas/excel")]
        [HttpGet("/export/AldebaranContext/areas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAreasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAreas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/auxactordens/csv")]
        [HttpGet("/export/AldebaranContext/auxactordens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuxactordensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAuxactordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/auxactordens/excel")]
        [HttpGet("/export/AldebaranContext/auxactordens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuxactordensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAuxactordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/auxitemsxcolors/csv")]
        [HttpGet("/export/AldebaranContext/auxitemsxcolors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuxitemsxcolorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAuxitemsxcolors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/auxitemsxcolors/excel")]
        [HttpGet("/export/AldebaranContext/auxitemsxcolors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuxitemsxcolorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAuxitemsxcolors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/auxordenes/csv")]
        [HttpGet("/export/AldebaranContext/auxordenes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuxordenesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAuxordenes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/auxordenes/excel")]
        [HttpGet("/export/AldebaranContext/auxordenes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuxordenesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAuxordenes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/bodegas/csv")]
        [HttpGet("/export/AldebaranContext/bodegas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBodegasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetBodegas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/bodegas/excel")]
        [HttpGet("/export/AldebaranContext/bodegas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBodegasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetBodegas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/cancelpedidos/csv")]
        [HttpGet("/export/AldebaranContext/cancelpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCancelpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCancelpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/cancelpedidos/excel")]
        [HttpGet("/export/AldebaranContext/cancelpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCancelpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCancelpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/cantprocesos/csv")]
        [HttpGet("/export/AldebaranContext/cantprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCantprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/cantprocesos/excel")]
        [HttpGet("/export/AldebaranContext/cantprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCantprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/cierrepedidos/csv")]
        [HttpGet("/export/AldebaranContext/cierrepedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCierrepedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCierrepedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/cierrepedidos/excel")]
        [HttpGet("/export/AldebaranContext/cierrepedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCierrepedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCierrepedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ciudades/csv")]
        [HttpGet("/export/AldebaranContext/ciudades/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCiudadesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCiudades(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ciudades/excel")]
        [HttpGet("/export/AldebaranContext/ciudades/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCiudadesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCiudades(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/clientes/csv")]
        [HttpGet("/export/AldebaranContext/clientes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetClientes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/clientes/excel")]
        [HttpGet("/export/AldebaranContext/clientes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportClientesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetClientes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/consecutivos/csv")]
        [HttpGet("/export/AldebaranContext/consecutivos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConsecutivosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetConsecutivos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/consecutivos/excel")]
        [HttpGet("/export/AldebaranContext/consecutivos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConsecutivosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetConsecutivos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/contactos/csv")]
        [HttpGet("/export/AldebaranContext/contactos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportContactosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetContactos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/contactos/excel")]
        [HttpGet("/export/AldebaranContext/contactos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportContactosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetContactos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/controlconcurrencia/csv")]
        [HttpGet("/export/AldebaranContext/controlconcurrencia/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportControlconcurrenciaToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetControlconcurrencia(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/controlconcurrencia/excel")]
        [HttpGet("/export/AldebaranContext/controlconcurrencia/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportControlconcurrenciaToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetControlconcurrencia(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/departamentos/csv")]
        [HttpGet("/export/AldebaranContext/departamentos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartamentosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDepartamentos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/departamentos/excel")]
        [HttpGet("/export/AldebaranContext/departamentos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartamentosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDepartamentos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detcantprocesos/csv")]
        [HttpGet("/export/AldebaranContext/detcantprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetcantprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detcantprocesos/excel")]
        [HttpGet("/export/AldebaranContext/detcantprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetcantprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detdevolpedidos/csv")]
        [HttpGet("/export/AldebaranContext/detdevolpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetdevolpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDetdevolpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detdevolpedidos/excel")]
        [HttpGet("/export/AldebaranContext/detdevolpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetdevolpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDetdevolpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detentregaspacts/csv")]
        [HttpGet("/export/AldebaranContext/detentregaspacts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetentregaspactsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDetentregaspacts(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detentregaspacts/excel")]
        [HttpGet("/export/AldebaranContext/detentregaspacts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetentregaspactsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDetentregaspacts(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detenvios/csv")]
        [HttpGet("/export/AldebaranContext/detenvios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetenviosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/detenvios/excel")]
        [HttpGet("/export/AldebaranContext/detenvios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDetenviosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/devolordens/csv")]
        [HttpGet("/export/AldebaranContext/devolordens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDevolordensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDevolordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/devolordens/excel")]
        [HttpGet("/export/AldebaranContext/devolordens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDevolordensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDevolordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/devolpedidos/csv")]
        [HttpGet("/export/AldebaranContext/devolpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDevolpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDevolpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/devolpedidos/excel")]
        [HttpGet("/export/AldebaranContext/devolpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDevolpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDevolpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/embalajes/csv")]
        [HttpGet("/export/AldebaranContext/embalajes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmbalajesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEmbalajes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/embalajes/excel")]
        [HttpGet("/export/AldebaranContext/embalajes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmbalajesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEmbalajes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/embarqueagentes/csv")]
        [HttpGet("/export/AldebaranContext/embarqueagentes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmbarqueagentesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEmbarqueagentes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/embarqueagentes/excel")]
        [HttpGet("/export/AldebaranContext/embarqueagentes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmbarqueagentesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEmbarqueagentes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/empresas/csv")]
        [HttpGet("/export/AldebaranContext/empresas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmpresasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEmpresas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/empresas/excel")]
        [HttpGet("/export/AldebaranContext/empresas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmpresasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEmpresas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/entregaspacts/csv")]
        [HttpGet("/export/AldebaranContext/entregaspacts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEntregaspactsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEntregaspacts(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/entregaspacts/excel")]
        [HttpGet("/export/AldebaranContext/entregaspacts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEntregaspactsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEntregaspacts(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/envios/csv")]
        [HttpGet("/export/AldebaranContext/envios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEnviosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEnvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/envios/excel")]
        [HttpGet("/export/AldebaranContext/envios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEnviosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEnvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/envioscorreos/csv")]
        [HttpGet("/export/AldebaranContext/envioscorreos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEnvioscorreosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEnvioscorreos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/envioscorreos/excel")]
        [HttpGet("/export/AldebaranContext/envioscorreos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEnvioscorreosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEnvioscorreos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erpdocumenttypes/csv")]
        [HttpGet("/export/AldebaranContext/erpdocumenttypes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErpdocumenttypesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetErpdocumenttypes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erpdocumenttypes/excel")]
        [HttpGet("/export/AldebaranContext/erpdocumenttypes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErpdocumenttypesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetErpdocumenttypes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erpshippingprocesses/csv")]
        [HttpGet("/export/AldebaranContext/erpshippingprocesses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErpshippingprocessesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetErpshippingprocesses(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erpshippingprocesses/excel")]
        [HttpGet("/export/AldebaranContext/erpshippingprocesses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErpshippingprocessesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetErpshippingprocesses(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erpshippingprocessdetails/csv")]
        [HttpGet("/export/AldebaranContext/erpshippingprocessdetails/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErpshippingprocessdetailsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetErpshippingprocessdetails(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erpshippingprocessdetails/excel")]
        [HttpGet("/export/AldebaranContext/erpshippingprocessdetails/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErpshippingprocessdetailsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetErpshippingprocessdetails(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erroresenvioscorreos/csv")]
        [HttpGet("/export/AldebaranContext/erroresenvioscorreos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErroresenvioscorreosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetErroresenvioscorreos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/erroresenvioscorreos/excel")]
        [HttpGet("/export/AldebaranContext/erroresenvioscorreos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportErroresenvioscorreosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetErroresenvioscorreos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/estadoinvinicials/csv")]
        [HttpGet("/export/AldebaranContext/estadoinvinicials/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEstadoinvinicialsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEstadoinvinicials(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/estadoinvinicials/excel")]
        [HttpGet("/export/AldebaranContext/estadoinvinicials/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEstadoinvinicialsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEstadoinvinicials(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/festivos/csv")]
        [HttpGet("/export/AldebaranContext/festivos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFestivosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFestivos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/festivos/excel")]
        [HttpGet("/export/AldebaranContext/festivos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFestivosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFestivos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/forwarders/csv")]
        [HttpGet("/export/AldebaranContext/forwarders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportForwardersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetForwarders(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/forwarders/excel")]
        [HttpGet("/export/AldebaranContext/forwarders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportForwardersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetForwarders(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/funcionarios/csv")]
        [HttpGet("/export/AldebaranContext/funcionarios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFuncionariosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFuncionarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/funcionarios/excel")]
        [HttpGet("/export/AldebaranContext/funcionarios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFuncionariosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFuncionarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/grupopcs/csv")]
        [HttpGet("/export/AldebaranContext/grupopcs/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGrupopcsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGrupopcs(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/grupopcs/excel")]
        [HttpGet("/export/AldebaranContext/grupopcs/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGrupopcsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGrupopcs(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/grupos/csv")]
        [HttpGet("/export/AldebaranContext/grupos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGruposToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGrupos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/grupos/excel")]
        [HttpGet("/export/AldebaranContext/grupos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGruposToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGrupos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/grupusus/csv")]
        [HttpGet("/export/AldebaranContext/grupusus/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGrupususToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGrupusus(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/grupusus/excel")]
        [HttpGet("/export/AldebaranContext/grupusus/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGrupususToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGrupusus(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisactpedidos/csv")]
        [HttpGet("/export/AldebaranContext/hisactpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisActpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisActpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisactpedidos/excel")]
        [HttpGet("/export/AldebaranContext/hisactpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisActpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisActpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisactxactpedidos/csv")]
        [HttpGet("/export/AldebaranContext/hisactxactpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisActxactpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisActxactpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisactxactpedidos/excel")]
        [HttpGet("/export/AldebaranContext/hisactxactpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisActxactpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisActxactpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisanuladetcantprocesos/csv")]
        [HttpGet("/export/AldebaranContext/hisanuladetcantprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisAnuladetcantprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisAnuladetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisanuladetcantprocesos/excel")]
        [HttpGet("/export/AldebaranContext/hisanuladetcantprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisAnuladetcantprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisAnuladetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisanulaprocesos/csv")]
        [HttpGet("/export/AldebaranContext/hisanulaprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisAnulaprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisAnulaprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisanulaprocesos/excel")]
        [HttpGet("/export/AldebaranContext/hisanulaprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisAnulaprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisAnulaprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisanulasubitemdetprocesos/csv")]
        [HttpGet("/export/AldebaranContext/hisanulasubitemdetprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisAnulasubitemdetprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisAnulasubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisanulasubitemdetprocesos/excel")]
        [HttpGet("/export/AldebaranContext/hisanulasubitemdetprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisAnulasubitemdetprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisAnulasubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hiscancelpedidos/csv")]
        [HttpGet("/export/AldebaranContext/hiscancelpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisCancelpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisCancelpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hiscancelpedidos/excel")]
        [HttpGet("/export/AldebaranContext/hiscancelpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisCancelpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisCancelpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hiscantprocesos/csv")]
        [HttpGet("/export/AldebaranContext/hiscantprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisCantprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisCantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hiscantprocesos/excel")]
        [HttpGet("/export/AldebaranContext/hiscantprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisCantprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisCantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisdetcantprocesos/csv")]
        [HttpGet("/export/AldebaranContext/hisdetcantprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisDetcantprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisDetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisdetcantprocesos/excel")]
        [HttpGet("/export/AldebaranContext/hisdetcantprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisDetcantprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisDetcantprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisdetenvios/csv")]
        [HttpGet("/export/AldebaranContext/hisdetenvios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisDetenviosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisDetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisdetenvios/excel")]
        [HttpGet("/export/AldebaranContext/hisdetenvios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisDetenviosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisDetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisenvios/csv")]
        [HttpGet("/export/AldebaranContext/hisenvios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisEnviosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisEnvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisenvios/excel")]
        [HttpGet("/export/AldebaranContext/hisenvios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisEnviosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisEnvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisitempedidos/csv")]
        [HttpGet("/export/AldebaranContext/hisitempedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisItempedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisItempedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisitempedidos/excel")]
        [HttpGet("/export/AldebaranContext/hisitempedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisItempedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisItempedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisitempedidoagotados/csv")]
        [HttpGet("/export/AldebaranContext/hisitempedidoagotados/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisItempedidoagotadosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisItempedidoagotados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisitempedidoagotados/excel")]
        [HttpGet("/export/AldebaranContext/hisitempedidoagotados/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisItempedidoagotadosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisItempedidoagotados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisitemreservas/csv")]
        [HttpGet("/export/AldebaranContext/hisitemreservas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisItemreservasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisItemreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisitemreservas/excel")]
        [HttpGet("/export/AldebaranContext/hisitemreservas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisItemreservasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisItemreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hismodpedidos/csv")]
        [HttpGet("/export/AldebaranContext/hismodpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisModpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisModpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hismodpedidos/excel")]
        [HttpGet("/export/AldebaranContext/hismodpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisModpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisModpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hispedidos/csv")]
        [HttpGet("/export/AldebaranContext/hispedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisPedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisPedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hispedidos/excel")]
        [HttpGet("/export/AldebaranContext/hispedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisPedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisPedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisreservas/csv")]
        [HttpGet("/export/AldebaranContext/hisreservas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisReservasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisReservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hisreservas/excel")]
        [HttpGet("/export/AldebaranContext/hisreservas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisReservasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisReservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hissubitemdetenvios/csv")]
        [HttpGet("/export/AldebaranContext/hissubitemdetenvios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisSubitemdetenviosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisSubitemdetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hissubitemdetenvios/excel")]
        [HttpGet("/export/AldebaranContext/hissubitemdetenvios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisSubitemdetenviosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisSubitemdetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hissubitemdetprocesos/csv")]
        [HttpGet("/export/AldebaranContext/hissubitemdetprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisSubitemdetprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisSubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/hissubitemdetprocesos/excel")]
        [HttpGet("/export/AldebaranContext/hissubitemdetprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisSubitemdetprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisSubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/histerroresenvioscorreos/csv")]
        [HttpGet("/export/AldebaranContext/histerroresenvioscorreos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisterroresenvioscorreosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHisterroresenvioscorreos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/histerroresenvioscorreos/excel")]
        [HttpGet("/export/AldebaranContext/histerroresenvioscorreos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHisterroresenvioscorreosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHisterroresenvioscorreos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/histfinanos/csv")]
        [HttpGet("/export/AldebaranContext/histfinanos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistfinanosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistfinanos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/histfinanos/excel")]
        [HttpGet("/export/AldebaranContext/histfinanos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistfinanosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistfinanos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/histinianos/csv")]
        [HttpGet("/export/AldebaranContext/histinianos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistinianosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistinianos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/histinianos/excel")]
        [HttpGet("/export/AldebaranContext/histinianos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistinianosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistinianos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/horarios/csv")]
        [HttpGet("/export/AldebaranContext/horarios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHorariosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHorarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/horarios/excel")]
        [HttpGet("/export/AldebaranContext/horarios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHorariosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHorarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/integrasaldos/csv")]
        [HttpGet("/export/AldebaranContext/integrasaldos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportIntegrasaldosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetIntegrasaldos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/integrasaldos/excel")]
        [HttpGet("/export/AldebaranContext/integrasaldos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportIntegrasaldosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetIntegrasaldos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemordens/csv")]
        [HttpGet("/export/AldebaranContext/itemordens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemordensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemordens/excel")]
        [HttpGet("/export/AldebaranContext/itemordens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemordensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemordens(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itempedidos/csv")]
        [HttpGet("/export/AldebaranContext/itempedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItempedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItempedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itempedidos/excel")]
        [HttpGet("/export/AldebaranContext/itempedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItempedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItempedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itempedidoagotados/csv")]
        [HttpGet("/export/AldebaranContext/itempedidoagotados/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItempedidoagotadosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItempedidoagotados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itempedidoagotados/excel")]
        [HttpGet("/export/AldebaranContext/itempedidoagotados/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItempedidoagotadosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItempedidoagotados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemreservas/csv")]
        [HttpGet("/export/AldebaranContext/itemreservas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemreservasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemreservas/excel")]
        [HttpGet("/export/AldebaranContext/itemreservas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemreservasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/items/csv")]
        [HttpGet("/export/AldebaranContext/items/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/items/excel")]
        [HttpGet("/export/AldebaranContext/items/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxareas/csv")]
        [HttpGet("/export/AldebaranContext/itemsxareas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxareasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemsxareas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxareas/excel")]
        [HttpGet("/export/AldebaranContext/itemsxareas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxareasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemsxareas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxbodegas/csv")]
        [HttpGet("/export/AldebaranContext/itemsxbodegas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxbodegasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemsxbodegas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxbodegas/excel")]
        [HttpGet("/export/AldebaranContext/itemsxbodegas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxbodegasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemsxbodegas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxcolors/csv")]
        [HttpGet("/export/AldebaranContext/itemsxcolors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxcolorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemsxcolors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxcolors/excel")]
        [HttpGet("/export/AldebaranContext/itemsxcolors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxcolorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemsxcolors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxproveedors/csv")]
        [HttpGet("/export/AldebaranContext/itemsxproveedors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxproveedorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemsxproveedors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxproveedors/excel")]
        [HttpGet("/export/AldebaranContext/itemsxproveedors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxproveedorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemsxproveedors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxtraslados/csv")]
        [HttpGet("/export/AldebaranContext/itemsxtraslados/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxtrasladosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemsxtraslados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemsxtraslados/excel")]
        [HttpGet("/export/AldebaranContext/itemsxtraslados/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsxtrasladosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemsxtraslados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemxitems/csv")]
        [HttpGet("/export/AldebaranContext/itemxitems/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemxitemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItemxitems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/itemxitems/excel")]
        [HttpGet("/export/AldebaranContext/itemxitems/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemxitemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItemxitems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/lineas/csv")]
        [HttpGet("/export/AldebaranContext/lineas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLineasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetLineas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/lineas/excel")]
        [HttpGet("/export/AldebaranContext/lineas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLineasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetLineas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/logalarmascantidadesminimas/csv")]
        [HttpGet("/export/AldebaranContext/logalarmascantidadesminimas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLogalarmascantidadesminimasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetLogalarmascantidadesminimas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/logalarmascantidadesminimas/excel")]
        [HttpGet("/export/AldebaranContext/logalarmascantidadesminimas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLogalarmascantidadesminimasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetLogalarmascantidadesminimas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/mensajesalarmas/csv")]
        [HttpGet("/export/AldebaranContext/mensajesalarmas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMensajesalarmasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMensajesalarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/mensajesalarmas/excel")]
        [HttpGet("/export/AldebaranContext/mensajesalarmas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMensajesalarmasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMensajesalarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/metodoembarques/csv")]
        [HttpGet("/export/AldebaranContext/metodoembarques/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMetodoembarquesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMetodoembarques(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/metodoembarques/excel")]
        [HttpGet("/export/AldebaranContext/metodoembarques/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMetodoembarquesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMetodoembarques(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/metodosenvios/csv")]
        [HttpGet("/export/AldebaranContext/metodosenvios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMetodosenviosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMetodosenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/metodosenvios/excel")]
        [HttpGet("/export/AldebaranContext/metodosenvios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMetodosenviosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMetodosenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/modordenes/csv")]
        [HttpGet("/export/AldebaranContext/modordenes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportModordenesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetModordenes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/modordenes/excel")]
        [HttpGet("/export/AldebaranContext/modordenes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportModordenesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetModordenes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/modpedidos/csv")]
        [HttpGet("/export/AldebaranContext/modpedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportModpedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetModpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/modpedidos/excel")]
        [HttpGet("/export/AldebaranContext/modpedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportModpedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetModpedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/modreservas/csv")]
        [HttpGet("/export/AldebaranContext/modreservas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportModreservasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetModreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/modreservas/excel")]
        [HttpGet("/export/AldebaranContext/modreservas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportModreservasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetModreservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/moneda/csv")]
        [HttpGet("/export/AldebaranContext/moneda/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMonedaToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMoneda(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/moneda/excel")]
        [HttpGet("/export/AldebaranContext/moneda/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMonedaToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMoneda(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/motivajustes/csv")]
        [HttpGet("/export/AldebaranContext/motivajustes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMotivajustesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMotivajustes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/motivajustes/excel")]
        [HttpGet("/export/AldebaranContext/motivajustes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMotivajustesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMotivajustes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/motivodevolucions/csv")]
        [HttpGet("/export/AldebaranContext/motivodevolucions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMotivodevolucionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetMotivodevolucions(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/motivodevolucions/excel")]
        [HttpGet("/export/AldebaranContext/motivodevolucions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportMotivodevolucionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetMotivodevolucions(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opciones/csv")]
        [HttpGet("/export/AldebaranContext/opciones/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOpciones(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opciones/excel")]
        [HttpGet("/export/AldebaranContext/opciones/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOpciones(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opcionesftpv1s/csv")]
        [HttpGet("/export/AldebaranContext/opcionesftpv1s/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionesftpV1SToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOpcionesftpV1S(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opcionesftpv1s/excel")]
        [HttpGet("/export/AldebaranContext/opcionesftpv1s/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionesftpV1SToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOpcionesftpV1S(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opcionesmails/csv")]
        [HttpGet("/export/AldebaranContext/opcionesmails/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionesmailsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOpcionesmails(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opcionesmails/excel")]
        [HttpGet("/export/AldebaranContext/opcionesmails/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionesmailsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOpcionesmails(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opcionessis/csv")]
        [HttpGet("/export/AldebaranContext/opcionessis/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionessisToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOpcionessis(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/opcionessis/excel")]
        [HttpGet("/export/AldebaranContext/opcionessis/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpcionessisToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOpcionessis(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ordenes/csv")]
        [HttpGet("/export/AldebaranContext/ordenes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdenesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOrdenes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ordenes/excel")]
        [HttpGet("/export/AldebaranContext/ordenes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdenesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOrdenes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/paises/csv")]
        [HttpGet("/export/AldebaranContext/paises/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPaisesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPaises(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/paises/excel")]
        [HttpGet("/export/AldebaranContext/paises/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPaisesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPaises(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/pedidos/csv")]
        [HttpGet("/export/AldebaranContext/pedidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPedidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/pedidos/excel")]
        [HttpGet("/export/AldebaranContext/pedidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPedidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPedidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/permisosalarmas/csv")]
        [HttpGet("/export/AldebaranContext/permisosalarmas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPermisosalarmasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPermisosalarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/permisosalarmas/excel")]
        [HttpGet("/export/AldebaranContext/permisosalarmas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPermisosalarmasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPermisosalarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/proveedores/csv")]
        [HttpGet("/export/AldebaranContext/proveedores/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProveedoresToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProveedores(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/proveedores/excel")]
        [HttpGet("/export/AldebaranContext/proveedores/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProveedoresToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProveedores(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rembalajes/csv")]
        [HttpGet("/export/AldebaranContext/rembalajes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRembalajesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRembalajes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rembalajes/excel")]
        [HttpGet("/export/AldebaranContext/rembalajes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRembalajesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRembalajes(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/reservas/csv")]
        [HttpGet("/export/AldebaranContext/reservas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReservasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetReservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/reservas/excel")]
        [HttpGet("/export/AldebaranContext/reservas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReservasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetReservas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rexistencia/csv")]
        [HttpGet("/export/AldebaranContext/rexistencia/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRexistenciaToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRexistencia(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rexistencia/excel")]
        [HttpGet("/export/AldebaranContext/rexistencia/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRexistenciaToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRexistencia(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ritems/csv")]
        [HttpGet("/export/AldebaranContext/ritems/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRitemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRitems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ritems/excel")]
        [HttpGet("/export/AldebaranContext/ritems/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRitemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRitems(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ritemsxcolors/csv")]
        [HttpGet("/export/AldebaranContext/ritemsxcolors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRitemsxcolorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRitemsxcolors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/ritemsxcolors/excel")]
        [HttpGet("/export/AldebaranContext/ritemsxcolors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRitemsxcolorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRitemsxcolors(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rlineas/csv")]
        [HttpGet("/export/AldebaranContext/rlineas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRlineasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRlineas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rlineas/excel")]
        [HttpGet("/export/AldebaranContext/rlineas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRlineasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRlineas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rmoneda/csv")]
        [HttpGet("/export/AldebaranContext/rmoneda/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRmonedaToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRmoneda(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/rmoneda/excel")]
        [HttpGet("/export/AldebaranContext/rmoneda/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRmonedaToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRmoneda(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/runidadesmedida/csv")]
        [HttpGet("/export/AldebaranContext/runidadesmedida/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRunidadesmedidaToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRunidadesmedida(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/runidadesmedida/excel")]
        [HttpGet("/export/AldebaranContext/runidadesmedida/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRunidadesmedidaToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRunidadesmedida(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/satelites/csv")]
        [HttpGet("/export/AldebaranContext/satelites/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSatelitesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSatelites(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/satelites/excel")]
        [HttpGet("/export/AldebaranContext/satelites/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSatelitesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSatelites(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/stransitos/csv")]
        [HttpGet("/export/AldebaranContext/stransitos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStransitosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetStransitos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/stransitos/excel")]
        [HttpGet("/export/AldebaranContext/stransitos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStransitosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetStransitos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/subitemdetenvios/csv")]
        [HttpGet("/export/AldebaranContext/subitemdetenvios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubitemdetenviosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSubitemdetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/subitemdetenvios/excel")]
        [HttpGet("/export/AldebaranContext/subitemdetenvios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubitemdetenviosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSubitemdetenvios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/subitemdetprocesos/csv")]
        [HttpGet("/export/AldebaranContext/subitemdetprocesos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubitemdetprocesosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/subitemdetprocesos/excel")]
        [HttpGet("/export/AldebaranContext/subitemdetprocesos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubitemdetprocesosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSubitemdetprocesos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tempcuadreinventarios/csv")]
        [HttpGet("/export/AldebaranContext/tempcuadreinventarios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTempCuadreinventariosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTempCuadreinventarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tempcuadreinventarios/excel")]
        [HttpGet("/export/AldebaranContext/tempcuadreinventarios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTempCuadreinventariosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTempCuadreinventarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tipidentificas/csv")]
        [HttpGet("/export/AldebaranContext/tipidentificas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTipidentificasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTipidentificas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tipidentificas/excel")]
        [HttpGet("/export/AldebaranContext/tipidentificas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTipidentificasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTipidentificas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tiposactividads/csv")]
        [HttpGet("/export/AldebaranContext/tiposactividads/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTiposactividadsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTiposactividads(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tiposactividads/excel")]
        [HttpGet("/export/AldebaranContext/tiposactividads/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTiposactividadsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTiposactividads(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tiposactxareas/csv")]
        [HttpGet("/export/AldebaranContext/tiposactxareas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTiposactxareasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTiposactxareas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tiposactxareas/excel")]
        [HttpGet("/export/AldebaranContext/tiposactxareas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTiposactxareasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTiposactxareas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tiposalarmas/csv")]
        [HttpGet("/export/AldebaranContext/tiposalarmas/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTiposalarmasToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTiposalarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/tiposalarmas/excel")]
        [HttpGet("/export/AldebaranContext/tiposalarmas/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTiposalarmasToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTiposalarmas(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/traslados/csv")]
        [HttpGet("/export/AldebaranContext/traslados/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTrasladosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTraslados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/traslados/excel")]
        [HttpGet("/export/AldebaranContext/traslados/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTrasladosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTraslados(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/unidadesmedida/csv")]
        [HttpGet("/export/AldebaranContext/unidadesmedida/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUnidadesmedidaToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUnidadesmedida(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/unidadesmedida/excel")]
        [HttpGet("/export/AldebaranContext/unidadesmedida/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUnidadesmedidaToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUnidadesmedida(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/usuarios/csv")]
        [HttpGet("/export/AldebaranContext/usuarios/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsuariosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUsuarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/usuarios/excel")]
        [HttpGet("/export/AldebaranContext/usuarios/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsuariosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUsuarios(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/usuarioscorreoseguimientos/csv")]
        [HttpGet("/export/AldebaranContext/usuarioscorreoseguimientos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsuarioscorreoseguimientosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUsuarioscorreoseguimientos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/usuarioscorreoseguimientos/excel")]
        [HttpGet("/export/AldebaranContext/usuarioscorreoseguimientos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsuarioscorreoseguimientosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUsuarioscorreoseguimientos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/validacioncomprometidos/csv")]
        [HttpGet("/export/AldebaranContext/validacioncomprometidos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportValidacioncomprometidosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetValidacioncomprometidos(), Request.Query), fileName);
        }

        [HttpGet("/export/AldebaranContext/validacioncomprometidos/excel")]
        [HttpGet("/export/AldebaranContext/validacioncomprometidos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportValidacioncomprometidosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetValidacioncomprometidos(), Request.Query), fileName);
        }
    }
}
