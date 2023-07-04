#r "../../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../../loaders/teamloader.fsx"
#endif

open Html

let createSocial (sc:Teamloader.Social) =
    let pro = sprintf "person__%s" (sc.String())
    let fab = sprintf "fab fa-%s" (sc.String())
    a [Class pro; Href (sc.toHrefStr())] [ i [Class fab] [] ]

let getInitials (name:string) =
    name.Split(' ')
    |> Array.map (fun s -> if s.Length > 0 then s.Substring(0,1) else "")
    |> String.concat ""

let layoutPerson (incRole:bool) (tm:Teamloader.TeamMember) =
    let roleStr = tm.Role.toString()
    let nrStr   = sprintf "%s (%s)" tm.Name roleStr
    let initials = getInitials tm.Name

    div [Class "person"] [
        match tm.Img with
        | Some imgStr ->
            img [Src imgStr; Alt nrStr; HtmlProperties.Title nrStr; Class "person__image" ]
        | None ->
            div [Class "person__avatar"] [!! initials]    
        h3 [Class "person__name"] [ !! tm.Name ]
        if incRole then span [Class "person__role"] [ !! roleStr ]        
        div [Class "person__socials"] [
            yield! tm.Socials |> List.map createSocial
        ]
    ]

let layoutAlumniPerYear (year:string) (persons:seq<string>) =
    li [] [
        div [Class "alumni__year"] [!!year]
        div [Class "alumni__people"] [
            for ps in persons -> 
                p [] [!!ps]
        ]
    ]

                        


let generate (ctx : SiteContents) (_: string) =
    let team =
        ctx.TryGetValues<Teamloader.TeamMember> ()
        |> Option.defaultValue Seq.empty
        |> Seq.toList
    let alumni =
        ctx.TryGetValues<Teamloader.AlumniDetails> ()
        |> Option.defaultValue Seq.empty
        |> Seq.toList
        |> List.groupBy (fun a -> a.Year)
        |> List.sortBy (fun (key,a) -> -key)
        |> List.map (fun (key,a) -> 
                sprintf "%i" key, 
                a |> List.map (fun p -> (sprintf "%s (%s)" p.Name (p.Role.toString())))
            )

    let masters,rest =
        team
        |> List.filter (fun p -> p.Role<>Teamloader.Role.Alumni)
        |> List.partition (fun p -> p.Role=Teamloader.Role.MasterStudent)
    let bachelor,persons =
        rest
        |> List.sortBy (fun p -> p.Index)
        |> List.partition (fun p -> p.Role=Teamloader.Role.BachelorStudent)
             

    section [Class "section section--people"; Id "team"] [
        div [Class "container"] [

            h2 [Class "section__title"] [!!"Team"]
            p  [Class "section__subtitle"] [!!"Our research team on CSB"]
            ul [Class "btn-group js-tab-team-toggle"] [
                li [] [ a [Href "#tab_staff"; Class "btn btn--small btn--active"] [!!"Scientific Staff"]]
                li [] [ a [Href "#tab_master"; Class "btn btn--small"] [!!"Master Students"]]
                li [] [ a [Href "#tab_bachelor"; Class "btn btn--small"] [!!"Bachelor Students"]]
                li [] [ a [Href "#tab_alumni"; Class "btn btn--small"] [!!"Alumni"]]
            ]
            
            div [Class "team-tabs"; Id "team-tabs"] [
                // Scientific Staff
                div [Class "team-tab team-tab--active"; Id "tab_staff"] [
                    div [Class "persons"] [
                            for pm in persons -> 
                                layoutPerson true pm
                    ]
                ]
                // Master Students
                div [Class "team-tab"; Id "tab_master"] [
                    div [Class "persons"] [
                            for pm in masters -> 
                                layoutPerson false pm
                    ]
                ]
                // Bachelor Students
                div [Class "team-tab"; Id "tab_bachelor"] [
                    div [Class "persons"] [
                            for pm in bachelor ->  
                                layoutPerson false pm
                    ]
                ]
                // Alumni
                div [Class "team-tab persons"; Id "tab_alumni"] [
                    div [Class "alumni-container"] [
                        ul [Class "alumni"] [
                            for y,p in alumni ->
                                layoutAlumniPerYear y p                            
                        ]
                    ]
                ]
            ]
        ]        
    ]

