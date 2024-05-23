using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var key = Encoding.ASCII.GetBytes("sayanakbalikkampungjgncuritokensaya1234567"); // Use a secure key and store it securely

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:5188",
        ValidAudience = "http://localhost:5188",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Login to create token
app.MapPost("/api/auth",([FromBody]User user)=>{
    if (user.Username == "test" && user.Password == "password")
    {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sayanakbalikkampungjgncuritokensaya1234567"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, "test token"),
                        new Claim(JwtRegisteredClaimNames.Name, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
                var token = new JwtSecurityToken(
                issuer: "http://localhost:5188",
                audience: "http://localhost:5188",
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                 return Results.Ok(new { Token = tokenString });
    }
    else{
            return Results.Unauthorized();
    }

});

#region endpoint baja
//Baja
app.MapGet("/api/baja",()=>{

    PermintaanBaja pb = new PermintaanBaja();
    pb.KodBaja = "AA";
    pb.TarikhPermohonan = DateTime.Now;
    pb.Kuantiti = 10;
    pb.IsApproved = false;
    pb.NOLo = "L111";

    return Results.Ok(pb);
}).RequireAuthorization();

#endregion

app.UseAuthentication();
app.UseAuthorization();

app.Run();

#region Model
    public class PermintaanBaja
    {
        public int Id { get; set; }
        public string NOLo {get;set;}
        public DateTime TarikhPermohonan {get;set;}
        public string KodBaja {get;set;}
        public int Kuantiti {get;set;}
        public bool IsApproved {get;set;}
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

#endregion
