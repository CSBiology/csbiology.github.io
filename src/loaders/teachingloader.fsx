#r "nuget: FsHttp, 10.0.0"
#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"

// open System.IO
// open Markdig

open System.IO
open FsHttp

type Teaching = {
    Id         : string // repoEntryKey
    ExternalRef: string // <externalRef>BIO-BTE-12-V-4</externalRef>
    CourseType : string
    Audience   : string
    Title      : string // title
    CP         : string
    SourceLink : string
    Material   : string option
    Location   : string option // location
    Summary    : string  // description
    Semester   : string // lifecycle > softKey
    ReproStatus: string
    Authors    : string
} 

open System.Text.Json
open System.Xml.Linq
open System.Xml
open System.Text.RegularExpressions
open System.Collections.Generic

let tryGetbyName (propertyName:string) (jsonE: JsonElement) =
    match jsonE.ValueKind with
    | JsonValueKind.Null -> None
    | _ -> 
        match jsonE.TryGetProperty(propertyName) with
        | true,value -> Some value
        | false,_ -> None 

//TODO: Better include all JsonVAlueKind match cases 
let optionDefaultStringValue (defaultValue:string) (jsonE: JsonElement option) =
    jsonE 
    |> Option.map (fun e -> 
        match e.ValueKind with
        | JsonValueKind.Null -> defaultValue
        | JsonValueKind.Number -> e.GetRawText()
        | _ -> e.GetString() )
    |> Option.defaultValue defaultValue


//let htmlString = "<ul><li>Übung/Tutorium zum Biophysikmodul</li></ul><p> </p>"
let removeHtmlTags (htmlString:string) = 
    let readerSettings = XmlReaderSettings(ConformanceLevel = ConformanceLevel.Fragment)
    let reader = XmlReader.Create(new StringReader(htmlString), readerSettings)
    let rec loop (str:string) =
        try 
            match reader.Read() with
            | true -> 
                match reader.NodeType with
                | XmlNodeType.Element ->
                    let fragmentReader = reader.ReadSubtree()
                    match fragmentReader.Read() with
                    | true -> 
                        let fragment = XNode.ReadFrom(fragmentReader) :?> XElement
                        loop (str + fragment.Value)
                    | false -> loop str
                | XmlNodeType.Text -> loop (str + reader.Value)
                | _ -> loop str
            | false -> str  
        with
            | :? System.Xml.XmlException -> htmlString


    loop ""
    |> fun str -> str.Trim()


let removeHtml_P_Tags (htmlString:string) =
    htmlString.Replace("<p>","").Replace("</p>","")

let rx = Regex(@"^(?<institution>RPTU KL|RPTU KL\s)[|](?<courseType>[^|]*)[|](?<title>[^|]*)[|](?<time>.*)", RegexOptions.Compiled)

let parseTitel input =
    let matchDictionary = new Dictionary<string,string list>()    
    for mtch in (rx.Matches(input)) do
        for m in mtch.Groups do
            if m.Success then
                if(matchDictionary.ContainsKey(m.Name)) then
                    matchDictionary.Item(m.Name) <- (m.Value.Trim())::matchDictionary.Item(m.Name)
                else
                    matchDictionary.Add(m.Name,[m.Value])
    matchDictionary


let tryGet groupIndex input =
    let mtch = rx.Match(input)
    match mtch.Success with
    | true ->
        mtch.Groups 
        |> Seq.tryFind (fun cpt -> cpt.Name = groupIndex)
        |> Option.map (fun cpt -> cpt.Value.Trim())
    | false -> Some input

let tryGetCourseType = tryGet "courseType"
let tryGetTitle      = tryGet "title"
let tryGetTime       = tryGet "time"

let password = 
    let path =  "./loaders/olat.p"
    let fileExists = File.Exists path
    if fileExists then //read password from file
        printfn ".env file found!"
        File.ReadAllText path
    else
        printfn "No .env file found!"
        let envVars = 
            System.Environment.GetEnvironmentVariables()
            |> Seq.cast<System.Collections.DictionaryEntry>
            |> Seq.map (fun d -> d.Key :?> string, d.Value :?> string)
            |> dict
        // tokenName must be synced with .github/workflow/build.gh-pages => name: Setup Env Variable
        let tokenName = "OLAT_TOKEN"
        let containsKey = envVars.ContainsKey tokenName
        if not containsKey then
            printfn "Uanble to find Olat token as environmental variable!"
        envVars.[tokenName].Trim()

