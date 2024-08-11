<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\MonoTouch\v1.0\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>

void Main()
{				   
	var errorMsg = "{\"Status\":200,\"Message\":{\"errors\":[{\"code\":432,\"message\":\"Customer VAT Number is incorrect\",\"param\":\"Customer_VAT_Number\",\"location\":\"validation\"}]},\"Confirmation_Number\":0}";
	var response = JsonConvert.DeserializeObject<CreateInvoiceResponse>(errorMsg);
	Console.WriteLine(response);
	
	var result = ParseError(errorMsg);
	Console.WriteLine(result);
}

APIErrorResponse ParseError(string ErrorRaw)
{
    /*
        // schema
        <Http-Error>|<Request-Error>

        -

        // sample
        The remote server returned an error: (401)Unauthorized.|{
            "error": "invalid_request",
            "error_description": "Redirect URI specified in the request is not configured in the client subscription"
        }

        -

        // schema types                
        {
            "httpCode": "401",
            "httpMessage": "Unauthorized",
            "moreInformation": "Invalid client id or secret."
        }

        {
            "error": "invalid_request",
            "error_description": "Redirect URI specified in the request is not configured in the client subscription"
        }

        {
            "Status": 406,
            "Message": "Not Acceptable",
            "Error_Id": "71523446191"
        }

        {
          "Status": 400,
          "Message": {
            "errors": [
              {
                "value": " ",
                "msg": "Value should be numeric of type int ",
                "param": "Phone_Of_Driver",
                "location": "body"
              }
            ]
          },
          "Error_Id": "04565273934"
        }

		{
		  "Status": 200,
		  "Message": {
		    "errors": [
		      {
		        "code": 432,
		        "message": "Customer VAT Number is incorrect",
		        "param": "Customer_VAT_Number",
		        "location": "validation"
		      }
		    ]
		  },
		  "Confirmation_Number": 0
		}
    */
    var errorRawParts = ErrorRaw.Split('|');
    var httpError = errorRawParts[0];
    var requestError = errorRawParts.Length > 1 ? errorRawParts[1] : errorRawParts[0];

    var result = new APIErrorResponse {
        Message = httpError?.Trim()                
    };

    // parse by Schema type
    if (requestError.Contains("error_description"))
    {
        var errorSchema = new
        {
            error = string.Empty,
            error_description = string.Empty
        };

        var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
        result.InnerMessage = (
            exData?.error?.Trim() ?? string.Empty,
            exData?.error_description?.Trim() ?? string.Empty
        );
    }
    else if(requestError.Contains("Error_Id"))  
    {
		// Single Message (Singular)
        var errorSchema = new
        {
            Message = string.Empty
        };

        var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
        result.InnerMessage = (
            exData?.Message?.Trim() ?? string.Empty,
            exData?.Message?.Trim() ?? string.Empty
        );        
    }
	else if(requestError.Contains("errors")){
		// Array of Messages (Plural)
        var itemSchema = new
        {
            value = string.Empty,
            msg = string.Empty,
			message = string.Empty,
            param = string.Empty
        };

        var messageSchemaErrors = new
        {
            errors = new[] { itemSchema }
        };

        var errorSchema = new
        {
            Message = messageSchemaErrors
        };

        var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
		
		var message = exData?.Message?.errors.FirstOrDefault().msg?.Trim() ?? exData?.Message?.errors.FirstOrDefault().message?.Trim() ?? string.Empty;
        result.InnerMessage = (
            message,
            exData?.Message?.errors.FirstOrDefault().param?.Trim() ?? string.Empty
        );        
	}
    else if (requestError.Contains("httpCode")) 
    {
        var errorSchema = new
        {
            httpCode = 0,
            httpMessage = string.Empty,
            moreInformation = string.Empty
        };

        var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
        result.InnerMessage = (
            $"{exData?.httpMessage?.Trim() ?? string.Empty} ({exData?.httpCode ?? -1})",
            exData?.moreInformation?.Trim() ?? string.Empty
        );
    }

    return result;
}

class APIErrorResponse
{
    public string Message { get; set; }
    public (string Error, string Details) InnerMessage { get; set; }

    public override string ToString()
    {
        return $"{this.Message} | {this.InnerMessage.Error} | {this.InnerMessage.Details}";
    }
}

class CreateInvoiceResponse
{            
    public int Status { get; set; }            
    public dynamic Message { get; set; }

    [JsonProperty(PropertyName = "Confirmation_Number")]
    public string Confirmation { get; set; }

    public override string ToString()
    {
        return $"{this.Status} | {this.Message} | {this.Confirmation}";
    }
}