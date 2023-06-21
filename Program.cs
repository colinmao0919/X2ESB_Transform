using Serilog;
using Serilog.Events;
using WebGetEventBus.common;

//serilog��ʼ������
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().
    MinimumLevel.Override("Default", LogEventLevel.Information).
    MinimumLevel.Override("Microsoft", LogEventLevel.Error).MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information).MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware", LogEventLevel.Information)
.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
.WriteTo.Console()
.WriteTo.File("").CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//���ȳ�ʼ�� Serilog ��ȱ���ǣ����� ASP.NET Core �����ķ��񣨰������ú�������ע�룩�в����á�appsettings.json
//Ϊ�˽��������⣬Serilog ֧�����׶γ�ʼ������ʼ����������¼���ڳ�������ʱ�������ã�һ���������أ��ü�¼��������ȫ���õļ�¼��ȡ����
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File($"Logs/.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}"));

//����ʵIP��443�˿�
//builder.WebHost.ConfigureKestrel((context, options) =>
//{
//    options.ListenAnyIP(443, listenOptions =>
//    {
//        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
//        listenOptions.UseHttps();
//        listenOptions.UseConnectionLogging();
//    });

//    options.ListenAnyIP(5001, listenOptions =>
//    {
//        listenOptions.Protocols = HttpProtocols.Http1;
//        listenOptions.UseHttps();
//        listenOptions.UseConnectionLogging();
//    });
//});

Log.Information("Starting web application!!!!");

//��������ע��
builder.Services.AddScoped<IGetxml, strBuder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
