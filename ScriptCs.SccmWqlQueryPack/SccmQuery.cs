using ScriptCs.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ScriptCs.SccmWqlQueryPack
{
    public class SccmQuery : IScriptPackContext
    {
        public string Execute(string serverName, string query)
        {
            string j = "";

            WqlConnectionManager conn = connect(serverName);
            if (conn != null)
            {
                IResultObject result = q(conn, query);

                if (result != null)
                {
                    try
                    {

                        List<Dictionary<string, string>> metas = new List<Dictionary<string, string>>() {
						conn.NamedValueDictionary
						.ToDictionary(k => k.Key, k => k.Value.ToString())
						.Union(
							new Dictionary<string, string>() { 
								{ "WQL-Query", query }
							})
						.ToDictionary(k => k.Key, k => k.Value)
					};

                        List<Dictionary<string, string>> datas = new List<Dictionary<string, string>>();

                        foreach (IResultObject u in result)
                        {
                            var data = new Dictionary<string, string>() {
								{ "ObjectClass", u.ObjectClass}
							}
                                .Union(u.PropertyList)
                                .ToDictionary(k => k.Key, k => k.Value);

                            datas.Add(data);

                            try
                            {
                                foreach (IResultObject g in u.GenericsArray)
                                {
                                    datas
                                    .Add(new Dictionary<string, string>() {
										{ "ObjectClass", g.ObjectClass}
									}
                                        .Union(g.PropertyList)
                                        .ToDictionary(k => k.Key, k => k.Value));
                                }
                            }
                            catch { }
                        }

                        j = JsonConvert.SerializeObject(new
                        {
                            meta = metas,
                            data = datas
                        });
                    }
                    catch
                    {
                        j = JsonConvert.SerializeObject(new { exception = "Query Failed" });
                    }
                }

                conn.Close();
            }
            else
            {
                j = JsonConvert.SerializeObject(new { exception = "Connection Failed" });
            }

            return j;
        }

        private WqlConnectionManager connect(string serverName)
        {
            try
            {
                SmsNamedValuesDictionary namedValues = new SmsNamedValuesDictionary();
                WqlConnectionManager connection = new WqlConnectionManager(namedValues);
                connection.Connect(serverName);
                return connection;
            }
            catch (SmsException e)
            {
                Console.WriteLine("Failed to Connect. Error: " + e.Message);
                return null;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Failed to authenticate. Error:" + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
                return null;
            }
        }

        private IResultObject q(WqlConnectionManager connection, string query)
        {
            Console.WriteLine("Query :" + query);

            try
            {
                IResultObject result = connection.QueryProcessor.ExecuteQuery(query);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Query Execution Error:" + e.Message);
                return null;
            }
        }
    }
}