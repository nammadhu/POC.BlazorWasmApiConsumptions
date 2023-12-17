To Consume API on Blazor Webassembly steps as below

Create Blazor Wasm with No Authentication
Create Web Api with No Authentication

//below required for localhost working
 builder.Services.AddCors(options =>
 {
     options.AddDefaultPolicy(builder =>
     {
         builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
     });
 });
//above required for localhost working
builder.Services.AddControllers();


app.UseCors(); // required for localhost
app.UseHttpsRedirection();

Configure Startup Projects both Client Wasm & Api as running

Run it

Get the APi url ,probably using swagger or browser controls like 
https://localhost:7298/WeatherForecast
or
http://localhost:5161/WeatherForecast

In BlazorWasm clientside method(ex:Weather.razor)
@inject HttpClient Http
....
 protected override async Task OnInitializedAsync()
 {
     var items = await Http.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7298/WeatherForecast");
     if (items != null)
         Console.WriteLine(items.ToString());
     forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
 }


 https://learn.microsoft.com/en-us/azure/cosmos-db/try-free?tabs=nosql
 https://github.com/AnkitSharma-007/azure-serverless-with-blazor/blob/master/FAQUIApp/FAQUIApp/Pages/CovidFAQ.razor
 https://learn.microsoft.com/en-us/events/azure-serverless-azure-serverless-conf/serverless-apps-with-blazor-webassembly-and-azure-static-web-apps
 https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
 https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/tutorial-dotnet-web-app
 https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-dotnet-get-started?tabs=azure-cli%2Cwindows

 https://github.com/Azure-Samples/cosmos-db-nosql-dotnet-samples/blob/main/300-query-items/Program.cs