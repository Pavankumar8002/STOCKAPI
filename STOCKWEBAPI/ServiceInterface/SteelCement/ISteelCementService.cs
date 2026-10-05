using STOCKWEBAPI.DataEntities.SteelCement;

namespace STOCKWEBAPI.ServiceInterface.SteelCement;

public interface ISteelCementService
{
    Task<IEnumerable<CustomerDto>> GetCustomers();
    Task<CustomerDto?> GetCustomer(Guid id);
    Task<CustomerDto> SaveCustomer(CustomerDto request);
    Task<CustomerDto> UpdateCustomer(Guid id, CustomerDto request);
    Task DeleteCustomer(Guid id);
    Task<IEnumerable<ProductDto>> GetProducts();
    Task<ProductDto?> GetProduct(Guid id);
    Task<ProductDto> SaveProduct(ProductDto request);
    Task<ProductDto> UpdateProduct(Guid id, ProductDto request);
    Task DeleteProduct(Guid id);
    Task<IEnumerable<InvoiceDto>> GetInvoices();
    Task<InvoiceDto?> GetInvoice(Guid id);
    Task<InvoiceDto> CreateInvoice(InvoiceDto request);
    Task<InvoiceDto> UpdateInvoice(Guid id, InvoiceDto request);
    Task CancelInvoice(Guid id);
    Task<IEnumerable<PaymentDto>> GetPayments();
    Task<PaymentDto> RecordPayment(PaymentDto request);
    Task<IEnumerable<PurchaseDto>> GetPurchases();
    Task<PurchaseDto> CreatePurchase(PurchaseDto request);
    Task<object> GetDashboard(DateTime? from, DateTime? to);
    Task<IEnumerable<object>> GetOutstanding();
    Task<object> GetReports(DateTime? from, DateTime? to);
    Task<ShopSettingsDto> GetSettings();
    Task<ShopSettingsDto> UpdateSettings(ShopSettingsDto request);
}
