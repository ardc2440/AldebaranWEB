-- Script: 3. Automata_Notification_Recipients.sql
-- Purpose: Create table to store notification recipients for the Automata service
-- Target: Aldebaran database

SET NOCOUNT ON;


IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AUTOMATA_NOTIFICATION_RECIPIENTS') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.AUTOMATA_NOTIFICATION_RECIPIENTS
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Email NVARCHAR(256) NOT NULL,
        NotificationType NVARCHAR(100) NOT NULL CONSTRAINT CHK_NotificationType CHECK (NotificationType IN ('CONNECTIVITY_DOWN','CONNECTIVITY_RECOVERED', 'BUSINESS_ERROR')),
        IsActive BIT NOT NULL CONSTRAINT DF_AUTOMATA_NOTIFICATION_RECIPIENTS_IsActive DEFAULT(1),
        CreatedAt DATETIME2(3) NOT NULL CONSTRAINT DF_AUTOMATA_NOTIFICATION_RECIPIENTS_CreatedAt DEFAULT(SYSUTCDATETIME()),
        UpdatedAt DATETIME2(3) NULL,
        Notes NVARCHAR(1000) NULL
    );

    CREATE INDEX IX_AUTOMATA_NOTIFICATION_RECIPIENTS_NotificationType ON dbo.AUTOMATA_NOTIFICATION_RECIPIENTS(NotificationType);
END

/* Example for insert into table 

INSERT INTO dbo.AUTOMATA_NOTIFICATION_RECIPIENTS(Email, NotificationType, IsActive)
	 VALUES ('ardc2440@gmail.com','CONNECTIVITY_DOWN',1)
*/ 


INSERT INTO dbo.AUTOMATA_NOTIFICATION_RECIPIENTS(Email, NotificationType, IsActive)
	 VALUES ('ardc2440@gmail.com','CONNECTIVITY_DOWN',1),
			('soporte@catalogospromocionales.com','CONNECTIVITY_DOWN',1), 
			('g.ramirez@catalogospromocionales.com','CONNECTIVITY_DOWN',1),		
			('analistainventarios@catalogospromocionales.com','CONNECTIVITY_DOWN',1),
			('ardc2440@gmail.com','CONNECTIVITY_RECOVERED',1),
			('soporte@catalogospromocionales.com','CONNECTIVITY_RECOVERED',1), 
			('g.ramirez@catalogospromocionales.com','CONNECTIVITY_RECOVERED',1),		
			('analistainventarios@catalogospromocionales.com','CONNECTIVITY_RECOVERED',1),
			('ardc2440@gmail.com','BUSINESS_ERROR',1),
			('soporte@catalogospromocionales.com','BUSINESS_ERROR',1); 