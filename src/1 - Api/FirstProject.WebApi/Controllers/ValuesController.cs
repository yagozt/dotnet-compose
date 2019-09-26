using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FirstProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;

        public ValuesController(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger("teste");
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            StringBuilder databases = new StringBuilder();
            string t = string.Empty;
            using(var cnn = new SqlConnection(@"Server=db;Database=msdb;user=sa;password=TesteSql@123"))
            {
                var lst = cnn.Query<string>("SELECT Name from sys.databases");
                t = lst.FirstOrDefault();
                var a = lst.Select(s => s);
                foreach (var item in lst)
                {
                    databases.Append(item);
                    databases.Append(",");
                }
            }
            return databases.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "pass", VirtualHost = "FirstProject" };
                using(var connection = factory.CreateConnection())
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "t1",
                        durable : true,
                        exclusive : false,
                        autoDelete : false,
                        arguments : null);

                    string message = value;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                        routingKey: "t1",
                        basicProperties : null,
                        body : body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }

                Console.WriteLine(" Mensagem enviada");
                Console.ReadLine();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}