let getMetadataBy (repoEntryKey:string) = 
    let uri = sprintf "https://olat.vcrp.de/restapi/repo/entries/%s/metadata" repoEntryKey
    http {
        GET uri
        AuthorizationUserPw  "muehlhaus@bio.uni-kl.de" password        
        Accept "application/json"               
    }
    |> Request.send
    |> Response.toJson

let response = 
    http {
        GET "https://olat.vcrp.de/restapi/users/2589917519/courses/owned?start=0&limit=25"
        AuthorizationUserPw  "muehlhaus@bio.uni-kl.de" password        
        Accept "application/json"                     
    }
    |> Request.send
    |> Response.toJsonArray
    |> Seq.map (
        fun json ->        
            let displayNameTmp = json |> tryGetbyName "displayName"  |> optionDefaultStringValue "Title-missing"  
            let repoEntryKey   = json |> tryGetbyName "repoEntryKey" |> optionDefaultStringValue "RepoEntryKey-missing"
            let metaDataJson   = getMetadataBy repoEntryKey
            {
                Id          = repoEntryKey
                ExternalRef = json |> tryGetbyName "externalRef" |> optionDefaultStringValue "KIS-ID-missing" 
                CourseType  = displayNameTmp |> tryGetCourseType |> Option.defaultValue "CourseType-missing"
                // TODO -- Zielgruppe:
                Audience    = 
                    //"Audience: " + "Master"
                    metaDataJson 
                    |> tryGetbyName "requirements" 
                    |> optionDefaultStringValue "KIS-ID-missing" 
                    |> removeHtml_P_Tags 
                    |> fun s -> s.Replace("Zielgruppe:","").Replace("Audience:","").Trim()
            
                Title       = displayNameTmp |> tryGetTitle |> Option.defaultValue "Title-missing"
                CP          = metaDataJson |> tryGetbyName "expenditureOfWork" |> optionDefaultStringValue "ExpenditureOfWork-missing"
                SourceLink  = 
                    let reproKey = json |> tryGetbyName "repoEntryKey" |> optionDefaultStringValue "RepoEntryKey-missing" 
                    "https://olat.vcrp.de/url/RepositoryEntry/" + reproKey
                Material    = None  
                Location    = json |> tryGetbyName "location" |> optionDefaultStringValue "Location-missing"  |> Some
                Summary     = json |> tryGetbyName "description" |> optionDefaultStringValue "Description-missing" |> removeHtml_P_Tags
                Semester    = 
                    json
                    |> tryGetbyName "lifecycle"
                    |> Option.bind (tryGetbyName "softkey")
                    |> optionDefaultStringValue "Semester-missing"   
                ReproStatus = json |> tryGetbyName "repoEntryStatus" |> optionDefaultStringValue "ReproStatus-missing"
                Authors     = json |> tryGetbyName "authors" |> optionDefaultStringValue "Authors-missing"
            }

        )
    |> Seq.toArray


let loader (projectRoot: string) (siteContent: SiteContents) =
    response
    |> Array.filter (fun item -> item.ReproStatus = "published")
    |> Array.filter (fun item -> item.Authors.Contains("Mühlhaus"))
    |> Array.iter siteContent.Add

    siteContent
    
































// https://olat.vcrp.de/restapi/repo/entries/3661758469/metadata //"repoEntryKey":3661758469

// https://olat.vcrp.de/restapi/api-docs/?url=/restapi/openapi.json

// https://olat.vcrp.de/olat/restapi/repo/courses?start=2&limit=2&managed=false

// "https://olat.vcrp.de/restapi/users/2589917519?withPortrait=false"


// Get course information
// https://olat.vcrp.de/restapi/repo/courses/infos?start=0&limit=2

