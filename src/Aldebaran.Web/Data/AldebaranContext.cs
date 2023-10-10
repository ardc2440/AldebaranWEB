using Microsoft.EntityFrameworkCore;

namespace Aldebaran.Web.Data
{
    public partial class AldebaranContext : DbContext
    {
        public AldebaranContext()
        {
        }

        public AldebaranContext(DbContextOptions<AldebaranContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajustesinv>().HasNoKey();

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Auxactorden>().HasNoKey();

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Auxitemsxcolor>().HasNoKey();

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Auxordene>().HasNoKey();

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Estadoinvinicial>().HasNoKey();

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Opcionessi>().HasNoKey();

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actxactpedido>().HasKey(table => new
            {
                table.IDTIPOACTIVIDAD,
                table.IDACTPEDIDO
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium>().HasKey(table => new
            {
                table.TIPO,
                table.IDTABLA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Grupopc>().HasKey(table => new
            {
                table.IDGRUPO,
                table.IDOPCION
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Grupusu>().HasKey(table => new
            {
                table.IDGRUPO,
                table.IDUSUARIO
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido>().HasKey(table => new
            {
                table.IDTIPOACTIVIDAD,
                table.IDACTPEDIDO
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisModpedido>().HasKey(table => new
            {
                table.IDPEDIDO,
                table.IDFUNCIONARIO,
                table.FECHA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Histfinano>().HasKey(table => new
            {
                table.ANNO,
                table.SEMESTRE,
                table.IDITEMXCOLOR,
                table.IDBODEGA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Histiniano>().HasKey(table => new
            {
                table.ANNO,
                table.SEMESTRE,
                table.IDITEMXCOLOR,
                table.IDBODEGA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Integrasaldo>().HasKey(table => new
            {
                table.IDITEMXCOLOR,
                table.IDBODEGA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxarea>().HasKey(table => new
            {
                table.IDITEM,
                table.IDAREA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega>().HasKey(table => new
            {
                table.IDITEMXCOLOR,
                table.IDBODEGA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor>().HasKey(table => new
            {
                table.IDITEM,
                table.IDPROVEEDOR
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima>().HasKey(table => new
            {
                table.IDALARMA,
                table.IDFUNCIONARIO
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma>().HasKey(table => new
            {
                table.IDMENSAJE,
                table.IDTIPOALARMA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modpedido>().HasKey(table => new
            {
                table.IDPEDIDO,
                table.IDFUNCIONARIO,
                table.FECHA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modreserva>().HasKey(table => new
            {
                table.IDRESERVA,
                table.IDFUNCIONARIO,
                table.FECHA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Permisosalarma>().HasKey(table => new
            {
                table.IDTIPOALARMA,
                table.IDUSUARIO
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea>().HasKey(table => new
            {
                table.IDTIPOACTIVIDAD,
                table.IDAREA
            });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actorden>()
              .HasOne(i => i.Ordene)
              .WithMany(i => i.Actordens)
              .HasForeignKey(i => i.IDORDEN)
              .HasPrincipalKey(i => i.IDORDEN);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actpedido>()
              .HasOne(i => i.Area)
              .WithMany(i => i.Actpedidos)
              .HasForeignKey(i => i.IDAREA)
              .HasPrincipalKey(i => i.IDAREA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actpedido>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Actpedidos)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actpedido>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Actpedidos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actxactpedido>()
              .HasOne(i => i.Actpedido)
              .WithMany(i => i.Actxactpedidos)
              .HasForeignKey(i => i.IDACTPEDIDO)
              .HasPrincipalKey(i => i.IDACTPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actxactpedido>()
              .HasOne(i => i.Tiposactividad)
              .WithMany(i => i.Actxactpedidos)
              .HasForeignKey(i => i.IDTIPOACTIVIDAD)
              .HasPrincipalKey(i => i.IDTIPOACTIVIDAD);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder>()
              .HasOne(i => i.Forwarder)
              .WithMany(i => i.Agentesforwarders)
              .HasForeignKey(i => i.IDFORWARDER)
              .HasPrincipalKey(i => i.IDFORWARDER);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder>()
              .HasOne(i => i.Paise)
              .WithMany(i => i.Agentesforwarders)
              .HasForeignKey(i => i.IDPAIS)
              .HasPrincipalKey(i => i.IDPAIS);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajuste>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Ajustes)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajuste>()
              .HasOne(i => i.Motivajuste)
              .WithMany(i => i.Ajustes)
              .HasForeignKey(i => i.IDMOTIVAJUSTE)
              .HasPrincipalKey(i => i.IDMOTIVAJUSTE);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem>()
              .HasOne(i => i.Ajuste)
              .WithMany(i => i.Ajustesxitems)
              .HasForeignKey(i => i.IDAJUSTE)
              .HasPrincipalKey(i => i.IDAJUSTE);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Ajustesxitems)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Ajustesxitems)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Alarma>()
              .HasOne(i => i.Tiposalarma)
              .WithMany(i => i.Alarmas)
              .HasForeignKey(i => i.IDTIPOALARMA)
              .HasPrincipalKey(i => i.IDTIPO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Alarmascantidadesminimas)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Anulacionreservas)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva>()
              .HasOne(i => i.Motivodevolucion)
              .WithMany(i => i.Anulacionreservas)
              .HasForeignKey(i => i.IDMOTIVO)
              .HasPrincipalKey(i => i.IDMOTIVODEV);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva>()
              .HasOne(i => i.Reserva)
              .WithMany(i => i.Anulacionreservas)
              .HasForeignKey(i => i.IDRESERVA)
              .HasPrincipalKey(i => i.IDRESERVA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Anuladetcantprocesos)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Anuladetcantprocesos)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Anuladetcantprocesos)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso>()
              .HasOne(i => i.Cantproceso)
              .WithMany(i => i.Anuladetcantprocesos)
              .HasForeignKey(i => i.IDPROCESO)
              .HasPrincipalKey(i => i.IDPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulaproceso>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Anulaprocesos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulaproceso>()
              .HasOne(i => i.Cantproceso)
              .WithMany(i => i.Anulaprocesos)
              .HasForeignKey(i => i.IDPROCESO)
              .HasPrincipalKey(i => i.IDPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulaproceso>()
              .HasOne(i => i.Satelite)
              .WithMany(i => i.Anulaprocesos)
              .HasForeignKey(i => i.IDSATELITE)
              .HasPrincipalKey(i => i.IDSATELITE);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Anulaproceso)
              .WithMany(i => i.Anulasubitemdetprocesos)
              .HasForeignKey(i => i.IDANULAPROCESO)
              .HasPrincipalKey(i => i.IDANULAPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Anulasubitemdetprocesos)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Anuladetcantproceso)
              .WithMany(i => i.Anulasubitemdetprocesos)
              .HasForeignKey(i => i.IDDETANULAPROCESO)
              .HasPrincipalKey(i => i.IDDETANULAPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Detcantproceso)
              .WithMany(i => i.Anulasubitemdetprocesos)
              .HasForeignKey(i => i.IDDETPROCESO)
              .HasPrincipalKey(i => i.IDDETCANTPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Anulasubitemdetprocesos)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Anulasubitemdetprocesos)
              .HasForeignKey(i => i.IDITEMARMADO)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Itemsxcolor1)
              .WithMany(i => i.Anulasubitemdetprocesos1)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso>()
              .HasOne(i => i.Cantproceso)
              .WithMany(i => i.Anulasubitemdetprocesos)
              .HasForeignKey(i => i.IDPROCESO)
              .HasPrincipalKey(i => i.IDPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cancelpedido>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Cancelpedidos)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cancelpedido>()
              .HasOne(i => i.Motivodevolucion)
              .WithMany(i => i.Cancelpedidos)
              .HasForeignKey(i => i.IDMOTIVO)
              .HasPrincipalKey(i => i.IDMOTIVODEV);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cancelpedido>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Cancelpedidos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cantproceso>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Cantprocesos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cantproceso>()
              .HasOne(i => i.Satelite)
              .WithMany(i => i.Cantprocesos)
              .HasForeignKey(i => i.IDSATELITE)
              .HasPrincipalKey(i => i.IDSATELITE);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cierrepedido>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Cierrepedidos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ciudade>()
              .HasOne(i => i.Departamento)
              .WithMany(i => i.Ciudades)
              .HasForeignKey(i => i.IDDEPTO)
              .HasPrincipalKey(i => i.IDDEPTO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cliente>()
              .HasOne(i => i.Ciudade)
              .WithMany(i => i.Clientes)
              .HasForeignKey(i => i.IDCIUDAD)
              .HasPrincipalKey(i => i.IDCIUDAD);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cliente>()
              .HasOne(i => i.Tipidentifica)
              .WithMany(i => i.Clientes)
              .HasForeignKey(i => i.IDTIPIDENTIFICA)
              .HasPrincipalKey(i => i.IDTIPIDENTIFICA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detcantproceso>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Detcantprocesos)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detcantproceso>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Detcantprocesos)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detcantproceso>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Detcantprocesos)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detcantproceso>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Detcantprocesos)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detcantproceso>()
              .HasOne(i => i.Cantproceso)
              .WithMany(i => i.Detcantprocesos)
              .HasForeignKey(i => i.IDPROCESO)
              .HasPrincipalKey(i => i.IDPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido>()
              .HasOne(i => i.Devolpedido)
              .WithMany(i => i.Detdevolpedidos)
              .HasForeignKey(i => i.IDDEVOLPEDIDO)
              .HasPrincipalKey(i => i.IDDEVOLPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido>()
              .HasOne(i => i.Motivodevolucion)
              .WithMany(i => i.Detdevolpedidos)
              .HasForeignKey(i => i.IDMOTIVODEV)
              .HasPrincipalKey(i => i.IDMOTIVODEV);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido>()
              .HasOne(i => i.Itemsxbodega)
              .WithMany(i => i.Detdevolpedidos)
              .HasForeignKey(i => new { i.IDITEMXCOLOR, i.IDBODEGA })
              .HasPrincipalKey(i => new { i.IDITEMXCOLOR, i.IDBODEGA });

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detentregaspact>()
              .HasOne(i => i.Entregaspact)
              .WithMany(i => i.Detentregaspacts)
              .HasForeignKey(i => i.IDENTREGAPACT)
              .HasPrincipalKey(i => i.IDENTREGAPACT);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detentregaspact>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Detentregaspacts)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detenvio>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Detenvios)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detenvio>()
              .HasOne(i => i.Envio)
              .WithMany(i => i.Detenvios)
              .HasForeignKey(i => i.IDENVIO)
              .HasPrincipalKey(i => i.IDENVIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detenvio>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Detenvios)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detenvio>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Detenvios)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Detenvio>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Detenvios)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Devolorden>()
              .HasOne(i => i.Motivodevolucion)
              .WithMany(i => i.Devolordens)
              .HasForeignKey(i => i.IDMOTIVODEV)
              .HasPrincipalKey(i => i.IDMOTIVODEV);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Devolorden>()
              .HasOne(i => i.Ordene)
              .WithMany(i => i.Devolordens)
              .HasForeignKey(i => i.IDORDEN)
              .HasPrincipalKey(i => i.IDORDEN);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Devolpedido>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Devolpedidos)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Devolpedido>()
              .HasOne(i => i.Motivodevolucion)
              .WithMany(i => i.Devolpedidos)
              .HasForeignKey(i => i.IDMOTIVODEV)
              .HasPrincipalKey(i => i.IDMOTIVODEV);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Devolpedido>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Devolpedidos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Embalaje>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Embalajes)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Embarqueagente>()
              .HasOne(i => i.Agentesforwarder)
              .WithMany(i => i.Embarqueagentes)
              .HasForeignKey(i => i.IDAGENTEFORWARDER)
              .HasPrincipalKey(i => i.IDAGENTEFORWARDER);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Embarqueagente>()
              .HasOne(i => i.Metodoembarque)
              .WithMany(i => i.Embarqueagentes)
              .HasForeignKey(i => i.IDMETEMBARQUE)
              .HasPrincipalKey(i => i.IDMETEMBARQUE);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Entregaspact>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Entregaspacts)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Entregaspact>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Entregaspacts)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envio>()
              .HasOne(i => i.Metodosenvio)
              .WithMany(i => i.Envios)
              .HasForeignKey(i => i.IDMETODOENV)
              .HasPrincipalKey(i => i.IDMETODOENV);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envio>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Envios)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envioscorreo>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Envioscorreos)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail>()
              .HasOne(i => i.Erpdocumenttype)
              .WithMany(i => i.Erpshippingprocessdetails)
              .HasForeignKey(i => i.IDERPDOCUMENTTYPE)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail>()
              .HasOne(i => i.Erpshippingprocess)
              .WithMany(i => i.Erpshippingprocessdetails)
              .HasForeignKey(i => i.IDERPSHIPPINGPROCESS)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Forwarder>()
              .HasOne(i => i.Ciudade)
              .WithMany(i => i.Forwarders)
              .HasForeignKey(i => i.IDCIUDAD)
              .HasPrincipalKey(i => i.IDCIUDAD);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Funcionario>()
              .HasOne(i => i.Area)
              .WithMany(i => i.Funcionarios)
              .HasForeignKey(i => i.IDAREA)
              .HasPrincipalKey(i => i.IDAREA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Funcionario>()
              .HasOne(i => i.Tipidentifica)
              .WithMany(i => i.Funcionarios)
              .HasForeignKey(i => i.IDTIPIDENTIFICA)
              .HasPrincipalKey(i => i.IDTIPIDENTIFICA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Grupopc>()
              .HasOne(i => i.Grupo)
              .WithMany(i => i.Grupopcs)
              .HasForeignKey(i => i.IDGRUPO)
              .HasPrincipalKey(i => i.IDGRUPO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Grupopc>()
              .HasOne(i => i.Opcione)
              .WithMany(i => i.Grupopcs)
              .HasForeignKey(i => i.IDOPCION)
              .HasPrincipalKey(i => i.IDOPCION);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Grupusu>()
              .HasOne(i => i.Grupo)
              .WithMany(i => i.Grupusus)
              .HasForeignKey(i => i.IDGRUPO)
              .HasPrincipalKey(i => i.IDGRUPO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Grupusu>()
              .HasOne(i => i.Usuario)
              .WithMany(i => i.Grupusus)
              .HasForeignKey(i => i.IDUSUARIO)
              .HasPrincipalKey(i => i.IDUSUARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Horario>()
              .HasOne(i => i.Grupo)
              .WithMany(i => i.Horarios)
              .HasForeignKey(i => i.IDGRUPO)
              .HasPrincipalKey(i => i.IDGRUPO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Integrasaldo>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Integrasaldos)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Integrasaldo>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Integrasaldos)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Integrasaldo>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Integrasaldos)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemorden>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Itemordens)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemorden>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Itemordens)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemorden>()
              .HasOne(i => i.Ordene)
              .WithMany(i => i.Itemordens)
              .HasForeignKey(i => i.IDORDEN)
              .HasPrincipalKey(i => i.IDORDEN);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedido>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Itempedidos)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedido>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Itempedidos)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedido>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Itempedidos)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedido>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Itempedidos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Itempedidoagotados)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Itempedidoagotados)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Itempedidoagotados)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Itempedidoagotados)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemreserva>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Itemreservas)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemreserva>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Itemreservas)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemreserva>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Itemreservas)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemreserva>()
              .HasOne(i => i.Reserva)
              .WithMany(i => i.Itemreservas)
              .HasForeignKey(i => i.IDRESERVA)
              .HasPrincipalKey(i => i.IDRESERVA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Item>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Item>()
              .HasOne(i => i.Moneda)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.IDMONEDA)
              .HasPrincipalKey(i => i.IDMONEDA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Item>()
              .HasOne(i => i.Unidadesmedidum)
              .WithMany(i => i.Items)
              .HasForeignKey(i => i.IDUNIDADCIF)
              .HasPrincipalKey(i => i.IDUNIDAD);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Item>()
              .HasOne(i => i.Unidadesmedidum1)
              .WithMany(i => i.Items1)
              .HasForeignKey(i => i.IDUNIDADFOB)
              .HasPrincipalKey(i => i.IDUNIDAD);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxarea>()
              .HasOne(i => i.Area)
              .WithMany(i => i.Itemsxareas)
              .HasForeignKey(i => i.IDAREA)
              .HasPrincipalKey(i => i.IDAREA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxarea>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Itemsxareas)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Itemsxbodegas)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Itemsxbodegas)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Itemsxcolors)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Itemsxproveedors)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor>()
              .HasOne(i => i.Proveedore)
              .WithMany(i => i.Itemsxproveedors)
              .HasForeignKey(i => i.IDPROVEEDOR)
              .HasPrincipalKey(i => i.IDPROVEEDOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Itemsxtraslados)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Itemsxtraslados)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado>()
              .HasOne(i => i.Linea)
              .WithMany(i => i.Itemsxtraslados)
              .HasForeignKey(i => i.IDLINEA)
              .HasPrincipalKey(i => i.IDLINEA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado>()
              .HasOne(i => i.Traslado)
              .WithMany(i => i.Itemsxtraslados)
              .HasForeignKey(i => i.IDTRASLADO)
              .HasPrincipalKey(i => i.IDTRASLADO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemxitem>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Itemxitems)
              .HasForeignKey(i => i.IDPRODUCTO)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemxitem>()
              .HasOne(i => i.Itemsxcolor1)
              .WithMany(i => i.Itemxitems1)
              .HasForeignKey(i => i.IDSUBPRODUCTO)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Logalarmascantidadesminimas)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modordene>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Modordenes)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modordene>()
              .HasOne(i => i.Ordene)
              .WithMany(i => i.Modordenes)
              .HasForeignKey(i => i.IDORDEN)
              .HasPrincipalKey(i => i.IDORDEN);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modpedido>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Modpedidos)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modpedido>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Modpedidos)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modreserva>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Modreservas)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modreserva>()
              .HasOne(i => i.Reserva)
              .WithMany(i => i.Modreservas)
              .HasForeignKey(i => i.IDRESERVA)
              .HasPrincipalKey(i => i.IDRESERVA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .HasOne(i => i.Agentesforwarder)
              .WithMany(i => i.Ordenes)
              .HasForeignKey(i => i.IDAGENTEFORWARDER)
              .HasPrincipalKey(i => i.IDAGENTEFORWARDER);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .HasOne(i => i.Empresa)
              .WithMany(i => i.Ordenes)
              .HasForeignKey(i => i.IDEMPRESA)
              .HasPrincipalKey(i => i.IDEMPRESA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Ordenes)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Pedido>()
              .HasOne(i => i.Cliente)
              .WithMany(i => i.Pedidos)
              .HasForeignKey(i => i.IDCLIENTE)
              .HasPrincipalKey(i => i.IDCLIENTE);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Pedido>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Pedidos)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Permisosalarma>()
              .HasOne(i => i.Tiposalarma)
              .WithMany(i => i.Permisosalarmas)
              .HasForeignKey(i => i.IDTIPOALARMA)
              .HasPrincipalKey(i => i.IDTIPO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Proveedore>()
              .HasOne(i => i.Tipidentifica)
              .WithMany(i => i.Proveedores)
              .HasForeignKey(i => i.IDTIPIDENTIFICA)
              .HasPrincipalKey(i => i.IDTIPIDENTIFICA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Reserva>()
              .HasOne(i => i.Cliente)
              .WithMany(i => i.Reservas)
              .HasForeignKey(i => i.IDCLIENTE)
              .HasPrincipalKey(i => i.IDCLIENTE);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Reserva>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Reservas)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Reserva>()
              .HasOne(i => i.Pedido)
              .WithMany(i => i.Reservas)
              .HasForeignKey(i => i.IDPEDIDO)
              .HasPrincipalKey(i => i.IDPEDIDO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Satelite>()
              .HasOne(i => i.Ciudade)
              .WithMany(i => i.Satelites)
              .HasForeignKey(i => i.IDCIUDAD)
              .HasPrincipalKey(i => i.IDCIUDAD);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Satelite>()
              .HasOne(i => i.Tipidentifica)
              .WithMany(i => i.Satelites)
              .HasForeignKey(i => i.IDTIPIDENTIFICA)
              .HasPrincipalKey(i => i.IDTIPIDENTIFICA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Subitemdetenvios)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio>()
              .HasOne(i => i.Envio)
              .WithMany(i => i.Subitemdetenvios)
              .HasForeignKey(i => i.IDENVIO)
              .HasPrincipalKey(i => i.IDENVIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Subitemdetenvios)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Subitemdetenvios)
              .HasForeignKey(i => i.IDITEMARMADO)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio>()
              .HasOne(i => i.Itemsxcolor1)
              .WithMany(i => i.Subitemdetenvios1)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Subitemdetprocesos)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso>()
              .HasOne(i => i.Item)
              .WithMany(i => i.Subitemdetprocesos)
              .HasForeignKey(i => i.IDITEM)
              .HasPrincipalKey(i => i.IDITEM);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso>()
              .HasOne(i => i.Itemsxcolor)
              .WithMany(i => i.Subitemdetprocesos)
              .HasForeignKey(i => i.IDITEMARMADO)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso>()
              .HasOne(i => i.Itemsxcolor1)
              .WithMany(i => i.Subitemdetprocesos1)
              .HasForeignKey(i => i.IDITEMXCOLOR)
              .HasPrincipalKey(i => i.IDITEMXCOLOR);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso>()
              .HasOne(i => i.Cantproceso)
              .WithMany(i => i.Subitemdetprocesos)
              .HasForeignKey(i => i.IDPROCESO)
              .HasPrincipalKey(i => i.IDPROCESO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea>()
              .HasOne(i => i.Area)
              .WithMany(i => i.Tiposactxareas)
              .HasForeignKey(i => i.IDAREA)
              .HasPrincipalKey(i => i.IDAREA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea>()
              .HasOne(i => i.Tiposactividad)
              .WithMany(i => i.Tiposactxareas)
              .HasForeignKey(i => i.IDTIPOACTIVIDAD)
              .HasPrincipalKey(i => i.IDTIPOACTIVIDAD);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Traslado>()
              .HasOne(i => i.Bodega)
              .WithMany(i => i.Traslados)
              .HasForeignKey(i => i.IDBODEGA)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Traslado>()
              .HasOne(i => i.Bodega1)
              .WithMany(i => i.Traslados1)
              .HasForeignKey(i => i.IDBODEGADEST)
              .HasPrincipalKey(i => i.IDBODEGA);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Usuario>()
              .HasOne(i => i.Funcionario)
              .WithMany(i => i.Usuarios)
              .HasForeignKey(i => i.IDFUNCIONARIO)
              .HasPrincipalKey(i => i.IDFUNCIONARIO);

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actorden>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actpedido>()
              .Property(p => p.FECHA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajuste>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Alarma>()
              .Property(p => p.ACTIVA)
              .HasDefaultValueSql(@"('S')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Auxordene>()
              .Property(p => p.ESTADO)
              .HasDefaultValueSql(@"('I')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cantproceso>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cliente>()
              .Property(p => p.ENVIARMAIL)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium>()
              .Property(p => p.FECHABLOQUEO)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envio>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Estadoinvinicial>()
              .Property(p => p.ESTADO)
              .HasDefaultValueSql(@"('A')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisActpedido>()
              .Property(p => p.FECHA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisCantproceso>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisEnvio>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisItempedido>()
              .Property(p => p.CANTIDADENPROC)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisItemreserva>()
              .Property(p => p.ENVIARAPED)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisItemreserva>()
              .Property(p => p.ESTADO)
              .HasDefaultValueSql(@"('R')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisModpedido>()
              .Property(p => p.FECHA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisPedido>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisReserva>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itempedido>()
              .Property(p => p.CANTIDADENPROC)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemreserva>()
              .Property(p => p.ENVIARAPED)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemreserva>()
              .Property(p => p.ESTADO)
              .HasDefaultValueSql(@"('R')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Item>()
              .Property(p => p.ACTIVO)
              .HasDefaultValueSql(@"('S')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Item>()
              .Property(p => p.CATALOGOVISIBLE)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor>()
              .Property(p => p.ACTIVO)
              .HasDefaultValueSql(@"('S')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor>()
              .Property(p => p.AGOTADO)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor>()
              .Property(p => p.USAALARMACANTIDADMINIMA)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Linea>()
              .Property(p => p.ACTIVO)
              .HasDefaultValueSql(@"('S')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modordene>()
              .Property(p => p.FECHA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modpedido>()
              .Property(p => p.FECHA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modreserva>()
              .Property(p => p.FECHA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Opcionesmail>()
              .Property(p => p.CORREOSINCEXISTENCIAS)
              .HasDefaultValueSql(@"(' ')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Opcionesmail>()
              .Property(p => p.USUARIOEXISTENCIAS)
              .HasDefaultValueSql(@"(' ')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Opcionesmail>()
              .Property(p => p.PASSWORDEXISTENCIAS)
              .HasDefaultValueSql(@"(' ')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Opcionesmail>()
              .Property(p => p.SUBIREXISTENCIAS)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Opcionesmail>()
              .Property(p => p.INICIOMONITORAUTOMATICO)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .Property(p => p.ESTADO)
              .HasDefaultValueSql(@"('I')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .Property(p => p.PUERTOEMBARQUE)
              .HasDefaultValueSql(@"(' ')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .Property(p => p.NROPROFORMA)
              .HasDefaultValueSql(@"(' ')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Pedido>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Permisosalarma>()
              .Property(p => p.VISUALIZA)
              .HasDefaultValueSql(@"('S')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Permisosalarma>()
              .Property(p => p.DESACTIVA)
              .HasDefaultValueSql(@"('N')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rembalaje>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Reserva>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rexistencia>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ritem>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rlinea>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rmoneda>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Stransito>()
              .Property(p => p.FECHA_INTEGRA)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Tiposalarma>()
              .Property(p => p.DESCRIPCION)
              .HasDefaultValueSql(@"('')");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Traslado>()
              .Property(p => p.FECHACREACION)
              .HasDefaultValueSql(@"(getdate())");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actorden>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Actpedido>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajuste>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ajustesinv>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima>()
              .Property(p => p.FECHAALARMA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Anulaproceso>()
              .Property(p => p.FECHAANULAPROCESO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Auxactorden>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Auxordene>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cantproceso>()
              .Property(p => p.FECHAPROCESO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cantproceso>()
              .Property(p => p.FECHAHORATRASLADO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cantproceso>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Cierrepedido>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium>()
              .Property(p => p.FECHABLOQUEO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Devolorden>()
              .Property(p => p.FECHADEV)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envio>()
              .Property(p => p.FECHAENVIO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envio>()
              .Property(p => p.FECHAHORA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envio>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Envioscorreo>()
              .Property(p => p.FECHAENVIO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype>()
              .Property(p => p.LASTEXECUTIONDATE)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype>()
              .Property(p => p.LASTCLEANINGDATE)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess>()
              .Property(p => p.EXECUTIONDATE)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo>()
              .Property(p => p.FECHAERROR)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisActpedido>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso>()
              .Property(p => p.FECHAANULAPROCESO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisCantproceso>()
              .Property(p => p.FECHAPROCESO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisCantproceso>()
              .Property(p => p.FECHAHORATRASLADO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisCantproceso>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisEnvio>()
              .Property(p => p.FECHAENVIO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisEnvio>()
              .Property(p => p.FECHAHORA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisEnvio>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisModpedido>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisPedido>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.HisReserva>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo>()
              .Property(p => p.FECHAERROR)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Horario>()
              .Property(p => p.INIPRIMPER)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Horario>()
              .Property(p => p.FINPRIMPER)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Horario>()
              .Property(p => p.INISEGPER)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Horario>()
              .Property(p => p.FINSEGPER)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima>()
              .Property(p => p.FECHAREVISION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modordene>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modpedido>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Modreserva>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ordene>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Pedido>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rembalaje>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Reserva>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rexistencia>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ritem>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rlinea>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Rmoneda>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Stransito>()
              .Property(p => p.FECHAESTRECIBO)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Stransito>()
              .Property(p => p.FECHA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Stransito>()
              .Property(p => p.FECHA_INTEGRA)
              .HasColumnType("datetime");

            builder.Entity<Aldebaran.Web.Models.AldebaranContext.Traslado>()
              .Property(p => p.FECHACREACION)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Actorden> Actordens { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Actpedido> Actpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Actxactpedido> Actxactpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Agentesforwarder> Agentesforwarders { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Ajuste> Ajustes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Ajustesinv> Ajustesinvs { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Ajustesxitem> Ajustesxitems { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Alarma> Alarmas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Alarmascantidadesminima> Alarmascantidadesminimas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Anulacionreserva> Anulacionreservas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Anuladetcantproceso> Anuladetcantprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Anulaproceso> Anulaprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Anulasubitemdetproceso> Anulasubitemdetprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Area> Areas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Auxactorden> Auxactordens { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Auxitemsxcolor> Auxitemsxcolors { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Auxordene> Auxordenes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Bodega> Bodegas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Cancelpedido> Cancelpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Cantproceso> Cantprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Cierrepedido> Cierrepedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Ciudade> Ciudades { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Cliente> Clientes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Consecutivo> Consecutivos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Contacto> Contactos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Controlconcurrencium> Controlconcurrencia { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Departamento> Departamentos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Detcantproceso> Detcantprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Detdevolpedido> Detdevolpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Detentregaspact> Detentregaspacts { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Detenvio> Detenvios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Devolorden> Devolordens { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Devolpedido> Devolpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Embalaje> Embalajes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Embarqueagente> Embarqueagentes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Empresa> Empresas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Entregaspact> Entregaspacts { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Envio> Envios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Envioscorreo> Envioscorreos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Erpdocumenttype> Erpdocumenttypes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocess> Erpshippingprocesses { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Erpshippingprocessdetail> Erpshippingprocessdetails { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Erroresenvioscorreo> Erroresenvioscorreos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Estadoinvinicial> Estadoinvinicials { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Festivo> Festivos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Forwarder> Forwarders { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Funcionario> Funcionarios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Grupopc> Grupopcs { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Grupo> Grupos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Grupusu> Grupusus { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisActpedido> HisActpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisActxactpedido> HisActxactpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisAnuladetcantproceso> HisAnuladetcantprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisAnulaproceso> HisAnulaprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisAnulasubitemdetproceso> HisAnulasubitemdetprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisCancelpedido> HisCancelpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisCantproceso> HisCantprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisDetcantproceso> HisDetcantprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisDetenvio> HisDetenvios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisEnvio> HisEnvios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisItempedido> HisItempedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisItempedidoagotado> HisItempedidoagotados { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisItemreserva> HisItemreservas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisModpedido> HisModpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisPedido> HisPedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisReserva> HisReservas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetenvio> HisSubitemdetenvios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.HisSubitemdetproceso> HisSubitemdetprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Histerroresenvioscorreo> Histerroresenvioscorreos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Histfinano> Histfinanos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Histiniano> Histinianos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Horario> Horarios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Integrasaldo> Integrasaldos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemorden> Itemordens { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itempedido> Itempedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itempedidoagotado> Itempedidoagotados { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemreserva> Itemreservas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Item> Items { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemsxarea> Itemsxareas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemsxbodega> Itemsxbodegas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemsxcolor> Itemsxcolors { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemsxproveedor> Itemsxproveedors { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemsxtraslado> Itemsxtraslados { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Itemxitem> Itemxitems { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Linea> Lineas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Logalarmascantidadesminima> Logalarmascantidadesminimas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Mensajesalarma> Mensajesalarmas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Metodoembarque> Metodoembarques { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Metodosenvio> Metodosenvios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Modordene> Modordenes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Modpedido> Modpedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Modreserva> Modreservas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Moneda> Moneda { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Motivajuste> Motivajustes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Motivodevolucion> Motivodevolucions { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Opcione> Opciones { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.OpcionesftpV1> OpcionesftpV1S { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Opcionesmail> Opcionesmails { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Opcionessi> Opcionessis { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Ordene> Ordenes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Paise> Paises { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Pedido> Pedidos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Permisosalarma> Permisosalarmas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Proveedore> Proveedores { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Rembalaje> Rembalajes { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Reserva> Reservas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Rexistencia> Rexistencia { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Ritem> Ritems { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Ritemsxcolor> Ritemsxcolors { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Rlinea> Rlineas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Rmoneda> Rmoneda { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Runidadesmedidum> Runidadesmedida { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Satelite> Satelites { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Stransito> Stransitos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Subitemdetenvio> Subitemdetenvios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Subitemdetproceso> Subitemdetprocesos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.TempCuadreinventario> TempCuadreinventarios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Tipidentifica> Tipidentificas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Tiposactividad> Tiposactividads { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Tiposactxarea> Tiposactxareas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Tiposalarma> Tiposalarmas { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Traslado> Traslados { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Unidadesmedidum> Unidadesmedida { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Usuario> Usuarios { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Usuarioscorreoseguimiento> Usuarioscorreoseguimientos { get; set; }

        public DbSet<Aldebaran.Web.Models.AldebaranContext.Validacioncomprometido> Validacioncomprometidos { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }

    }
}