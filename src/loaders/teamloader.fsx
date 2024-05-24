#r "../_lib/Fornax.Core.dll"

open System.IO    

type Role = 
    | Professor
    | PhD
    | MasterStudent
    | BachelorStudent
    | Alumni
    | Custom of string
    
    member this.toString() =
        match this with
        | Professor -> "Professor"
        | PhD -> "PhD Student"
        | MasterStudent -> "Master Student"
        | BachelorStudent -> "Bachelor Student"
        | Alumni -> "Alumni"
        | Custom str -> str

    static member parse(str:string) =
        match str.ToLowerInvariant() with
        | s when s.StartsWith("professor") -> Professor
        | s when s.StartsWith("phd") -> PhD
        | s when s.StartsWith("master") -> MasterStudent
        | s when s.StartsWith("bachelor") -> BachelorStudent
        | s when s.StartsWith("alumni") -> Alumni
        | _ -> Custom str

type Social = 
    | GitHub of string
    | Orcid of string
    | Twitter of string
    | Email of string
    member this.toHrefStr() =
        match this with
        | GitHub  handle -> "https://github.com/" + handle
        | Orcid   handle -> "https://orcid.org/" + handle
        | Twitter handle -> "https://twitter.com/" + handle
        | Email   handle -> "mailto:" + handle
    member this.Icon =
        match this with
        | GitHub  _ -> "fab fa-github"
        | Orcid   _ -> "fab fa-orcid"
        | Twitter _ -> "fab fa-twitter"
        | Email   _ -> "fa-solid fa-envelope"

    member this.CSSClass =
        match this with
        | GitHub  _ -> "person__github"
        | Orcid   _ -> "person__orcid"
        | Twitter _ -> "person__twitter"
        | Email   _ -> "person__email"

type TeamMember = {
    Index         : int
    Name          : string
    Img           : string option
    Phone         : string option
    Role          : Role
    Socials       : Social list
    //AlumniDetails : AlumniDetails list
}

type AlumniDetails = {
    Name : string
    Year : int
    Role : Role
}

let tryParseAluminiDetails name (str:string) =
    let splitStr = str.Split('-')
    if splitStr.Length = 2 then
        Some {Name = name; Year = ( splitStr.[0] |> int); Role= Role.parse splitStr.[1]} 
    else
        None


let contentDir = "content/team/"
let imgDir = "content/images/team/"

let isSeparator (input : string) =
    input.StartsWith "---"


///`fileContent` - content of page to parse. Usually whole content of `.md` file
///returns content of teamMember.md that should be used for the page
let getTeamData (fileContent : string) =
    let fileContent = fileContent.Split '\n'
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
    fileContent
    |> Array.splitAt indexOfSeperator
    |> fst
    |> Seq.choose splitKey
    |> Map.ofSeq


let trimString (str : string) =
    str.Trim().TrimEnd('"').TrimStart('"')

let loadFile (rootDir: string) (n: string) =
    let text = File.ReadAllText n

    let teamData = getTeamData text
    
    let chopLength =
        if rootDir.EndsWith(Path.DirectorySeparatorChar) then rootDir.Length
        else rootDir.Length + 1

    let dirPart =
        n
        |> Path.GetDirectoryName
        |> fun x -> x.[chopLength .. ]

    let index = teamData |> Map.tryFind "index"  |> Option.map trimString |> Option.defaultValue "9999" |> int 
    let name = teamData |> Map.find "name" |> trimString
    let img  = teamData |> Map.tryFind "img" |> Option.map trimString |> Option.map (fun s -> imgDir + s)
    let role = teamData |> Map.find "role" |> trimString 
    let phone = teamData |> Map.tryFind "phone" |> Option.map trimString
    let socials: Social list = 
        [
            teamData |> Map.tryFind "github"  |> Option.map (trimString >> Social.GitHub)
            teamData |> Map.tryFind "orcid"   |> Option.map (trimString >> Social.Orcid)
            teamData |> Map.tryFind "twitter" |> Option.map (trimString >> Social.Twitter)
            teamData |> Map.tryFind "email"   |> Option.map (trimString >> Social.Email)
        ] |> List.choose id
    
    let alumniDetails =
        teamData 
        |> Map.tryFind "alumni" 
        |> Option.map trimString
        |> Option.map (fun str -> str.Split(',') |> Seq.toList |> List.choose (tryParseAluminiDetails name))
        |> Option.defaultValue []

    //let published = config |> Map.tryFind "published" |> Option.map (trimString >> System.DateTime.Parse)


    { 
      Index   =  index
      Name    = name
      Img     = img
      Phone   = phone
      Role    = Role.parse role
      Socials = socials
    }, alumniDetails


let loader (projectRoot: string) (siteContent: SiteContents) =
    let postsPath = Path.Combine(projectRoot, contentDir)
    let options = EnumerationOptions(RecurseSubdirectories = true)
    let files = Directory.GetFiles(postsPath, "*", options)
    files
    |> Array.filter (fun n -> n.EndsWith ".md")
    |> Array.map (loadFile projectRoot)
    |> Array.iter ( fun (teamMember,alumni) ->
        siteContent.Add teamMember
        alumni |> List.iter siteContent.Add
        )
        

    // siteContent.Add({disableLiveRefresh = false})
    siteContent


loader contentDir 
