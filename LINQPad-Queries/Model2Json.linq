<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\MonoTouch\v1.0\Newtonsoft.Json.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{
	Console.WriteLine(JsonConvert.SerializeObject(new Item()));	
}

public class Item
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "barcode")]
    public string Barcode { get; set; }

    [JsonProperty(PropertyName = "providerId")]
    public int ProviderId { get; set; }

    [JsonProperty(PropertyName = "itemNameHE")]
    public string ItemNameHE { get; set; }

    [JsonProperty(PropertyName = "itemNameEN")]
    public string ItemNameEN { get; set; }

    [JsonProperty(PropertyName = "unit")]
    public string Unit { get; set; }

    [JsonProperty(PropertyName = "characteristic1")]
    public string Characteristic1 { get; set; }

    [JsonProperty(PropertyName = "characteristic2")]
    public string Characteristic2 { get; set; }

    [JsonProperty(PropertyName = "sortCode1")]
    public int SortCode1 { get; set; }

    [JsonProperty(PropertyName = "sortCode2")]
    public int SortCode2 { get; set; }

    [JsonProperty(PropertyName = "sortCode3")]
    public int SortCode3 { get; set; }

    [JsonProperty(PropertyName = "vatFlag")]
    public string VatFlag { get; set; }

    [JsonProperty(PropertyName = "price1")]
    public float Price1 { get; set; }

    [JsonProperty(PropertyName = "price2")]
    public float Price2 { get; set; }

    [JsonProperty(PropertyName = "price3")]
    public float Price3 { get; set; }

    [JsonProperty(PropertyName = "price4")]
    public float Price4 { get; set; }

    [JsonProperty(PropertyName = "price5")]
    public float Price5 { get; set; }

    [JsonProperty(PropertyName = "price6")]
    public float Price6 { get; set; }

    [JsonProperty(PropertyName = "price7")]
    public float Price7 { get; set; }

    [JsonProperty(PropertyName = "price8")]
    public float Price8 { get; set; }

    [JsonProperty(PropertyName = "price9")]
    public float Price9 { get; set; }

    [JsonProperty(PropertyName = "price10")]
    public float Price10 { get; set; }

    [JsonProperty(PropertyName = "currencyCode1")]
    public int CurrencyCode1 { get; set; }

    [JsonProperty(PropertyName = "currencyCode2")]
    public int CurrencyCode2 { get; set; }

    [JsonProperty(PropertyName = "currencyCode3")]
    public int CurrencyCode3 { get; set; }

    [JsonProperty(PropertyName = "currencyCode4")]
    public int CurrencyCode4 { get; set; }

    [JsonProperty(PropertyName = "currencyCode5")]
    public int CurrencyCode5 { get; set; }

    [JsonProperty(PropertyName = "currencyCode6")]
    public int CurrencyCode6 { get; set; }

    [JsonProperty(PropertyName = "currencyCode7")]
    public int CurrencyCode7 { get; set; }

    [JsonProperty(PropertyName = "currencyCode8")]
    public int CurrencyCode8 { get; set; }

    [JsonProperty(PropertyName = "currencyCode9")]
    public int CurrencyCode9 { get; set; }

    [JsonProperty(PropertyName = "currencyCode10")]
    public int CurrencyCode10 { get; set; }

    [JsonProperty(PropertyName = "supplerId1")]
    public int SupplerId1 { get; set; }

    [JsonProperty(PropertyName = "supplerId2")]
    public int SupplerId2 { get; set; }

    [JsonProperty(PropertyName = "supplerId3")]
    public int SupplerId3 { get; set; }

    [JsonProperty(PropertyName = "supplerId4")]
    public int SupplerId4 { get; set; }

    [JsonProperty(PropertyName = "createdDate")]
    public DateTime? CreatedDate { get; set; }

    [JsonProperty(PropertyName = "updatedDate")]
    public DateTime? UpdatedDate { get; set; }
}