// user Id = 2589917519 (Get by "https://olat.vcrp.de/restapi/users/me")
// https://olat.vcrp.de/restapi/users/2589917519/courses/owned?start=0&limit=25


    

// let contentDir = "posts"

// let markdownPipeline =
//     MarkdownPipelineBuilder()
//         .UsePipeTables()
//         .UseGridTables()
//         .Build()

// let isSeparator (input : string) =
//     input.StartsWith "---"

// let isSummarySeparator (input: string) =
//     input.Contains "<!--more-->"


// ///`fileContent` - content of page to parse. Usually whole content of `.md` file
// ///returns content of config that should be used for the page
// let getConfig (fileContent : string) =
//     let fileContent = fileContent.Split '\n'
//     let fileContent = fileContent |> Array.skip 1 //First line must be ---
//     let indexOfSeperator = fileContent |> Array.findIndex isSeparator
//     let splitKey (line: string) =
//         let seperatorIndex = line.IndexOf(':')
//         if seperatorIndex > 0 then
//             let key = line.[.. seperatorIndex - 1].Trim().ToLower()
//             let value = line.[seperatorIndex + 1 ..].Trim()
//             Some(key, value)
//         else
//             None
//     fileContent
//     |> Array.splitAt indexOfSeperator
//     |> fst
//     |> Seq.choose splitKey
//     |> Map.ofSeq

// ///`fileContent` - content of page to parse. Usually whole content of `.md` file
// ///returns HTML version of content of the page
// let getContent (fileContent : string) =
//     let fileContent = fileContent.Split '\n'
//     let fileContent = fileContent |> Array.skip 1 //First line must be ---
//     let indexOfSeperator = fileContent |> Array.findIndex isSeparator
//     let _, content = fileContent |> Array.splitAt indexOfSeperator

//     let summary, content =
//         match content |> Array.tryFindIndex isSummarySeparator with
//         | Some indexOfSummary ->
//             let summary, _ = content |> Array.splitAt indexOfSummary
//             summary, content
//         | None ->
//             content, content

//     let summary = summary |> Array.skip 1 |> String.concat "\n"
//     let content = content |> Array.skip 1 |> String.concat "\n"

//     Markdown.ToHtml(summary, markdownPipeline),
//     Markdown.ToHtml(content, markdownPipeline)

// let trimString (str : string) =
//     str.Trim().TrimEnd('"').TrimStart('"')

// let loadFile (rootDir: string) (n: string) =
//     let text = File.ReadAllText n

//     let config = getConfig text
//     let summary, content = getContent text

//     let chopLength =
//         if rootDir.EndsWith(Path.DirectorySeparatorChar) then rootDir.Length
//         else rootDir.Length + 1

//     let dirPart =
//         n
//         |> Path.GetDirectoryName
//         |> fun x -> x.[chopLength .. ]

//     let file = Path.Combine(dirPart, (n |> Path.GetFileNameWithoutExtension) + ".md").Replace("\\", "/")
//     let link = "/" + Path.Combine(dirPart, (n |> Path.GetFileNameWithoutExtension) + ".html").Replace("\\", "/")

//     let title = config |> Map.find "title" |> trimString
//     let author = config |> Map.tryFind "author" |> Option.map trimString
//     let published = config |> Map.tryFind "published" |> Option.map (trimString >> System.DateTime.Parse)

//     let tags =
//         let tagsOpt =
//             config
//             |> Map.tryFind "tags"
//             |> Option.map (trimString >> fun n -> n.Split ',' |> Array.toList)
//         defaultArg tagsOpt []

//     { file = file
//       link = link
//       title = title
//       author = author
//       published = published
//       tags = tags
//       content = content
//       summary = summary }

// let loader (projectRoot: string) (siteContent: SiteContents) =
//     let postsPath = Path.Combine(projectRoot, contentDir)
//     let options = EnumerationOptions(RecurseSubdirectories = true)
//     let files = Directory.GetFiles(postsPath, "*", options)
//     files
//     |> Array.filter (fun n -> n.EndsWith ".md")
//     |> Array.map (loadFile projectRoot)
//     |> Array.iter siteContent.Add

//     siteContent.Add({disableLiveRefresh = false})
//     siteContent
