namespace WebApplication1.Controllers

open System
open System.Net.Http
open System.Net.Http.Json
open System.Text.Json
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging


type SolatInfo() =
    member val Hijri = "" with get, set
    member val Day = "" with get, set
    member val Date = "" with get, set
    member val Imsak = "" with get,set
    member val Subuh = "" with get, set
    member val Syuruk = "" with get, set
    member val Zohor = "" with get, set
    member val Asar = "" with get, set
    member val Maghrib = "" with get, set
    member val Isyak = "" with get, set
    
[<ApiController>]
[<Route("api/[controller]")>]  // Explicitly defining the route with 'api' prefix
type SolatController (logger: ILogger<SolatController>) =
    inherit ControllerBase()
    
    member private this.parsePrayerTimes (jsonString: string) : List<SolatInfo> =
        use jsonDocument = JsonDocument.Parse(jsonString)
        let prayerTimeElements = 
            jsonDocument.RootElement
                .GetProperty("prayerTime")
                .EnumerateArray()
    
        prayerTimeElements
        |> Seq.map (fun element ->
            let solat = new SolatInfo()
            solat.Hijri <- element.GetProperty("hijri").GetString()
            solat.Date <- element.GetProperty("date").GetString()
            solat.Day <- element.GetProperty("day").GetString()
            solat.Imsak <- element.GetProperty("imsak").GetString()
            solat.Subuh <- element.GetProperty("fajr").GetString()
            solat.Syuruk <- element.GetProperty("syuruk").GetString()
            solat.Zohor <-  element.GetProperty("dhuhr").GetString()
            solat.Asar <-  element.GetProperty("asr").GetString()
            solat.Maghrib <-  element.GetProperty("maghrib").GetString()
            solat.Isyak <-  element.GetProperty("isha").GetString()
            solat
            )
        |> List.ofSeq
    
    [<HttpGet("{zoneId}")>]
    member this.GetSolat(zoneId: string)  =
        logger.LogInformation("GetSolat called with zoneId: {ZoneId}", zoneId)
        let result = task{
            let waktuSolat = new HttpClient()
            let! result = waktuSolat.GetStringAsync($"https://www.e-solat.gov.my/index.php?r=esolatApi/takwimsolat&period=week&zone={zoneId}")
            return result
        }
        result.Wait()
        let prayerTimes = this.parsePrayerTimes result.Result
        this.Ok(prayerTimes)
        
    [<HttpGet>]  // Adding a root endpoint for the controller
    member this.Get() : IActionResult =
        logger.LogInformation("Root Get method called")
        this.Ok ( "SolatController is working")
