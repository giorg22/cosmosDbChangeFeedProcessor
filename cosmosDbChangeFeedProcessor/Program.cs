using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Cosmos;

string endpointUri = "https://giorgiscosmosdb.documents.azure.com:443/";
string primaryKey = "pA6VZ29Sho70tLv1wZ8856SFwz4YLtGDN6PXUfbHVb5Ez5VEeXymTFnsbk2cTtEiRrgwEEfizxkAACDb5pP6IA==";
string databaseName = "testDB";
string containerName = "computers";
string leaseContainerName = "leases";

// Initialize Cosmos client
CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey);
Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
/*Container container = await database.CreateContainerIfNotExistsAsync(containerName, "/computerId");
Container leaseContainer = await database.CreateContainerIfNotExistsAsync(leaseContainerName, "/id");*/
Container container = cosmosClient.GetContainer(databaseName, containerName);
Container leaseContainer = cosmosClient.GetContainer(databaseName, leaseContainerName);

var changeFeedProcessorBuilder = container.GetChangeFeedProcessorBuilder("changefeedprocessor", 
    async (IReadOnlyCollection<dynamic> docs, CancellationToken cancellationToken) =>
{
    foreach (var doc in docs)
    {
        Console.WriteLine($"Document ID: {doc.id}");
        // Process or handle the changes here
    }
});

var changeFeedProcessor = changeFeedProcessorBuilder
    .WithInstanceName("instance")
    .WithLeaseContainer(leaseContainer)
    .WithMaxItems(10)
    .Build();

await changeFeedProcessor.StartAsync();
        
// Keep the application running
Console.WriteLine("Press any key to stop the processor...");
Console.ReadKey();

await changeFeedProcessor.StopAsync();