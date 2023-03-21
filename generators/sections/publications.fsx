#r "../../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../../loaders/bibloader.fsx"
#endif

open Html

let layoutPublication (pub:Bibloader.Publication) =
    let cLable = if pub.Featured then "publication publication--featured" else "publication publication"
    div [Class cLable] [
        div [Class "publication__publisher"] [!!pub.Publisher]
        div [Class "publication__date"] [!!(sprintf "%i" pub.Year)]
        div [Class "publication__title"] [!!pub.Title]
        div [Class "publication__authors"] [!!pub.Authors]
    ]
    

let generate (ctx : SiteContents) (_: string) =
    let pubs =
        ctx.TryGetValues<Bibloader.Publication> ()
        |> Option.defaultValue Seq.empty
        |> Seq.sortBy (fun pub -> -pub.Year)
        |> Seq.toList

    section [Class "section section--publication"; Id "publication"] [
        div [Class "container"] [
            
            h2 [Class "section__title"] [ !! "Publications" ]
            p  [Class "section__subtitle"] [ !! "View our selected publications"]
            
            div [Class "publication-container"] [
                yield! pubs |> List.map layoutPublication
            ]
            
            div [Class "more-block"] [
                a [Href "#publication"; Class "show-more js-toggle-publication"] [!!"Show more"; i [Class "fa fa-chevron-down";HtmlProperties.Custom("aria-hidden","true")] [] ]
            ]
        ] 
    ]
