var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSassCompiler();
builder.Services.AddFlashes();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SampleSession";
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.IsEssential = true;
});
// ����������� ���� ������ SQL Server ��� PostgreSQL
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SampleAppContext>(options => options.UseSqlServer(connection));
//builder.Services.AddDbContext<SampleAppContext>(options => options.UseNpgsql(connection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
