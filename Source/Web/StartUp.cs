using System;
using System.IO;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Owin.WebSocket.Extensions;
using System.Runtime.ExceptionServices;
using Microsoft.Owin.Hosting;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.ErrorHandling;

namespace MMOLauncher.Web
{
    class StartUp
    {
        IDisposable _server;

        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.UseCors(CorsOptions.AllowAll);
                app.MapWebSocketRoute<WebSocket.MyWebSocket>("/ws");
                app.Map("/signalr", map =>
                {
                    // Setup the CORS middleware to run before SignalR.
                    // By default this will allow all origins. You can 
                    // configure the set of origins and/or http verbs by
                    // providing a cors options with a different policy.
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration
                    {
                        EnableDetailedErrors = true
                        //EnableJSONP = true
                        // You can enable JSONP by uncommenting line below.
                        // JSONP requests are insecure but some older browsers (and some
                        // versions of IE) require JSONP to work cross domain
                        // EnableJSONP = true
                    };
                    // Run the SignalR pipeline. We're not using MapSignalR
                    // since this branch already runs under the "/signalr"
                    // path.
                    map.RunSignalR(hubConfiguration);
                });
                
                app.UseNancy();
                //.Use(MyMiddleware.DoIt())
                //.UseNancy()
                //.UseCors(CorsOptions.AllowAll)
            }
        }


        public void StartRun()
        {
            var options = new StartOptions()
            {
                ServerFactory = "Nowin",
                Port = 8888
            };

            Console.WriteLine("Running a http server on port 8888");
            _server = WebApp.Start<Startup>(options);
        }

    }


    //Response from ByteArray/MemorySteam -> Needed for On-the-Fly Image Creation
    public class ByteArrayResponse : Response
    {
        /// <summary>
        /// Byte array response
        /// </summary>
        /// <param name="body">Byte array to be the body of the response</param>
        /// <param name="contentType">Content type to use</param>
        public ByteArrayResponse(byte[] body, string contentType = null)
        {
            this.ContentType = contentType ?? "application/octet-stream";

            this.Contents = stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(body);
                }
            };
        }
    }

    public class NancyBoostrapper : DefaultNancyBootstrapper
    {



        protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {

            //CORS Enable
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                                .WithHeader("Access-Control-Allow-Methods", "POST,GET, OPTIONS")
                                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });
        }


        protected override void ConfigureConventions(NancyConventions conventions)
        {
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", @"app"));

            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("cache", "../Data/Cache"));
            

            base.ConfigureConventions(conventions);

        }
    }

    public static class NancyExtensions
    {
        public static Response FromByteArray(this IResponseFormatter formatter, byte[] body, string contentType = null)
        {
            return new ByteArrayResponse(body, contentType);
        }
    }

}
