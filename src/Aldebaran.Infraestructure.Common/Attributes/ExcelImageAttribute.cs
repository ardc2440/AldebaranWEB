using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Infrastructure.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExcelImageAttribute : Attribute
    {
        /// <summary>Propiedad (string) con el texto de una marca a dibujar sobre la imagen; vacío = sin marca.</summary>
        public string? BadgeProperty { get; set; }
    }
}