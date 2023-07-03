#r "../../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../../loaders/teachingloader.fsx"
#endif

open Html

// let layoutLecture (lex:Teachingloader.Teaching) =
//     div [Class "lecture"] [
//         div [Class "lecture-heading"] [
//             div [Class "lecture__id"]   [!!lex.ExternalRef]
//             div [Class "lecture__type"] [!!lex.CourseType]
//             div [Class "lecture__audience"] [!!lex.Audience] 
//             div [Class "lecture__title"] [
//                 b [] [!!lex.CourseType]
//                 br []
//                 !!lex.Title
//                 ]
//         ]
//         div [Class "lecture-body"] [
//             div [Class "lecture__cp"]   [!!(sprintf "%s - %s" lex.CP lex.Semester)]
//             div [Class "lecture__link"] [
//                 a [Href lex.SourceLink] [!!"OpenOlat"]
//             ]
//             div [Class "lecture__summary"] [!!lex.Summary]
//         ]
//     ]

// let generate (ctx : SiteContents) (_: string) =
//     let lexs: Teachingloader.Teaching list =
//         ctx.TryGetValues<Teachingloader.Teaching> ()
//         |> Option.defaultValue Seq.empty
//         |> Seq.toList

//     section [Class "section section--teaching"; Id "teaching"] [
//         div [Class "container"] [
            
//             h2 [Class "section__title"] [ !! "Teaching" ]
//             p  [Class "section__subtitle"] [ !! "Our teaching activity for bachelor and master students"]
            
//             ul [Class "btn-group js-tab-teaching-toggle"] [
//                 li [] [ a [Href "#tab_all_semesters"; Class "btn btn--small btn--active"] [!!"All semesters"]]
//                 li [] [ a [Href "#tab_summer_semester"; Class "btn btn--small"] [!!"Summer semester"]]
//                 li [] [ a [Href "#tab_winter_semester"; Class "btn btn--small"] [!!"Winter semester"]]
//             ]
            
//             div [Class "tabs"; Id "teaching-tabs"] [
//                 div [Class "tab tab--active"; Id "tab_all_semesters"] [
//                     div [Class "lecture-container"] [
//                         yield! lexs |> List.map layoutLecture     
//                     ]
//                 ]
//             ]
//         ] 
//     ]
