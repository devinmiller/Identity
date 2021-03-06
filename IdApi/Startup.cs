﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdApi.Data;
using IdApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            ConfigureIdentityServer(services);

            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvc();
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });

            if(Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();

                //builder
                //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                //    .AddInMemoryApiResources(Config.GetApis())
                //    .AddInMemoryClients(Config.GetClients());
            }
            else
            {

                X509Certificate2 cert = GetCertificate();

                if (cert == null)
                {
                    throw new Exception("Unable to load token signing credential.");
                }
                else
                {
                    builder.AddSigningCredential(cert);

                    //TODO: Implement certificate rollover
                    //builder.AddValidationKeys(new Microsoft.IdentityModel.Tokens.X509SecurityKey(cert));
                }
            }

            string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string identityConnection = Configuration.GetConnectionString("IdentityConnection");

            builder
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                            b.UseSqlServer(identityConnection,
                                sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(identityConnection,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                });

            // this configures IdentityServer to use the ASP.NET Identity implementations
            builder.AddAspNetIdentity<ApplicationUser>();
        }

        private X509Certificate2 GetCertificate()
        {
            X509Certificate2 cert = null;

            try
            {

                using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
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