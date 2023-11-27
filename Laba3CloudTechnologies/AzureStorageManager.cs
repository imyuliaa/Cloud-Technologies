using Microsoft.Azure.Cosmos.Table;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace Laba2CloudTechnologies;

public class AzureStorageManager
{
    private CloudTableClient tableClient;
    private CloudTable table;
    private string blobContainerName = "contactsimages";
    private BlobServiceClient blobServiceClient; // Додайте це поле
    private BlobContainerClient containerClient;  // Додайте це поле
    public AzureStorageManager(string storageConnectionString)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        table = tableClient.GetTableReference("Contacts");

        // Ініціалізація Azure Blob Storage
        blobServiceClient = new BlobServiceClient(storageConnectionString);
        containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

        if (!containerClient.Exists())
        {
            // Якщо контейнер не існує, створюємо його
            containerClient.Create();
            Console.WriteLine("Контейнер створено успішно.");
        }
        else
        {
            Console.WriteLine("Контейнер вже існує.");
        }
    }

    public async Task InitializeAsync()
    {
        await table.CreateIfNotExistsAsync();
    }

    public async Task InsertOrMergeEntityAsync(Contact entity)
    {
        TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
        await table.ExecuteAsync(insertOrMergeOperation);
    }
    public async Task DeleteEntityAsync(Contact entity)
    {
        TableOperation deleteOperation = TableOperation.Delete(entity);
        await table.ExecuteAsync(deleteOperation);
    }
    public List<Contact> GetAllContactsAsync()
    {
        return table.ExecuteQuery(new TableQuery<Contact>(), null).ToList();
    }
    public async Task UploadPhotoAsync(string blobName, Stream photoStream)
    {
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(photoStream, true);
    }


}