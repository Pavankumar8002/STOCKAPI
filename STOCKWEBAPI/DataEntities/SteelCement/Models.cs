namespace STOCKWEBAPI.DataEntities.SteelCement;

public sealed class CustomerDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Gstin { get; set; }
}

public sealed class ProductDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = "";
    public string? Sku { get; set; }
    public string Unit { get; set; } = "Nos";
    public decimal SaleRate { get; set; }
    public decimal PurchaseRate { get; set; }
    public decimal GstRate { get; set; } = 18;
    public decimal StockQty { get; set; }
    public decimal ReorderLevel { get; set; }
}

public sealed class InvoiceItemDto
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string Unit { get; set; } = "Nos";
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal GstRate { get; set; } = 18;
}

public sealed class InvoiceDto
{
    public Guid? Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal Discount { get; set; }
    public decimal PaidAmount { get; set; }
    public string PaymentMode { get; set; } = "Cash";
    public string? Notes { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public sealed class PaymentDto
{
    public Guid? Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMode { get; set; } = "Cash";
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class PurchaseDto
{
    public Guid? Id { get; set; }
    public string? PurchaseNumber { get; set; }
    public string? SupplierName { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public string PaymentMode { get; set; } = "Cash";
    public decimal PaidAmount { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
}

public sealed class PurchaseItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal GstRate { get; set; } = 18;
}

public sealed class ShopSettingsDto
{
    public string ShopName { get; set; } = "";
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Gstin { get; set; }
    public string? State { get; set; }
}

public sealed class ReportFilterDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
