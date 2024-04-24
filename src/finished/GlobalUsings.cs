﻿global using MaterializedViewProcessor.Activities;
global using MaterializedViewProcessor.Extensions;
global using MaterializedViewProcessor.Models;
global using MaterializedViewProcessor.Orchestrator;
global using MaterializedViewProcessor.Utilities;
global using Microsoft.Azure.Cosmos;
global using Microsoft.Azure.Cosmos.Fluent;
global using Microsoft.Azure.Functions.Worker;
global using Microsoft.Azure.Functions.Worker.Http;
global using Microsoft.DurableTask;
global using Microsoft.DurableTask.Client;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Serilog;
global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;