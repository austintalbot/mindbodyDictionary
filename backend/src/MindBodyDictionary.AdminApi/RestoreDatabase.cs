using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;


namespace MindBodyDictionary.AdminApi
{
    public class RestoreDatabase
    {
        private readonly ILogger<RestoreDatabase> _logger;

        public RestoreDatabase(ILogger<RestoreDatabase> logger)
        {
            _logger = logger;
        }

        [Function("RestoreDatabase")]
        public  async Task<IActionResult> Run(
                   [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
                   [CosmosDBInput(
                Connection = Core.CosmosDB.ConnectionStringSetting)] CosmosClient client)
        {
            var archiveFilename = Path.Combine(Path.GetTempPath(), "database.zip");
            var dbDirectory = Path.Combine(Path.GetTempPath(), "CosmosDB");

            try
            {
                _logger.LogInformation("--Start Restore of DB backup--");

                IFormFile? file = req.Form?.Files?.First() ?? null;
                if (file == null)
                {
                    return new BadRequestResult();
                }

                _logger.LogInformation($"Save file to {archiveFilename}");
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    File.WriteAllBytes(archiveFilename, ms.ToArray());
                }

                _logger.LogInformation("Extract database backup...");
                if (Directory.Exists(dbDirectory))
                {
                    Directory.Delete(dbDirectory, recursive: true);
                }
                ZipFile.ExtractToDirectory(archiveFilename, Path.GetTempPath());

                _logger.LogInformation("Import Ailments...");
                if (Directory.Exists(Path.Combine(dbDirectory, Core.CosmosDB.Containers.Ailments)))
                {
                    var directory = Path.Combine(dbDirectory, Core.CosmosDB.Containers.Ailments);
                    var files = Directory.GetFiles(directory, "*.json");
                    var container = client.GetContainer(Core.CosmosDB.DatabaseName, Core.CosmosDB.Containers.Ailments);
                    foreach (var importFile in files)
                    {
                        var json = File.ReadAllText(importFile);
                        var item = JsonConvert.DeserializeObject<Core.Entities.Ailment>(json);
                        _logger.LogInformation($"Importing Ailment[{item?.id}]: {item?.Name}");
                        await container.UpsertItemAsync(item);
                    }
                }
                else
                {
                    throw new Exception("Could not find Ailments in the database backup!");
                }

                _logger.LogInformation("Import Emails...");
                if (Directory.Exists(Path.Combine(dbDirectory, Core.CosmosDB.Containers.Emails)))
                {
                    var directory = Path.Combine(dbDirectory, Core.CosmosDB.Containers.Emails);
                    var files = Directory.GetFiles(directory, "*.json");
                    var container = client.GetContainer(Core.CosmosDB.DatabaseName, Core.CosmosDB.Containers.Emails);
                    foreach (var importFile in files)
                    {
                        var json = File.ReadAllText(importFile);
                        var item = JsonConvert.DeserializeObject<Core.Entities.EmailSubmission>(json);
                        _logger.LogInformation($"Importing Email[{item?.id}]: {item?.Email}");
                        await container.UpsertItemAsync(item);
                    }
                }
                else
                {
                    throw new Exception("Could not find Emails in the database backup!");
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogInformation("Cleanup...");

                if (Directory.Exists(dbDirectory))
                {
                    Directory.Delete(dbDirectory, recursive: true);
                }
                if (File.Exists(archiveFilename))
                {
                    File.Delete(archiveFilename);
                }
            }
        }
    }
}
