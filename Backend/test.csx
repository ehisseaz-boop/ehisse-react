using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

var client = new HttpClient(new HttpClientHandler { UseCookies = true });
var loginContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", "elcin"),
    new KeyValuePair<string, string>("password", "123")
});
await client.PostAsync("http://localhost:5000/Auth/Login", loginContent);

var addContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("Title", "Test"),
    new KeyValuePair<string, string>("Description", "Test"),
    new KeyValuePair<string, string>("Price", "100"),
    new KeyValuePair<string, string>("StockQuantity", "5"),
    new KeyValuePair<string, string>("WarrantyMonths", "12"),
    new KeyValuePair<string, string>("CategoryId", "1"),
    new KeyValuePair<string, string>("BrandId", "1"),
    new KeyValuePair<string, string>("DeviceModelId", "1"),
    new KeyValuePair<string, string>("PartTypeId", "1"),
    new KeyValuePair<string, string>("QualityTypeId", "1")
});
var res = await client.PostAsync("http://localhost:5000/Warehouse/AddPart", addContent);
Console.WriteLine(res.StatusCode);
var html = await res.Content.ReadAsStringAsync();
Console.WriteLine(html.Contains("Test") ? "Validation failed!" : "Success?");
