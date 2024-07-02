#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"

open System.IO
open Markdig

let key_image = "img"
let key_image_url: string = "img_url"

let contentDir = "content/research/"

let imgDir = "content/images/research/"


module ContentReader =

    let pipeline = 
      let n = new MarkdownPipelineBuilder()
      n
        .UseAdvancedExtensions()
        .Build();

    let isSeparator (input : string) =
      input.StartsWith "---"

    ///`fileContent` - content of page to parse. Usually whole content of `.md` file
    /// Returns map<string,string> with frontmatter and everything else as "content".
    let getData (fileContent : string []) =
        let fileContent = fileContent |> Array.skip 1 //First line must be ---
        let indexOfSeperator = fileContent |> Array.findIndex isSeparator
        let splitKey (line: string) =
            let seperatorIndex = line.IndexOf(':')
            if seperatorIndex > 0 then
                let key = line.[.. seperatorIndex - 1].Trim().ToLower()
                let value = line.[seperatorIndex + 1 ..].Trim()
                Some(key, value)
            else
                None
        let frontmatter, content =
            fileContent
            |> Array.splitAt indexOfSeperator
            |> fun (x, y) -> x,y.[1..] //skip closing ---
        let moreContentIndex = content |> Array.tryFindIndex isSeparator
        let content, moreContent =
          match moreContentIndex with
          | Some i ->
            let content, moreContent = content |> Array.splitAt i
            content, Some moreContent.[1..] //skip closing ---
          | None -> 
            content, None
        frontmatter
        |> Seq.choose splitKey
        |> Map.ofSeq
        |> fun x -> 
            match moreContent with
            | None -> x
            | Some moreContent ->
              let moreContent = moreContent |> String.concat "\n"
              x.Add("more", moreContent)
        |> fun x -> 
          x.Add(
            "content", 
            content
            |> String.concat "\n"
          )
          

type FrameSlide = {
    /// This will always be on the slider
    MainContent: string
    /// This is hidden behind "More" button. Only exists if content after empty line exists.
    MoreContent: string option
    Image: string option
    ImageUrl: string option
    Index: int
} with
  static member create(content: string, index:int, ?moreContent, ?image, ?imageurl) =
    { MainContent = content; Index = index; MoreContent = moreContent; Image = image; ImageUrl = imageurl }

let getImageUrl (imageName: string) =
    imgDir + imageName

open Markdig

let loadFile (filePath: string) =
    let lines = File.ReadAllLines(filePath)
    let metadata = ContentReader.getData lines
    let content = metadata.["content"] |> fun c -> Markdown.ToHtml(c, ContentReader.pipeline)
    let moreContent = metadata |> Map.tryFind "more" |> Option.map (fun mc -> Markdown.ToHtml(mc, ContentReader.pipeline))
    let image = metadata |> Map.tryFind key_image |> Option.map getImageUrl
    let imageUrl = metadata |> Map.tryFind key_image_url
    let index = metadata |> Map.tryFind "index" |> Option.map int |> Option.defaultValue System.Int32.MaxValue
    FrameSlide.create(content, index, ?moreContent=moreContent, ?image=image, ?imageurl=imageUrl)

let loader (projectRoot: string) (siteContent: SiteContents) =
    let postsPath = Path.Combine(projectRoot, contentDir)
    let options = EnumerationOptions(RecurseSubdirectories = true)
    let files = Directory.GetFiles(postsPath, "*", options)
    files
    |> Array.filter (fun n -> n.EndsWith ".md")
    |> Array.map (loadFile)
    |> Array.sortBy (fun x -> x.Index)
    |> Array.mapi (fun i x -> { x with Index = i }) // make index unique
    |> Array.iter siteContent.Add
    // siteContent.Add({disableLiveRefresh = false})
    siteContent