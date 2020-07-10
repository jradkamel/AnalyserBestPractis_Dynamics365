using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement;
using System.Collections;

using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace AnalyserBestPractis_Dynamics365
{
    class TeamServeceConnect
    {
        public static string getURI(string _uri)
        {
            string text = System.IO.File.ReadAllText(@""+ _uri);

            string[] lines = System.IO.File.ReadAllLines(@""+ _uri);
            string uri = "";
          
            foreach (string line in lines)
            {
                if(line.Contains("SccTeamFoundationServer"))
                {
                    uri = line.Split('=')[1].TrimStart(' ');
                    return uri;
                }
            }
            return "";
        }
        public static ArrayList getListTeamProjects(string _uri)
        {
            string text = System.IO.File.ReadAllText(@"" + _uri);
            
            string[] lines = System.IO.File.ReadAllLines(@"" + _uri);
            ArrayList projects = new ArrayList();
            int i = 0;
            foreach (string line in lines)
            {
                if (line.Contains("SccProjectName"))
                {
                    projects.Add(line.Split('=')[1].TrimStart(' '));
                }
                i++;
            }
            return projects;
        }
        public static void getListWorkitem(string _uri)
        {
            Uri collectionUri = new Uri(_uri);
            TfsTeamProjectCollection teamProjectCollection = new TfsTeamProjectCollection(collectionUri);
            teamProjectCollection.EnsureAuthenticated();

           
            //PrintOpenBugsAsync("", teamProjectCollection.Credentials, collectionUri);
        }
        

        
        public static async Task<IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>> QueryOpenBugs(string project, VssCredentials _credential, Uri _uri)
        {
           

            // create a wiql object and build our query
            var wiql = new Wiql()
            {
                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = "Select [Id],[TeamProject] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        //"And [System.TeamProject] = '" + project + "' " +
                        "And [System.State] <> 'Closed' " +
                        "Order By [State] Asc, [Changed Date] Desc",
            };

            // create instance of work item tracking http client
            using (var httpClient = new WorkItemTrackingHttpClient(_uri, _credential))
            {
                // execute the query to get the list of work items in the results
                var result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
                var ids = result.WorkItems.Select(item => item.Id).ToArray();

                

                // build a list of the fields we want to see
                var fields = new[] { "System.Id", "System.Title", "System.State" };

                // get work items for the ids found in query
                return await httpClient.GetWorkItemsAsync(ids, fields, result.AsOf).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Execute a WIQL (Work Item Query Language) query to print a list of open bugs.
        /// </summary>
        /// <param name="project">The name of your project within your organization.</param>
        /// <returns>An async task.</returns>
        public static async Task PrintOpenBugsAsync(string project, VssCredentials _credential, Uri _uri)
        {
            //VssCredentials vsc = new VssCredentials(new Microsoft.VisualStudio.Services.Common.WindowsCredential(
            //   new NetworkCredential(this._username, this._password)));
            //VssConnection connection = new VssConnection(url, vsc);
            //var workItems = await QueryOpenBugs(project, _credential, _uri).ConfigureAwait(false);

            //Console.WriteLine("Query Results: {0} items found", workItems.Count);

            //// loop though work items and write to console
            //foreach (var workItem in workItems)
            //{
            //    MessageBox.Show(workItem.Id.ToString());
            //    MessageBox.Show(workItem.Fields["System.Title"].ToString());
            //        MessageBox.Show(workItem.Fields["System.State"].ToString());
               
            //}
        }


    }
}
