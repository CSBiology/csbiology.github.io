#r "../../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../../loaders/teachingloader.fsx"
#endif

open Html
open System.Text

let createCP_Semester ((lex:Teachingloader.Teaching)) =
    let sb = new StringBuilder()
    if lex.CP <> "" then
        sb.Append (lex.CP + " - ") |> ignore
    sb.Append(lex.Semester) |> ignore
    sb.ToString()

let layoutLecture (lex:Teachingloader.Teaching) =
    let courseType = if lex.CourseType.ToLower() = "course" then "Practical Course" else lex.CourseType
    div [Class "column is-half"] [
        div [Class "box has-background-white has-text-black p-0"] [
            div [Class "is-flex is-flex-direction-row is-flex-grow-1 has-background-primary mb-2"] [ //header-container
                div [Class "p-2 has-text-white"] [!!lex.ExternalRef] // Up-most row
                div [Class "p-2 has-background-primary-light"; HtmlProperties.Style [CSSProperties.MarginLeft "auto"]] [!!courseType] // second row
            ]
            div [Class "p-2"] [ // body
                div [Class "field"] [
                    h1 [Class "title is-6"] [ 
                        !!lex.CourseType
                        span [Class "is-family-code has-text-weight-normal"] [!! " for "]
                        if lex.Audience <> "" then
                            !!lex.Audience 
                        else
                            !!"Mixed Audience"
                    ]
                    h2 [Class "subtitle"] [!!lex.Title]
                ]
                div [Class "field"] [
                    div [Class "content"] [ // cont
                        h6 [Class "has-text-primary"] [!!(createCP_Semester lex)]
                        div [Class "is-family-code"; HtmlProperties.Style [CSSProperties.MarginTop "-1rem"]] [a [Href lex.SourceLink] [!!"OpenOlat"]]
                    ]
                ]
                div [Class "field"] [
                    div [Class "content"] [ // content
                        p [] [!!lex.Summary]
                    ]
                ]
            ] 
        ]
    ]

let generate (ctx : SiteContents) (_: string) =
    let lexs: Teachingloader.Teaching list =
        ctx.TryGetValues<Teachingloader.Teaching> ()
        |> Option.defaultValue Seq.empty
        |> Seq.toList

    section [Class "section section--teaching"; Id "teaching"] [
        div [Class "container"] [
            
            h2 [Class "section__title"] [ !! "Teaching" ]
            p  [Class "section__subtitle"] [ !! "Our teaching activity for bachelor and master students"]
            
            ul [Class "btn-group js-tab-teaching-toggle"] [
                li [] [ a [Href "#tab_all_semesters"; Class "btn btn--small btn--active"] [!!"All semesters"]]
                li [] [ a [Href "#tab_summer_semester"; Class "btn btn--small"] [!!"Summer semester"]]
                li [] [ a [Href "#tab_winter_semester"; Class "btn btn--small"] [!!"Winter semester"]]
            ]
            div [Class "csb-tabs"; Id "teaching-tabs"] [
                div [Class "csb-tab csb-tab--active"; Id "tab_all_semesters"] [
                    div [Class "columns is-multiline"] [
                        yield! lexs |> List.map layoutLecture
                    ]
                ]
                div [Class "csb-tab"; Id "tab_summer_semester"] [
                    div [Class "columns is-multiline"] [
                        yield! lexs |> List.filter (fun x -> x.Semester.ToLower().Contains "sommersemester") |> List.map layoutLecture     
                    ]
                ]
                div [Class "csb-tab"; Id "tab_winter_semester"] [
                    div [Class "columns is-multiline"] [
                        yield! lexs |> List.filter (fun x -> x.Semester.ToLower().Contains "wintersemester") |> List.map layoutLecture     
                    ]
                ]
            ]
        ] 
    ]
