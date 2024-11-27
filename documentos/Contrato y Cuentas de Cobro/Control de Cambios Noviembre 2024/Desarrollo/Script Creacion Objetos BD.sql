INSERT INTO document_types (DOCUMENT_TYPE_NAME, DOCUMENT_TYPE_CODE)
     VALUES ('Solicitud Cancelación O.C.', 'C')
INSERT INTO document_types (DOCUMENT_TYPE_NAME, DOCUMENT_TYPE_CODE)
     VALUES ('Solicitud Cancelación Ped.', 'E')
INSERT INTO document_types (DOCUMENT_TYPE_NAME, DOCUMENT_TYPE_CODE)
     VALUES ('Solicitud Cierre de Pedido', 'F')

DECLARE @DocumentType int 

SELECT @DocumentType = DOCUMENT_TYPE_ID from document_types WHERE DOCUMENT_TYPE_CODE = 'C'

INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Pendiente', 'P', @DocumentType, 'La solicitud de cancelación de la orden de compra ha sido creada y esta pendiente de estudio', 0, 1)
INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Aprobada', 'A', @DocumentType, 'La solicitud de cancelación de la orden de compra ha sido aprobada, generando al cancelación de la orden de compra y la actualización de las referencias en tránsito', 0, 2)
INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Denegada', 'D', @DocumentType, 'La solicitud de cancelación de la orden de compra ha sido denegada, no se afecta la orden de compra ni las referencias en tránsito', 0, 3)


SELECT @DocumentType = DOCUMENT_TYPE_ID from document_types WHERE DOCUMENT_TYPE_CODE = 'E'

INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Pendiente', 'P', @DocumentType, 'La solicitud de cancelación del pedido ha sido creada y esta pendiente de estudio', 0, 1)
INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Aprobada', 'A', @DocumentType, 'La solicitud de cancelación del pedido ha sido aprobada, generando al cancelación del pedido y la actualización del inventario correspondiente', 0, 2)
INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Denegada', 'D', @DocumentType, 'La solicitud de cancelación del pedido ha sido denegada, no se afecta el pedido ni las referencias en descargadas', 0, 3)

SELECT @DocumentType = DOCUMENT_TYPE_ID from document_types WHERE DOCUMENT_TYPE_CODE = 'F'

INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Pendiente', 'P', @DocumentType, 'La solicitud de cierre del pedido ha sido creada y esta pendiente de estudio', 0, 1)
INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Aprobada', 'A', @DocumentType, 'La solicitud de cierre del pedido ha sido aprobada, generando el cierre del pedido y la actualización del inventario correspondiente', 0, 2)
INSERT INTO status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     VALUES('Denegada', 'D', @DocumentType, 'La solicitud de cierre del pedido ha sido denegada, no se afecta el pedido ni las referencias en descargadas', 0, 3)
GO

CREATE TABLE cancellation_requests (
    CANCELLATION_REQUEST_ID INT IDENTITY(1,1) CONSTRAINT PK_CANCELLATION_REQUEST PRIMARY KEY, -- ID ÚNICO DE LA SOLICITUD
	DOCUMENT_TYPE_ID SMALLINT NOT NULL CONSTRAINT FK_CANCELLATION_REQUEST_DOCUMENT_TYPE FOREIGN KEY REFERENCES DOCUMENT_TYPES (DOCUMENT_TYPE_ID),
    DOCUMENT_NUMBER INT NOT NULL, -- ID DEL PEDIDO O LA ORDEN ASOCIADA
    REQUEST_EMPLOYEE_ID INT NOT NULL, -- USUARIO QUE REALIZÓ LA SOLICITUD
    RESPONSE_EMPLOYEE_ID INT NULL,
	STATUS_DOCUMENT_TYPE_ID SMALLINT NOT NULL CONSTRAINT FK_CANCELLATION_REQUEST_STATUS_DOCUMENT_TYPE FOREIGN KEY REFERENCES STATUS_DOCUMENT_TYPES (STATUS_DOCUMENT_TYPE_ID), -- ESTADO DE LA SOLICITUD
    REQUEST_DATE DATETIME NOT NULL CONSTRAINT DF_CANCELLATION_REQUESTS_REQUESTED_DATE DEFAULT GETDATE(), 
	RESPONSE_DATE DATETIME, 
	RESPONSE_REASON VARCHAR(250),  
    CONSTRAINT FK_CANCELLATION_REQUESTS_REQUESTED_BY FOREIGN KEY (REQUEST_EMPLOYEE_ID) REFERENCES EMPLOYEES(EMPLOYEE_ID), 
	CONSTRAINT FK_CANCELLATION_REQUESTS_RESPONDED_BY FOREIGN KEY (RESPONSE_EMPLOYEE_ID) REFERENCES EMPLOYEES(EMPLOYEE_ID) 
)

CREATE TABLE authorized_cancellation_requests_approvers (
    AUTHORIZED_CANCELLATION_REQUESTS_APPROVER_ID INT IDENTITY(1,1) CONSTRAINT PK_AUTHORIZED_CANCELLATION_REQUESTS_APPROVER PRIMARY KEY, 
    EMPLOYEE_ID INT NOT NULL CONSTRAINT UQ_AUTHORIZED_CANCELLATION_REQUESTS_APPROVER_EMPLOYEE_ID UNIQUE, 
    IS_ACTIVE BIT NOT NULL CONSTRAINT DF_AUTHORIZED_CANCELLATION_REQUESTS_APPROVER_ISACTIVE DEFAULT 1,
    CONSTRAINT FK_AUTHORIZEDAPPROVERS_USERID FOREIGN KEY (EMPLOYEE_ID) REFERENCES EMPLOYEES(EMPLOYEE_ID) 
)
GO
