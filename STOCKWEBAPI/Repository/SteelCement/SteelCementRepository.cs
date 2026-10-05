using Npgsql;
using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.RepositoryInterface.SteelCement;
using System.Data;

namespace STOCKWEBAPI.Repository.SteelCement;

public sealed class SteelCementRepository : ISteelCementRepository
{
    private readonly string _connectionString;
    private readonly Guid _ownerId;

    public SteelCementRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");
        if (!Guid.TryParse(configuration["SteelCement:OwnerId"], out _ownerId))
            _ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    private NpgsqlConnection Connection() => new(_connectionString);

    private static decimal D(object? value) => value == null || value == DBNull.Value ? 0m : Convert.ToDecimal(value);

    public async Task<IEnumerable<CustomerDto>> GetCustomers()
    {
        await using var c = Connection(); await c.OpenAsync();
        const string sql = "select id,name,phone,email,address,gstin from customers where owner_id=@o order by name";
        await using var cmd = new NpgsqlCommand(sql,c); cmd.Parameters.AddWithValue("o",_ownerId);
        await using var r=await cmd.ExecuteReaderAsync(); var list=new List<CustomerDto>();
        while(await r.ReadAsync()) list.Add(new CustomerDto { Id=r.GetGuid(0),Name=r.GetString(1),Phone=r.IsDBNull(2)?null:r.GetString(2),Email=r.IsDBNull(3)?null:r.GetString(3),Address=r.IsDBNull(4)?null:r.GetString(4),Gstin=r.IsDBNull(5)?null:r.GetString(5) });
        return list;
    }

    public async Task<CustomerDto?> GetCustomer(Guid id) {
        return (await GetCustomers()).FirstOrDefault(x=>x.Id==id);
    }

    public async Task<CustomerDto> SaveCustomer(CustomerDto x) {
        await using var c=Connection(); await c.OpenAsync();
        const string sql="insert into customers(owner_id,name,phone,email,address,gstin) values(@o,@n,@p,@e,@a,@g) returning id";
        await using var cmd=new NpgsqlCommand(sql,c);
        cmd.Parameters.AddWithValue("o",_ownerId); cmd.Parameters.AddWithValue("n",x.Name); cmd.Parameters.AddWithValue("p",(object?)x.Phone??DBNull.Value); cmd.Parameters.AddWithValue("e",(object?)x.Email??DBNull.Value); cmd.Parameters.AddWithValue("a",(object?)x.Address??DBNull.Value); cmd.Parameters.AddWithValue("g",(object?)x.Gstin??DBNull.Value);
        x.Id=(Guid)await cmd.ExecuteScalarAsync()!; return x;
    }

    public async Task<CustomerDto> UpdateCustomer(Guid id, CustomerDto x) {
        await using var c=Connection(); await c.OpenAsync();
        const string sql="update customers set name=@n,phone=@p,email=@e,address=@a,gstin=@g,updated_at=now() where id=@id and owner_id=@o";
        await using var cmd=new NpgsqlCommand(sql,c);
        cmd.Parameters.AddWithValue("id",id);cmd.Parameters.AddWithValue("o",_ownerId);cmd.Parameters.AddWithValue("n",x.Name);cmd.Parameters.AddWithValue("p",(object?)x.Phone??DBNull.Value);cmd.Parameters.AddWithValue("e",(object?)x.Email??DBNull.Value);cmd.Parameters.AddWithValue("a",(object?)x.Address??DBNull.Value);cmd.Parameters.AddWithValue("g",(object?)x.Gstin??DBNull.Value);
        await cmd.ExecuteNonQueryAsync(); x.Id=id; return x;
    }

