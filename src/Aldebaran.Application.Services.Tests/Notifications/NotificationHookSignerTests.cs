using Aldebaran.Application.Services.Notifications;
using Aldebaran.Application.Services.Notificator.Model;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Notifications
{
    /// <summary>Firma de la URL de retorno de notificaciones y validación del POST de estado.</summary>
    public class NotificationHookSignerTests
    {
        private const string Key = "clave-de-pruebas-de-32-caracteres-o-mas";
        private readonly NotificationHookSigner _signer = new(Key);

        private static MessageModel Message(string uid = "abc-123", bool withStatus = true, bool withDate = true) => new()
        {
            Header = new MessageModel.EnvelopeHeader
            {
                MessageUid = uid,
                ReceiverUrn = new[] { "a@b.co" },
                Subject = "Sales",
                SentDate = withDate ? new DateTime(2026, 11, 7) : null
            },
            Body = new MessageModel.EnvelopeBody { Template = "T" },
            MessageDeliveryStatus = withStatus ? new MessageModel.DeliveryStatus { Success = true } : null!
        };

        private string SignatureOf(Uri url) => url.Segments.Last();

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("corta")]
        public void Clave_AusenteOCorta_FallaAlCrear(string? key) =>
            Assert.Throws<InvalidOperationException>(() => new NotificationHookSigner(key));

        [Theory]
        [InlineData("https://aldebaran/", NotificationHook.PurchaseOrderUpdate)]
        [InlineData("https://aldebaran", NotificationHook.CustomerOrderUpdate)]
        [InlineData("https://srv/aldebaran/", NotificationHook.CustomerReservationUpdate)]
        public void Url_IncluyeEndpointYFirma(string baseUri, NotificationHook hook)
        {
            var url = _signer.BuildHookUrl(baseUri, hook, "abc-123");
            Assert.Equal($"{baseUri.TrimEnd('/')}/Notification/{hook}/{_signer.Sign("abc-123")}", url.ToString());
        }

        [Fact]
        public void Firma_EsSeguraParaRuta_YDeterministica()
        {
            var signature = _signer.Sign("abc-123");
            Assert.Equal(43, signature.Length); // 32 bytes en Base64 URL sin relleno
            Assert.DoesNotContain('/', signature);
            Assert.DoesNotContain('+', signature);
            Assert.DoesNotContain('=', signature);
            Assert.Equal(signature, new NotificationHookSigner(Key).Sign("abc-123"));
        }

        [Fact]
        public void Firma_DependeDeLaClaveYDelMensaje()
        {
            Assert.NotEqual(_signer.Sign("abc-123"), _signer.Sign("abc-124"));
            Assert.NotEqual(_signer.Sign("abc-123"), new NotificationHookSigner(Key + "x").Sign("abc-123"));
        }

        [Fact]
        public void Check_FirmaDeLaUrl_Valida() =>
            Assert.Equal(NotificationHookCheck.Valid, _signer.Check(Message(), SignatureOf(_signer.BuildHookUrl("https://a/", NotificationHook.CustomerOrderUpdate, "abc-123"))));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("firma-alterada")]
        public void Check_FirmaAusenteOAlterada_Invalida(string? signature) =>
            Assert.Equal(NotificationHookCheck.InvalidSignature, _signer.Check(Message(), signature));

        [Fact]
        public void Check_FirmaDeOtroMensaje_Invalida() =>
            Assert.Equal(NotificationHookCheck.InvalidSignature, _signer.Check(Message("otro"), _signer.Sign("abc-123")));

        [Fact]
        public void Check_FirmaConOtraClave_Invalida() =>
            Assert.Equal(NotificationHookCheck.InvalidSignature, _signer.Check(Message(), new NotificationHookSigner(Key + "x").Sign("abc-123")));

        [Fact]
        public void Check_MensajeNulo_Incompleto() => Assert.Equal(NotificationHookCheck.Malformed, _signer.Check(null, "x"));

        [Fact]
        public void Check_SinEstadoDeEntrega_Incompleto() =>
            Assert.Equal(NotificationHookCheck.Malformed, _signer.Check(Message(withStatus: false), _signer.Sign("abc-123")));

        [Fact]
        public void Check_SinFechaDeEnvio_Incompleto() =>
            Assert.Equal(NotificationHookCheck.Malformed, _signer.Check(Message(withDate: false), _signer.Sign("abc-123")));

        [Fact]
        public void Check_SinMessageUid_Incompleto() =>
            Assert.Equal(NotificationHookCheck.Malformed, _signer.Check(Message(uid: " "), "x"));

        [Theory]
        [InlineData("", "abc")]
        [InlineData("https://a/", "")]
        public void Url_SinBaseOSinMensaje_Error(string baseUri, string uid) =>
            Assert.Throws<ArgumentException>(() => _signer.BuildHookUrl(baseUri, NotificationHook.PurchaseOrderUpdate, uid));
    }
}
