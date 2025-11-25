using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MindBodyDictionary.CosmosDB;
using Newtonsoft.Json;
using System.IO.Compression;
using Microsoft.Azure.Functions.Worker;

namespace MindBodyDictionary.AdminApi
{
	public  class CreateBackup
	{
		private readonly ILogger<CreateBackup> _logger;

		public CreateBackup(ILogger<CreateBackup> logger)
		{
			_logger = logger;
		}


		[Function("CreateBackup")]
		public  async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
			[CosmosDBInput(
				Connection = Core.CosmosDB.ConnectionStringSetting)] CosmosClient client)
		{
			try
			{
				_logger.LogInformation("--Start Creation of DB backup--");
				var dbTempExportDirectory = Path.Combine(Path.GetTempPath(), "CosmosDB");
				_logger.LogInformation($"Using export path: ${dbTempExportDirectory}");
				if (!Directory.Exists(dbTempExportDirectory))
				{
					Directory.CreateDirectory(dbTempExportDirectory);
				}

				_logger.LogInformation("Export Aliments...");
				var directory = Path.Combine(dbTempExportDirectory, Core.CosmosDB.Containers.Ailments);
				if (!Directory.Exists(directory))
				{
					_logger.LogInformation($"Create directory ${directory}");
					Directory.CreateDirectory(directory);
				}
				var ailments = await client.QueryAsync<Core.Entities.Ailment>(
					databaseName: Core.CosmosDB.DatabaseName,
					containerName: Core.CosmosDB.Containers.Ailments,
					query: "SELECT * FROM c");
				var total = ailments.Count;
				var working = 0;
				_logger.LogInformation($"Found ${total}");
				foreach (var item in ailments)
				{
					working++;
					var file = Path.Combine(directory, $"{item.id}.json");
					var json = JsonConvert.SerializeObject(item);
					_logger.LogInformation($"${working} of ${total}: ${file}");
					File.WriteAllText(file, json);
				}

				_logger.LogInformation("Export Emails...");
				directory = Path.Combine(dbTempExportDirectory, Core.CosmosDB.Containers.Emails);
				if (!Directory.Exists(directory))
				{
					_logger.LogInformation($"Create directory ${directory}");
					Directory.CreateDirectory(directory);
				}
				var emails = await client.QueryAsync<Core.Entities.EmailSubmission>(
					databaseName: Core.CosmosDB.DatabaseName,
					containerName: Core.CosmosDB.Containers.Emails,
					query: "SELECT * FROM c");
				total = emails.Count;
				working = 0;
				_logger.LogInformation($"Found ${total}");
				foreach (var item in emails)
				{
					working++;
					var file = Path.Combine(directory, $"{item.id}.json");
					var json = JsonConvert.SerializeObject(item);
					_logger.LogInformation($"${working} of ${total}: ${file}");
					File.WriteAllText(file, json);
				}

				_logger.LogInformation("Create Archive File...");
				var archiveFileName = Path.Combine(Path.GetTempPath(), "database.zip");
				ZipFile.CreateFromDirectory(dbTempExportDirectory, archiveFileName, CompressionLevel.Optimal, includeBaseDirectory: true);
				var fileContents = File.ReadAllBytes(archiveFileName);
				_logger.LogInformation("Cleanup...");
				File.Delete(archiveFileName);
				Directory.Delete(dbTempExportDirectory, recursive: true);

				return new FileContentResult(
					fileContents: fileContents,
					contentType: "application/octet-stream")
				{
					FileDownloadName = $"{DateTime.Now.ToString("yyyyMMdd_hhmmss")}_database.zip"
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in CreateBackup function");
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
