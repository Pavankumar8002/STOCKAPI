using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.RepositoryInterface.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Service.SteelCement;

public sealed class SteelCementService : ISteelCementService
{
    private readonly ISteelCementRepository _repo;
    public SteelCementService(ISteelCementRepository repo) => _repo = repo;

    public Task<IEnumerable<CustomerDto>> GetCustomers() => _repo.GetCustomers();
    public Task<CustomerDto?> GetCustomer(Guid id) => _repo.GetCustomer(id);
    public Task<CustomerDto> SaveCustomer(CustomerDto request) => _repo.SaveCustomer(request);
    public Task<CustomerDto> UpdateCustomer(Guid id, CustomerDto request) => _repo.UpdateCustomer(id, request);
    public Task DeleteCustomer(Guid id) => _repo.DeleteCustomer(id);
    public Task<IEnumerable<ProductDto>> GetProducts() => _repo.GetProducts();
    public Task<ProductDto?> GetProduct(Guid id) => _repo.GetProduct(id);
    public Task<ProductDto> SaveProduct(ProductDto request) => _repo.SaveProduct(request);
    public Task<ProductDto> UpdateProduct(Guid id, ProductDto request) => _repo.UpdateProduct(id, request);
    public Task DeleteProduct(Guid id) => _repo.DeleteProduct(id);
    public Task<IEnumerable<InvoiceDto>> GetInvoices() => _repo.GetInvoices();
    public Task<InvoiceDto?> GetInvoice(Guid id) => _repo.GetInvoice(id);
    public Task<InvoiceDto> CreateInvoice(InvoiceDto request) => _repo.CreateInvoice(request);
    public Task<InvoiceDto> UpdateInvoice(Guid id, InvoiceDto request) => _repo.UpdateInvoice(id, request);
    public Task CancelInvoice(Guid id) => _repo.CancelInvoice(id);
    public Task<IEnumerable<PaymentDto>> GetPayments() => _repo.GetPayments();
    public Task<PaymentDto> RecordPayment(PaymentDto request) => _repo.RecordPayment(request);
    public Task<IEnumerable<PurchaseDto>> GetPurchases() => _repo.GetPurchases();
    public Task<PurchaseDto> CreatePurchase(PurchaseDto request) => _repo.CreatePurchase(request);
    public Task<object> GetDashboard(DateTime? from, DateTime? to) => _repo.GetDashboard(from, to);
    public Task<IEnumerable<object>> GetOutstanding() => _repo.GetOutstanding();
    public Task<object> GetReports(DateTime? from, DateTime? to) => _repo.GetReports(from, to);
    public Task<ShopSettingsDto> GetSettings() => _repo.GetSettings();
    public Task<ShopSettingsDto> UpdateSettings(ShopSettingsDto request) => _repo.UpdateSettings(request);
}
