using OrderTracker.Models;

namespace OrderTracker.Resources;

public static class AppText
{
    public static string AppName => "OrderTracker";
    public static string AppTitleBase => "Order Tracker";
    public static string PackageEmoji => "📦";
    public static string ThemeIconDark => "☀";
    public static string ThemeIconLight => "◐";
    public static string EmptyValue => "—";
    public static string DropdownArrow => "▾";
    public static string ExternalLinkArrow => "↗";
    public static string PreviousPageArrow => "‹";
    public static string NextPageArrow => "›";
    public static string CloseIcon => "✕";
    public static string EditIcon => "✎";
    public static string TrackIcon => "⬡";
    public static string InfoIcon => "💬";
    public static string EmptyOrdersIcon => "📭";
    public static string WarningIcon => "⚠";
    public static string SuccessIcon => "✓";
    public static string EuroSymbol => "€";

    public static string PageTitleDashboard => FormatPageTitle(DashboardTitle);
    public static string PageTitleOrders => FormatPageTitle(OrdersTitle);
    public static string PageTitleSettings => FormatPageTitle(SettingsTitle);
    public static string PageTitleStatistics => FormatPageTitle(StatisticsTitle);

    public static string DashboardTitle => "Dashboard";
    public static string OrdersTitle => "Pedidos";
    public static string SettingsTitle => "Ajustes";
    public static string StatisticsTitle => "Estadísticas";

    public static string OpenMenuAriaLabel => "Abrir menú";
    public static string ChangeThemeTitle => "Cambiar tema";
    public static string NotFoundMessage => "Lo sentimos, esta página no existe.";

    public static string LoadMetrics => "Cargando métricas...";
    public static string LoadStatistics => "Cargando estadísticas...";
    public static string MetricPending => "Pendientes";
    public static string MetricInDelivery => "En reparto";
    public static string MetricDelayed => "Retrasados";
    public static string MetricTotalSpent => "Total gastado";
    public static string RecentOrdersTitle => "Últimos pedidos";
    public static string NoOrdersYetMessage => "No hay pedidos aún.";
    public static string CreateFirstOrderLinkText => "Crear el primero →";
    public static string ByStatusTitle => "Por estado";

    public static string NewOrderButton => "+ Nuevo pedido";
    public static string SearchOrdersPlaceholder => "Buscar por tienda o código...";
    public static string AllStatusesOption => "Todos los estados";
    public static string SortDateNewest => "Fecha: más reciente";
    public static string SortDateOldest => "Fecha: más antigua";
    public static string SortPriceHighest => "Precio: mayor primero";
    public static string SortPriceLowest => "Precio: menor primero";
    public static string LoadOrders => "Cargando pedidos...";
    public static string NoOrdersTitle => "No hay pedidos";
    public static string EmptyOrdersMessage => "Crea tu primer pedido para empezar a hacer seguimiento.";
    public static string NoOrdersWithFiltersMessage => "No se encontraron pedidos con los filtros actuales.";
    public static string CreateOrderButton => "+ Crear pedido";
    public static string ColumnStore => "Tienda";
    public static string ColumnDate => "Fecha";
    public static string ColumnCarrier => "Transportista";
    public static string ColumnTracking => "Código / Tracking";
    public static string ColumnStatus => "Estado";
    public static string ColumnEstimatedOrReceived => "Entrega estimada / Recibido";
    public static string ColumnTotal => "Total";
    public static string ColumnActions => "Acciones";
    public static string OpenTrackingTitle => "Abrir seguimiento";
    public static string ViewTrackingText => "Ver seguimiento";
    public static string DaysSincePurchaseTitle => "Días desde la compra";
    public static string DelayedLabel => "retrasado";
    public static string TrackingTitle => "Seguimiento";
    public static string EditTitle => "Editar";
    public static string DeleteTitle => "Eliminar";
    public static string OrdersPerPageLabel => "Pedidos por página";
    public static string EditOrderTitle => "Editar pedido";
    public static string CreateOrderTitle => "Nuevo pedido";
    public static string OrderInformationTitle => "Información del pedido";
    public static string StoreLabel => "Tienda *";
    public static string StorePlaceholder => "Amazon, Zara, AliExpress...";
    public static string PurchaseDateLabel => "Fecha de compra *";
    public static string StatusLabel => "Estado";
    public static string PaymentMethodLabel => "Método de pago";
    public static string NoPaymentMethodOption => "— Sin método —";
    public static string EstimatedDeliveryLabel => "Fecha estimada de llegada";
    public static string ReceivedDateLabel => "Fecha de recepción";
    public static string CarrierLabel => "Transportista";
    public static string NoCarrierOption => "— Sin transportista —";
    public static string TrackingCodeLabel => "Código de envío";
    public static string TrackingCodePlaceholder => "Número de seguimiento";
    public static string TrackingUrlLabel => "URL de seguimiento";
    public static string TrackingUrlPlaceholder => "Se genera automáticamente si está vacío";
    public static string NotesLabel => "Notas";
    public static string NotesPlaceholder => "Notas adicionales...";
    public static string ProductsTitle => "Productos";
    public static string AddProductButton => "+ Añadir producto";
    public static string ProductLabel => "Producto";
    public static string QuantityLabel => "Cantidad";
    public static string UnitPriceLabel => "Precio unitario";
    public static string SubtotalLabel => "Subtotal";
    public static string TotalUppercaseLabel => "TOTAL";
    public static string ProductNamePlaceholder => "Nombre del producto";
    public static string EmptyProductsMessage => "No hay productos. Pulsa \"+ Añadir producto\".";
    public static string CancelButton => "Cancelar";
    public static string SaveButton => "Guardar";
    public static string SaveOrderButton => "Guardar pedido";
    public static string Saving => "Guardando...";
    public static string DeleteOrderDialogTitle => "Eliminar pedido";
    public static string DeleteOrderWarning => "Esta acción no se puede deshacer.";
    public static string OrderStoreRequiredWithPeriod => "La tienda es obligatoria.";

