using Marten;
using MartenIssues;

var options = new StoreOptions();

options.Schema.For<MyMartenDocument>()
    .IdStrategy(new DeterministicId<MyMartenDocument>(x => x.Name));

var builder = WebApplication.CreateBuilder(args);
options.Connection(builder.Configuration.GetConnectionString("Psql") ?? throw new InvalidOperationException());
builder.Services.AddMarten(_ => options);

var app = builder.Build();

app.MapGet("/", async (IDocumentStore store) =>
{
    var doc = new MyMartenDocument("test");
    await store.BulkInsertAsync([doc], BulkInsertMode.OverwriteExisting);
    return "ok";
});

app.Run();