//TODO: Need to figure out how to use Require<T> inside of the ApiController class.

using System.Net.Http;

//TODO: can not require pack here because we're only going to sue it from ApiController
//var qe = Require<SccmQuery>();
var webApi = Require<WebApi>();
var server = webApi.CreateServer("http://localhost:8888");
server.OpenAsync().Wait();

Console.WriteLine("Listening...");
Console.ReadKey();
server.CloseAsync().Wait();

public class SccmQueryController : ApiController 
{
    public HttpResponseMessage Get([FromUri] string server, [FromUri] string query) 
	{
		//TODO: qe is non-existent
		//var result = qe.Execute(server,query);
		
		var qe = new SccmQuery();
		var result = qe.Execute(server,query);
		
        return new HttpResponseMessage 
		{
			Content = new StringContent(
				result, 
				System.Text.Encoding.UTF8, 
				"application/json")
		};
    }
}

// open the web to the following (returns machines with more than ~64GB RAM)
// http://localhost:8888/SccmQuery?server=your-sccm.domain.com&query=select%20distinct%20SMS_R_System.Name,%20SMS_R_System.ADSiteName,%20SMS_G_System_X86_PC_MEMORY.TotalPhysicalMemory,%20SMS_R_System.IPAddresses,%20SMS_R_System.LastLogonUserName,%20SMS_R_System.DistinguishedName%20from%20SMS_R_System%20inner%20join%20SMS_G_System_X86_PC_MEMORY%20on%20SMS_G_System_X86_PC_MEMORY.ResourceID%20=%20SMS_R_System.ResourceId%20where%20SMS_G_System_X86_PC_MEMORY.TotalPhysicalMemory%20%3E=%2064000000%20order%20by%20SMS_R_System.Name,%20SMS_R_System.ADSiteName,%20SMS_R_System.IPAddresses,%20SMS_R_System.LastLogonUserName,%20SMS_R_System.DistinguishedName