    public static string DataSectionTitle => "Datos";
    public static string DataSectionDescription => "Exporta todos tus pedidos a CSV o importa pedidos desde un archivo CSV exportado previamente.";
    public static string Exporting => "Exportando...";
    public static string ExportCsvButton => "↓ Exportar CSV";
    public static string ImportCsvButton => "↑ Importar CSV";
    public static string PaymentMethodsTitle => "Métodos de pago";
    public static string Loading => "Cargando...";
    public static string PaymentMethodPlaceholder => "Método de pago";
    public static string AddButton => "+ Añadir";
    public static string CarriersTitle => "Transportistas";
    public static string CarrierNameColumn => "Nombre";
    public static string CarrierTrackingUrlColumn => "URL de seguimiento";
    public static string CarrierTrackingUrlHint => "usa {0} como placeholder";
    public static string SortOrderColumn => "Orden";
    public static string CarrierNamePlaceholder => "Nombre";
    public static string CarrierTrackingUrlPlaceholder => "https://ejemplo.com/tracking/{0}";
    public static string ImportCsvTitle => "Importar CSV";
    public static string ImportCsvDescription => "Selecciona un archivo CSV exportado previamente desde esta aplicación. Los pedidos se añadirán sin reemplazar los existentes.";
    public static string Importing => "Importando...";
    public static string CloseButton => "Cerrar";
    public static string SaveSuccessMessage => "✓ Guardado";
    public static string GeneralSummaryTitle => "Resumen general";
    public static string TotalOrdersLabel => "Total pedidos";
    public static string AverageTicketLabel => "Ticket medio";
    public static string ReceivedOrdersLabel => "Pedidos recibidos";
    public static string CancelledOrdersLabel => "Cancelados";
    public static string AverageDeliveryLabel => "Entrega media";
    public static string DeliveryTimeByCarrierTitle => "Tiempo medio de entrega por transportista";
    public static string NoCarrierStatsMessage => "Sin datos. Aparecerá cuando haya pedidos recibidos con fecha de recepción.";
    public static string OrdersByStatusTitle => "Pedidos por estado";
    public static string OrdersByStoreTitle => "Pedidos por tienda";
    public static string NoDataMessage => "Sin datos.";
    public static string OrdersByPaymentMethodTitle => "Pedidos por método de pago";
    public static string NoPaymentMethodStatsMessage => "Sin datos. Aparecerá cuando los pedidos tengan método de pago asignado.";

    public static string CsvNoDataMessage => "El archivo CSV no contiene datos.";
    public static string CsvUnknownCarrier => "Desconocido";
    public static string CsvPaymentMethodsSectionLine => "[MetodosPago]";
    public static string CsvPaymentMethodsHeaderLine => "Nombre,Orden";
    public static string CsvCarriersSectionLine => "[Transportistas]";
    public static string CsvCarriersHeaderLine => "Nombre,UrlSeguimiento,Orden";
    public static string CsvOrdersSectionLine => "[Pedidos]";
    public static string CsvHeaderLine => "Tienda,FechaCompra,Transportista,CodigoEnvio,UrlSeguimiento,Estado,EntregaEstimada,FechaRecepcion,Total,MetodoPago,Notas,Productos";

