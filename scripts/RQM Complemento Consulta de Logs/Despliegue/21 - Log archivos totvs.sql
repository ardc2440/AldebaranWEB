/* =====================================================================================
   RQM Complemento Consulta de Logs - Log de ArchivosTotvs en el Log general
   -------------------------------------------------------------------------------------
   El servicio ArchivosTotvs (Totvs.FlatFileGenerator, NLog) escribe en log.Logs con origen
   'ArchivosTotvs' mediante este SP (target Database de su NLog.config). Misma lógica del script 13:
   - Nivel NLog Info/Warn → Information/Warning (mismo formato de Serilog).
   - Plantilla para agrupar: mensaje hasta el primer " | " con dígitos enmascarados.
   PERMISOS: el usuario de BD del servicio (ConnectionStrings:AldebaranDbConnection de su appsettings.json)
   necesita EXECUTE sobre este SP (ver GRANT al final; ajustar el usuario). Script re-ejecutable.
   Reversa: Reversa\21 - Reversa log archivos totvs.sql
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE log.SP_INSERT_ARCHIVOS_TOTVS_LOG
    @Level     NVARCHAR(20),
    @Message   NVARCHAR(MAX),
    @Exception NVARCHAR(MAX) = NULL,
    @Logger    NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* Los mensajes del servicio son texto interpolado: la plantilla para agrupar es el mensaje
       hasta el primer " | " con los dígitos enmascarados. */
    DECLARE @Template NVARCHAR(500) = LEFT(@Message, ISNULL(NULLIF(CHARINDEX(N' | ', @Message), 0) - 1, 500));
    DECLARE @Digit INT = 0;
    WHILE @Digit <= 9
    BEGIN
        SET @Template = REPLACE(@Template, CAST(@Digit AS NCHAR(1)), N'#');
        SET @Digit += 1;
    END

    INSERT INTO log.Logs (Message, MessageTemplate, Level, TimeStamp, Exception, Properties, Source)
    VALUES (@Message, @Template,
            CASE @Level WHEN N'Info' THEN N'Information' WHEN N'Warn' THEN N'Warning' ELSE @Level END,
            GETDATE(), NULLIF(@Exception, N''),
            N'<properties><property key=''SourceContext''>' + ISNULL((SELECT @Logger FOR XML PATH('')), N'') + N'</property><property key=''Source''>ArchivosTotvs</property></properties>',
            N'ArchivosTotvs');
END
GO

/* Ajustar al usuario de BD con el que corre ArchivosTotvs en producción (si no es sysadmin/db_owner). */
-- GRANT EXECUTE ON log.SP_INSERT_ARCHIVOS_TOTVS_LOG TO [usuario_archivos_totvs];
-- GO
