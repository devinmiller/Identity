// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdApi
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public ILogger Logger { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration, ILogger<Startup> logger)
        {
            Environment = environment;
            Configuration = configuration;
            Logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you wan to add an MVC-based UI
            //services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients());

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                X509Certificate2 cert = GetCertificate();

                if (cert == null)
                {
                    builder.AddDeveloperSigningCredential();
                }
                else
                {
                    builder.AddSigningCredential(cert);

                    //builder.AddValidationKeys(new Microsoft.IdentityModel.Tokens.X509SecurityKey(cert));
                }
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to support static files
            //app.UseStaticFiles();

            app.UseIdentityServer();

            // uncomment, if you wan to add an MVC-based UI
            //app.UseMvcWithDefaultRoute();
        }

        private X509Certificate2 GetCertificate()
        {
            X509Certificate2 cert = null;

            try
            {

                using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                {
                    certStore.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection certCollection = certStore.Certificates.Find(
                        X509FindType.FindByThumbprint,
                        "B2E1E2C93182DC45C1EAE43F56F851D26D85479C",
                        false);

                    if (certCollection.Count > 0)
                    {
                        cert = certCollection[0];
                        //Log.Logger.Information($"Successfully loaded cert from registry: {cert.Thumbprint}");
                    }

                    return cert;
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Error getting certificate from store");

                return null;
            }
        }
    }
}