    public static string ValidationOrderStoreRequired => "La tienda es obligatoria";
    public static string ValidationPurchaseDateRequired => "La fecha de compra es obligatoria";
    public static string ValidationProductNameRequired => "El nombre del producto es obligatorio";
    public static string ValidationQuantityRange => "La cantidad debe ser mayor que 0";
    public static string ValidationUnitPriceRange => "El precio debe ser mayor que 0";
    public static string ValidationCarrierNameRequired => "El nombre es obligatorio";

    public static string SeedPaymentMethodCreditCard => "Tarjeta de crédito";
    public static string SeedPaymentMethodDebitCard => "Tarjeta de débito";
    public static string SeedPaymentMethodPayPal => "PayPal";
    public static string SeedPaymentMethodBizum => "Bizum";
    public static string SeedPaymentMethodTransfer => "Transferencia";
    public static string SeedPaymentMethodCashOnDelivery => "Contra reembolso";

    public static string SeedCarrierCorreos => "Correos";
    public static string SeedCarrierCorreosExpress => "Correos Express";
    public static string SeedCarrierSeur => "SEUR";
    public static string SeedCarrierMrw => "MRW";
    public static string SeedCarrierDhl => "DHL";
    public static string SeedCarrierGls => "GLS";
    public static string SeedCarrierUps => "UPS";
    public static string SeedCarrierFedEx => "FedEx";
    public static string SeedCarrierAmazonLogistics => "Amazon Logistics";
    public static string SeedCarrierNacex => "Nacex";
    public static string SeedCarrierCtt => "CTT";
    public static string SeedCarrierZeleris => "Zeleris";
    public static string SeedCarrierEcoScooting => "EcoScooting";
    public static string SeedCarrierOther => "Otro";

    public static string StatusPurchased => "Comprado";
    public static string StatusShipped => "Enviado";
    public static string StatusOutForDelivery => "En reparto";
    public static string StatusIssue => "Incidencia";
    public static string StatusReceived => "Recibido";
    public static string StatusCancelled => "Cancelado";

    public static string FormatPageTitle(string sectionTitle) => $"{PackageEmoji} {AppTitleBase} — {sectionTitle}";
    public static string FormatCurrency(decimal value) => $"{value:N2}{EuroSymbol}";
    public static string FormatAverageDays(double value) => value > 0 ? $"{value:N1} días" : EmptyValue;
    public static string FormatCarrierDays(double value) => $"{value:N1} días";
    public static string FormatOrderCount(int count) => $"{count} pedido{(count != 1 ? "s" : string.Empty)}";
    public static string FormatImportedOrders(int count) => $"{SuccessIcon} {count} pedido{(count != 1 ? "s" : string.Empty)} importado{(count != 1 ? "s" : string.Empty)}";
    public static string FormatSkippedRows(int count) => $"{WarningIcon} {count} fila{(count != 1 ? "s" : string.Empty)} omitida{(count != 1 ? "s" : string.Empty)}";
    public static string FormatPaginationSummary(int start, int end, int total) => $"{start}–{end} de {FormatOrderCount(total)}";
    public static string FormatDeleteOrderMessage(string store) => $"¿Seguro que quieres eliminar el pedido de {store}?";
    public static string FormatGenericError(string message) => $"Error: {message}";
    public static string FormatReadFileError(string message) => $"Error al leer el archivo: {message}";
    public static string FormatImportLineError(int lineNumber, string message) => $"Línea {lineNumber}: {message}";
    public static string FormatExportFileName(DateTime timestamp) => $"pedidos_{timestamp:yyyyMMdd_HHmmss}.csv";

    public static string FormatCsvProduct(string productName, int quantity, decimal unitPrice) =>
        string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            "{0} x{1} ({2}EUR)",
            productName,
            quantity,
            unitPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));

    public static string GetOrderStatusLabel(OrderStatus status) => status switch
    {
        OrderStatus.Comprado => StatusPurchased,
        OrderStatus.Enviado => StatusShipped,
        OrderStatus.EnReparto => StatusOutForDelivery,
        OrderStatus.Incidencia => StatusIssue,
        OrderStatus.Recibido => StatusReceived,
        OrderStatus.Cancelado => StatusCancelled,
        _ => status.ToString()
    };

    public static OrderStatus ParseOrderStatus(string value) => value.Trim().ToLowerInvariant() switch
    {
        "comprado" => OrderStatus.Comprado,
        "enviado" => OrderStatus.Enviado,
        "en reparto" => OrderStatus.EnReparto,
        "incidencia" => OrderStatus.Incidencia,
        "recibido" => OrderStatus.Recibido,
        "cancelado" => OrderStatus.Cancelado,
        _ => OrderStatus.Comprado
    };
}
