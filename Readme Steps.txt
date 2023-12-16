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

Configure Startup Projects

Run it

Get the APi url ,probably using swagger or browser controls like 
https://localhost:7298/WeatherForecast
or
http://localhost:5161

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