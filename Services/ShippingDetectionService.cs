using OrderTracker.Models;
using System.Text.RegularExpressions;

namespace OrderTracker.Services;

public class ShippingDetectionService
{
    private static readonly Dictionary<ShippingCompany, (string[] Patterns, string UrlTemplate)> ShippingData = new()
    {
        [ShippingCompany.Correos] = (
            new[] { @"^[A-Z]{2}\d{9}[A-Z]{2}$", @"^[0-9]{13}$", @"^EC\d{9}ES$" },
            "https://www.correos.es/es/es/herramientas/localizador/envios/detalle?tracking-number={0}"
        ),
        [ShippingCompany.CorreosExpress] = (
            new[] { @"^\d{12}$" },
            "https://www.correos.es/es/es/herramientas/localizador/envios/detalle?tracking-number={0}"
        ),
        [ShippingCompany.SEUR] = (
            new[] { @"^\d{15}$", @"^1\d{14}$" },
            "https://www.seur.com/livetracking/?segOnlineIdentificador={0}"
        ),
        [ShippingCompany.MRW] = (
            new[] { @"^\d{10}$", @"^[A-Z]\d{9}$" },
            "https://www.mrw.es/seguimiento_envios/MRW_resultados_consulta.asp?Franquicia=&Abonado=&Dep=&Numero={0}"
        ),
        [ShippingCompany.DHL] = (
            new[] { @"^\d{10}$", @"^[0-9]{12}$", @"^1Z[A-Z0-9]{16}$" },
            "https://www.dhl.com/es-es/home/tracking.html?tracking-id={0}"
        ),
        [ShippingCompany.GLS] = (
            new[] { @"^\d{11}$", @"^[A-Z]{2}\d{9}$" },
            "https://gls-group.eu/track/{0}"
        ),
        [ShippingCompany.UPS] = (
            new[] { @"^1Z[A-Z0-9]{16}$", @"^\d{18}$" },
            "https://www.ups.com/track?loc=es_ES&tracknum={0}"
        ),
        [ShippingCompany.FedEx] = (
            new[] { @"^\d{12}$", @"^\d{15}$", @"^\d{20}$" },
            "https://www.fedex.com/fedextrack/?trknbr={0}"
        ),
        [ShippingCompany.AmazonLogistics] = (
            new[] { @"^TBA\d{12}$", @"^[A-Z]{2}\d{9}[A-Z]{2}$" },
            "https://www.amazon.es/progress-tracker/package/?ref_=pe_2640170_620568780&_encoding=UTF8&itemId={0}"
        ),
        [ShippingCompany.Nacex] = (
            new[] { @"^\d{9}-\d$", @"^\d{10}$" },
            "https://www.nacex.es/seguimientoDetalle.do?agencia_origen=&numero_albaran={0}"
        ),
        [ShippingCompany.Zeleris] = (
            new[] { @"^\d{11}$" },
            "https://www.zeleris.com/seguimiento?referencia={0}"
        ),
    };

    public (ShippingCompany company, string? url) DetectShipping(string? trackingCode)
    {
        if (string.IsNullOrWhiteSpace(trackingCode))
            return (ShippingCompany.Desconocido, null);

        var code = trackingCode.Trim().ToUpper();

        foreach (var (company, (patterns, urlTemplate)) in ShippingData)
        {
            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(code, pattern, RegexOptions.IgnoreCase))
                {
                    return (company, string.Format(urlTemplate, code));
                }
            }
        }

        return (ShippingCompany.Desconocido, null);
    }

    public string? GetTrackingUrl(ShippingCompany company, string? trackingCode)
    {
        if (string.IsNullOrWhiteSpace(trackingCode) || company == ShippingCompany.Desconocido || company == ShippingCompany.Otro)
            return null;

        if (ShippingData.TryGetValue(company, out var data))
        {
            return string.Format(data.UrlTemplate, trackingCode.Trim());
        }

        return null;
    }

    public string GetCompanyDisplayName(ShippingCompany company) => company switch
    {
        ShippingCompany.Correos => "Correos",
        ShippingCompany.CorreosExpress => "Correos Express",
        ShippingCompany.SEUR => "SEUR",
        ShippingCompany.MRW => "MRW",
        ShippingCompany.DHL => "DHL",
        ShippingCompany.GLS => "GLS",
        ShippingCompany.UPS => "UPS",
        ShippingCompany.FedEx => "FedEx",
        ShippingCompany.AmazonLogistics => "Amazon Logistics",
        ShippingCompany.Nacex => "Nacex",
        ShippingCompany.Ctt => "CTT",
        ShippingCompany.Zeleris => "Zeleris",
        ShippingCompany.Otro => "Otro",
        _ => "Desconocido"
    };
}
