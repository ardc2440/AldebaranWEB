﻿namespace Aldebaran.Services.Notificator.Notify.Model
{
    public class MessageModel
    {
        /// <summary>
        /// Cabecera de la notificacion
        /// </summary>
        public required EnvelopeHeader Header { get; set; }
        /// <summary>
        /// Contenido de la notificacion
        /// </summary>
        public required EnvelopeBody Body { get; set; }
        public class EnvelopeHeader
        {
            /// <summary>
            /// Identificador unico de la presente notificacion
            /// Se establece por el originador del mensaje
            /// Se genera por ejemplo como Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower()
            /// ba25c466fb6e447699fcb6cb043abbff
            /// </summary>
            public string MessageUid { get; set; }

            /// <summary>
            /// Proveedor de configuarcion que se usara para el envio de la notificacion
            /// Se establece por el originador del mensaje
            /// Ej: Cartera
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            /// Identificador del destinatario del mensaje
            /// Se establece por el originador del mensaje
            /// Ej: john.doe@somedomain.com;jane.doe@somedomain.com
            /// Ej: +573001112222;+573003334444
            /// </summary>
            public string[] ReceiverUrn { get; set; }
            /// <summary>
            /// Identificador del destinatario del mensaje (copia de la notificacion)
            /// Se establece por el originador del mensaje
            /// Ej: john.doe@somedomain.com;jane.doe@somedomain.com
            /// </summary>
            public string[] ReceiverUrnCc { get; set; }
            /// <summary>
            /// Identificador del destinatario del mensaje (copia oculta de la notificacion)
            /// Se establece por el originador del mensaje
            /// Ej: john.doe@somedomain.com;jane.doe@somedomain.com
            /// </summary>
            public string[] ReceiverUrnBcc { get; set; }
            /// <summary>
            /// Fecha de envio de la notificacion
            /// Establecido por el sistema
            /// Ej: 2012-04-21T18:25:43.511Z
            /// </summary>
            public DateTime? SentDate { get; set; }
        }

        /// <summary>
        /// Estructura del cuerpo de la notificacion
        /// </summary>
        public class EnvelopeBody
        {
            /// <summary>
            /// Asunto del mensaje
            /// </summary>
            public string Subject { get; set; }
            /// <summary>
            /// Mensaje a enviar
            /// Ej: cliente: John Doe
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// Contiene los contenidos de multimedia de la notificacion
            /// </summary>
            public List<MediaContent> Medias { get; set; }

            /// <summary>
            /// Estructura de contenido multimedia de la notificacion
            /// </summary>
            public class MediaContent
            {
                /// <summary>
                /// Contiene el nombre del contenido multimedia
                /// </summary>
                public string FileName { get; set; }

                /// <summary>
                /// Contiene el tipo mime del contenido multimedia
                /// Ej: image/png
                /// </summary>
                public string ContentType { get; set; }

                /// <summary>
                /// Contiene el hash md5 del contenido multimedia
                /// Ej: f39750ba2be51117f8e10580893ab1aa
                /// </summary>
                public string Hash { get; set; }
            }
        }
    }
}
