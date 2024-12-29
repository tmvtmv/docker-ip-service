// Import required namespaces
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Added for AllowAnonymous
using System;
using System.Linq;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

// Controller to handle the IP address request
public class IpController : ControllerBase
{
    [HttpGet("/ip")]
    [AllowAnonymous]
    public IActionResult GetIpAddress()
    {
        try
        {
            // Get host name
            string hostName = Dns.GetHostName();

            // Retrieve the IP address of the host
            var ipAddresses = Dns.GetHostAddresses(hostName);
            var dockerIp = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            if (dockerIp != null)
            {
                return Ok(new { ip = dockerIp.ToString() });
            }
            else
            {
                return NotFound(new { message = "No IPv4 address found." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the IP address.", error = ex.Message });
        }
    }
}

