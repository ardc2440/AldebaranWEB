-- Script: 4. Automata_Connectivity_Error_Patterns.sql
-- Purpose: Create table to store connectivity error patterns for Automata
-- Target: Aldebaran database

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AUTOMATA_CONNECTIVITY_ERROR_PATTERNS') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.AUTOMATA_CONNECTIVITY_ERROR_PATTERNS
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Pattern NVARCHAR(1000) NOT NULL,
        Target NVARCHAR(1) NOT NULL CONSTRAINT DF_AUTOMATA_CONNECTIVITY_ERROR_PATTERNS_Target DEFAULT('D'), -- 'D'=Destination, 'O'=Origin, 'B'=Both
        IsActive BIT NOT NULL CONSTRAINT DF_AUTOMATA_CONNECTIVITY_ERROR_PATTERNS_IsActive DEFAULT(1),
        CreatedAt DATETIME2(3) NOT NULL CONSTRAINT DF_AUTOMATA_CONNECTIVITY_ERROR_PATTERNS_CreatedAt DEFAULT(SYSUTCDATETIME()),
        UpdatedAt DATETIME2(3) NULL,
        Notes NVARCHAR(1000) NULL
    );

    CREATE INDEX IX_AUTOMATA_CONNECTIVITY_ERROR_PATTERNS_Pattern ON dbo.AUTOMATA_CONNECTIVITY_ERROR_PATTERNS(Pattern);
END
GO
-- Sample inserts (uncomment and adapt patterns as needed)
-- INSERT INTO dbo.AUTOMATA_CONNECTIVITY_ERROR_PATTERNS(Pattern, Target, IsActive)
-- VALUES ('A network-related or instance-specific error', 'D', 1),
--        ('The server was not found or was not accessible', 'D', 1),
--        ('Login failed for user', 'D', 1),
--        ('Timeout expired', 'D', 1),
--        ('No such host is known', 'D', 1);

INSERT INTO Automata_connectivity_Error_Patterns (Pattern, Target, IsActive, CreatedAt, UpdatedAt, Notes) 
	 VALUES (N'TIMEOUT EXPIRED', N'B', 1, SYSUTCDATETIME(), NULL, N'Connectivity timeout'), 
			(N'THE TIMEOUT PERIOD ELAPSED PRIOR TO COMPLETION', N'B', 1, SYSUTCDATETIME(), NULL, N'Connectivity timeout variant'), 
			(N'A NETWORK-RELATED OR INSTANCE-SPECIFIC ERROR OCCURRED WHILE ESTABLISHING A CONNECTION', N'B', 1, SYSUTCDATETIME(), NULL, N'SQL/Network connection error'), 
			(N'THE SERVER WAS NOT FOUND OR WAS NOT ACCESSIBLE', N'B', 1, SYSUTCDATETIME(), NULL, N'Server not reachable / DNS'), 
			(N'NAMED PIPES PROVIDER', N'B', 1, SYSUTCDATETIME(), NULL, N'Named Pipes provider errors'), 
			(N'TCP PROVIDER', N'B', 1, SYSUTCDATETIME(), NULL, N'TCP provider errors'), 
			(N'NO CONNECTION COULD BE MADE BECAUSE THE TARGET MACHINE ACTIVELY REFUSED IT', N'B', 1, SYSUTCDATETIME(), NULL, N'Connection refused'), 
			(N'AN EXISTING CONNECTION WAS FORCIBLY CLOSED BY THE REMOTE HOST', N'B', 1, SYSUTCDATETIME(), NULL, N'Connection closed by remote'), 
			(N'A TRANSPORT-LEVEL ERROR HAS OCCURRED WHEN SENDING THE REQUEST TO THE SERVER', N'B', 1, SYSUTCDATETIME(), NULL, N'Transport-level error'), 
			(N'THE UNDERLYING PROVIDER FAILED ON OPEN', N'B', 1, SYSUTCDATETIME(), NULL, N'Provider failed on open (EF/ADO)'), 
			(N'LOGIN FAILED FOR USER', N'B', 1, SYSUTCDATETIME(), NULL, N'Authentication/credentials issue'), 
			(N'CANNOT OPEN DATABASE', N'B', 1, SYSUTCDATETIME(), NULL, N'Database not available / permissions'), 
			(N'LOGIN TIMEOUT EXPIRED', N'B', 1, SYSUTCDATETIME(), NULL, N'Auth timeout'), 
			(N'SYSTEM.NET.SOCKETSEXCEPTION', N'B', 1, SYSUTCDATETIME(), NULL, N'Socket exceptions'), 
			(N'NO SUCH HOST IS KNOWN', N'B', 1, SYSUTCDATETIME(), NULL, N'DNS resolution failure'), 
			(N'UNABLE TO CONNECT TO THE REMOTE SERVER', N'B', 1, SYSUTCDATETIME(), NULL, N'Remote server unreachable'), 
			(N'SSL', N'B', 1, SYSUTCDATETIME(), NULL, N'TLS/SSL problems (cert, handshake)'), 
			(N'BROKEN PIPE', N'B', 1, SYSUTCDATETIME(), NULL, N'Broken pipe / socket closed'), 
			(N'CONNECTION RESET BY PEER', N'B', 1, SYSUTCDATETIME(), NULL, N'Connection reset by peer'), 
			(N'CANNOT CONNECT TO REMOTE DATABASE SERVER', N'B', 1, SYSUTCDATETIME(), NULL, N'Generic remote DB connection failure'), 
			(N'CANNOT CONNECT TO SERVER', N'B', 1, SYSUTCDATETIME(), NULL, N'Generic server connection failure'), 
			(N'CANNOT GENERATE SSPI CONTEXT', N'B', 1, SYSUTCDATETIME(), NULL, N'Kerberos/SSPI auth issues'), 
			(N'TRANSPORT CONNECTION', N'B', 1, SYSUTCDATETIME(), NULL, N'Transport connection errors (contains)'), 
			(N'PROVIDER: COULD NOT OPEN A CONNECTION TO SQL SERVER', N'B', 1, SYSUTCDATETIME(), NULL, N'SQL Server provider-specific'), 
			(N'NETWORK IS UNREACHABLE', N'B', 1, SYSUTCDATETIME(), NULL, N'Network unreachable');