To Consume API on #Blazor Webassembly steps as below (Using DotNet 8.0 versions for all)

Create Blazor Wasm with No Authentication
Create Web Api with No Authentication

<img width="247" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/e36ee6b9-e3d6-4fc1-ac3b-389422c92996">

ClientSide(wasm) No specific change required(unless credentials)
<img width="808" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/dd3c40a7-e272-4710-8eaa-70f5562c8746">

On Api Side, By default its like below
<img width="917" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/91b6277d-d5a8-493b-b33b-0f1c177e5cd2">

Then Try Consuming API by calling at clientside as below,
<img width="929" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/d7113248-2d14-4703-964e-13d91bf1fc30">
APi URL can be taken from running instance on browser or cmd prompt
<img width="793" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/acb255c6-276e-4315-b45a-5b9e098df394">
Http or Https both as per the needs,
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



Then CORS Error appears as like below,

<img width="941" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/8230005d-efc6-4563-b935-3e30fcd578c8">


Then Changes required are as below ,
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
It becomes as below,
<img width="898" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/954fdc46-435a-46f2-aa16-2f9f307ca6da">

Configure Startup Projects both Client Wasm & Api as running( Right click on Solution Explorer Solution name & choose )
<img width="300" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/17d882d8-ef51-41ac-9bec-f3496537cfb4">
Then
<img width="751" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/1e01f795-d95b-4462-bcdb-b75df824ad61">

Then Run & enjoy further as per the need
