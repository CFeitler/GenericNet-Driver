using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace zenBot.BotService
{
  public class ChatBotConnection
  {
    //type: POST
    private string UriEndpoint =>
      "https://www.botlibre.com/rest/api/form-chat?&application={0}&instance={1}&message={2}";

    //request.setRequestHeader('Content-Type', 'application/xml');
    private string applicationId => "1591608432481105475";
    public string InstanceId { get; set; }
    private static readonly HttpClient client = new HttpClient();

    public ChatBotConnection(string instanceId)
    {
      InstanceId = instanceId;
    }
    public async Task<string> SendMessage(string message)
    {
      try
      {
        var urlWithcontent = string.Format(UriEndpoint, applicationId, InstanceId, message.Replace(" ", "+"));

        var response = await client.GetStringAsync(urlWithcontent);
        var xdoc = XDocument.Parse(response);
        var messagenode = xdoc.Descendants("message").FirstOrDefault();
                    
        return messagenode?.Value ?? "message not found";


      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return "Error - probably maximum of API calls per day reached.";
      }
    }
  }
}
