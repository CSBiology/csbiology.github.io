#r "nuget: Genometric.BibitemParser, 2.5.0"
#r "../_lib/Fornax.Core.dll"

open System.IO
open Genometric


// #r "nuget: FSharpAux, 1.1.0"
// open FSharpAux

// let fileName = "C:/Users/muehl/Desktop/stage/test.bib"

// let g = 
//     System.IO.File.ReadAllLines fileName
//     //|> Seq.filter (fun line -> not <| line.StartsWith('\n')) 
//     |> Seq.filter (fun line -> line <> "") 
//     |> Seq.skip 1
//     |> Seq.groupWhen (fun line -> line.StartsWith "@article{")
//     |> Seq.iter (fun gr ->
//         let tmp = gr |> Array.ofSeq
//         let fn = tmp.[0].Replace("@article{","").Split('.')
//         let fn' = (fn.[1] + "_" + fn.[0] + ".bib").Replace(",","")
//         System.IO.File.WriteAllLines ("C:/Users/muehl/Desktop/bib/"+fn', gr)
//         )

type Publication = {
    Publisher : string
    Title     : string
    Year      : int
    Authors   : string
    Featured  : bool
}

let contentDir = "content/publications/"

let tryParse (parser:BibitemParser.Parser) (bibFileNamne:string) =
    let text = File.ReadAllText bibFileNamne
    let tryParseBibTex (item:string) =
        match (parser.TryParse(item)) with
        | true,result -> Some result
        | false,_ -> None 

    tryParseBibTex text


let toPublication (featured:bool) (item:BibitemParser.Model.Publication) =
    let formatAuthor (firstName:string) (lastName:string) =
        let fnLetter = if firstName.Length > 0 then firstName.Substring(0,1) else ""
        let star,lastName' = 
            if lastName.EndsWith("*") then
                "*", lastName.Substring(0,lastName.Length-1)
            else
                "",lastName 
        let tmp = sprintf "%s %s%s" lastName' fnLetter star
        if tmp.StartsWith("Mühlhaus") then
            sprintf "<strong>%s</strong>" tmp
        else
            tmp
    
    let year = if item.Year.HasValue then item.Year.Value else 3000
    let tmp = 
        item.Authors
        |> Seq.map (fun a -> formatAuthor a.FirstName a.LastName)
        |> String.concat ", "

    {
        Publisher = item.Journal
        Title     = item.Title
        Year      = year
        Authors   = tmp
        Featured  = featured
    }


let loadFile (featured:bool) (bibFile: string) =    
    let parser = new BibitemParser.Parser()
    let tmp = tryParse parser bibFile
    toPublication featured tmp.Value


let loader (projectRoot: string) (siteContent: SiteContents) =
    let postsPath = Path.Combine(projectRoot, contentDir)
    printfn "%s" postsPath
    let options = EnumerationOptions(RecurseSubdirectories = false)
    let files = Directory.GetFiles(postsPath, "*", options)
    files
    |> Array.filter (fun n -> n.EndsWith ".bib")
    |> Array.map (loadFile false)
    |> Array.iter siteContent.Add
    
    let files = Directory.GetFiles(postsPath+"/featured/", "*", options)
    files
    |> Array.filter (fun n -> n.EndsWith ".bib")
    |> Array.map (loadFile true)
    |> Array.iter siteContent.Add

    // siteContent.Add({disableLiveRefresh = false})
    siteContent

// <div class="publication publication--featured">
//     <div class="publication__publisher">Plant Physiology</div>
//     <div class="publication__date">2020</div>
//     <div class="publication__title">Identification of chloroplast envelope proteins with critical importance for cold acclimation</div>
//     <div class="publication__authors">Trentmann O*, <strong>Mühlhaus T*</strong>, Zimmer D, Sommer F, Schroda M, Haferkamp I, Keller I, Pommerrenig B, and Neuhaus HE</div>
// </div><!-- /.publication --> 