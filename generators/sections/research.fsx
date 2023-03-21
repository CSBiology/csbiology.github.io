#r "../../_lib/Fornax.Core.dll"

open Html

let generate (ctx : SiteContents) (_: string) =
    section [Class "section research"; Id "research"] [
        div [Class "container container--full"] [
            div [Class "research__title"] [
                div [] [
                    h2 [] [ !! "Welcome to CSB" ];
                    p [] [ !! "What our research is aiming for"];
                ]
            ];
        
            div [Class "research__text"] [
                p [] [!! "The main focus of our group is the application and development of computational methods to process and integrate quantitative biological data from modern high-throughput measurements in order to gain novel insights into biological responses to environment changes. The main challenge is the rigorous integration of different system level analyses and present knowledge into biological interpretable models. Therefore, we want to drive theory and technology forward with a combination of biological science, applied informatics, statistical and machine learning approaches."]
            ]
        ] 
    ]