using TP2_ISI_2024.Data;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TP2_ISI_2024.Interface;
using TP2_ISI_2024.Models;
using SoapCore;
using TP2_ISI_2024.Services;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false,
				ValidateIssuerSigningKey = true,
				ValidIssuer = builder.Configuration["Jwt:Issuer"],
				ValidAudience = builder.Configuration["Jwt:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
			};
		});

		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Version = "v1",
				Title = "Hostify API V1",
				Description = "Api para gestão de condominios",
				TermsOfService = new Uri("https://github.com/yabukithiago"),
				Contact = new OpenApiContact
				{
					Name = "Thiago Yabuki de Araujo",
					Email = "yabukithiago@gmail.com",
					Url = new Uri("https://github.com/yabukithiago"),
				},
				License = new OpenApiLicense
				{
					Name = "Termo de Licença de Uso",
					Url = new Uri("https://github.com/yabukithiago")
				}
			});

			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
				Type = SecuritySchemeType.ApiKey,
				In = ParameterLocation.Header,
				BearerFormat = "JWT",
				Scheme = "Bearer"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
						},
					Array.Empty<string>()
				}
			});
		});

		builder.Services.AddSoapCore();

		builder.Services.AddScoped<IWeatherSoapService, WeatherSoapService>();
		builder.Services.AddScoped<IMessageService, MessageService>();

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseRouting();
		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			// Mapeando os endpoints REST
			endpoints.MapControllers();

			// Mapeando os endpoints SOAP
			endpoints.UseSoapEndpoint<IWeatherSoapService>(
				"/WeatherSoapService.asmx",
				new SoapEncoderOptions(),
				SoapSerializer.XmlSerializer
			);
			endpoints.UseSoapEndpoint<IMessageService>(
				"/MessageService.asmx",
				new SoapEncoderOptions(),
				SoapSerializer.XmlSerializer
			);
		});

		app.Run();
	}
}
