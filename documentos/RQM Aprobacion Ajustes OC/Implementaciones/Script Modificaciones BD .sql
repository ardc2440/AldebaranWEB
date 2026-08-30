/* Nuevo estado para la Orden de Compra */
insert into status_document_types (STATUS_DOCUMENT_TYPE_NAME, STATUS_DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_ID, NOTES, EDIT_MODE, STATUS_ORDER)
     values ('Ajuste en aprobación','J',1,'Orden de Compra por confirmar con diferencias entre cantidad recibida y cantidad solicitada', 1, 4)