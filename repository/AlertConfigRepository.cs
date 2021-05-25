using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using meatmonitorapi.Models;
using meatmonitorapi.utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace meatmonitorapi.repository
{
    public class AlertConfigRepository : IAlertConfig
    {
        private readonly IConfiguration _config;
        public AlertConfigRepository(IConfiguration config)
        {
            _config = config;
        }

        public List<AlertConfigEntity> GetAllAlertConfig(string partitionKey)
        {
            TableQuery<AlertConfigEntity> query = new TableQuery<AlertConfigEntity>().Where(
            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var tableClient = new AzureTableStorage<AlertConfigEntity>(_config["ConnectionStrings:StorageAccount"], _config["AppSettings:AlertConfigTable"]);
            return tableClient.GetMany(query).Result;
        }

        public AlertConfigEntity UpdateAlertConfig(AlertConfig pc)
        {
            if (pc.phoneNumber != null || pc.phoneNumber != "")
            {
                var rule = "^\\(*\\+*[1-9]{0,3}\\)*-*[1-9]{0,3}[-. \\/]*\\(*[2-9]\\d{2}\\)*[-. \\/]*\\d{3}[-. \\/]*\\d{4} *e*x*t*\\.* *\\d{0,4}$";
                Match match = Regex.Match(pc.phoneNumber, rule, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    Console.WriteLine("Successfully matched phone number");
                    var insert = new AlertConfigEntity()
                    {
                        RowKey = pc.phoneNumber,
                        firstName = pc.firstName,
                        lastName = pc.lastName,
                        phoneNumber = pc.phoneNumber
                    };

                    var tableClient = new AzureTableStorage<AlertConfigEntity>(_config["ConnectionStrings:StorageAccount"], _config["AppSettings:AlertConfigTable"]);
                    return tableClient.InsertOrUpdateAsync(insert).Result;
                }
                else
                {
                    throw new Exception($"Phone number {pc.phoneNumber} does not appear to be a valid phone number");
                }
            }
            else
            {
                throw new Exception("Phone Number cannot be null");

            }
        }
    }
}