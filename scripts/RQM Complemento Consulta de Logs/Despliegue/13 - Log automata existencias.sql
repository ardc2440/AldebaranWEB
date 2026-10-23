/* =====================================================================================
   RQM Complemento Consulta de Logs - T15b: Log del Autómata de Existencias en el Log general
   -------------------------------------------------------------------------------------
   El autómata (NLog) escribe en log.Logs con origen 'AutomataExistencias' mediante este SP
   (target Database del NLog.config de AutomataExistencias.Console).
   - Nivel NLog Info/Warn → Information/Warning (mismo formato de Serilog).
   - Plantilla para agrupar: mensaje hasta el primer " | " con dígitos enmascarados.
   PERMISOS: el usuario de BD del autómata (cadena "Aldebaran" de su .config) necesita
   EXECUTE sobre este SP (ver GRANT al final; ajustar el usuario). Script re-ejecutable.
   ===================================================================================== */
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE log.SP_INSERT_AUTOMATA_EXISTENCIAS_LOG
    @Level     NVARCHAR(20),
    @Message   NVARCHAR(MAX),
    @Exception NVARCHAR(MAX) = NULL,
    @Logger    NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* Los mensajes del autómata son texto interpolado: la plantilla para agrupar es el mensaje
       hasta el primer " | " (antes de datos y excepción) con los dígitos enmascarados. */
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
            N'<properties><property key=''SourceContext''>' + ISNULL((SELECT @Logger FOR XML PATH('')), N'') + N'</property><property key=''Source''>AutomataExistencias</property></properties>',
            N'AutomataExistencias');
END
GO

/* Ajustar al usuario de BD con el que corre el autómata en producción (si no es sysadmin/db_owner). */
-- GRANT EXECUTE ON log.SP_INSERT_AUTOMATA_EXISTENCIAS_LOG TO [usuario_automata];
-- GO
