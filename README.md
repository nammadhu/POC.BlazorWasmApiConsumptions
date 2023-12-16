Section1> Blazor webassembly consuming API
 (Using DotNet 8.0 versions for all)

Create Blazor Wasm with No Authentication
Create Web Api with No Authentication

<img width="247" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/e36ee6b9-e3d6-4fc1-ac3b-389422c92996">

ClientSide(wasm) No specific change required(unless credentials)
<img width="808" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/dd3c40a7-e272-4710-8eaa-70f5562c8746">

On Api Side, By default its like below
<img width="917" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/91b6277d-d5a8-493b-b33b-0f1c177e5cd2">

Then Try Consuming API by calling at clientside as below,
<img width="896" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/33e1e133-3507-4926-a286-84c3458a3dd8">
@inject HttpClient Http
 .... 
protected override async Task OnInitializedAsync() { 
var items = await Http.GetFromJsonAsync<WeatherForecast[]>("https://localhost:7298/WeatherForecast"); if (items != null) forecasts = items;//await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json"); 
}


APi URL can be taken from running instance on browser or cmd prompt
<img width="793" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/acb255c6-276e-4315-b45a-5b9e098df394">
Http or Https both as per the needs,
https://localhost:7298/WeatherForecast
or
http://localhost:5161/WeatherForecast


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

Continuation of this thread are as below,
https://stackoverflow.com/questions/68585377/access-to-xmlhttprequest-at-https-login-microsoftonline-com-has-been-blocked-b

https://www.freecodecamp.org/news/how-to-implement-azure-serverless-with-blazor-web-assembly/

Section2> Blazor webassembly consuming Azure Functions
Add Az functions project to solution 
<img width="713" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/395352e2-f608-4961-9ce0-18040f1c4013">
Then it becomes like this,
<img width="874" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/db90ed49-1196-4273-982c-8da13ecc4672">
To enable add CORS like below(also can mention mention website name to allow instead of * ALL)
<img width="558" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/c9b1b986-0925-4493-ba73-4267191a38f0">

<img width="440" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/6ce51e29-e173-4a5a-bad6-484e2e564497">


Get the az function local URL from cmd prompt & test the same on browser for quick check
<img width="897" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/41ab45ed-3ead-45e6-b741-6295435673a0">

Then at clientside change consumption as below
<img width="856" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/a0aad1d6-5702-45ad-829d-dd98a6c4b685">

Final result as like below,
<img width="945" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/4516c942-0e75-42f1-b5ff-5ccdc827536d">









Note: 
1>For working on local environment wrt Azure functions,it needs some extra permission to run. Usually in many corporate provided systems its not allowed to run since its not trucsted properly.So try for suitable respective solution .
2> For working locally,its better to maintain configurations is separate config files & not to check in . ANd always preferred to not check in this file. Still if you want to check in change on gitignore to include.
Like Developement.settings.json   or local.settings.json
![image](https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/7e2a52df-2777-4a83-ab3e-d3d43ac4c24a)

3>For enabling CORS on local make it like this
<img width="919" alt="image" src="https://github.com/nammadhu/POC.BlazorWasmApiConsumptions/assets/3640748/9f7cc3f5-9a29-4abc-83f8-c9b08bf52b5f">


