global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using MindBodyDictionary.Shared;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Azure.Cosmos;
global using Microsoft.Azure.Functions.Worker;
global using Microsoft.Azure.Functions.Worker.Builder;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Newtonsoft.Json;

global using SharedMbdCondition = MindBodyDictionary.Shared.MbdCondition;
global using MindBodyDictionary.Shared.Enumerations;
global using backend.CosmosDB;
