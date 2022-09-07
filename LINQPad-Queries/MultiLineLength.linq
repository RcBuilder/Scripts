<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>System.Threading</Namespace>
</Query>

void Main()
{
	var maxLineLength = 20;
	
	var lst = new string[] {
		"SaleId",
        "SaleCompanyCode",
        "SaleCompanyName",
        "SaleBranchCode",
        "SaleBranchName",
        "SaleStorageCode",
        "SaleStorageName",
        "SalePosCode",
        "SalePosName",
        "SaleCashierCode",
        "SaleCashierName",
        "SaleSellerCode",
        "SaleSellerName",
        "SaleDate",
        "SaleTime",
        "SaleDocumentNumber",
        "SaleDocumentTypeCode",
        "SaleDocumentTypeName",
        "SaleDocumentTotalNoTax",
        "SaleDocumentTotalWithTax",
        "SaleCustomerCode",
        "SaleCustomerName",
        "SaleSupplierCode",
        "SaleSupplierName",
        "SaleItemCode",
        "SaleItemName",
        "SaleQuantity",
        "SaleUnitPriceWithTax",
        "SaleUnitFinalPriceWithTax",
        "SaleUnitFinalPriceNoTax",
        "SaleUnitCostNoTax",
        "SaleItemTagsVector",
        "SaleItemBargainsVector",
        "SaleLineDiscount",
        "SaleTaxValue",
        "SaleDocZIndex",
        "SaleVatIdNo",
        "SaleUnitPriceNoTax",
        "SaleDestStorageCode",
        "SaleDestStorageName",
        "SaleLoaded",
        "SaleCustomerAccount"
	};
	
	foreach(var s in lst.Where(s => s.Trim().Length > maxLineLength))
		Console.WriteLine(s.Substring(0, maxLineLength));
}
