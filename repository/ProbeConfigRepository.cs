using tempaastapi.Models;
using tempaastapi.utils;
using Microsoft.Extensions.Configuration;

namespace tempaastapi.repository
{
    public class ProbeConfigRepository : IProbeConfig
    {
        private readonly IConfiguration _config;
        private string probePartition = "probeconfig";
        public ProbeConfigRepository(IConfiguration config) {
            _config = config;
        }

        public ProbeConfig GetProbeConfig(string rowKey)
        {
            var tableClient = new AzureTableStorage<ProbeConfig>(_config["ConnectionStrings:StorageAccount"], _config["ProbeConfigTable"]);
            return tableClient.Get(probePartition, rowKey).Result;
        }

        public ProbeConfig UpdateProbeConfig(ProbeConfig pc)
        {
            var tableClient = new AzureTableStorage<ProbeConfig>(_config["ConnectionStrings:StorageAccount"], _config["ProbeConfigTable"]);

            return tableClient.InsertOrUpdateAsync(pc).Result;
        }
    }
}