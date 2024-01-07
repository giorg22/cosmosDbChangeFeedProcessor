using Microsoft.Azure.Cosmos;

string endpointUri = "https://giorgiscosmosdb.documents.azure.com:443/";
string primaryKey = "pA6VZ29Sho70tLv1wZ8856SFwz4YLtGDN6PXUfbHVb5Ez5VEeXymTFnsbk2cTtEiRrgwEEfizxkAACDb5pP6IA==";
string databaseName = "testDB";
string containerName = "computers";
string leaseContainerName = "leases";

// Initialize Cosmos client
CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey);
Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
Container container = cosmosClient.GetContainer(databaseName, containerName);
Container leaseContainer = cosmosClient.GetContainer(databaseName, leaseContainerName);

dynamic newItem = new
{
    id = Guid.NewGuid().ToString(),
    name = "New Item",
    category = "Example",
    // other properties...
};

ItemResponse<dynamic> createdItem = await container.CreateItemAsync(newItem);

Console.WriteLine($"Created Item: {createdItem.Resource.id}");

// Retrieve an item by its ID
string itemId = createdItem.Resource.id;
ItemResponse<dynamic> retrievedItem = await container.ReadItemAsync<dynamic>(itemId, new PartitionKey(itemId));

Console.WriteLine($"Retrieved Item: {retrievedItem.Resource.id}");

// Update an item
dynamic updatedItem = retrievedItem.Resource;
updatedItem.category = "Updated Category";
ItemResponse<dynamic> replacedItem = await container.ReplaceItemAsync(updatedItem, updatedItem.id, new PartitionKey(updatedItem.id));

Console.WriteLine($"Updated Item: {replacedItem.Resource.id}");

// Delete an item
ItemResponse<dynamic> deletedItem = await container.DeleteItemAsync<dynamic>(updatedItem.id, new PartitionKey(updatedItem.id));

Console.WriteLine($"Deleted Item: {deletedItem.Resource.id}");