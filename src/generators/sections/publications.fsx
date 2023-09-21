#r "../../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../../loaders/bibloader.fsx"
#endif

open Html

let layoutPublication (pub:Bibloader.Publication) =
    let cLable = if pub.Featured then "publication--featured" else "publication"
    div [Class $"column is-one-third {cLable}"] [
        div [Class "box has-background-white has-text-black p-0"] [
            div [Class "is-flex is-flex-direction-row is-flex-grow-1 has-background-primary mb-2"] [ //header-container
                b [Class "p-2 has-text-white"] [!!pub.Publisher] // Up-most row
                div [Class "p-2 has-background-primary-light"; HtmlProperties.Style [CSSProperties.MarginLeft "auto"]] [!!(sprintf "%i" pub.Year)] // second row
            ]
            div [Class "content p-2 mb-1 is-size-5"] [ // body 
                !!pub.Title
            ]
            div [Class "publication__authors content p-2 is-size-7 is-family-code"] [!!pub.Authors]
        ]
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
            
            div [Class "columns is-multiline publication-container"] [
                yield! pubs |> List.map layoutPublication
            ]
            
            div [Class "more-block"] [
                a [Href "#publication"; Class "show-more js-toggle-publication"] [!!"Show more"; i [Class "fa fa-chevron-down";HtmlProperties.Custom("aria-hidden","true")] [] ]
            ]
        ] 
    ]