    public async Task DeleteCustomer(Guid id) {
        await using var c=Connection(); await c.OpenAsync();
        await using var cmd=new NpgsqlCommand("delete from customers where id=@id and owner_id=@o",c);cmd.Parameters.AddWithValue("id",id);cmd.Parameters.AddWithValue("o",_ownerId);await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<ProductDto>> GetProducts() {
        await using var c=Connection();await c.OpenAsync();
        const string sql="select id,name,sku,unit,sale_rate,purchase_rate,gst_rate,stock_qty,reorder_level from products where owner_id=@o order by name";
        await using var cmd=new NpgsqlCommand(sql,c);cmd.Parameters.AddWithValue("o",_ownerId);await using var r=await cmd.ExecuteReaderAsync();var list=new List<ProductDto>();
        while(await r.ReadAsync()) list.Add(new ProductDto{Id=r.GetGuid(0),Name=r.GetString(1),Sku=r.IsDBNull(2)?null:r.GetString(2),Unit=r.GetString(3),SaleRate=D(r.GetValue(4)),PurchaseRate=D(r.GetValue(5)),GstRate=D(r.GetValue(6)),StockQty=D(r.GetValue(7)),ReorderLevel=D(r.GetValue(8))}); return list;
    }
    public async Task<ProductDto?> GetProduct(Guid id)=> (await GetProducts()).FirstOrDefault(x=>x.Id==id);
    public async Task<ProductDto> SaveProduct(ProductDto x) {
        await using var c=Connection();await c.OpenAsync();
        const string sql="insert into products(owner_id,name,sku,unit,sale_rate,purchase_rate,gst_rate,stock_qty,reorder_level) values(@o,@n,@s,@u,@sr,@pr,@g,@q,@rl) returning id";
        await using var cmd=new NpgsqlCommand(sql,c);cmd.Parameters.AddWithValue("o",_ownerId);cmd.Parameters.AddWithValue("n",x.Name);cmd.Parameters.AddWithValue("s",(object?)x.Sku??DBNull.Value);cmd.Parameters.AddWithValue("u",x.Unit);cmd.Parameters.AddWithValue("sr",x.SaleRate);cmd.Parameters.AddWithValue("pr",x.PurchaseRate);cmd.Parameters.AddWithValue("g",x.GstRate);cmd.Parameters.AddWithValue("q",x.StockQty);cmd.Parameters.AddWithValue("rl",x.ReorderLevel);x.Id=(Guid)await cmd.ExecuteScalarAsync()!;return x;
    }
    public async Task<ProductDto> UpdateProduct(Guid id, ProductDto x) {
        await using var c=Connection();await c.OpenAsync();
        const string sql="update products set name=@n,sku=@s,unit=@u,sale_rate=@sr,purchase_rate=@pr,gst_rate=@g,reorder_level=@rl,updated_at=now() where id=@id and owner_id=@o";
        await using var cmd=new NpgsqlCommand(sql,c);cmd.Parameters.AddWithValue("id",id);cmd.Parameters.AddWithValue("o",_ownerId);cmd.Parameters.AddWithValue("n",x.Name);cmd.Parameters.AddWithValue("s",(object?)x.Sku??DBNull.Value);cmd.Parameters.AddWithValue("u",x.Unit);cmd.Parameters.AddWithValue("sr",x.SaleRate);cmd.Parameters.AddWithValue("pr",x.PurchaseRate);cmd.Parameters.AddWithValue("g",x.GstRate);cmd.Parameters.AddWithValue("rl",x.ReorderLevel);await cmd.ExecuteNonQueryAsync();x.Id=id;return x;
    }
    public async Task DeleteProduct(Guid id) {
        await using var c=Connection();await c.OpenAsync();await using var cmd=new NpgsqlCommand("delete from products where id=@id and owner_id=@o",c);cmd.Parameters.AddWithValue("id",id);cmd.Parameters.AddWithValue("o",_ownerId);await cmd.ExecuteNonQueryAsync();
    }

    private static decimal LineSubtotal(InvoiceItemDto x)=>x.Quantity*x.Rate;
    private static decimal LineTax(InvoiceItemDto x)=>LineSubtotal(x)*x.GstRate/100m;

    private async Task<InvoiceDto> LoadInvoice(NpgsqlConnection c, Guid id, NpgsqlTransaction? tx=null) {
        await using var cmd=new NpgsqlCommand("select i.id,i.invoice_number,i.customer_id,coalesce(c.name,''),i.invoice_date,i.discount,i.total_tax,i.grand_total,i.status from invoices i left join customers c on c.id=i.customer_id where i.id=@id and i.owner_id=@o",c,tx);
        cmd.Parameters.AddWithValue("id",id);cmd.Parameters.AddWithValue("o",_ownerId);
        await using var r=await cmd.ExecuteReaderAsync();
        if(!await r.ReadAsync()) throw new KeyNotFoundException("Invoice not found.");
        var x=new InvoiceDto{Id=r.GetGuid(0),InvoiceNumber=r.GetString(1),CustomerId=r.IsDBNull(2)?null:r.GetGuid(2),CustomerName=r.GetString(3),InvoiceDate=r.GetDateTime(4),Discount=D(r.GetValue(5))};
        r.Close();
        await using var ic=new NpgsqlCommand("select ii.id,ii.product_id,p.name,p.unit,ii.quantity,ii.rate,ii.gst_rate from invoice_items ii join products p on p.id=ii.product_id where ii.invoice_id=@id order by ii.created_at",c,tx);ic.Parameters.AddWithValue("id",id);
        await using var ir=await ic.ExecuteReaderAsync();while(await ir.ReadAsync()) x.Items.Add(new InvoiceItemDto{Id=ir.GetGuid(0),ProductId=ir.GetGuid(1),ProductName=ir.GetString(2),Unit=ir.GetString(3),Quantity=D(ir.GetValue(4)),Rate=D(ir.GetValue(5)),GstRate=D(ir.GetValue(6))});ir.Close();
        await using var pc=new NpgsqlCommand("select coalesce(sum(amount),0) from payments where invoice_id=@id and owner_id=@o",c,tx);pc.Parameters.AddWithValue("id",id);pc.Parameters.AddWithValue("o",_ownerId);x.PaidAmount=D(await pc.ExecuteScalarAsync());
        return x;
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoices() {
        await using var c=Connection();await c.OpenAsync();await using var cmd=new NpgsqlCommand("select id from invoices where owner_id=@o and status<>'cancelled' order by invoice_date desc",c);cmd.Parameters.AddWithValue("o",_ownerId);await using var r=await cmd.ExecuteReaderAsync();var ids=new List<Guid>();while(await r.ReadAsync())ids.Add(r.GetGuid(0));r.Close();var list=new List<InvoiceDto>();foreach(var id in ids)list.Add(await LoadInvoice(c,id));return list;
    }
    public async Task<InvoiceDto?> GetInvoice(Guid id) { try { await using var c=Connection();await c.OpenAsync();return await LoadInvoice(c,id); } catch(KeyNotFoundException){return null;} }

    private async Task ApplyStock(NpgsqlConnection c,NpgsqlTransaction tx, Guid productId, decimal delta, string type, Guid? invoiceId=null, Guid? purchaseId=null) {
        await using var cmd=new NpgsqlCommand("update products set stock_qty=stock_qty+@d,updated_at=now() where id=@p and owner_id=@o returning stock_qty",c,tx);cmd.Parameters.AddWithValue("d",delta);cmd.Parameters.AddWithValue("p",productId);cmd.Parameters.AddWithValue("o",_ownerId);var result=await cmd.ExecuteScalarAsync();if(result==null)throw new InvalidOperationException("Product not found.");
        await using var m=new NpgsqlCommand("insert into stock_movements(owner_id,product_id,movement_type,quantity,reference_invoice_id,reference_purchase_id) values(@o,@p,@t,@q,@i,@pi)",c,tx);m.Parameters.AddWithValue("o",_ownerId);m.Parameters.AddWithValue("p",productId);m.Parameters.AddWithValue("t",type);m.Parameters.AddWithValue("q",Math.Abs(delta));m.Parameters.AddWithValue("i",(object?)invoiceId??DBNull.Value);m.Parameters.AddWithValue("pi",(object?)purchaseId??DBNull.Value);await m.ExecuteNonQueryAsync();
    }

    public async Task<InvoiceDto> CreateInvoice(InvoiceDto x) {
        if(x.Items.Count==0)throw new InvalidOperationException("Invoice must contain at least one item.");
        await using var c=Connection();await c.OpenAsync();await using var tx=await c.BeginTransactionAsync();
        try {
            x.Id=Guid.NewGuid();x.InvoiceNumber=string.IsNullOrWhiteSpace(x.InvoiceNumber)?$"INV-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}":x.InvoiceNumber;
            decimal subtotal=x.Items.Sum(LineSubtotal),tax=x.Items.Sum(LineTax),total=subtotal+tax-x.Discount;
            await using var ic=new NpgsqlCommand("insert into invoices(id,owner_id,invoice_number,customer_id,invoice_date,subtotal,total_tax,discount,grand_total,status,notes) values(@id,@o,@no,@c,@d,@s,@t,@di,@gt,@st,@n)",c,tx);
            ic.Parameters.AddWithValue("id",x.Id.Value);ic.Parameters.AddWithValue("o",_ownerId);ic.Parameters.AddWithValue("no",x.InvoiceNumber);ic.Parameters.AddWithValue("c",(object?)x.CustomerId??DBNull.Value);ic.Parameters.AddWithValue("d",x.InvoiceDate.ToUniversalTime());ic.Parameters.AddWithValue("s",subtotal);ic.Parameters.AddWithValue("t",tax);ic.Parameters.AddWithValue("di",x.Discount);ic.Parameters.AddWithValue("gt",total);ic.Parameters.AddWithValue("st",x.PaidAmount>=total?"paid":x.PaidAmount>0?"partial":"unpaid");ic.Parameters.AddWithValue("n",(object?)x.Notes??DBNull.Value);await ic.ExecuteNonQueryAsync();
            foreach(var item in x.Items){await using var q=new NpgsqlCommand("insert into invoice_items(invoice_id,product_id,quantity,rate,gst_rate) values(@i,@p,@q,@r,@g)",c,tx);q.Parameters.AddWithValue("i",x.Id.Value);q.Parameters.AddWithValue("p",item.ProductId);q.Parameters.AddWithValue("q",item.Quantity);q.Parameters.AddWithValue("r",item.Rate);q.Parameters.AddWithValue("g",item.GstRate);await q.ExecuteNonQueryAsync();await ApplyStock(c,tx,item.ProductId,-item.Quantity,"sale",x.Id);}
            if(x.PaidAmount>0){await using var p=new NpgsqlCommand("insert into payments(owner_id,invoice_id,amount,payment_mode,payment_date) values(@o,@i,@a,@m,@d)",c,tx);p.Parameters.AddWithValue("o",_ownerId);p.Parameters.AddWithValue("i",x.Id.Value);p.Parameters.AddWithValue("a",Math.Min(x.PaidAmount,total));p.Parameters.AddWithValue("m",x.PaymentMode);p.Parameters.AddWithValue("d",DateTime.UtcNow);await p.ExecuteNonQueryAsync();}
            await tx.CommitAsync();return await LoadInvoice(c,x.Id.Value);
        }catch{await tx.RollbackAsync();throw;}
    }

    public async Task<InvoiceDto> UpdateInvoice(Guid id, InvoiceDto x) {
        if(x.Items.Count==0)throw new InvalidOperationException("Invoice must contain at least one item.");
        await using var c=Connection();await c.OpenAsync();await using var tx=await c.BeginTransactionAsync();
        try {
            await using var oldItems=new NpgsqlCommand("select product_id,quantity from invoice_items where invoice_id=@i",c,tx);oldItems.Parameters.AddWithValue("i",id);await using var rr=await oldItems.ExecuteReaderAsync();var old=new List<(Guid p,decimal q)>();while(await rr.ReadAsync())old.Add((rr.GetGuid(0),D(rr.GetValue(1))));rr.Close();
            foreach(var o in old)await ApplyStock(c,tx,o.p,o.q,"invoice_edit_reverse",id);
            await using var del=new NpgsqlCommand("delete from invoice_items where invoice_id=@i",c,tx);del.Parameters.AddWithValue("i",id);await del.ExecuteNonQueryAsync();
            decimal subtotal=x.Items.Sum(LineSubtotal),tax=x.Items.Sum(LineTax),total=subtotal+tax-x.Discount;
            await using var up=new NpgsqlCommand("update invoices set customer_id=@c,invoice_date=@d,subtotal=@s,total_tax=@t,discount=@di,grand_total=@gt,status=@st,notes=@n,updated_at=now() where id=@i and owner_id=@o",c,tx);
            up.Parameters.AddWithValue("c",(object?)x.CustomerId??DBNull.Value);up.Parameters.AddWithValue("d",x.InvoiceDate.ToUniversalTime());up.Parameters.AddWithValue("s",subtotal);up.Parameters.AddWithValue("t",tax);up.Parameters.AddWithValue("di",x.Discount);up.Parameters.AddWithValue("gt",total);up.Parameters.AddWithValue("st",x.PaidAmount>=total?"paid":x.PaidAmount>0?"partial":"unpaid");up.Parameters.AddWithValue("n",(object?)x.Notes??DBNull.Value);up.Parameters.AddWithValue("i",id);up.Parameters.AddWithValue("o",_ownerId);if(await up.ExecuteNonQueryAsync()==0)throw new KeyNotFoundException("Invoice not found.");
            await using var dp=new NpgsqlCommand("delete from payments where invoice_id=@i and owner_id=@o",c,tx);dp.Parameters.AddWithValue("i",id);dp.Parameters.AddWithValue("o",_ownerId);await dp.ExecuteNonQueryAsync();
            foreach(var item in x.Items){await using var q=new NpgsqlCommand("insert into invoice_items(invoice_id,product_id,quantity,rate,gst_rate) values(@i,@p,@q,@r,@g)",c,tx);q.Parameters.AddWithValue("i",id);q.Parameters.AddWithValue("p",item.ProductId);q.Parameters.AddWithValue("q",item.Quantity);q.Parameters.AddWithValue("r",item.Rate);q.Parameters.AddWithValue("g",item.GstRate);await q.ExecuteNonQueryAsync();await ApplyStock(c,tx,item.ProductId,-item.Quantity,"sale_edit",id);}
            if(x.PaidAmount>0){await using var p=new NpgsqlCommand("insert into payments(owner_id,invoice_id,amount,payment_mode,payment_date) values(@o,@i,@a,@m,@d)",c,tx);p.Parameters.AddWithValue("o",_ownerId);p.Parameters.AddWithValue("i",id);p.Parameters.AddWithValue("a",Math.Min(x.PaidAmount,total));p.Parameters.AddWithValue("m",x.PaymentMode);p.Parameters.AddWithValue("d",DateTime.UtcNow);await p.ExecuteNonQueryAsync();}
            await tx.CommitAsync();return await LoadInvoice(c,id);
        }catch{await tx.RollbackAsync();throw;}
    }

    public async Task CancelInvoice(Guid id) {
        await using var c=Connection();await c.OpenAsync();await using var tx=await c.BeginTransactionAsync();
        try {
            await using var q=new NpgsqlCommand("select product_id,quantity from invoice_items where invoice_id=@i",c,tx);q.Parameters.AddWithValue("i",id);await using var r=await q.ExecuteReaderAsync();var items=new List<(Guid,decimal)>();while(await r.ReadAsync())items.Add((r.GetGuid(0),D(r.GetValue(1))));r.Close();
            foreach(var item in items)await ApplyStock(c,tx,item.Item1,item.Item2,"sale_cancel_reverse",id);
            await using var u=new NpgsqlCommand("update invoices set status='cancelled',updated_at=now() where id=@i and owner_id=@o",c,tx);u.Parameters.AddWithValue("i",id);u.Parameters.AddWithValue("o",_ownerId);if(await u.ExecuteNonQueryAsync()==0)throw new KeyNotFoundException("Invoice not found.");await tx.CommitAsync();
        }catch{await tx.RollbackAsync();throw;}
    }

    public async Task<IEnumerable<PaymentDto>> GetPayments() {
        await using var c=Connection();await c.OpenAsync();const string s="select id,invoice_id,amount,payment_mode,payment_date,reference_no,notes from payments where owner_id=@o order by payment_date desc";await using var cmd=new NpgsqlCommand(s,c);cmd.Parameters.AddWithValue("o",_ownerId);await using var r=await cmd.ExecuteReaderAsync();var l=new List<PaymentDto>();while(await r.ReadAsync())l.Add(new PaymentDto{Id=r.GetGuid(0),InvoiceId=r.GetGuid(1),Amount=D(r.GetValue(2)),PaymentMode=r.GetString(3),PaymentDate=r.GetDateTime(4),ReferenceNo=r.IsDBNull(5)?null:r.GetString(5),Notes=r.IsDBNull(6)?null:r.GetString(6)});return l;
    }
    public async Task<PaymentDto> RecordPayment(PaymentDto x) {
        await using var c=Connection();await c.OpenAsync();await using var tx=await c.BeginTransactionAsync();
        await using var ins=new NpgsqlCommand("insert into payments(owner_id,invoice_id,amount,payment_mode,payment_date,reference_no,notes) values(@o,@i,@a,@m,@d,@r,@n) returning id",c,tx);ins.Parameters.AddWithValue("o",_ownerId);ins.Parameters.AddWithValue("i",x.InvoiceId);ins.Parameters.AddWithValue("a",x.Amount);ins.Parameters.AddWithValue("m",x.PaymentMode);ins.Parameters.AddWithValue("d",x.PaymentDate.ToUniversalTime());ins.Parameters.AddWithValue("r",(object?)x.ReferenceNo??DBNull.Value);ins.Parameters.AddWithValue("n",(object?)x.Notes??DBNull.Value);x.Id=(Guid)await ins.ExecuteScalarAsync()!;
        await using var up=new NpgsqlCommand("update invoices i set status=case when coalesce((select sum(amount) from payments p where p.invoice_id=i.id and p.owner_id=i.owner_id),0)>=i.grand_total then 'paid' when coalesce((select sum(amount) from payments p where p.invoice_id=i.id and p.owner_id=i.owner_id),0)>0 then 'partial' else 'unpaid' end where i.id=@i and i.owner_id=@o",c,tx);up.Parameters.AddWithValue("i",x.InvoiceId);up.Parameters.AddWithValue("o",_ownerId);await up.ExecuteNonQueryAsync();await tx.CommitAsync();return x;
    }

    public async Task<IEnumerable<PurchaseDto>> GetPurchases() {
        await using var c=Connection();await c.OpenAsync();const string s="select id,purchase_number,supplier_name,purchase_date,payment_mode,paid_amount from purchases where owner_id=@o order by purchase_date desc";await using var cmd=new NpgsqlCommand(s,c);cmd.Parameters.AddWithValue("o",_ownerId);await using var r=await cmd.ExecuteReaderAsync();var l=new List<PurchaseDto>();while(await r.ReadAsync()){var p=new PurchaseDto{Id=r.GetGuid(0),PurchaseNumber=r.GetString(1),SupplierName=r.IsDBNull(2)?null:r.GetString(2),PurchaseDate=r.GetDateTime(3),PaymentMode=r.GetString(4),PaidAmount=D(r.GetValue(5))};l.Add(p);}return l;
    }
    public async Task<PurchaseDto> CreatePurchase(PurchaseDto x) {
        if(x.Items.Count==0)throw new InvalidOperationException("Purchase must contain at least one item.");
        await using var c=Connection();await c.OpenAsync();await using var tx=await c.BeginTransactionAsync();x.Id=Guid.NewGuid();x.PurchaseNumber=string.IsNullOrWhiteSpace(x.PurchaseNumber)?$"PUR-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}":x.PurchaseNumber;
        try{decimal total=x.Items.Sum(i=>i.Quantity*i.Rate);await using var pc=new NpgsqlCommand("insert into purchases(id,owner_id,purchase_number,supplier_name,purchase_date,subtotal,total_tax,grand_total,payment_mode,paid_amount,status) values(@id,@o,@n,@s,@d,@sub,@tax,@gt,@m,@pa,@st)",c,tx);pc.Parameters.AddWithValue("id",x.Id.Value);pc.Parameters.AddWithValue("o",_ownerId);pc.Parameters.AddWithValue("n",x.PurchaseNumber);pc.Parameters.AddWithValue("s",(object?)x.SupplierName??DBNull.Value);pc.Parameters.AddWithValue("d",x.PurchaseDate.ToUniversalTime());pc.Parameters.AddWithValue("sub",total);pc.Parameters.AddWithValue("tax",0m);pc.Parameters.AddWithValue("gt",total);pc.Parameters.AddWithValue("m",x.PaymentMode);pc.Parameters.AddWithValue("pa",x.PaidAmount);pc.Parameters.AddWithValue("st",x.PaidAmount>=total?"paid":x.PaidAmount>0?"partial":"unpaid");await pc.ExecuteNonQueryAsync();foreach(var i in x.Items){await using var q=new NpgsqlCommand("insert into purchase_items(purchase_id,product_id,quantity,rate,gst_rate) values(@p,@pr,@q,@r,@g)",c,tx);q.Parameters.AddWithValue("p",x.Id.Value);q.Parameters.AddWithValue("pr",i.ProductId);q.Parameters.AddWithValue("q",i.Quantity);q.Parameters.AddWithValue("r",i.Rate);q.Parameters.AddWithValue("g",i.GstRate);await q.ExecuteNonQueryAsync();await ApplyStock(c,tx,i.ProductId,i.Quantity,"purchase",null,x.Id);}await tx.CommitAsync();return x;}catch{await tx.RollbackAsync();throw;}
    }

    public async Task<IEnumerable<object>> GetOutstanding() {
        await using var c=Connection();await c.OpenAsync();const string s="select i.id,i.invoice_number,i.invoice_date,coalesce(c.name,'Walk-in') customer_name,i.grand_total-coalesce((select sum(p.amount) from payments p where p.invoice_id=i.id and p.owner_id=i.owner_id),0) outstanding from invoices i left join customers c on c.id=i.customer_id where i.owner_id=@o and i.status<>'cancelled' and i.grand_total-coalesce((select sum(p.amount) from payments p where p.invoice_id=i.id and p.owner_id=i.owner_id),0)>0 order by i.invoice_date desc";await using var cmd=new NpgsqlCommand(s,c);cmd.Parameters.AddWithValue("o",_ownerId);await using var r=await cmd.ExecuteReaderAsync();var l=new List<object>();while(await r.ReadAsync())l.Add(new{Id=r.GetGuid(0),InvoiceNumber=r.GetString(1),InvoiceDate=r.GetDateTime(2),CustomerName=r.GetString(3),Outstanding=D(r.GetValue(4))});return l;
    }

    public async Task<object> GetDashboard(DateTime? from, DateTime? to) {
        await using var c=Connection();await c.OpenAsync();var f=from?.ToUniversalTime()??DateTime.UnixEpoch;var t=to?.ToUniversalTime()??DateTime.UtcNow.AddDays(1);
        const string s="select count(*) invoices,coalesce(sum(grand_total),0) sales,coalesce(sum((select coalesce(sum(p.amount),0) from payments p where p.invoice_id=i.id and p.owner_id=i.owner_id)),0) collected,coalesce((select sum(stock_qty*purchase_rate) from products where owner_id=@o),0) inventory_value from invoices i where i.owner_id=@o and i.status<>'cancelled' and i.invoice_date>=@f and i.invoice_date<@t";
        await using var cmd=new NpgsqlCommand(s,c);cmd.Parameters.AddWithValue("o",_ownerId);cmd.Parameters.AddWithValue("f",f);cmd.Parameters.AddWithValue("t",t);await using var r=await cmd.ExecuteReaderAsync();if(!await r.ReadAsync())return new{};return new{Invoices=r.GetInt64(0),Sales=D(r.GetValue(1)),Collected=D(r.GetValue(2)),InventoryValue=D(r.GetValue(3))};
    }

    public async Task<object> GetReports(DateTime? from, DateTime? to) {
        var dashboard=await GetDashboard(from,to);var outstanding=await GetOutstanding();return new{Dashboard=dashboard,Outstanding=outstanding};
    }

    public async Task<ShopSettingsDto> GetSettings() {
        await using var c=Connection();await c.OpenAsync();await using var cmd=new NpgsqlCommand("select shop_name,address,phone,email,gstin,state from shop_settings where owner_id=@o",c);cmd.Parameters.AddWithValue("o",_ownerId);await using var r=await cmd.ExecuteReaderAsync();if(!await r.ReadAsync())return new ShopSettingsDto();return new ShopSettingsDto{ShopName=r.GetString(0),Address=r.IsDBNull(1)?null:r.GetString(1),Phone=r.IsDBNull(2)?null:r.GetString(2),Email=r.IsDBNull(3)?null:r.GetString(3),Gstin=r.IsDBNull(4)?null:r.GetString(4),State=r.IsDBNull(5)?null:r.GetString(5)};
    }
    public async Task<ShopSettingsDto> UpdateSettings(ShopSettingsDto x) {
        await using var c=Connection();await c.OpenAsync();const string s="insert into shop_settings(owner_id,shop_name,address,phone,email,gstin,state) values(@o,@n,@a,@p,@e,@g,@st) on conflict(owner_id) do update set shop_name=excluded.shop_name,address=excluded.address,phone=excluded.phone,email=excluded.email,gstin=excluded.gstin,state=excluded.state,updated_at=now()";await using var cmd=new NpgsqlCommand(s,c);cmd.Parameters.AddWithValue("o",_ownerId);cmd.Parameters.AddWithValue("n",x.ShopName);cmd.Parameters.AddWithValue("a",(object?)x.Address??DBNull.Value);cmd.Parameters.AddWithValue("p",(object?)x.Phone??DBNull.Value);cmd.Parameters.AddWithValue("e",(object?)x.Email??DBNull.Value);cmd.Parameters.AddWithValue("g",(object?)x.Gstin??DBNull.Value);cmd.Parameters.AddWithValue("st",(object?)x.State??DBNull.Value);await cmd.ExecuteNonQueryAsync();return x;
    }
}
