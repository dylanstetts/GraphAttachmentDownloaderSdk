using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;

namespace GraphAttachmentDownloaderSdk.Services
{
    public static class GraphClientFactory
    {
        public static ClientSecretCredential Credential { get; private set; }

        public static GraphServiceClient Create(IConfiguration config)
        {
            Credential = new ClientSecretCredential(
                config["AzureAd:TenantId"],
                config["AzureAd:ClientId"],
                config["AzureAd:ClientSecret"]
            );

            return new GraphServiceClient(Credential);
        }
    }

}
