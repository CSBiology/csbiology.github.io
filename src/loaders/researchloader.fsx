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
        frontmatter
        |> Seq.choose splitKey
        |> Map.ofSeq
        |> fun x -> x.Add(
            "content", 
            content.[1..] //skip closing ---
            |> String.concat "\n"
        )

type FrameSlide = {
    Content: string
    Image: string option
    ImageUrl: string option
} with
  static member create(content: string, ?image, ?imageurl) =
    { Content = content; Image = image; ImageUrl = imageurl }

let getImageUrl (imageName: string) =
    imgDir + imageName

open Markdig

let loadFile (filePath: string) =
    let lines = File.ReadAllLines(filePath)
    let metadata = ContentReader.getData lines
    let content = metadata.["content"] |> fun c -> Markdown.ToHtml(c, ContentReader.pipeline)
    let image = metadata |> Map.tryFind key_image |> Option.map getImageUrl
    let imageUrl = metadata |> Map.tryFind key_image_url
    let index = metadata |> Map.tryFind "index" |> Option.map int |> Option.defaultValue System.Int32.MaxValue
    index, FrameSlide.create(content, ?image=image, ?imageurl=imageUrl)

let loader (projectRoot: string) (siteContent: SiteContents) =
    let postsPath = Path.Combine(projectRoot, contentDir)
    let options = EnumerationOptions(RecurseSubdirectories = true)
    let files = Directory.GetFiles(postsPath, "*", options)
    files
    |> Array.filter (fun n -> n.EndsWith ".md")
    |> Array.map (loadFile)
    |> Array.sortBy fst
    |> Array.map snd
    |> Array.iter siteContent.Add
    // siteContent.Add({disableLiveRefresh = false})
    siteContent