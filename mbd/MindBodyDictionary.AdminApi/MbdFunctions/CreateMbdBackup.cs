using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO.Compression;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class CreateMbdBackup(ILogger<CreateMbdBackup> logger, CosmosClient client)
{
    private readonly ILogger<CreateMbdBackup> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("CreateMbdBackup")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("--Start Creation of MbdBackup--");
            var dbTempExportDirectory = Path.Combine(Path.GetTempPath(), "CosmosDB_Mbd");
            _logger.LogInformation("Using temp directory for export: {Path}", dbTempExportDirectory);

            if (Directory.Exists(dbTempExportDirectory))
            {
                 _logger.LogInformation("Cleaning up existing temp directory.");
                 Directory.Delete(dbTempExportDirectory, true);
            }
            Directory.CreateDirectory(dbTempExportDirectory);

            await ExportContainer<MbdCondition>(dbTempExportDirectory, CosmosDbConstants.Containers.MbdConditions);
            await ExportContainer<EmailSubmission>(dbTempExportDirectory, CosmosDbConstants.Containers.Emails);

            _logger.LogInformation("Creating Archive File...");
            var archiveFileName = Path.Combine(Path.GetTempPath(), "mbd_database.zip");
            if (File.Exists(archiveFileName))
            {
                _logger.LogInformation("Deleting existing archive file: {Path}", archiveFileName);
                File.Delete(archiveFileName);
            }

            ZipFile.CreateFromDirectory(dbTempExportDirectory, archiveFileName, CompressionLevel.Optimal, includeBaseDirectory: true);
            _logger.LogInformation("Archive created: {Path}", archiveFileName);
            
            var fileContents = await File.ReadAllBytesAsync(archiveFileName);
            _logger.LogInformation("Read {Size} bytes from archive.", fileContents.Length);

            _logger.LogInformation("Cleanup temp files and directory...");
            File.Delete(archiveFileName);
            Directory.Delete(dbTempExportDirectory, recursive: true);

             _logger.LogInformation("--MbdBackup Creation Completed Successfully--");
             return new FileContentResult(
                fileContents: fileContents,
                contentType: "application/octet-stream")
            {
                FileDownloadName = $"{DateTime.Now:yyyyMMdd_HHmmss}_mbd_database.zip"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateMbdBackup during execution.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    private async Task ExportContainer<T>(string basePath, string containerName)
    {
         _logger.LogInformation("Exporting {ContainerName}...", containerName);
         var directory = Path.Combine(basePath, containerName);
         Directory.CreateDirectory(directory);

         var items = await _client.QueryAsync<T>(
             databaseName: CosmosDbConstants.DatabaseName,
             containerName: containerName,
             query: "SELECT * FROM c");

         _logger.LogInformation("Found {Count} items in {ContainerName}", items.Count, containerName);

         int working = 0;
         foreach (var item in items)
         {
             working++;
             // Use reflection/dynamic to get ID since T is generic but we expect Id property
             dynamic dItem = item!;
             string id = dItem.Id ?? dItem.id ?? Guid.NewGuid().ToString();

             var file = Path.Combine(directory, $"{id}.json");
             var json = JsonConvert.SerializeObject(item);
             await File.WriteAllTextAsync(file, json);
         }
    }
}
