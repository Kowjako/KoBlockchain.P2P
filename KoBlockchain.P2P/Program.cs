using Hangfire;
using Hangfire.MemoryStorage;
using KoBlockchain.P2P.Data;
using KoBlockchain.P2P.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(x => x.UseMemoryStorage());
builder.Services.AddHangfireServer(x => x.WorkerCount = 1);

builder.Services.AddScoped<IBlockRepository, BlockRepository>();
builder.Services.AddScoped<IPeerRepository, PeerRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDatabaseMigrator, DatabaseMigrator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();

// Migrate database SQLite
var dbContext = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
await dbContext.MigrateAsync();

var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

// Register block syncing job each 15 seconds
recurringJobManager.AddOrUpdate<BlockSyncJob>(
    BlockSyncJob.HangfireJobId,
    j => j.SyncBlocks(),
    "0/15 * * * * *");

// Register generation new transactions each minute
recurringJobManager.AddOrUpdate<TransactionPusherJob>(
    TransactionPusherJob.HangfireJobId,
    j => j.PushNewTransactions(),
    "* * * * *");

// Register new block mining process each minute
recurringJobManager.AddOrUpdate<BlockMiningJob>(
    BlockMiningJob.HangfireJobId,
    j => j.CreateBlock(),
    "* * * * *");

app.MapControllers();
app.Run();
