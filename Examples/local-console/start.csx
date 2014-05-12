var qe = Require<SccmQuery>();

// SCCM server FQDN
var s = "your-sccm-server.domain.com";

// sample queries

var q1 = @"SELECT DISTINCT *
			FROM SMS_R_System AS Sys 
			INNER JOIN SMS_G_System_ADD_REMOVE_PROGRAMS AS ARP ON  
				ARP.ResourceId = Sys.ResourceId
			WHERE 
				ARP.DisplayName LIKE '%Visio%'";
var q2 = @"select R.ResourceID, R.ResourceType, R.Name, R.SMSUniqueIdentifier ,R.ResourceDomainORWorkgroup, R.Client 
			from SMS_R_System as r full join SMS_R_System as s1 on s1.ResourceId = r.ResourceId 
			full join SMS_R_System as s2 on 
				s2.Name = s1.Name where s1.Name = s2.Name and s1.ResourceId != s2.ResourceId and s1.Name <> 'Unknown'";
				
Console.WriteLine(qe.Execute(s,q2));
